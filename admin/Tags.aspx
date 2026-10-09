<%@ Page Title="Tags" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="Tags.aspx.vb" Inherits="IndiaBobbles.Tags" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">

    <!-- ============ Heading ============ -->
    <div class="adm-head">
        <div>
            <h1>Tags</h1>
            <p>Tags group products into category pages, like indiabobbles.com/tag/collectibles.</p>
        </div>
        <a href="#tagForm" class="btn btn-ib" onclick="ibNewTag(); return true;"><i class="fa fa-plus"></i> New tag</a>
    </div>

    <asp:Label ID="MessageLabel" runat="server" CssClass="adm-flash" Visible="false" EnableViewState="false" />

    <div class="row g-3">
        <!-- ============ List ============ -->
        <div class="col-xl-8">
            <div class="tg-bar">
                <div class="search"><i class="fa fa-search"></i>
                    <input type="search" id="tgSearch" placeholder="Search tags…" aria-label="Search tags" />
                </div>
                <div class="tg-tabs" id="tgTabs">
                    <button type="button" data-f="live" class="on">Live <span class="c" data-c="live"></span></button>
                    <button type="button" data-f="0">Active <span class="c" data-c="0"></span></button>
                    <button type="button" data-f="1">Inactive <span class="c" data-c="1"></span></button>
                    <button type="button" data-f="2">Deleted <span class="c" data-c="2"></span></button>
                </div>
            </div>

            <div class="adm-card table-responsive">
                <table class="table adm-table tg-table mb-0">
                    <thead>
                        <tr><th>Tag</th><th>Description</th><th class="text-center">Products</th><th>Status</th><th class="text-end"></th></tr>
                    </thead>
                    <tbody id="tgBody">
                        <asp:Repeater ID="TagRepeater" runat="server">
                            <ItemTemplate>
                                <tr class='<%# "st-" & Eval("Status") & If(Convert.ToInt32(Eval("ID")) = EditingId, " editing", "") %>'
                                    data-status='<%# Eval("Status") %>'
                                    data-search='<%#: (Convert.ToString(Eval("DisplayName")) & " " & Convert.ToString(Eval("UrlName")) & " " & Convert.ToString(Eval("Description"))).ToLower() %>'>
                                    <td>
                                        <div class="tg-name"><%#: Eval("DisplayName") %><%# If(IsSystemTag(Eval("UrlName")), "<span class=""tg-sys"" title=""Used by the website, e.g. the home page"">Website</span>", "") %></div>
                                        <a class="tg-url" target="_blank" rel="noopener" href='<%#: ResolveUrl("~/tag/" & Convert.ToString(Eval("UrlName"))) %>'>/tag/<%#: Eval("UrlName") %> <i class="fa fa-external-link"></i></a>
                                    </td>
                                    <td class="tg-desc"><%# If(String.IsNullOrWhiteSpace(Convert.ToString(Eval("Description"))), "<span class=""none"">–</span>", HttpUtility.HtmlEncode(Convert.ToString(Eval("Description")))) %></td>
                                    <td class="text-center">
                                        <span class='<%# "tg-count" & If(Convert.ToInt32(Eval("ProductCount")) = 0, " zero", "") %>'
                                              title='<%# If(Convert.ToInt32(Eval("ProductCount")) = 0, "No products – this page will be empty", "Products with this tag") %>'><%# Eval("ProductCount") %></span>
                                    </td>
                                    <td><span class='<%# "ts ts-" & Eval("Status") %>'><%# StatusText(Eval("Status")) %></span></td>
                                    <td class="text-end text-nowrap">
                                        <asp:LinkButton runat="server" CssClass="tg-act" CommandName="EditTag" CommandArgument='<%# Eval("ID") %>' ToolTip="Edit" CausesValidation="false"><i class="fa fa-pencil"></i></asp:LinkButton>
                                        <asp:LinkButton runat="server" CssClass="tg-act" CommandName="Toggle" CommandArgument='<%# Eval("ID") %>' CausesValidation="false"
                                            Visible='<%# Convert.ToInt32(Eval("Status")) <> 2 %>'
                                            ToolTip='<%# If(Convert.ToInt32(Eval("Status")) = 0, "Hide (make inactive)", "Activate") %>'>
                                            <i class='<%# If(Convert.ToInt32(Eval("Status")) = 0, "fa fa-eye-slash", "fa fa-eye") %>'></i></asp:LinkButton>
                                        <asp:LinkButton runat="server" CssClass="tg-act del" CommandName="DeleteTag" CommandArgument='<%# Eval("ID") %>' CausesValidation="false"
                                            Visible='<%# Convert.ToInt32(Eval("Status")) <> 2 %>' ToolTip="Delete"
                                            OnClientClick='<%# DeleteConfirm(Eval("UrlName"), Eval("ProductCount")) %>'><i class="fa fa-trash"></i></asp:LinkButton>
                                        <asp:LinkButton runat="server" CssClass="tg-act" CommandName="Restore" CommandArgument='<%# Eval("ID") %>' CausesValidation="false"
                                            Visible='<%# Convert.ToInt32(Eval("Status")) = 2 %>' ToolTip="Restore as inactive"><i class="fa fa-undo"></i></asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
                <div class="adm-empty d-none" id="tgNone"><i class="fa fa-tags"></i> No tags here.</div>
            </div>
            <div class="mt-2" style="font-size:.8rem;color:#6b5f72;">
                <span class="tg-sys" style="margin-left:0;">Website</span> tags (highlight, trending) are used by the website itself, so take care renaming or deleting them.
                Add tags to a product from its detail page.
            </div>
        </div>

        <!-- ============ Add / edit form ============ -->
        <div class="col-xl-4">
            <div class="adm-sticky">
                <div class="adm-panel tg-form" id="tagForm">
                    <h2><i class='<%= If(EditingId > 0, "fa fa-pencil", "fa fa-plus") %>'></i> <asp:Literal ID="FormTitleLiteral" runat="server" Text="New tag" /></h2>
                    <asp:HiddenField ID="EditIdHidden" runat="server" Value="0" />
                    <asp:HiddenField ID="UserIDHidden" runat="server" />
                    <asp:Panel ID="EditingNote" runat="server" CssClass="editing-note" Visible="false">
                        <i class="fa fa-info-circle"></i> Changing the URL name breaks old links to this tag page.
                    </asp:Panel>

                    <div class="mb-3">
                        <label for="DisplayNameTextBox" class="form-label">Display name <span class="req">*</span></label>
                        <asp:TextBox ID="DisplayNameTextBox" ClientIDMode="Static" MaxLength="100" CssClass="form-control" runat="server" placeholder="e.g. Bollywood Legends" autocomplete="off"></asp:TextBox>
                        <asp:RequiredFieldValidator ControlToValidate="DisplayNameTextBox" Display="Dynamic" ValidationGroup="valgrp" ID="RequiredFieldValidator3" runat="server" CssClass="val" ErrorMessage="Enter a display name"></asp:RequiredFieldValidator>
                        <div class="hint">Shown to customers as the page heading.</div>
                    </div>

                    <div class="mb-3">
                        <label for="UrlNameTextBox" class="form-label">URL name <span class="req">*</span></label>
                        <div class="slug-row">
                            <span class="pre">/tag/</span>
                            <asp:TextBox ID="UrlNameTextBox" ClientIDMode="Static" MaxLength="300" CssClass="form-control" runat="server" placeholder="bollywood-legends" autocomplete="off"></asp:TextBox>
                        </div>
                        <asp:RequiredFieldValidator ControlToValidate="UrlNameTextBox" Display="Dynamic" ValidationGroup="valgrp" ID="RequiredFieldValidator1" runat="server" CssClass="val" ErrorMessage="Enter a URL name"></asp:RequiredFieldValidator>
                        <asp:Label ID="UrlErrorLabel" runat="server" CssClass="val" Visible="false" EnableViewState="false" />
                        <div class="hint">Filled in from the display name. Lowercase letters, numbers and hyphens.</div>
                    </div>

                    <div class="mb-3">
                        <label for="DescriptionTextBox" class="form-label">Description</label>
                        <asp:TextBox ID="DescriptionTextBox" ClientIDMode="Static" MaxLength="2000" TextMode="MultiLine" Rows="3" CssClass="form-control" runat="server" placeholder="Optional – a line about this collection or a note for yourself"></asp:TextBox>
                    </div>

                    <div class="mb-2">
                        <label for="StatusDropdown" class="form-label">Status</label>
                        <asp:DropDownList ID="StatusDropdown" ClientIDMode="Static" CssClass="form-select" runat="server">
                            <asp:ListItem Text="Active – page is live" Value="0"></asp:ListItem>
                            <asp:ListItem Text="Inactive – hidden" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Deleted" Value="2"></asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="tg-preview" aria-hidden="true">
                        <div class="lbl">Preview</div>
                        <span class="chip" id="pvName">Tag name</span>
                        <span class="link">indiabobbles.com/tag/<b id="pvUrl">tag-name</b></span>
                    </div>

                    <asp:Button ID="SaveButton" runat="server" Text="Create tag" ValidationGroup="valgrp" CssClass="btn btn-ib w-100" CausesValidation="true" />
                    <asp:LinkButton ID="CancelButton" runat="server" CssClass="d-block text-center mt-2 text-muted" CausesValidation="false" Visible="false">Cancel editing</asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <script>
        (function () {
            var dn = $('#DisplayNameTextBox'), url = $('#UrlNameTextBox');
            var editing = $('#<%= EditIdHidden.ClientID %>').val() !== '0';
            var urlTouched = editing || url.val() !== '';

            function slug(s) {
                return (s || '').toLowerCase().replace(/&/g, ' and ').replace(/[^a-z0-9]+/g, '-').replace(/^-+|-+$/g, '').substring(0, 80);
            }
            function preview() {
                $('#pvName').text(dn.val() || 'Tag name');
                $('#pvUrl').text(url.val() || 'tag-name');
            }
            dn.on('input', function () { if (!urlTouched) url.val(slug(dn.val())); preview(); });
            url.on('input', function () { urlTouched = url.val() !== ''; preview(); });
            url.on('blur', function () { url.val(slug(url.val())); preview(); });
            preview();

            // ---------- Filters ----------
            var rows = $('#tgBody tr'), f = 'live';
            try { f = sessionStorage.getItem('ibTagFilter') || 'live'; } catch (e) { }
            ['live', '0', '1', '2'].forEach(function (k) {
                $('[data-c="' + k + '"]').text(rows.filter(function () { var s = '' + $(this).data('status'); return k === 'live' ? s !== '2' : s === k; }).length);
            });
            function apply() {
                var q = ($('#tgSearch').val() || '').toLowerCase().trim(), shown = 0;
                rows.each(function () {
                    var s = '' + $(this).data('status');
                    var ok = (f === 'live' ? s !== '2' : s === f) && ('' + $(this).data('search')).indexOf(q) > -1;
                    $(this).toggle(ok); if (ok) shown++;
                });
                $('#tgTabs button').removeClass('on').filter('[data-f="' + f + '"]').addClass('on');
                $('#tgNone').toggleClass('d-none', shown > 0);
                try { sessionStorage.setItem('ibTagFilter', f); } catch (e) { }
            }
            $('#tgTabs button').on('click', function () { f = '' + $(this).data('f'); apply(); });
            $('#tgSearch').on('input', apply);
            apply();

            window.ibNewTag = function () {
                if (editing) { location.href = 'tags.aspx'; return; }
                dn.focus();
            };
        })();
    </script>
</asp:Content>