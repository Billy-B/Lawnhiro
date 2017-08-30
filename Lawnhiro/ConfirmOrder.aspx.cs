using BB;
using Lawnhiro.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Lawnhiro
{
    public partial class ConfirmOrder : System.Web.UI.Page
    {
        private API.Order CurrentOrder
        {
            get { return (API.Order)Session["CurrentOrder"]; }
            set { Session["CurrentOrder"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.GetPostBackEventReference(this, string.Empty);
            if (!IsPostBack)
            {
                var queryString = Request.QueryString;
                string address = queryString["address"];
                string city = queryString["city"];
                string state = queryString["state"];
                string zip = queryString["zip"];
                string placeId = queryString["placeId"];

                if (address == null
                    || city == null
                    || placeId == null
                    || state == null
                    || zip == null)
                {
                    Response.Redirect("~/Order.aspx");
                    return;
                }

                ServiceArea serviceArea = Repository.Query<ServiceArea>().FirstOrDefault(r => r.City == city && r.State == state && r.Active);
                if (serviceArea == null)
                {
                    Response.Redirect("~/Order.aspx");
                    return;
                }

                Residence residence = Repository.Query<Residence>().FirstOrDefault(r => r.GooglePlaceId == placeId);
                bool isNewResidence = residence == null;

                if (isNewResidence)
                {
                    ZillowResidenceInfo residenceInfo = ZillowResidenceInfo.GetResidenceInfo(address, city, state, zip);
                    if (residenceInfo == null)
                    {
                        Response.Redirect("~/Order.aspx");
                        return;
                    }
                    decimal mowableSqFt = PriceCalculator.CalculateMowableSqFt(residenceInfo);
                    residence = new Residence
                    {
                        Address = address,
                        City = city,
                        GooglePlaceId = placeId,
                        State = state,
                        Zip = zip,
                        MowableSqFt = mowableSqFt
                    };
                    Repository.Add(residence);
                }

                decimal price = residence.PriceOverride ?? PriceCalculator.CalculatePrice(serviceArea, residence.MowableSqFt);

                API.Order order = new API.Order
                {
                    Price = price,
                    Residence = residence
                };

                string couponCode = queryString["couponCode"];
                if (string.IsNullOrWhiteSpace(couponCode))
                {
                    label_couponCode.Text = "No coupon code provided.";
                }
                else
                {
                    Coupon coupon = Repository.Query<Coupon>().Where(c => c.Code == couponCode).SingleOrDefault();
                    if (coupon == null)
                    {
                        label_couponCode.Text = "Coupon code " + couponCode + " is invalid.";
                    }
                    else
                    {
                        bool codeHasBeenRedeemedByResidence;
                        if (isNewResidence)
                        {
                            codeHasBeenRedeemedByResidence = false;
                        }
                        else
                        {
                            codeHasBeenRedeemedByResidence = Repository.Query<API.Order>().Any(o => o.Residence == residence && o.Coupon == coupon);
                        }
                        if (codeHasBeenRedeemedByResidence)
                        {
                            label_couponCode.Text = $"Sorry, coupon code {couponCode} has already been redeemed at this address.";
                        }
                        else
                        {
                            order.Coupon = coupon;
                            price = price - coupon.Discount;
                            label_couponCode.Text = $"Redeeming coupon code {couponCode} saved you {coupon.Discount.ToString("C")}!";
                        }
                    }
                }

                HeardAboutUsSource[] sources = Repository.Query<HeardAboutUsSource>().ToArray();
                ddl_heardAboutUsSource.DataSource = sources;
                ddl_heardAboutUsSource.DataBind();

                priceField.Value = price.ToString("0.##");

                label_address.Text = string.Join(" ", address, city, state, zip);
                label_price.Text = price.ToString("C");

                div_headAboutUsSource.Visible = isNewResidence;

                CurrentOrder = order;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            this.Page.ClientScript.GetPostBackEventReference(paypalOrderId, string.Empty);
        }

        protected void paypalOrderId_ValueChanged(object sender, EventArgs e)
        {
            Lawnhiro.API.Order order = CurrentOrder;
            string email = txt_email.Text.ToLower();
            Customer customer = Repository.Query<Customer>().FirstOrDefault(c => c.Email == email);
            if (customer == null)
            {
                customer = new Customer();
                customer.Email = email;
                Repository.Add(customer);
            }
            order.Customer = customer;
            order.CustomerNotes = txt_notes.Text;
            order.Placed = localTime.BrowserLocalTimeOffset;
            order.PayPalOrderId = paypalOrderId.Value;
            Repository.Add(order);
            Repository.CommitChanges();
            ClientScript.RegisterStartupScript(typeof(Page), "closePage", "window.close();", true);

            var fromAddress = new MailAddress("order.lawnhiro@gmail.com", "Lawnhiro Order Status");
            var toAddress = new MailAddress(customer.Email);
            const string fromPassword = "lawnhiro1";
            const string subject = "Your Lawnhiro Order Has Been Submitted!";

            MailMessage message = new MailMessage();
            message.From = fromAddress;
            message.To.Add(toAddress);
#if !DEBUG
            message.CC.Add("info@lawnhiro.com");
#endif
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = "This is an automatic notification to inform you your Lawnhiro order has been submitted. Stay tuned for more updates!";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            smtp.Send(message);

#if !DEBUG
            Provider[] providersToNotify = Repository.Query<Provider>().Where(p => p.City == order.Residence.City && p.State == order.Residence.State && p.NotificationsEnabled).ToArray();
            MailMessage noticicationMessage = new MailMessage
            {
                From = fromAddress,
                Subject = "Lawnhiro - order recieved",
                Body = $"This is an automatic notification. An order has been received from {order.Residence.Address}."
            };
            foreach (Provider provider in providersToNotify)
            {
                noticicationMessage.To.Add(provider.Email);
            }
            smtp.Send(noticicationMessage);
#endif
        }
    }
}