<%@ Page Language="C#" MasterPageFile="~/Resources/Masters/GuestMaster.Master" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="Zoggr.Web.Upload" Title="Zoggr - Upload" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <table cellspacing="0" cellpadding="4" style="text-align: left;">
        <tr>
            <td class="SoftText">
                Replay XML File :<br />
                <asp:FileUpload ID="GuideFileUpload" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="UploadButton" Text="Upload File" runat="server" /><br /><br />
                <asp:Label ID="MessageLabel" CssClass="Warn" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
