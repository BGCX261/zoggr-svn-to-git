<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="Zoggr.Web.Controls.Header" %>

<table cellspacing="0" cellpadding="0" style="width: 100%; border-bottom: 2px solid #3D107B;">
    <tr>
        <td valign="top">
            <asp:HyperLink ID="ZoggrLink" NavigateUrl="~/" runat="Server">
                <asp:Image ID="ZoggrImage" ImageUrl="~/Resources/Images/ZoggrLogo.gif" AlternateText="Zoggr" ToolTip="Zoggr" runat="server" />
            </asp:HyperLink>
            
        </td>
        <td align="right" valign="bottom" style="padding: 0px 8px 4px 0px;">
            <asp:HyperLink ID="BrowseLink" Text="Browse" NavigateUrl="#" runat="server" />
            |
            <asp:HyperLink ID="StatsLink" Text="Stats" NavigateUrl="#" runat="server" />
            |
            <asp:HyperLink ID="ProfileLink" Text="Profile" NavigateUrl="#" runat="server" />
            |
            <asp:HyperLink ID="ForumsLink" Text="Forums" NavigateUrl="#" runat="server" />
        </td>
    </tr>
</table>

