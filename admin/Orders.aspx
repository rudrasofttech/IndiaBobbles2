<%@ Page Title="Orders" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="Orders.aspx.vb" Inherits="IndiaBobbles.Orders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">

    <asp:SqlDataSource ID="OrderDataSource" runat="server" CancelSelectOnNullParameter="false"
        ConnectionString="<%$ ConnectionStrings:indiabobblesConnectionString %>"
        SelectCommand="SELECT O.ID, O.DateCreated, O.Name, O.Email, O.Phone, O.MemberID, O.Status, O.ShippingTrackCode, O.Coupon, O.Total, O.TransactionCode, O.TransactionDate,
            CONCAT_WS(', ', NULLIF(LTRIM(RTRIM(O.ShippingAddress)), ''), NULLIF(LTRIM(RTRIM(O.ShippingCity)), ''), NULLIF(LTRIM(RTRIM(O.ShippingState)), ''), NULLIF(LTRIM(RTRIM(O.ShippingZip)), '')) AS Shipping,
            COUNT(OI.ID) AS ItemCount
        FROM [Order] AS O INNER JOIN OrderItem AS OI ON O.ID = OI.OrderID
        WHERE
            (   (@status = '0'    AND O.Status <> 8)
             OR (@status = 'ship' AND O.Status IN (3, 4))
             OR (@status = 'lead' AND O.Status = 1 AND (ISNULL(O.Email, '') <> '' OR ISNULL(O.Phone, '') <> ''))
             OR (CAST(O.Status AS varchar(10)) = @status) )
        AND (@hide = 0 OR @status = '1' OR NOT (O.Status = 1 AND ISNULL(O.Email, '') = '' AND ISNULL(O.Phone, '') = ''))
        AND (@q = '' OR CAST(O.ID AS varchar(20)) = @q OR O.Name LIKE '%' + @q + '%' OR O.Email LIKE '%' + @q + '%' OR O.Phone LIKE '%' + @q + '%'
             OR O.ShippingTrackCode LIKE '%' + @q + '%' OR O.TransactionCode LIKE '%' + @q + '%' OR O.ShippingCity LIKE '%' + @q + '%' OR O.ShippingZip = @q)
        GROUP BY O.ID, O.DateCreated, O.Name, O.Email, O.Phone, O.MemberID, O.Status, O.ShippingTrackCode, O.Coupon, O.Total, O.TransactionCode, O.TransactionDate,
                 O.ShippingAddress, O.ShippingCity, O.ShippingState, O.ShippingZip
        ORDER BY O.DateCreated DESC">
        <SelectParameters>
            <asp:ControlParameter ControlID="StatusDropDown" Name="status" PropertyName="SelectedValue" DefaultValue="0" ConvertEmptyStringToNull="false" />
            <asp:ControlParameter ControlID="HideCartsCheckBox" Name="hide" PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="SearchTextBox" Name="q" PropertyName="Text" DefaultValue="" ConvertEmptyStringToNull="false" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>

    <!-- ============ Heading ============ -->
    <div class="adm-head">
        <div>
            <h1>Orders</h1>
            <p>Track payments, shipping and follow up on unfinished checkouts.</p>
        </div>
    </div>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="ExportButton" />
        </Triggers>
        <ContentTemplate>

            <asp:Label ID="MessageLabel" runat="server" CssClass="adm-flash" Visible="false" EnableViewState="false" />

            <!-- ============ Stats ============ -->
            <div class="od-stats">
                <button type="button" class="od-stat ship <%= If(StatusDropDown.SelectedValue = "ship", "on", "") %>" onclick="ibOrderFilter('ship')">
                    <span class="n"><asp:Literal ID="ToShipLiteral" runat="server" Text="0" /></span><span class="l"><i class="fa fa-truck"></i> Paid – to ship</span></button>
                <button type="button" class="od-stat <%= If(StatusDropDown.SelectedValue = "5", "on", "") %>" onclick="ibOrderFilter('5')">
                    <span class="n"><asp:Literal ID="ShippedLiteral" runat="server" Text="0" /></span><span class="l"><i class="fa fa-paper-plane"></i> On the way</span></button>
                <button type="button" class="od-stat lead <%= If(StatusDropDown.SelectedValue = "lead", "on", "") %>" onclick="ibOrderFilter('lead')">
                    <span class="n"><asp:Literal ID="LeadLiteral" runat="server" Text="0" /></span><span class="l"><i class="fa fa-phone"></i> Unpaid, with contact (30 days)</span></button>
                <div class="od-stat money">
                    <span class="n"><asp:Literal ID="MonthSalesLiteral" runat="server" Text="₹0" /></span>
                    <span class="l"><asp:Literal ID="MonthLabelLiteral" runat="server" Text="Sales this month" /></span></div>
            </div>

            <!-- ============ Toolbar ============ -->
            <asp:Panel ID="ToolbarPanel" runat="server" CssClass="od-bar" DefaultButton="SearchButton">
                <div class="search">
                    <i class="fa fa-search"></i>
                    <asp:TextBox ID="SearchTextBox" runat="server" ClientIDMode="Static" placeholder="Order ID, name, email, phone, track or payment code…" aria-label="Search orders" />
                </div>
                <asp:Button ID="SearchButton" runat="server" Text="Search" CssClass="btn btn-ib btn-sm" CausesValidation="false" />
                <asp:DropDownList ID="StatusDropDown" ClientIDMode="Static" CssClass="form-select form-select-sm" runat="server" AutoPostBack="True" aria-label="Status">
                    <asp:ListItem Text="All orders" Value="0" />
                    <asp:ListItem Text="Paid – to ship" Value="ship" />
                    <asp:ListItem Text="Unpaid, with contact" Value="lead" />
                    <asp:ListItem Text="New / unpaid" Value="1" />
                    <asp:ListItem Text="Processing" Value="2" />
                    <asp:ListItem Text="Card paid" Value="3" />
                    <asp:ListItem Text="Cash on delivery" Value="4" />
                    <asp:ListItem Text="Shipped" Value="5" />
                    <asp:ListItem Text="Complete" Value="6" />
                    <asp:ListItem Text="Refund" Value="7" />
                    <asp:ListItem Text="Deleted" Value="8" />
                </asp:DropDownList>
                <label class="chk"><asp:CheckBox ID="HideCartsCheckBox" runat="server" Checked="true" AutoPostBack="true" /> Hide empty carts</label>
                <asp:LinkButton ID="ClearButton" runat="server" CssClass="btn btn-light btn-sm" CausesValidation="false"><i class="fa fa-times"></i> Reset</asp:LinkButton>
                <asp:LinkButton ID="ExportButton" runat="server" CssClass="btn btn-ib-o btn-sm" CausesValidation="false" ToolTip="Download the current list as a spreadsheet"><i class="fa fa-download"></i> Export CSV</asp:LinkButton>
            </asp:Panel>

            <asp:UpdateProgress ID="UpdateProgress1" DisplayAfter="150" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                <ProgressTemplate><div class="od-loading"></div></ProgressTemplate>
            </asp:UpdateProgress>

            <!-- ============ Grid ============ -->
            <div class="adm-card table-responsive">
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="ID" DataSourceID="OrderDataSource"
                    AllowSorting="True" AllowPaging="True" PageSize="25" EnableSortingAndPagingCallbacks="False"
                    CssClass="table adm-table od-table align-middle mb-0" GridLines="None">
                    <EmptyDataTemplate>
                        <div class="adm-empty"><i class="fa fa-inbox fa-2x d-block mb-2"></i>No orders match these filters.</div>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="Order" SortExpression="ID">
                            <ItemTemplate>
                                <a class="oid" href='<%# "manageorder.aspx?orderid=" & Eval("ID") %>'>#<%# Eval("ID") %></a>
                                <div class="sm"><%# Convert.ToDateTime(Eval("DateCreated")).ToString("d MMM yyyy, h:mm tt") %></div>
                                <div class="sm"><%# Eval("ItemCount") %> item<%# If(Convert.ToInt32(Eval("ItemCount")) = 1, "", "s") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Status" SortExpression="Status">
                            <ItemTemplate>
                                <span class='<%# "os " & StatusCss(Eval("Status")) %>'><%# StatusText(Eval("Status")) %></span>
                                <%# If(String.IsNullOrWhiteSpace(Convert.ToString(Eval("ShippingTrackCode"))), "",
                                       "<div><span class=""track"" title=""Tracking code""><i class=""fa fa-truck""></i> " & HttpUtility.HtmlEncode(Convert.ToString(Eval("ShippingTrackCode"))) & "</span></div>") %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Customer" SortExpression="Name">
                            <ItemTemplate>
                                <%# If(String.IsNullOrWhiteSpace(Convert.ToString(Eval("Name"))) AndAlso String.IsNullOrWhiteSpace(Convert.ToString(Eval("Email"))) AndAlso String.IsNullOrWhiteSpace(Convert.ToString(Eval("Phone"))),
                                       "<span class=""empty"">No contact details</span>", "") %>
                                <div class="cname"><%#: Eval("Name") %><%# If(IsDBNull(Eval("MemberID")) OrElse Convert.ToString(Eval("MemberID")) = "" OrElse Convert.ToString(Eval("MemberID")) = "0", "", "<span class=""mem"" title=""Member ID " & Eval("MemberID") & """>Member</span>") %></div>
                                <%# If(String.IsNullOrWhiteSpace(Convert.ToString(Eval("Email"))), "",
                                       "<div class=""cline""><a target=""_blank"" href=""../sendmail.aspx?email=" & HttpUtility.UrlEncode(Convert.ToString(Eval("Email"))) & "&amp;name=" & HttpUtility.UrlEncode(Convert.ToString(Eval("Name"))) & """ title=""Send email""><i class=""fa fa-envelope-o""></i> " & HttpUtility.HtmlEncode(Convert.ToString(Eval("Email"))) & "</a></div>") %>
                                <%# PhoneHtml(Eval("Phone"), Eval("ID")) %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Ship to" SortExpression="Shipping">
                            <ItemTemplate>
                                <div class="addr"><%# If(String.IsNullOrWhiteSpace(Convert.ToString(Eval("Shipping"))), "<span class=""empty"">Not entered</span>", HttpUtility.HtmlEncode(Convert.ToString(Eval("Shipping")))) %></div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Total" SortExpression="Total">
                            <ItemTemplate>
                                <div class="amt"><%# Money(Eval("Total")) %></div>
                                <%# If(String.IsNullOrWhiteSpace(Convert.ToString(Eval("Coupon"))), "", "<div class=""sm""><i class=""fa fa-tag""></i> " & HttpUtility.HtmlEncode(Convert.ToString(Eval("Coupon"))) & "</div>") %>
                                <%# If(String.IsNullOrWhiteSpace(Convert.ToString(Eval("TransactionCode"))), "", "<div class=""mono"" title=""Payment transaction code"">" & HttpUtility.HtmlEncode(Convert.ToString(Eval("TransactionCode"))) & "</div>") %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="" ItemStyle-CssClass="text-end text-nowrap">
                            <ItemTemplate>
                                <a class="act view" href='<%# "manageorder.aspx?orderid=" & Eval("ID") %>'><i class="fa fa-eye"></i> View</a>
                                <asp:LinkButton runat="server" CssClass="act del" CausesValidation="false" ToolTip="Remove order"
                                    Visible='<%# Convert.ToString(Eval("Status")) <> "8" %>'
                                    OnClientClick="return confirm('Remove this order?');"
                                    CommandName="Remove" CommandArgument='<%# Eval("ID") %>'><i class="fa fa-trash"></i></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerSettings Position="Bottom" Mode="NumericFirstLast" PageButtonCount="10" />
                    <PagerStyle CssClass="adm-pager" HorizontalAlign="Center" />
                </asp:GridView>
            </div>
            <div class="sm text-muted mt-2" style="font-size:.8rem;"><asp:Literal ID="CountLiteral" runat="server" /></div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <script>
        // Stat cards set the status filter and refresh the list
        function ibOrderFilter(v) {
            var dd = document.getElementById('StatusDropDown');
            dd.value = (dd.value === v) ? '0' : v;     // click again to clear
            __doPostBack('<%= StatusDropDown.UniqueID %>', '');
        }
        // "/" jumps to search
        document.addEventListener('keydown', function (e) {
            if (e.key === '/' && !/INPUT|TEXTAREA|SELECT/.test(document.activeElement.tagName)) {
                e.preventDefault(); var s = document.getElementById('SearchTextBox'); if (s) s.focus();
            }
        });
    </script>
</asp:Content>