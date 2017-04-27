using BB;
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
        private Residence ExistingResidence
        {
            get { return (Residence)Session["existingResidence"]; }
            set { Session["existingResidence"] = value; }
        }

        private Place SelectedPlace
        {
            get { return (Place)Session["selectedPlace"]; }
            set { Session["selectedPlace"] = value; }
        }

        private decimal MowableSqFt
        {
            get { return (decimal)Session["MowableSqFt"]; }
            set { Session["MowableSqFt"] = value; }
        }

        private decimal Price
        {
            get { return (decimal)Session["Price"]; }
            set { Session["Price"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.GetPostBackEventReference(this, string.Empty);
        }

        class GooglePlace
        {
            public string place_id;
            public addressComponent[] address_components;
        }

        class addressComponent
        {
            public string long_name, short_name;
            public string[] types;
        }

        protected void onAddressPicked(object sender, EventArgs e)
        {
            string json = addressData.Value;
            var serializer = new JavaScriptSerializer();
            GooglePlace googlePlace = serializer.Deserialize<GooglePlace>(json);
            addressComponent[] components = googlePlace.address_components;
            addressComponent streetNumber = components.FirstOrDefault(c => c.types.Contains("street_number"));
            addressComponent route = components.FirstOrDefault(c => c.types.Contains("route"));
            addressComponent locality = components.FirstOrDefault(c => c.types.Contains("locality"));
            addressComponent state = components.FirstOrDefault(c => c.types.Contains("administrative_area_level_1"));
            addressComponent postalCode = components.FirstOrDefault(c => c.types.Contains("postal_code"));
            ZillowResidenceInfo residenceInfo;
            Place place;
            if (streetNumber == null || route == null || locality == null || state == null || postalCode == null)
            {
                residenceInfo = null;
                place = null;
            }
            else
            {
                place = new Place
                {
                    Address = streetNumber.long_name + " " + route.long_name,
                    City = locality.long_name,
                    State = state.short_name,
                    Zip = postalCode.long_name,
                    PlaceId = googlePlace.place_id
                };
                string address = streetNumber.long_name + " " + route.long_name;
                residenceInfo = ZillowResidenceInfo.GetResidenceInfo(place.Address, place.City, place.State, place.Zip);
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
                ServiceArea selectedArea = areas.FirstOrDefault(a => a.City == place.City && a.State == place.State && a.StartDate <= DateTime.Today);
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
                    MowableSqFt = mowableSqFt;
                    Price = price;
                    ExistingResidence = existingResidence;
                    SelectedPlace = place;
                    NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

                    queryString["address"] = place.Address;
                    queryString["city"] = place.City;
                    queryString["state"] = place.State;
                    queryString["zip"] = place.Zip;
                    queryString["placeId"] = place.PlaceId;
                    btn_placeOrder.OnClientClick = $"javascript:window.open('ConfirmOrder.aspx?{queryString}');";
                    label_price.Text = "Your lawn is only " + price.ToString("C") + "!";
                }
            }
        }

        private void clearPage()
        {
            div_orderConfirmation.Visible = false;
            txt_address.Text = "";
        }

        protected void btn_placeOrder_Click(object sender, EventArgs e)
        {
            /*Place selectedPlace = SelectedPlace;
            //string redirect = 
            Lawnhiro.API.Order newOrder = new Lawnhiro.API.Order();
            newOrder.Price = Price;
            string email = txt_email.Text.ToLower();
            Customer[] allCustomers = Repository.Query<Customer>().ToArray();
            Customer customer = allCustomers.SingleOrDefault(c => c.Email == email);
            if (customer == null)
            {
                customer = new Customer();
                customer.Email = email;
                Repository.Add(customer);
            }
            Residence residence = ExistingResidence;
            if (residence == null)
            {
                Place selectedPlace = SelectedPlace;
                residence = new Residence();
                residence.Address = selectedPlace.Address;
                residence.City = selectedPlace.City;
                residence.GooglePlaceId = selectedPlace.PlaceId;
                residence.State = selectedPlace.State;
                residence.Zip = selectedPlace.Zip;
                HeardAboutUsSource selectedSource = Repository.Query<HeardAboutUsSource>().ToArray().First(s => s.Name == ddl_heardAboutUsSource.SelectedValue);
                residence.Source = selectedSource;
                residence.CouponCode = txt_couponCode.Text;
                Repository.Add(residence);
            }
            residence.MowableSqFt = MowableSqFt;
            newOrder.Customer = customer;
            newOrder.Residence = residence;
            newOrder.CustomerNotes = txt_notes.Text;
            newOrder.Price = Price;
            newOrder.Placed = DateTime.Now;
            newOrder.PayPalOrderId = "TESTTESTTEST";
            
            Repository.Add(newOrder);
            Repository.CommitChanges();*/
            clearPage();
        }

        /*protected void paypalOrderId_ValueChanged(object sender, EventArgs e)
        {
            Lawnhiro.API.Order newOrder = new Lawnhiro.API.Order();
            newOrder.Price = Price;
            string email = txt_email.Text.ToLower();
            Customer[] allCustomers = Repository.Query<Customer>().ToArray();
            Customer customer = allCustomers.SingleOrDefault(c => c.Email == email);
            if (customer == null)
            {
                customer = new Customer();
                customer.Email = email;
                Repository.Add(customer);
            }
            Residence residence = ExistingResidence;
            if (residence == null)
            {
                Place selectedPlace = SelectedPlace;
                residence = new Residence();
                residence.Address = selectedPlace.Address;
                residence.City = selectedPlace.City;
                residence.GooglePlaceId = selectedPlace.PlaceId;
                residence.State = selectedPlace.State;
                residence.Zip = selectedPlace.Zip;
                HeardAboutUsSource selectedSource = Repository.Query<HeardAboutUsSource>().ToArray().First(s => s.Name == ddl_heardAboutUsSource.SelectedValue);
                residence.Source = selectedSource;
                if(txt_couponCode.Visible && !string.IsNullOrWhiteSpace(txt_couponCode.Text))
                {
                    residence.CouponCode = txt_couponCode.Text;
                }
                Repository.Add(residence);
            }
            newOrder.Customer = customer;
            newOrder.Residence = residence;
            newOrder.CustomerNotes = txt_notes.Text;
            newOrder.Price = Price;
            newOrder.Placed = DateTime.Now;
            newOrder.PayPalOrderId = paypalOrderId.Value;
            Repository.Add(newOrder);
            Repository.CommitChanges();
            clearPage();
        }*/

        
    }
}