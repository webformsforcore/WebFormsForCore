<%@ Page Title="Session Test" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SessionTest.aspx.cs" Inherits="WebFormsForCore.TestApp.SessionTest" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <h2 id="title"><%: Title %></h2>
        <p>
            Exercises <code>AspNetCoreSessionProvider</code> (see <code>Web.config</code>'s
            <code>&lt;sessionState mode="Custom" customProvider="AspNetCoreSession"&gt;</code>) by
            reading/writing <code>Session</code> across postbacks and reloads.
        </p>

        <table class="table">
            <tr>
                <th>Session ID</th>
                <td><asp:Label ID="SessionIdLabel" runat="server" /></td>
            </tr>
            <tr>
                <th>Is New Session</th>
                <td><asp:Label ID="IsNewSessionLabel" runat="server" /></td>
            </tr>
            <tr>
                <th>Counter</th>
                <td><asp:Label ID="CounterLabel" runat="server" /></td>
            </tr>
            <tr>
                <th>Stored Note</th>
                <td><asp:Label ID="NoteLabel" runat="server" /></td>
            </tr>
        </table>

        <p>
            <asp:Button ID="IncrementButton" runat="server" Text="Increment Counter" OnClick="IncrementButton_Click" CssClass="btn btn-primary" />
            <asp:Button ID="ReloadButton" runat="server" Text="Reload Page" OnClick="ReloadButton_Click" CssClass="btn btn-secondary" />
        </p>

        <p>
            <asp:TextBox ID="NoteTextBox" runat="server" placeholder="Type something to store in session" />
            <asp:Button ID="SaveNoteButton" runat="server" Text="Save Note" OnClick="SaveNoteButton_Click" CssClass="btn btn-primary" />
        </p>

        <p>
            <asp:Button ID="AbandonButton" runat="server" Text="Abandon Session" OnClick="AbandonButton_Click" CssClass="btn btn-danger" />
        </p>

        <asp:Label ID="StatusLabel" runat="server" CssClass="text-success" />
    </main>
</asp:Content>
