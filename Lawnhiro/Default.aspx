<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Lawnhiro.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
                <div id="paypal-button">
                </div>
        <script src="https://www.paypalobjects.com/api/checkout.js"></script>
            <script src="../Scripts/jquery-3.2.1.min.js"></script>
            <script type="text/javascript" async="async">
                paypal.Button.render(
                {
                    env: 'sandbox', // Specify 'sandbox' for the test environment, 'production'

                    client: {
                        sandbox: 'AeL5Z6IirMijkry6LzbZ8aS9E47B0AH2tHizjdJxrvMprG6X93w7w5I1zjJYQsOkYKzF0ZWLt5CcpkJ-',
                        production: 'AZyOmu5y4n9XGmYgARgR3KB4Slw-hDqh0uqfEKsfsASZTVAbpnTvmLKx6LTdDE6-BVq2dC85w4Xxm1k_'
                    },

                    payment: function (price) {
                        // Set up the payment here, when the buyer clicks on the button
                        if (true) {
                            var env = this.props.env;
                            var client = this.props.client;
                            orderAmount = 52;
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
                            //alert(JSON.stringify(data));
                            //document.getElementById('paypalOrderId').value = data.paymentID;
                            //__doPostBack('paypalOrderId', 'ValueChanged');
                            alert('Your order has been submitted! Stay tuned for email updates.');
                        });
                    }
                }, '#paypal-button');
            </script>
    </div>
    </form>
</body>
</html>
