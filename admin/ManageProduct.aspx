<%@ Page Title="Manage Product" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="ManageProduct.aspx.vb" Inherits="IndiaBobbles.ManageProduct" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">
    <h1>
        <asp:Literal ID="HeadingLiteral" runat="server">Add Product</asp:Literal></h1>
    <div class="p-2">
        <label for="NameTextBox" class="form-label">Name</label>
        <asp:TextBox ID="NameTextBox" ClientIDMode="Static" MaxLength="300" CssClass="form-control" runat="server"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required" ControlToValidate="NameTextBox"></asp:RequiredFieldValidator>
    </div>
    <div class="p-2">
        <label for="URLTextBox" class="form-label">URL</label>
        <asp:TextBox ID="URLTextBox" ClientIDMode="Static" MaxLength="300" CssClass="form-control" runat="server"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="Required" ControlToValidate="URLTextBox"></asp:RequiredFieldValidator>
    </div>
    <div class="p-2">
        <label for="DescTextBox" class="form-label">Description</label>
        <asp:TextBox ID="DescTextBox" Rows="7" TextMode="MultiLine" CssClass="form-control" runat="server"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Required" ControlToValidate="DescTextBox"></asp:RequiredFieldValidator>
    </div>
    <div class="p-2">
        <label for="MRPTextBox" class="form-label">MRP</label>
        <asp:TextBox ID="MRPTextBox" TextMode="Number" ClientIDMode="Static" CssClass="form-control" runat="server"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Required" ControlToValidate="MRPTextBox"></asp:RequiredFieldValidator>
    </div>
    <div class="p-2">
        <label for="SaleTextBox" class="form-label">Sale Price</label>
        <asp:TextBox ID="SaleTextBox" TextMode="Number" ClientIDMode="Static" CssClass="form-control" runat="server"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Required" ControlToValidate="SaleTextBox"></asp:RequiredFieldValidator>
    </div>
    <div class="p-2">
        <label for="DimensionTextBox" class="form-label">Dimension</label>
        <asp:TextBox ID="DimensionTextBox" ClientIDMode="Static" MaxLength="50" CssClass="form-control" runat="server"></asp:TextBox>
    </div>
    <div class="p-2">
        <label for="ColorTextBox" class="form-label">Color</label>
        <asp:TextBox ID="ColorTextBox" ClientIDMode="Static" MaxLength="50" CssClass="form-control" runat="server"></asp:TextBox>
    </div>
    <div class="p-2">
        <label for="WeightTextBox" class="form-label">Weight</label>
        <asp:TextBox ID="WeightTextBox" ClientIDMode="Static" MaxLength="50" CssClass="form-control" runat="server"></asp:TextBox>
    </div>
    <div class="p-2">
        <label for="MaterialTextBox" class="form-label">Material</label>
        <asp:TextBox ID="MaterialTextBox" ClientIDMode="Static" MaxLength="50" CssClass="form-control" runat="server"></asp:TextBox>
    </div>
    <div class="p-2">
        <label for="ManufacturerTextBox" class="form-label">Manufacturer</label>
        <asp:TextBox ID="ManufacturerTextBox" ClientIDMode="Static" MaxLength="50" CssClass="form-control" runat="server"></asp:TextBox>
    </div>
    <div class="p-2">
        <label for="CareTextBox" class="form-label">Care Instructions</label>
        <asp:TextBox ID="CareTextBox" ClientIDMode="Static" MaxLength="1000" CssClass="form-control" runat="server"></asp:TextBox>
    </div>
    <div class="p-2">
        <label for="RecommendAgeTextBox" class="form-label">Recommended Age</label>
        <asp:TextBox ID="RecommendAgeTextBox" ClientIDMode="Static" MaxLength="50" CssClass="form-control" runat="server"></asp:TextBox>
    </div>
    <div class="p-2">
        <label for="CountryOriginTextBox" class="form-label">Country of Origin</label>
        <asp:TextBox ID="CountryOriginTextBox" ClientIDMode="Static" MaxLength="100" CssClass="form-control" runat="server"></asp:TextBox>
    </div>
    <div class="p-2">
        <asp:CheckBox ID="FragileCheckBox" Text="Fragile" TextAlign="Right" runat="server" />
    </div>
    <div class="p-2">
        <asp:CheckBox ID="HandmadeCheckBox" Text="Handmade" TextAlign="Right" runat="server" />
    </div>
    <div class="p-2">
        <asp:CheckBox ID="OutofStockCheckBox" Text="Out of Stock" TextAlign="Right" runat="server" />
    </div>
    <div class="p-2">
        <label for="ShippingTimeTextBox" class="form-label">Shipping Time</label>
        <asp:TextBox ID="ShippingTimeTextBox" ClientIDMode="Static" MaxLength="50" CssClass="form-control" runat="server"></asp:TextBox>
    </div>
    <div class="well p-3">
        <asp:Button ID="SaveButton" runat="server" Text="Save" CssClass="btn btn-primary" CausesValidation="true" />
    </div>
</asp:Content>
