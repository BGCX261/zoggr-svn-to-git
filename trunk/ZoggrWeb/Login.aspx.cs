using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

namespace Zoggr.Web
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UsernameTextBox.Focus();
            }
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (Membership.ValidateUser(UsernameTextBox.Text, PasswordTextBox.Text))
                {
                    FormsAuthentication.RedirectFromLoginPage(UsernameTextBox.Text, RememberMeCheckBox.Checked);
                }
                else
                {
                    MessageLabel.Text = "Invalid login.";
                }
            }
        }
    }
}
