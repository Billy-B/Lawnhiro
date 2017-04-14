using BB;
using Lawnhiro.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Lawnhiro
{
    public partial class Order : System.Web.UI.Page
    {
        const decimal BASE_PRICE = 25;
        const decimal PRICE_PER_SQ_FT = 0.0023M;

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.GetPostBackEventReference(this, string.Empty);
            if (!IsPostBack)
            {
                HeardAboutUsSource[] sources = Repository.Query<HeardAboutUsSource>().ToArray();
                ddl_heardAboutUsSource.DataSource = sources;
                ddl_heardAboutUsSource.DataBind();
            }
        }

        private ZillowResidenceInfo SelectedResidenceInfo
        {
            get { return (ZillowResidenceInfo)Session["selectedResidenceInfo"]; }
            set { Session["selectedResidenceInfo"] = value; }
        }

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

        private decimal Price
        {
            get { return (decimal)Session["Price"]; }
            set { Session["Price"] = value; }
        }

        private decimal MowableSqFt
        {
            get { return (decimal)Session["MowableSqFt"]; }
            set { Session["MowableSqFt"] = value; }
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

        class Place
        {
            public string PlaceId, Address, City, State, Zip;
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
                SelectedPlace = place;
                string address = streetNumber.long_name + " " + route.long_name;
                residenceInfo = ZillowResidenceInfo.GetResidenceInfo(place.Address, place.City, place.State, place.Zip);
            }
            if (residenceInfo == null)
            {
                label_invalidAddress.Text = "The address you have chosen is not a valid residence.";
                label_invalidAddress.Visible = true;
                div_orderDetails.Visible = false;
            }
            else
            {
                ServiceArea[] areas = Repository.Query<ServiceArea>().ToArray();
                ServiceArea selectedArea = areas.FirstOrDefault(a => a.City == place.City && a.State == place.State);
                if (selectedArea == null)
                {
                    label_invalidAddress.Text = "It looks like we haven't expanded to your city yet.  Sign up for updates to be the first to know where we go next!";
                    label_invalidAddress.Visible = true;
                    div_orderDetails.Visible = false;
                }
                else
                {
                    label_invalidAddress.Visible = false;
                    div_orderDetails.Visible = true;
                    decimal mowableSqFt = CalculateMowableSqFt(residenceInfo);
                    decimal price = CalculatePrice(mowableSqFt);
                    priceField.Value = price.ToString();
                    Price = price;
                    MowableSqFt = mowableSqFt;
                    label_price.Text = "Your lawn is only " + price.ToString("C") + "!";
                    SelectedResidenceInfo = residenceInfo;
                    Residence[] allResidences = Repository.Query<Residence>().ToArray();
                    Residence existing = allResidences.SingleOrDefault(r => r.GooglePlaceId == googlePlace.place_id);
                    ExistingResidence = existing;
                    bool isNewResidence = existing == null;
                    div_headAboutUsSource.Visible = isNewResidence;
                    div_providerCode.Visible = isNewResidence;
                }
            }
        }

        private void clearPage()
        {
            div_orderDetails.Visible = false;
            txt_address.Text = "";
        }

        protected void btn_placeOrder_Click(object sender, EventArgs e)
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
                residence.ProviderCode = txt_providerCode.Text;
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
            Repository.CommitChanges();
            clearPage();
        }

        protected void paypalOrderId_ValueChanged(object sender, EventArgs e)
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
                residence.ProviderCode = txt_providerCode.Text;
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
        }

        public static decimal CalculateMowableSqFt(ZillowResidenceInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            decimal estimatedFootprint;
            decimal lotSqFt;
            if (info.LotSizeSqFt == null)
            {
                lotSqFt = info.FinishedSqFt * 3;
            }
            else
            {
                lotSqFt = (decimal)info.LotSizeSqFt;
            }
            if (info.NumberOfFloors == null) // actual number of floors not available, must estimate.
            {
                estimatedFootprint = Math.Min(info.FinishedSqFt, lotSqFt / 2);
            }
            else
            {
                estimatedFootprint = (info.FinishedSqFt / (decimal)info.NumberOfFloors);
            }
            return lotSqFt - estimatedFootprint;
        }

        public static decimal CalculatePrice(decimal mowableSqFt)
        {
            decimal ret = Math.Floor(BASE_PRICE + (PRICE_PER_SQ_FT * mowableSqFt));
            return ret;
        }
    }
}