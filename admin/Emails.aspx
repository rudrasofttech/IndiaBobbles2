<%@ Page Title="Emails" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="Emails.aspx.vb" Inherits="IndiaBobbles.Emails" %>

<%@ Import Namespace="IndiaBobbles" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:indiabobblesConnectionString %>"
        SelectCommand="SELECT DISTINCT [EmailGroup] FROM [EmailMessage] WHERE ISNULL([EmailGroup], '') <> '' ORDER BY [EmailGroup]"></asp:SqlDataSource>

    <!-- ============ Heading ============ -->
    <div class="adm-head">
        <div>
            <h1>Emails</h1>
            <p>Every email the site has sent or queued: receipts, OTPs, newsletters and bulk campaigns.</p>
        </div>
        <a href="bulkemail.aspx" class="btn btn-ib"><i class="fa fa-paper-plane"></i> New bulk email</a>
    </div>

    <asp:Label ID="MessageLabel" runat="server" CssClass="adm-flash" Visible="false" EnableViewState="false" />

    <!-- ============ Stats for the current filter ============ -->
    <div class="em-stats">
        <div class="em-stat"><span class="n"><asp:Literal ID="TotalLiteral" runat="server" Text="0" /></span><span class="l">Emails in this view</span></div>
        <div class="em-stat ok"><span class="n"><asp:Literal ID="SentLiteral" runat="server" Text="0" /></span><span class="l">Sent</span></div>
        <div class="em-stat dark"><span class="n"><asp:Literal ID="OpenRateLiteral" runat="server" Text="–" /></span><span class="l"><asp:Literal ID="OpenedLiteral" runat="server" Text="0 opened" /></span></div>
        <div class="em-stat warn"><span class="n"><asp:Literal ID="PendingLiteral" runat="server" Text="0" /></span><span class="l">Waiting to send</span></div>
    </div>

    <!-- ============ Recent campaigns ============ -->
    <div class="adm-panel mb-0">
        <h2><i class="fa fa-bullhorn"></i> Recent campaigns <span style="font-weight:600;font-size:.78rem;color:#6b5f72;">– click one to see its emails</span></h2>
        <div class="em-camps">
            <asp:Repeater ID="CampaignRepeater" runat="server">
                <ItemTemplate>
                    <asp:LinkButton runat="server" CommandName="Group" CommandArgument='<%# Eval("Group") %>' CausesValidation="false"
                        CssClass='<%# "em-camp" & If(Convert.ToString(Eval("Group")) = GroupDropDown.SelectedValue, " on", "") %>'>
                        <div class="g"><%#: Eval("Group") %></div>
                        <div class="m"><%# Eval("Total", "{0:#,##0}") %> emails · last <%# Convert.ToDateTime(Eval("Last")).ToString("d MMM yyyy") %></div>
                        <div class="bar"><span style='<%# "width:" & Pct(Eval("Read"), Eval("Sent")) & "%" %>'></span></div>
                        <div class="r"><span><b><%# Pct(Eval("Read"), Eval("Sent")) %>%</b> opened</span><span><%# Eval("Sent", "{0:#,##0}") %> sent</span></div>
                    </asp:LinkButton>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <!-- ============ Filters ============ -->
    <asp:Panel ID="FilterPanel" runat="server" CssClass="em-bar" DefaultButton="SubmitButton">
        <div class="search">
            <i class="fa fa-search"></i>
            <asp:TextBox ID="KeywordTextBox" runat="server" MaxLength="200" placeholder="Search recipient, subject or message…" aria-label="Search emails" />
        </div>
        <asp:DropDownList ID="GroupDropDown" CssClass="form-select form-select-sm" runat="server" DataSourceID="SqlDataSource1"
            DataTextField="EmailGroup" DataValueField="EmailGroup" AppendDataBoundItems="true" aria-label="Group">
            <asp:ListItem Value="">All groups</asp:ListItem>
        </asp:DropDownList>
        <asp:DropDownList ID="TypeDropDown" CssClass="form-select form-select-sm" runat="server" aria-label="Type">
            <asp:ListItem Selected="True" Value="">All types</asp:ListItem>
            <asp:ListItem Value="1">Activation</asp:ListItem>
            <asp:ListItem Value="2">Unsubscribe</asp:ListItem>
            <asp:ListItem Value="3">Newsletter</asp:ListItem>
            <asp:ListItem Value="4">Change password</asp:ListItem>
            <asp:ListItem Value="5">Reminder</asp:ListItem>
            <asp:ListItem Value="6">Communication</asp:ListItem>
        </asp:DropDownList>
        <asp:DropDownList ID="SentDropDown" runat="server" CssClass="form-select form-select-sm" aria-label="Sent">
            <asp:ListItem Selected="True" Value="">Sent or not</asp:ListItem>
            <asp:ListItem Value="1">Sent</asp:ListItem>
            <asp:ListItem Value="0">Not sent yet</asp:ListItem>
        </asp:DropDownList>
        <asp:DropDownList ID="ReadDropDown" runat="server" CssClass="form-select form-select-sm" aria-label="Opened">
            <asp:ListItem Selected="True" Value="">Opened or not</asp:ListItem>
            <asp:ListItem Value="1">Opened</asp:ListItem>
            <asp:ListItem Value="0">Not opened</asp:ListItem>
        </asp:DropDownList>
        <asp:Button ID="SubmitButton" CssClass="btn btn-ib btn-sm" runat="server" Text="Apply" OnClick="SubmitButton_Click" />
        <asp:LinkButton ID="ResetButton" runat="server" CssClass="btn btn-light btn-sm" CausesValidation="false"><i class="fa fa-times"></i> Reset</asp:LinkButton>
    </asp:Panel>

    <!-- ============ Selection bar ============ -->
    <div class="em-sel idle" id="selBar">
        <span class="cnt"><b id="selCount">0</b> selected</span>
        <asp:Button ID="DeleteButton" runat="server" Text="Remove selected" CssClass="btn btn-danger btn-sm" CausesValidation="false"
            OnClientClick="return confirm('Remove the selected emails from the log? This cannot be undone.')" OnClick="DeleteButton_Click" />
    </div>

    <!-- ============ Grid ============ -->
    <div class="adm-card table-responsive">
        <asp:GridView ID="EmailGrid" runat="server" AllowPaging="True" AllowCustomPaging="True" PageSize="50"
            AutoGenerateColumns="False" CssClass="table adm-table em-table align-middle mb-0" GridLines="None" DataKeyNames="ID"
            OnPageIndexChanging="EmailGrid_PageIndexChanging">
            <EmptyDataTemplate>
                <div class="adm-empty"><i class="fa fa-envelope-o fa-2x d-block mb-2"></i>No emails match these filters.</div>
            </EmptyDataTemplate>
            <Columns>
                <asp:TemplateField ItemStyle-CssClass="cb" ItemStyle-Width="40px">
                    <HeaderTemplate><input type="checkbox" id="selectchk" title="Select all on this page" aria-label="Select all" /></HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="cbSelect" CssClass="gridCB" runat="server" />
                        <asp:Literal ID="EmailIDLt" Text='<%# Eval("ID") %>' Visible="false" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="To">
                    <ItemTemplate>
                        <div class="em-to">
                            <div class="n"><%#: If(String.IsNullOrWhiteSpace(Convert.ToString(Eval("ToName"))), Convert.ToString(Eval("ToAddress")), Convert.ToString(Eval("ToName"))) %></div>
                            <div class="a"><%#: Eval("ToAddress") %></div>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Subject">
                    <ItemTemplate>
                        <div class="em-subj" title='<%#: Eval("Subject") %>'><%#: Eval("Subject") %></div>
                        <div class="em-tags">
                            <%# If(String.IsNullOrWhiteSpace(Convert.ToString(Eval("EmailGroup"))), "", "<span class=""em-chip"">" & HttpUtility.HtmlEncode(Convert.ToString(Eval("EmailGroup"))) & "</span>") %>
                            <%# TypeChip(Eval("EmailType")) %>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate><%# StatusHtml(Eval("IsSent"), Eval("IsRead"), Eval("LastAttempt")) %></ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="When">
                    <ItemTemplate>
                        <div class="em-when"><%# WhenHtml(Eval("CreateDate"), Eval("SentDate"), Eval("ReadDate")) %></div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField ItemStyle-CssClass="text-end">
                    <ItemTemplate>
                        <a class="em-act" href='<%# ResolveUrl("~/account/email/" & Convert.ToString(Eval("ID"))) %>' target="_blank" title="View email"><i class="fa fa-eye"></i></a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <PagerSettings Position="Bottom" Mode="NumericFirstLast" PageButtonCount="10" />
            <PagerStyle CssClass="adm-pager" HorizontalAlign="Center" />
        </asp:GridView>
    </div>
    <div class="em-pageinfo"><asp:Literal ID="PageInfoLiteral" runat="server" /></div>

    <script>
        (function () {
            var boxes = $('.gridCB input');
            function count() {
                var n = boxes.filter(':checked').length;
                $('#selCount').text(n);
                $('#selBar').toggleClass('idle', n === 0);
                $('#selectchk').prop('checked', n > 0 && n === boxes.length);
            }
            $('#selectchk').on('change', function () { boxes.prop('checked', this.checked); count(); });
            boxes.on('change', count);
            $('.em-table tbody tr').on('click', function (e) {
                if ($(e.target).closest('a, input, button, label').length) return;
                var c = $(this).find('.gridCB input'); c.prop('checked', !c.prop('checked')); count();
            });
            count();
        })();
    </script>
</asp:Content>