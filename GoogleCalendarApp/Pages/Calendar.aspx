<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Calendar.aspx.cs" Inherits="GoogleCalendarApp.Pages.Calendar" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Google Calendar Viewer</title>
</head>
<body>
    <form runat="server">
        <div style="margin-bottom: 10px;">
            <asp:Button ID="btnLogout" runat="server" Text="Logout" OnClick="btnLogout_Click" />
        </div>

        <div>
            <asp:Label ID="lblStartDate" runat="server" Text="Start Date:" />
            <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" />
            
            <asp:Label ID="lblViewMode" runat="server" Text="View Mode:" />
            <asp:DropDownList ID="ddlViewMode" runat="server">
                <asp:ListItem Text="Weekly" Value="week" />
                <asp:ListItem Text="Monthly" Value="month" Selected="True"/>
            </asp:DropDownList>

            <asp:Button ID="btnFilter" runat="server" Text="Apply" OnClick="btnFilter_Click" />
        </div>

        <asp:GridView ID="gvEvents" runat="server" AutoGenerateColumns="False" CssClass="table" GridLines="None">
            <Columns>
                <asp:BoundField DataField="Start" HeaderText="Start Time" />
                <asp:BoundField DataField="Summary" HeaderText="Event Title" />
            </Columns>
        </asp:GridView>
    </form>
</body>
</html>
