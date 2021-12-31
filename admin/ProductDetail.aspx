<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="ProductDetail.aspx.vb" Inherits="IndiaBobbles.ProductDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">
    <asp:SqlDataSource ID="ProductDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:indiabobblesConnectionString %>" SelectCommand="SELECT [ID], [Name], [MRP], [SalePrice], [Status], [Dimension], [Color], [Weight], [Material], [Manufacturer], [CareInstructions], [RecommendedAge], [CountryofOrigin], [Fragile], [ShippingTime], [Handmade], [Inventory], [ThumbPath], [URL], [ProductCode] FROM [Product] WHERE ([ID] = @ID)">
        <SelectParameters>
            <asp:QueryStringParameter DefaultValue="0" Name="ID" QueryStringField="id" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="PhotoDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:indiabobblesConnectionString %>" SelectCommand="SELECT [ID], [ProductID], [ImagePath], [Sequence] FROM [ProductPhoto] WHERE ([ProductID] = @ProductID)">
        <SelectParameters>
            <asp:QueryStringParameter DefaultValue="0" Name="ProductID" QueryStringField="id" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <h1>Product Detail</h1>
    <asp:DetailsView ID="ProductDetailsView" CssClass="table" runat="server" AutoGenerateRows="False" DataKeyNames="ID" DataSourceID="ProductDataSource" Height="50px" Width="100%">
        <Fields>
            <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False" ReadOnly="True" SortExpression="ID" />
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
            <asp:BoundField DataField="MRP" HeaderText="MRP" SortExpression="MRP" />
            <asp:BoundField DataField="SalePrice" HeaderText="SalePrice" SortExpression="SalePrice" />
            <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
            <asp:BoundField DataField="Dimension" HeaderText="Dimension" SortExpression="Dimension" />
            <asp:BoundField DataField="Color" HeaderText="Color" SortExpression="Color" />
            <asp:BoundField DataField="Weight" HeaderText="Weight" SortExpression="Weight" />
            <asp:BoundField DataField="Material" HeaderText="Material" SortExpression="Material" />
            <asp:BoundField DataField="Manufacturer" HeaderText="Manufacturer" SortExpression="Manufacturer" />
            <asp:BoundField DataField="CareInstructions" HeaderText="CareInstructions" SortExpression="CareInstructions" />
            <asp:BoundField DataField="RecommendedAge" HeaderText="RecommendedAge" SortExpression="RecommendedAge" />
            <asp:BoundField DataField="CountryofOrigin" HeaderText="CountryofOrigin" SortExpression="CountryofOrigin" />
            <asp:CheckBoxField DataField="Fragile" HeaderText="Fragile" SortExpression="Fragile" />
            <asp:BoundField DataField="ShippingTime" HeaderText="ShippingTime" SortExpression="ShippingTime" />
            <asp:CheckBoxField DataField="Handmade" HeaderText="Handmade" SortExpression="Handmade" />
            <asp:BoundField DataField="Inventory" HeaderText="Inventory" SortExpression="Inventory" />
            <asp:BoundField DataField="URL" HeaderText="URL" SortExpression="URL" />
            <asp:BoundField DataField="ProductCode" HeaderText="ProductCode" SortExpression="ProductCode" />
            <asp:ImageField DataImageUrlField="ThumbPath" HeaderText="Thumbpath" ReadOnly="True" ControlStyle-Height="70px">
            </asp:ImageField>
        </Fields>
        <HeaderStyle Font-Bold="True" />
    </asp:DetailsView>
    <h4>Product Photos</h4>
    <asp:ListView ID="PhotoListView" runat="server" DataKeyNames="ID" DataSourceID="PhotoDataSource" GroupItemCount="100">
        <EmptyDataTemplate>
            <div runat="server" style="">
                No photos found.
            </div>
        </EmptyDataTemplate>
        <EmptyItemTemplate>
        </EmptyItemTemplate>
        <ItemTemplate>
            <div class="card" runat="server" style="max-width: 18rem;">
                <img src='<%# Eval("ImagePath") %>' class="card-img-top" alt="..." />
                <div class="card-body">
                    <h5 class="card-title">Sequence: <%# Eval("Sequence") %></h5>
                </div>
            </div>
        </ItemTemplate>
        <LayoutTemplate>
            <div>
                <div class="card-group" id="groupPlaceholder" runat="server">
                </div>
            </div>
        </LayoutTemplate>
        <GroupTemplate>
            <div class="card-group">
                <div class="card" id="itemPlaceholder" runat="server">
                </div>
            </div>
        </GroupTemplate>
    </asp:ListView>


</asp:Content>
