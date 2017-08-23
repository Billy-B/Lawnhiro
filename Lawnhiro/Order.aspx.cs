using BB;
using BB.Web;
using Lawnhiro.API;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Lawnhiro
{
    public partial class Order : System.Web.UI.Page
    {

        private decimal Price
        {
            get { return (decimal)Session["Price"]; }
            set { Session["Price"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.GetPostBackEventReference(this, string.Empty);
        }

        protected void onAddressPicked(object sender, PlaceSelectedEventArgs e)
        {
            Place place = e.SelectedPlace;
            ZillowResidenceInfo residenceInfo;
            if (place.StreetNumber == null || place.StreetName == null || place.City == null || place.StateProvinceAbbreviation == null || place.PostalCode == null)
            {
                residenceInfo = null;
            }
            else
            {
                string address = place.StreetNumber + " " + place.StreetName;
                residenceInfo = ZillowResidenceInfo.GetResidenceInfo(address, place.City, place.StateProvinceAbbreviation, place.PostalCode);
            }
            if (residenceInfo == null)
            {
                label_invalidAddress.Text = "The address you have chosen is not a valid residence.";
                label_invalidAddress.Visible = true;
                div_orderConfirmation.Visible = false;
            }
            else
            {
                ServiceArea[] areas = Repository.Query<ServiceArea>().ToArray();
                ServiceArea selectedArea = areas.FirstOrDefault(a => a.City == place.City && a.State == place.StateProvinceAbbreviation && a.Active);
                if (selectedArea == null)
                {
                    label_invalidAddress.Text = "It looks like we haven't expanded to your city yet.  Sign up for updates to be the first to know where we go next!";
                    label_invalidAddress.Visible = true;
                    div_orderConfirmation.Visible = false;
                }
                else
                {
                    label_invalidAddress.Visible = false;
                    div_orderConfirmation.Visible = true;
                    decimal mowableSqFt = PriceCalculator.CalculateMowableSqFt(residenceInfo);
                    decimal price;
                    Residence existingResidence = Repository.Query<Residence>().ToArray().FirstOrDefault(r => r.GooglePlaceId == place.PlaceId);
                    if (existingResidence != null && existingResidence.PriceOverride != null)
                    {
                        price = existingResidence.PriceOverride.Value;
                    }
                    else
                    {
                        price = PriceCalculator.CalculatePrice(selectedArea, mowableSqFt);
                    }

                    Price = price;
                    label_price.Text = $"Your lawn is only {price.ToString("C")}!";

                    NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);

                    queryString["address"] = place.StreetNumber + " " + place.StreetName;
                    queryString["city"] = place.City;
                    queryString["state"] = place.StateProvinceAbbreviation;
                    queryString["zip"] = place.PostalCode;
                    queryString["placeId"] = place.PlaceId;

                    string script = $"javascript:window.open('ConfirmOrder.aspx?{queryString}&couponCode=' + document.getElementById('{txt_couponCode.ClientID}').value);";
                    btn_placeOrder.OnClientClick = script;
                }
            }
        }

        private void clearPage()
        {
            div_orderConfirmation.Visible = false;
            addressPicker.Text = "";
            txt_couponCode.Text = "";
        }

        protected void btn_placeOrder_Click(object sender, EventArgs e)
        {
            /*NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
            Place place = addressPicker.SelectedPlace;
            queryString["address"] = place.StreetNumber + " " + place.StreetName;
            queryString["city"] = place.City;
            queryString["state"] = place.StateProvinceAbbreviation;
            queryString["zip"] = place.PostalCode;
            queryString["placeId"] = place.PlaceId;

            string couponCode = txt_couponCode.Text;

            if (!string.IsNullOrWhiteSpace(couponCode))
            {
                queryString["couponCode"] = couponCode;
            }
            string script = $"javascript:window.open('ConfirmOrder.aspx?{queryString}');";
            ClientScript.RegisterClientScriptBlock(typeof(Page), new Guid().ToString(), script, true);*/
            clearPage();
        }
    }
}