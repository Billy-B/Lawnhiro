using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace BB.Web
{
    public class LocalTime : WebControl
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        private HiddenField _field = new HiddenField();

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            string key = new Guid().ToString();
            string fieldId = this.ID + "_dateField";
            _field.ID = fieldId;
            _field.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            this.Controls.Add(_field);
            string script =
                @"var date = new Date();
                var day = date.getDate();
                var month = date.getMonth() + 1;
                var year = date.getFullYear();
                var hour = date.getHours();
                var minute = date.getMinutes();
                var second = date.getSeconds();
                var timezone = date.getTimezoneOffset();
                var time = month + '/' + day + '/' + year + ' ' + hour + ':' + minute + ':' + second + '|' + timezone; 
                document.getElementById('" + fieldId + "').value = time;";
            Page.ClientScript.RegisterOnSubmitStatement(Page.GetType(), key, script);
        }

        public DateTime BrowserLocalTime
        {
            get
            {
                string value = _field.Value;
                if (string.IsNullOrEmpty(value))
                {
                    throw new InvalidOperationException("No submit behavior has ocurred.");
                }
                string[] split = value.Split('|');
                return DateTime.Parse(split[0]);
            }
        }

        public DateTimeOffset BrowserLocalTimeOffset
        {
            get
            {
                string value = _field.Value;
                if (string.IsNullOrEmpty(value))
                {
                    throw new InvalidOperationException("No submit behavior has ocurred.");
                }
                string[] split = value.Split('|');
                TimeSpan timezoneOffset = TimeSpan.FromMinutes(int.Parse(split[1]));
                return new DateTimeOffset(DateTime.Parse(split[0]), timezoneOffset);
            }
        }
    }
}
