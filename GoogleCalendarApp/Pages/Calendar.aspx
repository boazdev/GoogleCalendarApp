<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Calendar.aspx.cs" Inherits="GoogleCalendarApp.Pages.Calendar" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form runat="server">
        <asp:GridView ID="gvEvents" runat="server" AutoGenerateColumns="False" CssClass="table" GridLines="None">
            <Columns>
                <asp:BoundField DataField="Start" HeaderText="Start Time" />
                <asp:BoundField DataField="Summary" HeaderText="Event Title" />
            </Columns>
        </asp:GridView>
    </form>
</body>
</html>
