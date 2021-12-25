<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="Orders.aspx.vb" Inherits="IndiaBobbles.Orders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">
    <h1>Order List</h1>
    <asp:SqlDataSource ID="OrderDataSource" runat="server"
        ConnectionString="<%$ ConnectionStrings:indiabobblesConnectionString %>"
        SelectCommand="SELECT O.ID, O.DateCreated, O.Name, O.Email, O.Phone, O.BillingAddress, O.BillingCity, O.MemberID, O.BillingState, O.BillingCountry, O.BillingZip, O.ShippingAddress + ' ' + O.ShippingCity + ' ' + O.ShippingState + ' ' + O.ShippingCountry + ' ' + O.ShippingZip AS 'Shipping', O.Coupon, O.Status, O.ShippingTrackCode, O.DateModified, O.Amount, O.Tax, O.Discount, O.Total, O.TransactionCode, O.TransactionDate, O.ShippingPrice, COUNT(OI.ID) AS ItemCount FROM [Order] AS O INNER JOIN OrderItem AS OI ON O.ID = OI.OrderID GROUP BY O.ID, O.DateCreated, O.Name, O.Email, O.Phone, O.BillingAddress, O.BillingCity, O.MemberID, O.BillingState, O.BillingCountry, O.BillingZip, O.ShippingAddress, O.ShippingCity, O.ShippingState, O.ShippingCountry, O.ShippingZip, O.Coupon, O.Status, O.ShippingTrackCode, O.DateModified, O.Amount, O.Tax, O.Discount, O.Total, O.TransactionCode, O.TransactionDate, O.ShippingPrice HAVING (O.Status = @status) OR (@status = '0') ORDER BY O.DateCreated DESC">
        <SelectParameters>
            <asp:ControlParameter ControlID="StatusDropDown" Name="status" PropertyName="SelectedValue" DefaultValue="0" />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="row">
                <div class="col-md-3">
                    <div class="mb-2">
                        <asp:DropDownList ID="StatusDropDown" CssClass="form-select" runat="server" AutoPostBack="True">
                            <asp:ListItem Text="Select" Value="0"></asp:ListItem>
                            <asp:ListItem Text="New" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Process" Value="2"></asp:ListItem>
                            <asp:ListItem Text="CardPaid" Value="3"></asp:ListItem>
                            <asp:ListItem Text="CODPaid" Value="4"></asp:ListItem>
                            <asp:ListItem Text="Shipped" Value="5"></asp:ListItem>
                            <asp:ListItem Text="Complete" Value="6"></asp:ListItem>
                            <asp:ListItem Text="Refund" Value="7"></asp:ListItem>
                            <asp:ListItem Text="Deleted" Value="8"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <asp:UpdateProgress ID="UpdateProgress1" DisplayAfter="0" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                <ProgressTemplate>
                    <div class="progress my-2">
                        <div class="progress-bar progress-bar-striped bg-primary" role="progressbar" style="width: 100%" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100"></div>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <div class="table-responsive">
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="ID" DataSourceID="OrderDataSource" AllowSorting="True" CssClass="table table-striped table-bordered table-condensed" OnRowDataBound="GridView1_RowDataBound" EmptyDataText="No Orders Found." OnRowCommand="GridView1_RowCommand" EnableSortingAndPagingCallbacks="True" AllowPaging="True">
                    <Columns>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButton1" OnClientClick="return confirm('Are you sure you want to remove this order?');" runat="server" CausesValidation="false" CommandArgument='<%# Eval("ID") %>' CommandName="Remove" Text="Remove"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:HyperLinkField DataNavigateUrlFields="ID" DataNavigateUrlFormatString="manageorder.aspx?orderid={0}" Text="View" />
                        <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False" ReadOnly="True" SortExpression="ID" />
                        <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:BoundField DataField="DateCreated" HeaderText="Date Created" SortExpression="DateCreated" DataFormatString="{0:d MMM yyyy h:m tt}" />
                        <asp:BoundField DataField="ItemCount" HeaderText="Item Count" ReadOnly="True" SortExpression="ItemCount" />
                        <asp:TemplateField ShowHeader="False" HeaderText="Email">
                            <ItemTemplate>
                                <%# DataBinder.Eval(Container.DataItem, "Name") %><br />
                                <a target="_blank" href='<%# string.Format("../sendmail.aspx?email={0}&name={1}", DataBinder.Eval(Container.DataItem, "Email"), DataBinder.Eval(Container.DataItem, "Name")) %>'><%# DataBinder.Eval(Container.DataItem, "Email") %></a>
                                <br />
                                <%# DataBinder.Eval(Container.DataItem, "Phone") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="MemberID" HeaderText="MemberID" SortExpression="MemberID" />
                        <asp:BoundField DataField="Shipping" HeaderText="Shipping" SortExpression="Shipping" />
                        <asp:BoundField DataField="ShippingTrackCode" HeaderText="TrackCode" SortExpression="ShippingTrackCode" />
                        <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" DataFormatString="{0:##00.00}" />
                        <asp:BoundField DataField="TransactionCode" HeaderText="Transaction Code" SortExpression="TransactionCode" />
                        <asp:BoundField Visible="false" DataField="TransactionDate" HeaderText="Transaction Date" SortExpression="TransactionDate" DataFormatString="{0:d MMM yyyy h:m tt}" />

                    </Columns>
                    <PagerSettings Position="TopAndBottom" Mode="NumericFirstLast" />
                    <PagerStyle HorizontalAlign="Left" VerticalAlign="Middle" CssClass="paging" />
                </asp:GridView>

            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
