using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Lawnhiro
{
    public partial class PayPalTest : System.Web.UI.Page
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Request.IsSecureConnection && !Request.Url.AbsoluteUri.Contains("localhost"))
            {
                Response.Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}