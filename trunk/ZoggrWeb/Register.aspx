<%@ Page Language="C#" MasterPageFile="~/Resources/Masters/GuestMaster.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="Zoggr.Web.Register" Title="Zoggr - Register" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <table cellspacing="0" cellpadding="2" style="text-align: left;">
        <tr>
            <td class="SoftText">
                Username:<br />
                <asp:TextBox ID="UsernameTextBox" MaxLength="25" AutoPostBack="True" OnTextChanged="UsernameTextBox_TextChanged" runat="server" /><br />
                <asp:RegularExpressionValidator ID="UsernameValid" ControlToValidate="UsernameTextBox" ValidationExpression="[A-Za-z0-9_]*" ErrorMessage="Invalid characters.<br />" Display="None" runat="server" />
                <asp:RequiredFieldValidator ID="UsernameRequired" ControlToValidate="UsernameTextBox" ErrorMessage="Username is required.<br />" Display="None" runat="server" />
                <asp:UpdatePanel ID="UpdateUsernamePanel" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="UsernameTakenLabel" Text="&nbsp;" CssClass="Warn" runat="server" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="UsernameTextBox" EventName="TextChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="SoftText">
                Email:<br />
                <asp:TextBox ID="EmailTextBox" MaxLength="100" AutoPostback="True" OnTextChanged="EmailTextBox_TextChanged" runat="server" /><br />
                <asp:RequiredFieldValidator ID="EmailRequired" ControlToValidate="EmailTextBox" ErrorMessage="Email is required.<br />" Display="None" runat="server" />
                <asp:UpdatePanel ID="UpdateEmailPanel" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="EmailTakenLabel" Text="&nbsp;" CssClass="Warn" runat="server" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="EmailTextbox" EventName="TextChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="SoftText">
                Password:<br />
                <asp:TextBox ID="PasswordTextBox" TextMode="Password" MaxLength="16" runat="server" /><br />
                <asp:RequiredFieldValidator ID="PasswordRequired" ControlToValidate="PasswordTextBox" ErrorMessage="Password required.<br />" Display="None" runat="server" />
                <asp:CompareValidator ID="ComparePasswordValidator" ControlToValidate="ConfirmPasswordTextBox" ControlToCompare="PasswordTextBox" Type="String" Operator="Equal" ErrorMessage="Passwords do not match.<br />" Display="None" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="SoftText">
                <br />
                Confirm Password:<br />
                <asp:TextBox ID="ConfirmPasswordTextBox" TextMode="Password" MaxLength="16" runat="server" /><br />
                <asp:RequiredFieldValidator ID="ConfirmPasswordRequired" ControlToValidate="ConfirmPasswordTextBox" ErrorMessage="Confirm password required.<br />" Display="None" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:Button ID="RegisterButton" Text="Register" OnClick="RegisterButton_Click" runat="server" /><br />
            </td>
        </tr>
    </table>

    <table cellspacing="0" cellpadding="0" style="text-align: left;">
        <tr>
            <td>
                <br />
                <asp:ValidationSummary ID="ValidationSummary1" DisplayMode="BulletList" CssClass="Warn" ForeColor="" runat="server" />
            </td>
        </tr>
    </table>

    <br /><br /><br />
    <center>
        <asp:HyperLink ID="LoginLink" Text="Return To Login" NavigateUrl="~/login.aspx" runat="server" />
    </center>
</asp:Content>
