<%@ Page Title="Members" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="Members.aspx.vb" Inherits="IndiaBobbles.Members" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">

    <!-- ============ Heading ============ -->
    <div class="adm-head">
        <div>
            <h1>Members</h1>
            <p>Customers and newsletter sign-ups. Clean out spam accounts and export lists for email campaigns.</p>
        </div>
    </div>

    <asp:Label ID="MessageLabel" runat="server" CssClass="adm-flash" Visible="false" EnableViewState="false" />

    <!-- ============ Stats ============ -->
    <div class="mb-stats">
        <div class="mb-stat"><span class="n"><asp:Literal ID="TotalLiteral" runat="server" Text="0" /></span><span class="l">Active members</span></div>
        <div class="mb-stat"><span class="n"><asp:Literal ID="SubscribedLiteral" runat="server" Text="0" /></span><span class="l">Newsletter subscribers</span></div>
        <div class="mb-stat dark"><span class="n"><asp:Literal ID="BuyersLiteral" runat="server" Text="0" /></span><span class="l">Have placed a paid order</span></div>
        <div class="mb-stat"><span class="n"><asp:Literal ID="NewLiteral" runat="server" Text="0" /></span><span class="l">Joined in last 30 days</span></div>
    </div>

    <!-- ============ Filters ============ -->
    <asp:Panel ID="FilterPanel" runat="server" CssClass="mb-bar" DefaultButton="SubmitButton">
        <div class="search">
            <i class="fa fa-search"></i>
            <asp:TextBox ID="FilterTextBox" runat="server" ClientIDMode="Static" placeholder="Search name, email or mobile…" MaxLength="60" aria-label="Search members" />
        </div>
        <asp:DropDownList ID="StatusDropDown" CssClass="form-select form-select-sm" runat="server" aria-label="Status">
            <asp:ListItem Value="">Any status</asp:ListItem>
            <asp:ListItem Value="0">Active</asp:ListItem>
            <asp:ListItem Value="1">Inactive</asp:ListItem>
            <asp:ListItem Value="2">Deleted</asp:ListItem>
        </asp:DropDownList>
        <asp:DropDownList ID="SubscribeList" CssClass="form-select form-select-sm" runat="server" aria-label="Newsletter">
            <asp:ListItem Value="">Newsletter: any</asp:ListItem>
            <asp:ListItem Value="1">Subscribed</asp:ListItem>
            <asp:ListItem Value="0">Not subscribed</asp:ListItem>
        </asp:DropDownList>
        <asp:DropDownList ID="KindDropDown" CssClass="form-select form-select-sm" runat="server" aria-label="Type">
            <asp:ListItem Value="">Everyone</asp:ListItem>
            <asp:ListItem Value="buyers">Buyers (paid orders)</asp:ListItem>
            <asp:ListItem Value="none">Never ordered</asp:ListItem>
            <asp:ListItem Value="spam">Looks like spam</asp:ListItem>
        </asp:DropDownList>
        <asp:Button ID="SubmitButton" runat="server" Text="Apply" CssClass="btn btn-ib btn-sm" CausesValidation="false" />
        <asp:LinkButton ID="ResetButton" runat="server" CssClass="btn btn-light btn-sm" CausesValidation="false"><i class="fa fa-times"></i> Reset</asp:LinkButton>
    </asp:Panel>

    <!-- ============ Bulk actions ============ -->
    <div class="mb-bulk idle" id="bulkBar">
        <span class="cnt"><b id="selCount">0</b> selected · <asp:Literal ID="FilteredCountLiteral" runat="server" Text="0" /> in this view</span>
        <asp:Button ID="UnsubscribeButton" runat="server" Text="Unsubscribe selected" CssClass="btn btn-light btn-sm sel-only" CausesValidation="false"
            OnClientClick="return confirm('Remove the selected members from the newsletter?');" />
        <asp:Button ID="DeleteButton" runat="server" Text="Remove selected" CssClass="btn btn-danger btn-sm sel-only" CausesValidation="false"
            OnClientClick="return confirm('Remove the selected members? They will be marked Deleted.');" />
        <asp:Button ID="DeleteFilteredButton" runat="server" CausesValidation="false" Text="Remove all in this view" CssClass="btn btn-outline-light btn-sm" />
        <asp:LinkButton ID="ExportButton" runat="server" CssClass="btn btn-outline-light btn-sm" CausesValidation="false" ToolTip="Download this view as a spreadsheet"><i class="fa fa-download"></i> Export CSV</asp:LinkButton>
    </div>

    <!-- ============ Grid ============ -->
    <div class="adm-card table-responsive">
        <asp:GridView ID="MemberGridView" runat="server" AutoGenerateColumns="False" AllowPaging="True" PageSize="50" DataKeyNames="ID"
            CssClass="table adm-table mb-table align-middle mb-0" GridLines="None">
            <EmptyDataTemplate>
                <div class="adm-empty"><i class="fa fa-users fa-2x d-block mb-2"></i>No members match these filters.</div>
            </EmptyDataTemplate>
            <Columns>
                <asp:TemplateField ItemStyle-CssClass="cb" HeaderStyle-CssClass="cb" ItemStyle-Width="40px">
                    <HeaderTemplate><input type="checkbox" id="cbAll" title="Select all on this page" aria-label="Select all" /></HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="cbSelect" CssClass="gridCB" runat="server" />
                        <asp:Literal ID="MemberIDLt" Text='<%# Eval("ID") %>' runat="server" Visible="false" />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Member">
                    <ItemTemplate>
                        <div class="mb-who">
                            <span class='<%# "mb-av" & If(Convert.ToInt32(Eval("Orders")) > 0, " buyer", "") %>'><%#: Initial(Eval("MemberName"), Eval("Email")) %></span>
                            <div style="min-width:0;">
                                <div class="mb-name" title='<%#: Eval("MemberName") %>'><%#: If(String.IsNullOrWhiteSpace(Convert.ToString(Eval("MemberName"))), "(no name)", Convert.ToString(Eval("MemberName"))) %></div>
                                <div class="mb-email" title='<%#: Eval("Email") %>'><%#: Eval("Email") %></div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Mobile">
                    <ItemTemplate><span class="mb-mob"><%# If(String.IsNullOrWhiteSpace(Convert.ToString(Eval("Mobile"))), "<span class=""mb-id"">–</span>", HttpUtility.HtmlEncode(Convert.ToString(Eval("Mobile")))) %></span></ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Joined">
                    <ItemTemplate>
                        <div style="white-space:nowrap;"><%# Convert.ToDateTime(Eval("Createdate")).ToString("d MMM yyyy") %></div>
                        <div class="mb-id">#<%# Eval("ID") %></div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Orders" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center">
                    <ItemTemplate>
                        <a href='<%#: "orders.aspx?q=" & HttpUtility.UrlEncode(Convert.ToString(Eval("Email"))) %>' style="text-decoration:none;" title="Paid orders – click to see all orders">
                            <span class='<%# "mb-ord" & If(Convert.ToInt32(Eval("Orders")) > 0, " has", "") %>'><%# Eval("Orders") %></span></a>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='<%# "ms ms-" & Convert.ToString(Eval("Status")) %>'><%# StatusText(Eval("Status")) %></span>
                        <%# If(IsAdminType(Eval("UserType")), "<span class=""ms ms-admin"">Admin</span>", "") %>
                        <%# If(Convert.ToBoolean(Eval("NewsletterOn")), "<span class=""ms ms-nl"" title=""Subscribed to newsletter""><i class=""fa fa-envelope""></i> News</span>", "") %>
                        <%# If(Convert.ToBoolean(Eval("Spam")), "<div><span class=""ms ms-spam"" title=""Spam-like email or name and no orders""><i class=""fa fa-flag""></i> Looks like spam</span></div>", "") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField ItemStyle-CssClass="text-end text-nowrap">
                    <ItemTemplate>
                        <a class="mb-act" href='<%# Eval("ID", "managemember.aspx?id={0}&mode=edit") %>' title="Edit"><i class="fa fa-pencil"></i></a>
                        <a class="mb-act" href='<%# Eval("ID", "SendMail.aspx?id={0}") %>' title="Send email"><i class="fa fa-envelope-o"></i></a>
                        <asp:LinkButton runat="server" CssClass="mb-act del" CausesValidation="false" CommandName="Remove" CommandArgument='<%# Eval("ID") %>'
                            Visible='<%# Convert.ToString(Eval("Status")) <> "2" %>' ToolTip="Remove"
                            OnClientClick="return confirm('Remove this member? They will be marked Deleted.');"><i class="fa fa-trash"></i></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <PagerSettings Position="Bottom" Mode="NumericFirstLast" PageButtonCount="10" />
            <PagerStyle CssClass="adm-pager" HorizontalAlign="Center" />
        </asp:GridView>
    </div>

    <script>
        (function () {
            var boxes = $('.gridCB input');
            function count() {
                var n = boxes.filter(':checked').length;
                $('#selCount').text(n);
                $('#bulkBar').toggleClass('idle', n === 0);
                $('#cbAll').prop('checked', n > 0 && n === boxes.length);
            }
            $('#cbAll').on('change', function () { boxes.prop('checked', this.checked); count(); });
            boxes.on('change', count);
            // Click anywhere on a row (except links/buttons) to tick it
            $('.mb-table tbody tr').on('click', function (e) {
                if ($(e.target).closest('a, input, button, label').length) return;
                var c = $(this).find('.gridCB input'); c.prop('checked', !c.prop('checked')); count();
            });
            count();
            document.addEventListener('keydown', function (e) {
                if (e.key === '/' && !/INPUT|TEXTAREA|SELECT/.test(document.activeElement.tagName)) { e.preventDefault(); $('#FilterTextBox').focus(); }
            });
        })();
    </script>
</asp:Content>