using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class MethodReplacer
    {
        private static MethodInfo _dynamicMethodHandleAccessor = typeof(DynamicMethod).GetMethod("GetMethodDescriptor", BindingFlags.NonPublic | BindingFlags.Instance);

        [Flags]
        private enum MethodDescClassification : ushort
        {
            // Method is IL, FCall etc., see MethodClassification above.
            mdcClassification                   = 0x0007,
            mdcClassificationCount              = mdcClassification+1,

            // Note that layout of code:MethodDesc::s_ClassificationSizeTable depends on the exact values 
            // of mdcHasNonVtableSlot and mdcMethodImpl

            // Has local slot (vs. has real slot in MethodTable)
            mdcHasNonVtableSlot                 = 0x0008,

            // Method is a body for a method impl (MI_MethodDesc, MI_NDirectMethodDesc, etc)
            // where the function explicitly implements IInterface.foo() instead of foo().
            mdcMethodImpl                       = 0x0010,

            // Method is static
            mdcStatic                           = 0x0020,

            // Temporary Security Interception.
            // Methods can now be intercepted by security. An intercepted method behaves
            // like it was an interpreted method. The Prestub at the top of the method desc
            // is replaced by an interception stub. Therefore, no back patching will occur.
            // We picked this approach to minimize the number variations given IL and native
            // code with edit and continue. E&C will need to find the real intercepted method
            // and if it is intercepted change the real stub. If E&C is enabled then there
            // is no back patching and needs to fix the pre-stub.
            mdcIntercepted                      = 0x0040,

            // Method requires linktime security checks.
            mdcRequiresLinktimeCheck            = 0x0080,

            // Method requires inheritance security checks.
            // If this bit is set, then this method demands inheritance permissions
            // or a method that this method overrides demands inheritance permissions
            // or both.
            mdcRequiresInheritanceCheck         = 0x0100,

            // The method that this method overrides requires an inheritance security check.
            // This bit is used as an optimization to avoid looking up overridden methods
            // during the inheritance check.
            mdcParentRequiresInheritanceCheck   = 0x0200,

            // Duplicate method. When a method needs to be placed in multiple slots in the
            // method table, because it could not be packed into one slot. For eg, a method
            // providing implementation for two interfaces, MethodImpl, etc
            mdcDuplicate                        = 0x0400,

            // Has this method been verified?
            mdcVerifiedState                    = 0x0800,

            // Is the method verifiable? It needs to be verified first to determine this
            mdcVerifiable                       = 0x1000,

            // Is this method ineligible for inlining?
            mdcNotInline                        = 0x2000,

            // Is the method synchronized
            mdcSynchronized                     = 0x4000,

            // Does the method's slot number require all 16 bits
            mdcRequiresFullSlotNumber           = 0x8000
        }
        [Flags]
        private enum methodFlags : ushort
        {
            enum_flag_TokenRangeMask                           = 0x03FF, // This must equal METHOD_TOKEN_RANGE_MASK calculated higher in this file
                                                                     // These are seperate to allow the flags space available and used to be obvious here
                                                                     // and for the logic that splits the token to be algorithmically generated based on the 
                                                                     // #define
            enum_flag_HasCompactEntrypoints                    = 0x4000, // Compact temporary entry points
            enum_flag_IsZapped                                 = 0x8000, // This chunk lives in NGen module
        }
        [Flags]
        private enum flags2 : byte
        {
            // enum_flag2_HasPrecode implies that enum_flag2_HasStableEntryPoint is set.
            enum_flag2_HasStableEntryPoint      = 0x01,   // The method entrypoint is stable (either precode or actual code)
            enum_flag2_HasPrecode               = 0x02,   // Precode has been allocated for this method

            enum_flag2_IsUnboxingStub           = 0x04,
            enum_flag2_HasNativeCodeSlot        = 0x08,   // Has slot for native code

            enum_flag2_Transparency_Mask        = 0x30,
            enum_flag2_Transparency_Unknown     = 0x00,   // The transparency has not been computed yet
            enum_flag2_Transparency_Transparent = 0x10,   // Method is transparent
            enum_flag2_Transparency_Critical    = 0x20,   // Method is critical
            enum_flag2_Transparency_TreatAsSafe = 0x30,   // Method is treat as safe. Also implied critical.

            // CAS Demands: Demands for Permissions that are CAS Permissions. CAS Perms are those 
            // that derive from CodeAccessPermission and need a stackwalk to evaluate demands
            // Non-CAS perms are those that don't need a stackwalk and don't derive from CodeAccessPermission. The implementor 
            // specifies the behavior on a demand. Examples: CAS: FileIOPermission. Non-CAS: PrincipalPermission.
            // This bit gets set if the demands are BCL CAS demands only. Even if there are non-BCL CAS demands, we don't set this
            // bit.
            enum_flag2_CASDemandsOnly           = 0x40,

            enum_flag2_HostProtectionLinkCheckOnly = 0x80, // Method has LinkTime check due to HP only.
        }
        [Flags]
        private enum ExtendedFlags : uint
        {
            nomdAttrs = 0x0000FFFF, // method attributes (LCG)
            nomdILStubAttrs = mdMemberAccessMask | mdStatic, //  method attributes (IL stubs)

            // attributes (except mdStatic and mdMemberAccessMask) have different meaning for IL stubs
            mdMemberAccessMask     = 0x0007,
            nomdReverseStub = 0x0008,
            mdStatic               = 0x0010,
            nomdCALLIStub = 0x0020,
            nomdDelegateStub = 0x0040,
            nomdCopyCtorArgs = 0x0080,
            nomdUnbreakable = 0x0100,
            nomdDelegateCOMStub = 0x0200,  // CLR->COM or COM->CLR call via a delegate (WinRT specific)
            nomdSignatureNeedsRestore = 0x0400,
            nomdStubNeedsCOMStarted = 0x0800,  // EnsureComStarted must be called before executing the method
            nomdMulticastStub = 0x1000,
            nomdUnboxingILStub = 0x2000,
            nomdSecureDelegateStub = 0x4000,

            nomdILStub = 0x00010000,
            nomdLCGMethod = 0x00020000,
            nomdStackArgSize = 0xFFFC0000, // native stack arg size for IL stubs
        }

        public static unsafe void ReplaceMethod(MethodBase methodToInject, MethodInfo methodToReplace)
        {
            //First, let's make sure that methodToReplace is not eligable for inlining, as inlining will prevent the injection from working. 
            //This process is eqivalent to marking the method with MethodImplOptions.NoInlining.
            //Note: this process only prevents future inlining. If calling this process from a method that calls methodToReplace, chances are your test will fail. This is because the parent method was already JIT-ed when methodToReplace was inlinable, and the JIT-ed method cannot be changed.
            
            ushort* handleToReplace = (ushort*)methodToReplace.MethodHandle.Value;
            handleToReplace[3] |= (ushort)MethodDescClassification.mdcNotInline;

            RuntimeMethodHandle handleToInject = GetHandle(methodToInject);

            RuntimeHelpers.PrepareMethod(handleToInject);
            RuntimeHelpers.PrepareMethod(GetHandle(methodToReplace));

            if (methodToReplace.IsVirtual)
            {
                InjectVirtualMethod(methodToInject, methodToReplace, methodToReplace.DeclaringType);
                UInt64* methodDesc = (UInt64*)(methodToReplace.MethodHandle.Value.ToPointer());
                int index = (int)(((*methodDesc) >> 32) & 0xFF);
                if (IntPtr.Size == 4)
                {

                    uint* classStart = (uint*)methodToReplace.DeclaringType.TypeHandle.Value.ToPointer();
                    classStart += 10;
                    classStart = (uint*)*classStart;
                    uint* tar = classStart + index;

                    uint* inj = (uint*)handleToInject.Value.ToPointer() + 2;
                    //int* tar = (int*)methodToReplace.MethodHandle.Value.ToPointer() + 2;
                    *tar = *inj;
                }
                else
                {
                    ulong* classStart = (ulong*)methodToReplace.DeclaringType.TypeHandle.Value.ToPointer();
                    classStart += 8;
                    classStart = (ulong*)*classStart;
                    ulong* tar = classStart + index;

                    ulong* inj = (ulong*)methodToInject.MethodHandle.Value.ToPointer() + 1;
                    //ulong* tar = (ulong*)methodToReplace.MethodHandle.Value.ToPointer() + 1;
                    *tar = *inj;
                }
            }
            else
            {
                if (IntPtr.Size == 4) // 32-bit
                {
                    int* locToInject, locToTarget;

                    locToTarget = (int*)methodToReplace.MethodHandle.Value.ToPointer() + 2;
                    if (methodToInject is DynamicMethod)
                    {
                        RuntimeMethodHandle h = GetHandle(methodToInject);
                        locToInject = (int*)h.Value + 7;
                    }
                    else
                    {
                        locToInject = (int*)methodToInject.MethodHandle.Value.ToPointer() + 2;
                    }
#if DEBUG
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        byte* injInst = (byte*)*locToInject;
                        byte* tarInst = (byte*)*locToTarget;


                        int* injSrc = (int*)(injInst + 1);
                        int* tarSrc = (int*)(tarInst + 1);

                        *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);

                    }
                    else
                    {
                        *locToTarget = *locToInject;
                    }
#else
                    *locToTarget = *locToInject;
#endif
                }
                else // 64-bit
                {
                    long* inj = (long*)methodToInject.MethodHandle.Value.ToPointer() + 1;
                    long* tar = (long*)methodToReplace.MethodHandle.Value.ToPointer() + 1;
                }
                /*if (methodToInject is DynamicMethod)
                {
                    RuntimeMethodHandle p = GetDynamicMethodRuntimeHandle(methodToInject);
                    int* inj = (int*)p.Value + 7;
                    int* tar = (int*)methodToReplace.MethodHandle.Value + 2;
                    *tar = *inj;
                }
                else if (IntPtr.Size == 4)
                {
                    int* inj = (int*)methodToInject.MethodHandle.Value + 2;
                    int* tar = (int*)methodToReplace.MethodHandle.Value + 2;
    #if DEBUG
                    Console.WriteLine("\nVersion x84 Debug\n");

                    byte* injInst = (byte*)*inj;
                    byte* tarInst = (byte*)*tar;

                    int* injSrc = (int*)(injInst + 1);
                    int* tarSrc = (int*)(tarInst + 1);

                    *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
    #else
                    Console.WriteLine("\nVersion x84 Release\n");
                    *tar = *inj;
    #endif
                }
                else
                {

                    long* inj = (long*)methodToInject.MethodHandle.Value.ToPointer() + 1;
                    long* tar = (long*)methodToReplace.MethodHandle.Value.ToPointer() + 1;
    #if DEBUG
                    Console.WriteLine("\nVersion x64 Debug\n");
                    byte* injInst = (byte*)*inj;
                    byte* tarInst = (byte*)*tar;


                    int* injSrc = (int*)(injInst + 1);
                    int* tarSrc = (int*)(tarInst + 1);

                    *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
    #else
                    Console.WriteLine("\nVersion x64 Release\n");
                    *tar = *inj;
    #endif
                }*/
            }
        }

        public static unsafe void InjectVirtualMethod(MethodBase methodToInject, MethodInfo virtualMethod, Type type)
        {
            if (!virtualMethod.IsVirtual)
            {
                throw new ArgumentException("Must be virtual.", "virtualMethod");
            }
            UInt64* methodDesc = (UInt64*)(virtualMethod.MethodHandle.Value.ToPointer());
            int index = (int)(((*methodDesc) >> 32) & 0xFF);
            if (IntPtr.Size == 4)
            {
                uint* classStart = (uint*)type.TypeHandle.Value.ToPointer();
                classStart += 10;
                classStart = (uint*)*classStart;
                uint* tar = classStart + index;

                uint* inj;
                RuntimeMethodHandle injHandle = GetHandle(methodToInject);
                if (methodToInject is DynamicMethod)
                {
                    inj = (uint*)injHandle.Value.ToPointer() + 7;
                }
                else
                {
                    inj = (uint*)injHandle.Value.ToPointer() + 2;
                }
                *tar = *inj;
            }
            else
            {
                ulong* classStart = (ulong*)type.TypeHandle.Value.ToPointer();
                classStart += 8;
                classStart = (ulong*)*classStart;
                ulong* tar = classStart + index;

                ulong* inj = (ulong*)GetHandle(methodToInject).Value.ToPointer() + 1;
                *tar = *inj;
            }
        }

        /*public static unsafe IntPtr GetMethodAddress(MethodBase method)
        {
            return new IntPtr(((int*)method.MethodHandle.Value.ToPointer() + 2));
        }*/

        private static RuntimeMethodHandle GetHandle(MethodBase method)
        {
            if (method is DynamicMethod)
            {
                return ((RuntimeMethodHandle)_dynamicMethodHandleAccessor.Invoke(method, null));
            }
            return method.MethodHandle;
        }
    }
}
