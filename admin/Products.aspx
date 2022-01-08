<%@ Page Title="Products" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="Products.aspx.vb" Inherits="IndiaBobbles.Products" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">
    <asp:SqlDataSource ID="ProductsDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:indiabobblesConnectionString %>" SelectCommand="SELECT P.ID, P.Name, P.MRP, P.SalePrice, P.CreateDate, P.ModifyDate, 
CASE WHEN P.Status = 0 THEN 'Active' WHEN P.Status = 1 THEN 'Inactive' WHEN P.Status = 2 THEN 'Deleted' ELSE '' END AS Status, 
P.ThumbPath, P.OutofStock, STRING_AGG(CT.DisplayName,',') as Tags
FROM     CategoryTag AS CT INNER JOIN
                  ProductTag AS PT ON CT.ID = PT.TagID RIGHT OUTER JOIN
                  Product AS P ON PT.ProductID = P.ID
				  GROUP BY P.ID, P.Name, P.MRP, P.SalePrice, P.CreateDate, P.ModifyDate, P.ThumbPath, P.OutofStock, P.Status"></asp:SqlDataSource>
    <a runat="server" href="~/admin/manageproduct.aspx" class="btn btn-primary mt-3 float-end">Create Product</a>
    <h1>Products</h1>
    <asp:GridView ID="ProductsGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CssClass="table " DataKeyNames="ID" DataSourceID="ProductsDataSource" PageSize="30" EnableSortingAndPagingCallbacks="True">
        <Columns>
            <asp:ImageField DataImageUrlField="ThumbPath" HeaderText="Product Image" ReadOnly="True">
                <ControlStyle Height="80px" />
            </asp:ImageField>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
            
            <asp:CheckBoxField DataField="OutofStock" HeaderText="Out of Stock" ReadOnly="True" SortExpression="OutofStock" />
            
            <asp:BoundField DataField="MRP" HeaderText="MRP" SortExpression="MRP" DataFormatString="{0:C}" />
            <asp:BoundField DataField="SalePrice" HeaderText="Sale Price" SortExpression="SalePrice" DataFormatString="{0:C}" />
            <asp:BoundField DataField="CreateDate" DataFormatString="{0:d.MMM.yyyy}" HeaderText="Create Date" SortExpression="CreateDate" />
            <asp:BoundField DataField="ModifyDate" DataFormatString="{0:d.MMM.yyyy}" HeaderText="Modify Date" SortExpression="ModifyDate" />
            <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
            <asp:BoundField DataField="Tags" HeaderText="Tags" SortExpression="Tags" />
            <asp:HyperLinkField DataNavigateUrlFields="ID" DataNavigateUrlFormatString="productdetail.aspx?id={0}" Text="Detail" />
            <asp:HyperLinkField DataNavigateUrlFields="ID" DataNavigateUrlFormatString="manageproduct.aspx?id={0}" Text="Edit" />
        </Columns>
        <PagerSettings Position="TopAndBottom" />
        <PagerStyle CssClass="paging" HorizontalAlign="Center" />
    </asp:GridView>
</asp:Content>
