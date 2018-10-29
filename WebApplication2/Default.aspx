<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication2._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <table style="width: 100%">
        <tr>
            <td style="padding-bottom: 5px;">From Email:</td>
            <td style="padding-bottom: 5px;">
                <asp:TextBox ID="txtFromEmail" runat="server" Text="trind09@yahoo.com"></asp:TextBox></td>
        </tr>
        <tr>
            <td style="padding-bottom: 5px;">To Email:</td>
            <td style="padding-bottom: 5px;">
                <asp:TextBox ID="txtToEmail" runat="server" Text="tnguyen482@csc.com"></asp:TextBox></td>
        </tr>
        <tr>
            <td style="padding-bottom: 5px;"></td>
            <td style="padding-bottom: 5px;">
                <asp:Button ID="btnSendEmail" runat="server" Text="Send" OnClick="btnSendEmail_Click" />&nbsp;
                <asp:Button ID="btnSendSignEmail" runat="server" Text="Send Signed Email" OnClick="btnSendSignEmail_Click" />
            </td>
        </tr>
        <tr>
            <td style="padding-bottom: 5px;"></td>
            <td style="padding-bottom: 5px;">
                <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label></td>
        </tr>
    </table>

</asp:Content>
