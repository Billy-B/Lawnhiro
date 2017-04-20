<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfirmOrder.aspx.cs" Inherits="Lawnhiro.ConfirmOrder" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <script src="https://www.paypalobjects.com/api/checkout.js"></script>
            <script src="../Scripts/jquery-3.1.1.min.js"></script>

            <script type="text/javascript">
                function CheckBoxRequired_ClientValidate(sender, e) {
                    e.IsValid = document.getElementById('chk_agreeToTerms').checked;
                }
                paypal.Button.render(
        
            
                {

                    env: 'sandbox', // Specify 'sandbox' for the test environment, 'production'

                    client: {
                        sandbox: 'AeL5Z6IirMijkry6LzbZ8aS9E47B0AH2tHizjdJxrvMprG6X93w7w5I1zjJYQsOkYKzF0ZWLt5CcpkJ-',
                        production: 'AZyOmu5y4n9XGmYgARgR3KB4Slw-hDqh0uqfEKsfsASZTVAbpnTvmLKx6LTdDE6-BVq2dC85w4Xxm1k_'
                    },

                    payment: function (price) {
                        // Set up the payment here, when the buyer clicks on the button
                        if (Page_ClientValidate()) {
                            var env = this.props.env;
                            var client = this.props.client;
                            orderAmount = document.getElementById('priceField').value;
                            //alert(orderAmount);
                            return paypal.rest.payment.create(env, client, {
                                transactions: [
                                    {
                                        amount: { total: orderAmount, currency: 'USD' }
                                    }
                                ]
                            });
                        }
                        else {
                            return false;
                        }
                    },

                    commit: true,

                    onAuthorize: function (data, actions) {
                        // Execute the payment here, when the buyer approves the transaction
                        return actions.payment.execute().then(function () {
                            // Show a success page to the buyer
                            ///v1/payments/orders/order_id
                            document.getElementById('paypalOrderId').value = data.paymentID;
                            __doPostBack('paypalOrderId', 'ValueChanged');
                            alert('Your order has been submitted! Stay tuned for email updates.');
                        });
                    }

                }, '#paypal-button');
            </script>
            <div id="div_orderDetails">
                <table>
                    <tr>
                        <td><b>Address:</b></td>
                        <td><asp:Label ID="label_address" runat="server" /></td>
                    </tr>
                    <tr>
                        <td><b>Price:</b></td>
                        <td><asp:Label ID="label_price" runat="server" /></td>
                    </tr>
                    <tr>
                        <td><b>Email Address:</b></td>
                        <td>
                            <asp:TextBox ID="txt_email" runat="server" TextMode="Email" MaxLength="254" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txt_email" ForeColor="Red" Text="Required" />
                        </td>
                    </tr>
                    <tr id="div_couponCode" runat="server">
                        <td><b>Coupon Code: (optional)</b></td>
                        <td><asp:TextBox ID="txt_couponCode" runat="server" MaxLength="10" /></td>
                    </tr>
                    <tr id="div_headAboutUsSource" runat="server">
                        <td><b>How did you hear about us?</b></td>
                        <td><asp:DropDownList ID="ddl_heardAboutUsSource" runat="server" /></td>
                    </tr>
                    <tr>
                        <td><b>Additional Notes/Instructions:</b></td>
                        <td><asp:TextBox ID="txt_notes" runat="server" TextMode="MultiLine" Width="300" /></td>
                    </tr>
                </table>
                <asp:CheckBox ID="chk_agreeToTerms" runat="server" ClientIDMode="Static" />
                <b>
                    Agree to <a target="_new" href="http://www.lawnhiro.com/#!terms-and-conditions/x6s6o">Terms &amp; Conditions</a>
                </b>
                <asp:CustomValidator runat="server" EnableClientScript="true" Text="Required" ForeColor="Red" ClientValidationFunction="CheckBoxRequired_ClientValidate" />
                <br />
                <div id="paypal-button">
                </div>
                <%--<asp:Button ID="btn_placeOrder" runat="server" Text="Place Order" OnClick="btn_placeOrder_Click" BackColor="CadetBlue" />--%>
            </div>
        </div>
        <asp:HiddenField ID="paypalOrderId" runat="server" ClientIDMode="Static" OnValueChanged="paypalOrderId_ValueChanged" />
        <asp:HiddenField ID="priceField" runat="server" ClientIDMode="Static" />
    </form>
</body>
</html>
