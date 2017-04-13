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

        class Place
        {
            public string PlaceId, Address, City, State, Zip;
        }

        protected void onAddressPicked(object sender, EventArgs e)
        {
            string json = addressData.Value;
            var serializer = new JavaScriptSerializer();
            Place place = serializer.Deserialize<Place>(json);
            SelectedPlace = place;
            ZillowResidenceInfo residenceInfo = ZillowResidenceInfo.GetResidenceInfo(place.Address, place.City, place.State, place.Zip);
            if (residenceInfo == null)
            {
                label_invalidAddress.Visible = true;
                div_orderDetails.Visible = false;
            }
            else
            {
                label_invalidAddress.Visible = false;
                div_orderDetails.Visible = true;
                decimal price = ZillowResidenceInfo.CalculatePrice(residenceInfo);
                priceField.Value = price.ToString();
                Price = price;
                label_price.Text = "Your lawn is only " + price.ToString("C") + "!";
                SelectedResidenceInfo = residenceInfo;
                Residence[] allResidences = Repository.Query<Residence>().ToArray();
                Residence existing = allResidences.SingleOrDefault(r => r.GooglePlaceId == place.PlaceId);
                ExistingResidence = existing;
                bool isNewResidence = existing == null;
                div_headAboutUsSource.Visible = isNewResidence;
                div_providerCode.Visible = isNewResidence;
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
    }
}