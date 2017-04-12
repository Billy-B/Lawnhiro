using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    public class Startup
    {
        public static void AppStartupMethod()
        {
            AssemblyPreparer.PrepareAssemblies();
            /*var assms = AppDomain.CurrentDomain.GetAssemblies();
            Console.WriteLine("Test");
            System.Diagnostics.Debug.WriteLine("Test");*/
        }
    }
}
