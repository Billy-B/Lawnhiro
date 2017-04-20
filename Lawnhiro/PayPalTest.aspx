<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PayPalTest.aspx.cs" Inherits="Lawnhiro.PayPalTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <base target="_parent" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
            <script src="https://www.paypalobjects.com/api/checkout.js"></script>
            <script src="../Scripts/jquery-3.1.1.min.js"></script>

            <script type="text/javascript">
                paypal.Button.render(
        
            
                {

                    env: 'sandbox', // Specify 'sandbox' for the test environment, 'production'

                    client: {
                        sandbox: 'AeL5Z6IirMijkry6LzbZ8aS9E47B0AH2tHizjdJxrvMprG6X93w7w5I1zjJYQsOkYKzF0ZWLt5CcpkJ-',
                        production: 'AZyOmu5y4n9XGmYgARgR3KB4Slw-hDqh0uqfEKsfsASZTVAbpnTvmLKx6LTdDE6-BVq2dC85w4Xxm1k_'
                    },

                    payment: function (price) {
                        // Set up the payment here, when the buyer clicks on the button
                        //if (Page_ClientValidate()) {
                            var env = this.props.env;
                            var client = this.props.client;
                            orderAmount = 22.22;// document.getElementById('priceField').value;
                            //alert(orderAmount);
                            return paypal.rest.payment.create(env, client, {
                                transactions: [
                                    {
                                        amount: { total: orderAmount, currency: 'USD' }
                                    }
                                ]
                            });
                        //}
                        //else {
                        //    return false;
                        //}
                    },

                    commit: true,

                    onAuthorize: function (data, actions) {
                        // Execute the payment here, when the buyer approves the transaction
                        return actions.payment.execute().then(function () {
                            // Show a success page to the buyer
                            ///v1/payments/orders/order_id
                            //document.getElementById('paypalOrderId').value = data.paymentID;
                            //__doPostBack('paypalOrderId', 'ValueChanged');
                            alert('Your order has been submitted! Stay tuned for email updates.');
                        });
                    }

                }, '#paypal-button');

                
            </script>
            <div id="paypal-button">
            </div>
        </div>
    </form>
</body>
</html>
