<%@ Page Title="Products" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="Products.aspx.vb" Inherits="IndiaBobbles.Products" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">
    <asp:SqlDataSource ID="ProductsDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:indiabobblesConnectionString %>" SelectCommand="SELECT [ID], [Name], [MRP], [SalePrice], [CreateDate], [ModifyDate], [Status], [ThumbPath], [Inventory] FROM [Product] ORDER BY [Name]"></asp:SqlDataSource>
    <a href="~/manageproduct.aspx" class="btn btn-primary mt-3 float-end">Create Product</a>
    <h1>Products</h1>
    <asp:GridView ID="ProductsGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CssClass="table " DataKeyNames="ID" DataSourceID="ProductsDataSource" PageSize="20">
        <Columns>
            <asp:ImageField DataImageUrlField="ThumbPath" HeaderText="Product Image" ReadOnly="True">
                <ControlStyle Height="80px" />
            </asp:ImageField>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
            <asp:BoundField DataField="Inventory" HeaderText="Inventory" SortExpression="Inventory" />
            <asp:BoundField DataField="MRP" HeaderText="MRP" SortExpression="MRP" DataFormatString="{0:C}" />
            <asp:BoundField DataField="SalePrice" HeaderText="Sale Price" SortExpression="SalePrice" DataFormatString="{0:C}" />
            <asp:BoundField DataField="CreateDate" DataFormatString="{0:d.MMM.yyyy}" HeaderText="Create Date" SortExpression="CreateDate" />
            <asp:BoundField DataField="ModifyDate" DataFormatString="{0:d.MMM.yyyy}" HeaderText="Modify Date" SortExpression="ModifyDate" />
            <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
            <asp:HyperLinkField DataNavigateUrlFields="ID" DataNavigateUrlFormatString="productdetail.aspx?id={0}" Text="Detail" />
        </Columns>
        <PagerSettings Position="TopAndBottom" />
        <PagerStyle CssClass="paging" HorizontalAlign="Center" />
    </asp:GridView>
</asp:Content>
