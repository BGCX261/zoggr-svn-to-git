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
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UsernameTextBox.Focus();
            }
        }

        protected void RegisterButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                UsernameTakenLabel.Visible = false;
                EmailTakenLabel.Visible = false;

                if (UserExists(UsernameTextBox.Text))
                {
                    UsernameTakenLabel.Visible = true;
                }
                else if (EmailExists(EmailTextBox.Text))
                {
                    EmailTakenLabel.Visible = true;
                }
                else
                {
                    Membership.CreateUser(UsernameTextBox.Text, PasswordTextBox.Text, EmailTextBox.Text);
                    FormsAuthentication.SetAuthCookie(UsernameTextBox.Text, false);
                    Response.Redirect("~/default.aspx");
                }
            }
        }

        #region Username and Email Checks
        protected void UsernameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (UserExists(UsernameTextBox.Text))
            {
                UsernameTakenLabel.Text = "Username unavailable.";
            }
            else
            {
                UsernameTakenLabel.Text = "&nbsp;";
            }
        }

        protected void EmailTextBox_TextChanged(object sender, EventArgs e)
        {
            if (EmailExists(EmailTextBox.Text))
            {
                EmailTakenLabel.Text = "Email already in use.";
            }
            else
            {
                EmailTakenLabel.Text = "&nbsp;";
            }
        }

        private bool UserExists(string Username)
        {
            bool _userExists = true;
            if (Membership.GetUser(Username) == null)
            {
                _userExists = false;
            }
            else
            {
                _userExists = true;
            }
            return _userExists;
        }

        private bool EmailExists(string Email)
        {
            bool _emailExists = true;
            if (Membership.GetUserNameByEmail(Email) == null)
            {
                _emailExists = false;
            }
            else
            {
                _emailExists = true;
            }
            return _emailExists;
        }
        #endregion
    }
}
