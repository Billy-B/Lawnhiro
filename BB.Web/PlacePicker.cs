using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.Script.Serialization;

namespace BB.Web
{
    public class PlacePicker : TextBox, IPostBackEventHandler
    {
        public event EventHandler<PlaceSelectedEventArgs> PlaceSelected;

        private HiddenField _field = new HiddenField();

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            string fieldId = this.ID + "_dateField";
            _field.ID = fieldId;
            _field.ClientIDMode = ClientIDMode.Static;
            Controls.Add(_field);
            Page.ClientScript.GetPostBackEventReference(Page, string.Empty);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            foreach (Control c in Controls)
            {
                c.RenderControl(writer);
            }
        }

        public string GoogleAPIKey
        {
            get
            {
                return (string)ViewState["GoogleAPIKey"] ?? "";
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (value != GoogleAPIKey)
                {
                    ViewState["GoogleAPIKey"] = value;
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            string googleAPIScript = $"https://maps.googleapis.com/maps/api/js?key={GoogleAPIKey}&v=3.exp&sensor=false&libraries=places";
            Page.ClientScript.RegisterClientScriptInclude(googleAPIScript, googleAPIScript);

            string script = @"
                google.maps.event.addDomListener(window, 'load', function () {
                    var input = document.getElementById('" + this.ClientID + @"');
                    google.maps.event.addDomListener(input, 'keydown', function (e) {
                        if (e.keyCode == 13)
                        {
                            e.preventDefault();
                        }
                    });
                    var places = new google.maps.places.Autocomplete(input);
                    google.maps.event.addListener(places, 'place_changed', function () {
                        var place = places.getPlace();
                        var obj = new Object();
                        obj.address_components = place.address_components;
                        obj.place_id = place.place_id;
                        //alert(JSON.stringify(place));
                        document.getElementById('" + _field.ClientID + @"').value = JSON.stringify(places.getPlace()).replace(/<\/?[^>]+(>|$)/g, '');
                        __doPostBack('" + this.ClientID + @"', 'PlaceSelected');
                    });
                });";
            Page.ClientScript.RegisterClientScriptBlock(typeof(Page), new Guid().ToString(), script, true);
        }

        

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument == "PlaceSelected")
            {
                PlaceSelected?.Invoke(this, new PlaceSelectedEventArgs { SelectedPlace = this.SelectedPlace });
            }
        }

        public Place SelectedPlace
        {
            get
            {
                string data = _field.Value;
                if (string.IsNullOrWhiteSpace(data))
                {
                    return null;
                }
                else
                {
                    return Place.ParseJson(data);
                }
            }
        }
    }
}
