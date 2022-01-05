<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="ProductDetail.aspx.vb" Inherits="IndiaBobbles.ProductDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">
    <asp:SqlDataSource ID="ProductDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:indiabobblesConnectionString %>" SelectCommand="SELECT ID, Name, MRP, SalePrice, CASE WHEN Status = 0 THEN 'Active' WHEN Status = 1 THEN 'Inactive' WHEN Status = 2 THEN 'Deleted' ELSE '' END AS Status, Dimension, Color, Weight, Material, Manufacturer, CareInstructions, RecommendedAge, CountryofOrigin, Fragile, ShippingTime, Handmade, OutofStock, ThumbPath, URL, ProductCode FROM Product WHERE (ID = @ID)">
        <SelectParameters>
            <asp:QueryStringParameter DefaultValue="0" Name="ID" QueryStringField="id" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="ProductTagDataSource" runat="server"
        ConnectionString="<%$ ConnectionStrings:indiabobblesConnectionString %>"
        SelectCommand="SELECT CT.DisplayName AS 'Display Name', PT.ID FROM CategoryTag AS CT INNER JOIN ProductTag AS PT ON CT.ID = PT.TagID WHERE (PT.ProductID = @ProductID) ORDER BY 'Display Name'"
        DeleteCommand="DELETE FROM ProductTag WHERE (ProductID = @ProductID) AND (ID = @ID)"
        InsertCommand="INSERT INTO ProductTag(ProductID, TagID) VALUES (@ProductID, @TagID)">
        <DeleteParameters>
            <asp:QueryStringParameter Name="ProductID" QueryStringField="id" />
            <asp:Parameter Name="ID" />
        </DeleteParameters>
        <InsertParameters>
            <asp:QueryStringParameter Name="ProductID" QueryStringField="id" />
            <asp:ControlParameter ControlID="TagDropDown" Name="TagID" PropertyName="SelectedValue" />
        </InsertParameters>
        <SelectParameters>
            <asp:QueryStringParameter DefaultValue="0" Name="ProductID" QueryStringField="id" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="TagDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:indiabobblesConnectionString %>" SelectCommand="SELECT [ID], [DisplayName] FROM [CategoryTag]"></asp:SqlDataSource>
    <div class="row">
        <div class="col-md-6">
            <h2>Product Detail</h2>
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
                    <asp:CheckBoxField DataField="OutofStock" HeaderText="OutofStock" SortExpression="OutofStock" />
                    <asp:BoundField DataField="URL" HeaderText="URL" SortExpression="URL" />
                    <asp:BoundField DataField="ProductCode" HeaderText="ProductCode" SortExpression="ProductCode" />
                    <asp:ImageField DataImageUrlField="ThumbPath" HeaderText="Thumbpath" ReadOnly="True" ControlStyle-Height="70px">
                    </asp:ImageField>
                </Fields>
                <HeaderStyle Font-Bold="True" />
            </asp:DetailsView>
        </div>
        <div class="col-md-6">
            <h2>Product Photos</h2>
            <asp:UpdatePanel ID="PhotoUP" runat="server">
                <ContentTemplate>
                    <asp:GridView ID="PhotoGridView" CssClass="table" runat="server" AutoGenerateColumns="False" AutoGenerateDeleteButton="True" AutoGenerateEditButton="True">
                        <Columns>
                            <asp:TemplateField HeaderText="ID" Visible="false">

                                <ItemTemplate>
                                    <asp:Label ID="PhotoIDLabel" runat="server" Text='<%# Eval("ID") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Picture">
                                <EditItemTemplate>
                                    <asp:TextBox ID="ImagePathTextBox" CssClass="form-control" runat="server" Text='<%# Eval("ImagePath") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Image ID="Image1" Height="100px" CssClass="img-thumbnail" runat="server" ImageUrl='<%# Eval("ImagePath") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="150px" SortExpression="Sequence" HeaderText="Sequence">
                                <EditItemTemplate>
                                    <asp:TextBox ID="SequenceTextBox" CssClass="form-control" runat="server" Text='<%# Bind("Sequence") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Sequence") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="SaveButton" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>

            <h4>Add Photo</h4>
            <div class="p-1">
                <label for="PhotoPathTextBox" class="form-label">Photo Path</label>
                <asp:TextBox ID="PhotoPathTextBox" ValidationGroup="photogrp" ClientIDMode="Static" MaxLength="300" CssClass="form-control" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ForeColor="Red" ControlToValidate="PhotoPathTextBox" Display="Dynamic" ValidationGroup="photogrp" ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required"></asp:RequiredFieldValidator>

            </div>
            <div class="p-1">
                <label for="SequenceTextBox" class="form-label">Sequence</label>
                <asp:TextBox ID="SequenceTextBox" TextMode="Number" ClientIDMode="Static" MaxLength="3" CssClass="form-control" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator Display="Dynamic" ForeColor="Red" ControlToValidate="SequenceTextBox" ValidationGroup="photogrp" ID="RequiredFieldValidator2" runat="server" ErrorMessage="Required"></asp:RequiredFieldValidator>
            </div>
            <div class="well p-1">
                <asp:Button ID="SaveButton" runat="server" Text="Save Photo" ValidationGroup="photogrp" CssClass="btn btn-primary" CausesValidation="true" />
            </div>
        </div>
    </div>
    <h2 class="mt-2">Product Category Tags
    </h2>
    <div class="row">
        <div class="col-md-6">
            <asp:UpdatePanel ID="ProductTagUP" runat="server">
                <ContentTemplate>
                    <asp:GridView ID="ProductTagGridView" runat="server" AllowSorting="True" AutoGenerateDeleteButton="True" CssClass="table" DataSourceID="ProductTagDataSource" EmptyDataText="No Tags Found" AutoGenerateColumns="False" DataKeyNames="ID" EnableSortingAndPagingCallbacks="True">
                        <Columns>
                            <asp:BoundField DataField="Display Name" HeaderText="Display Name" SortExpression="Display Name" />
                            <asp:BoundField DataField="ID" Visible="false" HeaderText="ID" InsertVisible="False" ReadOnly="True" SortExpression="ID" />
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="SaveTagButton" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div class="col-md-6">
            <h4>Add Tag</h4>
            <div class="p-1">
                <label for="TagDropDown" class="form-label">Tag</label>
                <asp:DropDownList ID="TagDropDown" ClientIDMode="Static" runat="server" CssClass="form-select" DataSourceID="TagDataSource" DataTextField="DisplayName" DataValueField="ID"></asp:DropDownList>
            </div>
            <div class="well p-1">
                <asp:Button ID="SaveTagButton" runat="server" Text="Save Tag" CssClass="btn btn-primary" CausesValidation="false" />
            </div>
        </div>
    </div>

</asp:Content>
