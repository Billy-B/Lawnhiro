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
                var queryString = Request.QueryString;
                Place selectedPlace = new Place
                {
                    Address = queryString["address"],
                    City = queryString["city"],
                    State = queryString["state"],
                    Zip = queryString["zip"],
                    PlaceId = queryString["placeId"]
                };
                if (selectedPlace.Address == null
                    || selectedPlace.City == null
                    || selectedPlace.PlaceId == null
                    || selectedPlace.State == null
                    || selectedPlace.Zip == null)
                {
                    Response.Redirect("~/Order.aspx");
                    return;
                }
                decimal price, mowableSqFt;

                Residence existingResidence = Repository.Query<Residence>().ToArray().FirstOrDefault(r => r.GooglePlaceId == selectedPlace.PlaceId);

                if (existingResidence != null && existingResidence.PriceOverride.HasValue)
                {
                    price = existingResidence.PriceOverride.Value;
                    mowableSqFt = existingResidence.MowableSqFt;
                }
                else
                {
                    ZillowResidenceInfo residenceInfo = ZillowResidenceInfo.GetResidenceInfo(selectedPlace.Address, selectedPlace.City, selectedPlace.State, selectedPlace.Zip);
                    if (residenceInfo == null)
                    {
                        Response.Redirect("~/Order.aspx");
                        return;
                    }
                    mowableSqFt = PriceCalculator.CalculateMowableSqFt(residenceInfo);
                    ServiceArea serviceArea = Repository.Query<ServiceArea>().ToArray().FirstOrDefault(r => r.City == selectedPlace.City && r.State == selectedPlace.State);
                    if (serviceArea == null)
                    {
                        Response.Redirect("~/Order.aspx");
                        return;
                    }
                    price = PriceCalculator.CalculatePrice(serviceArea, mowableSqFt);
                }

                HeardAboutUsSource[] sources = Repository.Query<HeardAboutUsSource>().ToArray();
                ddl_heardAboutUsSource.DataSource = sources;
                ddl_heardAboutUsSource.DataBind();

                priceField.Value = price.ToString();

                label_address.Text = string.Join(" ", selectedPlace.Address, selectedPlace.City, selectedPlace.State, selectedPlace.Zip);
                label_price.Text = price.ToString("C");

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
                ExistingResidence = existingResidence;
                Price = price;
                MowableSqFt = mowableSqFt;
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
            newOrder.Placed = localTime.BrowserLocalTimeOffset;
            newOrder.PayPalOrderId = paypalOrderId.Value;
            Repository.Add(newOrder);
            Repository.CommitChanges();
            ClientScript.RegisterStartupScript(typeof(Page), "closePage", "window.close();", true);
        }
    }
}