<%@ Page Language="C#" MasterPageFile="~/Resources/Masters/Center.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Zoggr.Web.Default" Title="Zoggr Shows" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:Repeater ID="ShowRepeater" runat="server">
        <HeaderTemplate>
            <table cellspacing="0" cellpadding="4" class="GridView">
                <tr class="HeaderStyle">
                    <td>
                        Show / Episode
                    </td>
                    <td>
                        Date / Time
                    </td>
                    <td>
                        Quality / Duration
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="RowStyle">
                <td>
                    <b><%# Eval("ShowTitle") %></b><br />
                    <span style="font-size: smaller;"><%# Eval("EpisodeTitle") %></span>
                </td>
                <td>
                    <%# Convert.ToDateTime(Eval("EpisodeStartTimeGMT")).ToString("MMM d, yyyy") %><br />
                    <%# Convert.ToDateTime(Eval("EpisodeStartTimeGMT")).ToShortTimeString() %>                    
                </td>
                <td>
                    <%# Eval("EpisodeQuality") %><br />
                    <%# Eval("EpisodeDuration") %> minutes
                </td>
            </tr>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <tr class="AlternatingRowStyle">
                <td>
                    <b><%# Eval("ShowTitle") %></b><br />
                    <span style="font-size: smaller;"><%# Eval("EpisodeTitle") %></span>
                </td>
                <td>
                    <%# Convert.ToDateTime(Eval("EpisodeStartTimeGMT")).ToString("MMM d, yyyy") %><br />
                    <%# Convert.ToDateTime(Eval("EpisodeStartTimeGMT")).ToShortTimeString() %>                    
                </td>
                <td>
                    <%# Eval("EpisodeQuality") %><br />
                    <%# Eval("EpisodeDuration") %> minutes
                </td>
            </tr>
        </AlternatingItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</asp:Content>

