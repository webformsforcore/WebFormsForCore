<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AjaxTest.aspx.cs" Inherits="EstrellasDeEsperanza.WebFormsForCore.TestApp.AjaxTest" %>
<%@ Register TagPrefix="ajaxToolkit" Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <h2 id="title"><%: Title %>.</h2>
        <h3>Your application description page.</h3>
        <p>Use this area to provide additional information.</p>
        <p>
            <asp:TextBox ID="text" runat="server" />
            <ajaxToolkit:CalendarExtender runat="server"
                TargetControlID="text"
                CssClass="ClassName"
                Format="MMMM d, yyyy" />
        </p>
     </main>
</asp:Content>
