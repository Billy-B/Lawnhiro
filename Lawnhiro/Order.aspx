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
            <script src="../Scripts/jquery-3.1.1.min.js"></script>

            <script type="text/javascript">
                google.maps.event.addDomListener(window, 'load', function () {
                    var input = document.getElementById('txt_address');
                    google.maps.event.addDomListener(input, 'keydown', function (e) {
                        if (e.keyCode == 13) {
                            e.preventDefault();
                        }
                    });
                    var places = new google.maps.places.Autocomplete(input);
                    google.maps.event.addListener(places, 'place_changed', function () {

                        var place = places.getPlace();
                        var address = place.address_components[0].long_name + ' ' + place.address_components[1].long_name;
                        var city = place.address_components[3].long_name;
                        var state = place.address_components[5].short_name;
                        var zip = place.address_components[7].long_name;
                        var obj = new Object();
                        obj.address_components = place.address_components;
                        obj.place_id = place.place_id;
                        //alert(JSON.stringify(place.address_components));
                        document.getElementById('addressData').value = JSON.stringify(obj);
                        __doPostBack('txt_address', 'TextChanged');
                    });
                });
            </script>
            <div>
                <b>Select your address:   </b>
                <asp:TextBox ID="txt_address" runat="server" Width="360px" ClientIDMode="Static" OnTextChanged="onAddressPicked" Font-Size="Medium" />
            </div>
            <asp:Label ID="label_invalidAddress" runat="server" ForeColor="Red" Visible="false" />
            <div id="div_orderConfirmation" runat="server" visible="false">
                <asp:Label ID="label_price" runat="server" ForeColor="Red" Font-Size="X-Large" />
                <asp:Button ID="btn_placeOrder" runat="server" Text="I'll Take it!" OnClick="btn_placeOrder_Click" />
            </div>
        </div>
        <asp:HiddenField ID="addressData" runat="server" ClientIDMode="Static" />
    </form>
</body>
</html>
