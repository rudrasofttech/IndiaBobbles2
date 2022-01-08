<%@ Page Title="Manage Product" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="ManageProduct.aspx.vb" Inherits="IndiaBobbles.ManageProduct" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">
    <h1>
        <asp:Literal ID="HeadingLiteral" runat="server">Add Product</asp:Literal></h1>
    <div class="row">
        <div class="col">
            <div class="p-2">
                <label for="NameTextBox" class="form-label">Name</label>
                <asp:TextBox ID="NameTextBox" ClientIDMode="Static" MaxLength="300" CssClass="form-control" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required" ControlToValidate="NameTextBox"></asp:RequiredFieldValidator>
            </div>
            <div class="p-2">
                <label for="MRPTextBox" class="form-label">MRP</label>
                <div class="input-group">
                    <span class="input-group-text" id="basic-addon1">₹</span>
                    <asp:TextBox ID="MRPTextBox" aria-describedby="basic-addon1" TextMode="Number" ClientIDMode="Static" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Required" ControlToValidate="MRPTextBox"></asp:RequiredFieldValidator>
            </div>
            <div class="p-2">
                <label for="SaleTextBox" class="form-label">Sale Price</label>
                <div class="input-group">
                    <span class="input-group-text" id="basic-addon2">₹</span>
                    <asp:TextBox ID="SaleTextBox" aria-describedby="basic-addon2" TextMode="Number" ClientIDMode="Static" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Required" ControlToValidate="SaleTextBox"></asp:RequiredFieldValidator>
            </div>
            <div class="p-2">
                <label for="DescTextBox" class="form-label">Description</label>
                <asp:TextBox ID="DescTextBox" Rows="15" TextMode="MultiLine" CssClass="form-control" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Required" ControlToValidate="DescTextBox"></asp:RequiredFieldValidator>
            </div>
            <div class="p-2">
                <label for="StatusDropDown" class="form-label">Product Status</label>
                <asp:DropDownList ID="StatusDropDown" CssClass="form-select" ClientIDMode="Static" runat="server">
                    <asp:ListItem Text="Active" Value="0"></asp:ListItem>
                    <asp:ListItem Text="Inactive" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Deleted" Value="2"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="col">
            <div class="p-2">
                <label for="DimensionTextBox" class="form-label">Product Dimension</label>
                <asp:TextBox ID="DimensionTextBox" ClientIDMode="Static" MaxLength="50" CssClass="form-control" runat="server"></asp:TextBox>
                <span class="text-italic">Length x Width x Height for example 1.5" x 1.5" x 6"</span>
            </div>
            <div class="p-2">
                <label for="ColorTextBox" class="form-label">Color</label>
                <asp:TextBox ID="ColorTextBox" ClientIDMode="Static" MaxLength="50" CssClass="form-control" runat="server"></asp:TextBox>
                <span class="text-italic">Color of the product, write multi colour if product has many prominent colours.</span>
            </div>
            <div class="p-2">
                <label for="WeightTextBox" class="form-label">Weight (in grams)</label>
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
                <asp:CheckBox ID="FragileCheckBox" CssClass="me-2" Text="Fragile" TextAlign="Right" runat="server" />
                <asp:CheckBox ID="HandmadeCheckBox" CssClass="me-2" Text="Handmade" TextAlign="Right" runat="server" />
                <asp:CheckBox ID="OutofStockCheckBox" CssClass="me-2" Text="Out of Stock" TextAlign="Right" runat="server" />
            </div>
            <div class="p-2">
                <label for="ShippingTimeTextBox" class="form-label">Shipping Time</label>
                <asp:TextBox ID="ShippingTimeTextBox" ClientIDMode="Static" MaxLength="50" CssClass="form-control" runat="server"></asp:TextBox>
            </div>
        </div>
    </div>
    <div class="p-2">
        <label for="ThumbPathTextBox" class="form-label">
            Thumbnail Path (<a role="button" class="btn btn-link" data-bs-toggle="modal" data-bs-target="#driveModal">View Drive</a>)</label>
        <asp:TextBox ID="ThumbPathTextBox" ClientIDMode="Static" MaxLength="300" CssClass="form-control" runat="server"></asp:TextBox>
    </div>
    <div class="well p-3">
        <asp:Button ID="SaveButton" runat="server" Text="Save" CssClass="btn btn-primary" CausesValidation="true" />
    </div>
    
</asp:Content>
