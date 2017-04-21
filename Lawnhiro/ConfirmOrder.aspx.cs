using BB;
using Lawnhiro.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Lawnhiro
{
    public partial class ConfirmOrder : System.Web.UI.Page
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
            if (!IsPostBack)
            {
                Place selectedPlace = SelectedPlace;
                if (selectedPlace == null)
                {
                    Response.Redirect("~/Order.aspx");
                    return;
                }

                HeardAboutUsSource[] sources = Repository.Query<HeardAboutUsSource>().ToArray();
                ddl_heardAboutUsSource.DataSource = sources;
                ddl_heardAboutUsSource.DataBind();

                /*string placeId = Request.Params["placeId"];
                string address = Request.Params["address"];
                string city = Request.Params["city"];
                string state = Request.Params["state"];
                string zip = Request.Params["zip"];
                decimal price = decimal.Parse(Request.Params["price"]);
                MowableSqFt = decimal.Parse(Request.Params["mowableSqFt"]);*/

                priceField.Value = Price.ToString();

                label_address.Text = string.Join(" ", selectedPlace.Address, selectedPlace.City, selectedPlace.State, selectedPlace.Zip);
                label_price.Text = Price.ToString("C");

                Residence[] residences = Repository.Query<Residence>().ToArray();
                Residence existingResidence = ExistingResidence;
                if (existingResidence == null)
                {
                    div_headAboutUsSource.Visible = true;
                    div_couponCode.Visible = true;
                }
                else
                {
                    div_headAboutUsSource.Visible = false;
                    div_couponCode.Visible = existingResidence.CouponCode != null;
                }
            }
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
                if (txt_couponCode.Visible && !string.IsNullOrWhiteSpace(txt_couponCode.Text))
                {
                    residence.CouponCode = txt_couponCode.Text;
                }
                Repository.Add(residence);
            }
            residence.MowableSqFt = MowableSqFt;
            newOrder.Customer = customer;
            newOrder.Residence = residence;
            newOrder.CustomerNotes = txt_notes.Text;
            newOrder.Placed = DateTime.Now;
            newOrder.PayPalOrderId = paypalOrderId.Value;
            Repository.Add(newOrder);
            Repository.CommitChanges();
            ClientScript.RegisterStartupScript(typeof(Page), "closePage", "window.close();", true);
        }
    }
}