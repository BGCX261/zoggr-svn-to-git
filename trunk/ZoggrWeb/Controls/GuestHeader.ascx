<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GuestHeader.ascx.cs" Inherits="Zoggr.Web.Controls.GuestHeader" %>

<table cellspacing="0" cellpadding="2">
    <tr>
        <td>
            <asp:HyperLink ID="ZoggrLink" NavigateUrl="~/" runat="Server">
                <asp:Image ID="ZoggrImage" ImageUrl="~/Resources/Images/ZoggrLogoLarge.gif" AlternateText="Zoggr" ToolTip="Zoggr" Width="426px" Height="202px" runat="server" />
            </asp:HyperLink>
        </td>
    </tr>
</table>
