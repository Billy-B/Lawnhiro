<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Order.aspx.cs" Inherits="Lawnhiro.Order" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="background-color:transparent">
    <form id="form1" runat="server">
        <div>
            <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyAxJqkqEcfHvornc9l38rrrZ53iux1X2lY&v=3.exp&sensor=false&libraries=places"></script>
            <script src="https://www.paypalobjects.com/api/checkout.js"></script>
            <script src="../Scripts/jquery-3.1.1.min.js"></script>
            <script src="../Scripts/knockout-3.4.1.js"></script>

            <script type="text/javascript">
                google.maps.event.addDomListener(window, 'load', function () {
                    var places = new google.maps.places.Autocomplete(document.getElementById('txt_address'));
                    google.maps.event.addListener(places, 'place_changed', function () {

                        var place = places.getPlace();
                        var address = place.address_components[0].long_name + ' ' + place.address_components[1].long_name;
                        var city = place.address_components[3].long_name;
                        var state = place.address_components[5].short_name;
                        var zip = place.address_components[7].long_name;
                        var obj = new Object();
                        obj.Address = address;
                        obj.City = city;
                        obj.State = state;
                        obj.Zip = zip;
                        obj.PlaceId = place.place_id;
                        //alert(JSON.stringify(place.place_id));
                        document.getElementById('addressData').value = JSON.stringify(obj);
                        __doPostBack('txt_address', 'TextChanged');
                    });
                });
                function CheckBoxRequired_ClientValidate(sender, e) {
                    e.IsValid = document.getElementById('chk_agreeToTerms').checked;
                }
                paypal.Button.render(
        
            
                {

                    env: 'production', // Specify 'sandbox' for the test environment, 'production'

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

                }, '#paypal_button');
            </script>
            <%--<div style="background-color: white;">
                <img src="../Images/lawnhiro_logo.png" alt="Lawnhiro" style="margin: auto auto;" />
            </div>
            <div style="color:white">
                Ordering a Lawnhiro is easy as
                <br />
                A. Enter an address below to reveal your custom price
                <br />
                B. Add to cart
                <br />
                C. PayPal and you're done.
                <br />
                <br />
                *Every order receives:
                <br />
                1. Grass cut to 4"
                <br />
                2. Light trim around structures
                <br />
                3. Blow grass clippings off driveways and sidewalks
                <br />--%>
                <div>
                    <b>Select your address:   </b>
                    <asp:TextBox ID="txt_address" runat="server" Width="300px" ClientIDMode="Static" OnTextChanged="onAddressPicked" />
                </div>
                <asp:Label ID="label_invalidAddress" runat="server" Text="The address you have chosen is not a valid residence." ForeColor="Red" Visible="false" />
                <div id="div_orderDetails" runat="server" visible="false">
                    <asp:Label ID="label_price" runat="server" Font-Size="X-Large" />
                    <br />
                    <table>
                        <tr>
                            <td><b>Email Address:</b></td>
                            <td>
                                <asp:TextBox ID="txt_email" runat="server" TextMode="Email" MaxLength="254" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txt_email" ForeColor="Orange" Text="Required" />
                            </td>
                        </tr>
                        <tr id="div_providerCode" runat="server">
                            <td><b>Provider Code: (optional)</b></td>
                            <td><asp:TextBox ID="txt_providerCode" runat="server" MaxLength="10" /></td>
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
                    <asp:CustomValidator runat="server" EnableClientScript="true" Text="Required" ForeColor="Orange" ClientValidationFunction="CheckBoxRequired_ClientValidate" />
                    <br />
                    <div id="paypal_button">
                    </div>
                    <%--<asp:Button ID="btn_placeOrder" runat="server" Text="Place Order" OnClick="btn_placeOrder_Click" BackColor="CadetBlue" />--%>
                    <br />
                    *Price is calculated by using Zillow® home data to determine how big your lawn is.
                </div>
            </div>
            <asp:HiddenField ID="addressData" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="paypalOrderId" runat="server" ClientIDMode="Static" OnValueChanged="paypalOrderId_ValueChanged" />
            <asp:HiddenField ID="priceField" runat="server" ClientIDMode="Static" />
        </div>
    </form>
</body>
</html>
