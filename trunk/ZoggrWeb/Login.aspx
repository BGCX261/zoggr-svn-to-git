<%@ Page Language="C#" MasterPageFile="~/Resources/Masters/GuestMaster.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Zoggr.Web.Login" Title="Zoggr - Login" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <table cellspacing="0" cellpadding="4" style="text-align: left;">
        <tr>
            <td class="SoftText">
                Username (or Email):<br />
                <asp:TextBox ID="UsernameTextBox" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="SoftText">
                Password:<br />
                <asp:TextBox ID="PasswordTextBox" TextMode="Password" runat="server" /><br />
                <asp:CheckBox ID="RememberMeCheckBox" Text="Remember Me" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="LoginButton" Text="Login" OnClick="LoginButton_Click" runat="server" /><br /><br />
                <asp:Label ID="MessageLabel" CssClass="Warn" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:HyperLink ID="RegisterLink" Text="Register" NavigateUrl="~/register.aspx" runat="server" />
                |
                <asp:HyperLink ID="ForgotPasswordLink" Text="Forgot Password" NavigateUrl="~/forgotpassword.aspx" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
