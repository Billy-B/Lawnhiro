using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace BB.Web
{
    public class PayPalOrderEventArgs : EventArgs
    {

    }

    public class PayPalCheckout : WebControl
    {
        public event EventHandler<PayPalOrderEventArgs> OrderComplete;

        public decimal? Price
        {
            get { return (decimal?)ViewState["Price"]; }
            set
            {
                if (value != Price)
                {
                    ViewState["Price"] = value;
                }
            }
        }

        public bool Sandbox
        {
            get { return (bool?)ViewState["Sandbox"] ?? false; }
            set
            {
                if (value != Sandbox)
                {
                    ViewState["Sandbox"] = value;
                }
            }
        }

        public string ProductionKey
        {
            get { return (string)ViewState["ProductionKey"]; }
            set
            {
                if (value != ProductionKey)
                {
                    ViewState["ProductionKey"] = value;
                }
            }
        }

        public string SandboxKey
        {
            get { return (string)ViewState["SandboxKey"]; }
            set
            {
                if (value != SandboxKey)
                {
                    ViewState["SandboxKey"] = value;
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (Visible)
            {
                bool sandbox = Sandbox;

                if (sandbox)
                {

                }

                decimal? price = Price;
                if (price == null)
                {
                    throw new InvalidOperationException("Cannot render PayPalCheckout when Price property is null.");
                }
                HiddenField priceField = new HiddenField
                {
                    ID = this.ID + "_priceField",
                    Value = Price.ToString()
                };
                Controls.Add(priceField);
            }
        }
    }
}
