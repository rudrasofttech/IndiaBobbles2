<%@ Page Title="Coupon Codes" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="ManageCoupons.aspx.vb" Inherits="IndiaBobbles.ManageCoupons" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">

    <!-- ============ Heading ============ -->
    <div class="adm-head">
        <div>
            <h1>Coupon Codes</h1>
            <p>Create discount codes for newsletters, WhatsApp offers and returning customers.</p>
        </div>
        <a href="#couponForm" class="btn btn-ib" onclick="ibNewCoupon(); return true;"><i class="fa fa-plus"></i> New coupon</a>
    </div>

    <asp:Label ID="MessageLabel" runat="server" CssClass="adm-flash" Visible="false" EnableViewState="false" />

    <!-- ============ Stats ============ -->
    <div class="cp-stats">
        <div class="cp-stat"><span class="n"><asp:Literal ID="ActiveCountLiteral" runat="server" Text="0" /></span><span class="l">Active coupons</span></div>
        <div class="cp-stat"><span class="n"><asp:Literal ID="UsedCountLiteral" runat="server" Text="0" /></span><span class="l">Paid orders with a coupon</span></div>
        <div class="cp-stat dark"><span class="n"><asp:Literal ID="RevenueLiteral" runat="server" Text="₹0" /></span><span class="l">Sales from coupon orders</span></div>
        <div class="cp-stat"><span class="n" style="font-size:1.1rem;"><asp:Literal ID="TopLiteral" runat="server" Text="–" /></span><span class="l">Most used code</span></div>
    </div>

    <div class="row g-3">
        <!-- ============ List ============ -->
        <div class="col-xl-8">
            <div class="cp-bar">
                <div class="search"><i class="fa fa-search"></i>
                    <input type="search" id="cpSearch" placeholder="Search code…" aria-label="Search coupons" />
                </div>
                <div class="cp-tabs" id="cpTabs">
                    <button type="button" data-f="live" class="on">Live <span class="c" data-c="live"></span></button>
                    <button type="button" data-f="0">Active <span class="c" data-c="0"></span></button>
                    <button type="button" data-f="1">Inactive <span class="c" data-c="1"></span></button>
                    <button type="button" data-f="2">Deleted <span class="c" data-c="2"></span></button>
                </div>
            </div>

            <div class="adm-card table-responsive">
                <table class="table adm-table cp-table mb-0">
                    <thead>
                        <tr><th>Code</th><th>Discount</th><th>Rules</th><th>Status</th><th>Used</th><th>Created</th><th class="text-end"></th></tr>
                    </thead>
                    <tbody id="cpBody">
                        <asp:Repeater ID="CouponRepeater" runat="server">
                            <ItemTemplate>
                                <tr class='<%# "st-" & Eval("Status") & If(Convert.ToInt32(Eval("Id")) = EditingId, " editing", "") %>' data-status='<%# Eval("Status") %>' data-code='<%#: Convert.ToString(Eval("Name")).ToLower() %>'>
                                    <td>
                                        <span class="cp-code"><%#: Eval("Name") %>
                                            <button type="button" class="cp-copy" data-copy='<%#: Eval("Name") %>' title="Copy code"><i class="fa fa-clone"></i></button>
                                        </span>
                                    </td>
                                    <td>
                                        <div class="cp-off"><%# OffText(Eval("IsPercent"), Eval("Value")) %></div>
                                        <div class="cp-type"><%# If(Convert.ToBoolean(Eval("IsPercent")), "Percent", "Flat amount") %></div>
                                    </td>
                                    <td style="max-width:220px;"><%# RulesHtml(Container.DataItem) %></td>
                                    <td><%# StatusHtml(Container.DataItem) %></td>
                                    <td class="cp-use"><%# UsageHtml(Eval("Id"), Eval("Name")) %></td>
                                    <td class="sm"><%# Convert.ToDateTime(Eval("DateCreated")).ToString("d MMM yyyy") %></td>
                                    <td class="text-end text-nowrap">
                                        <asp:LinkButton runat="server" CssClass="cp-act" CommandName="EditCoupon" CommandArgument='<%# Eval("Id") %>' ToolTip="Edit" CausesValidation="false"><i class="fa fa-pencil"></i></asp:LinkButton>
                                        <asp:LinkButton runat="server" CssClass="cp-act" CommandName="Toggle" CommandArgument='<%# Eval("Id") %>' CausesValidation="false"
                                            Visible='<%# Convert.ToInt32(Eval("Status")) <> 2 %>'
                                            ToolTip='<%# If(Convert.ToInt32(Eval("Status")) = 0, "Pause (make inactive)", "Activate") %>'>
                                            <i class='<%# If(Convert.ToInt32(Eval("Status")) = 0, "fa fa-pause", "fa fa-play") %>'></i></asp:LinkButton>
                                        <asp:LinkButton runat="server" CssClass="cp-act del" CommandName="DeleteCoupon" CommandArgument='<%# Eval("Id") %>' CausesValidation="false"
                                            Visible='<%# Convert.ToInt32(Eval("Status")) <> 2 %>' ToolTip="Delete"
                                            OnClientClick="return confirm('Delete this coupon? Customers will no longer be able to use it.');"><i class="fa fa-trash"></i></asp:LinkButton>
                                        <asp:LinkButton runat="server" CssClass="cp-act" CommandName="Restore" CommandArgument='<%# Eval("Id") %>' CausesValidation="false"
                                            Visible='<%# Convert.ToInt32(Eval("Status")) = 2 %>' ToolTip="Restore as inactive"><i class="fa fa-undo"></i></asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
                <div class="adm-empty d-none" id="cpNone"><i class="fa fa-ticket"></i> No coupons here.</div>
            </div>
            <div class="hint mt-2" style="font-size:.8rem;color:#6b5f72;">"Used" counts paid, shipped and completed orders where the coupon was applied. "First order" and "Past buyers" are checked by the customer's phone or email.</div>
        </div>

        <!-- ============ Add / edit form ============ -->
        <div class="col-xl-4">
            <div class="adm-sticky">
                <div class="adm-panel cp-form" id="couponForm">
                    <h2><i class='<%= If(EditingId > 0, "fa fa-pencil", "fa fa-plus") %>'></i> <asp:Literal ID="FormTitleLiteral" runat="server" Text="New coupon" /></h2>
                    <asp:HiddenField ID="EditIdHidden" runat="server" Value="0" />
                    <asp:Panel ID="EditingNote" runat="server" CssClass="editing-note" Visible="false">
                        <i class="fa fa-info-circle"></i> Editing a coupon changes it for everyone who has it.
                    </asp:Panel>

                    <div class="mb-3">
                        <label for="CouponCodeTextBox" class="form-label">Coupon code <span class="req">*</span></label>
                        <div class="code-row">
                            <asp:TextBox ID="CouponCodeTextBox" runat="server" ClientIDMode="Static" MaxLength="100" CssClass="form-control" placeholder="e.g. WELCOMEBACK" autocomplete="off"></asp:TextBox>
                            <button type="button" class="btn btn-ib-o btn-sm text-nowrap" id="genBtn" title="Suggest a random code"><i class="fa fa-random"></i></button>
                        </div>
                        <asp:RequiredFieldValidator ID="CouponCodeReqVal" runat="server" ValidationGroup="CouponGrp" ControlToValidate="CouponCodeTextBox" CssClass="val" ErrorMessage="Enter a code" Display="Dynamic"></asp:RequiredFieldValidator>
                        <asp:Label ID="CodeErrorLabel" runat="server" CssClass="val" Visible="false" EnableViewState="false" />
                        <div class="hint">Short and easy to type. Saved in capitals.</div>
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Discount type</label>
                        <div class="seg">
                            <input type="radio" name="cpType" id="tFlat" value="0" <%= If(TypeDropDown.SelectedValue = "0", "checked", "") %> /><label for="tFlat">₹ Flat amount</label>
                            <input type="radio" name="cpType" id="tPct" value="1" <%= If(TypeDropDown.SelectedValue = "1", "checked", "") %> /><label for="tPct">% Percent</label>
                        </div>
                        <asp:DropDownList ID="TypeDropDown" runat="server" ClientIDMode="Static" CssClass="d-none">
                            <asp:ListItem Text="Flat Amount" Value="0" />
                            <asp:ListItem Text="Percent" Value="1" />
                        </asp:DropDownList>
                    </div>

                    <div class="mb-3">
                        <label for="ValueTextBox" class="form-label">Discount value <span class="req">*</span></label>
                        <div class="in-grp">
                            <span class="pre" id="valPre">₹</span>
                            <asp:TextBox ID="ValueTextBox" runat="server" ClientIDMode="Static" MaxLength="12" CssClass="form-control" inputmode="decimal" placeholder="100"></asp:TextBox>
                        </div>
                        <asp:RequiredFieldValidator ID="ValueReqVal" runat="server" ValidationGroup="CouponGrp" ControlToValidate="ValueTextBox" CssClass="val" ErrorMessage="Enter a value" Display="Dynamic"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="ValueRegexVal" runat="server" ValidationGroup="CouponGrp" ControlToValidate="ValueTextBox" CssClass="val" ErrorMessage="Numbers only, up to 2 decimals" ValidationExpression="^\d+(\.\d{1,2})?$" Display="Dynamic"></asp:RegularExpressionValidator>
                        <asp:Label ID="ValueErrorLabel" runat="server" CssClass="val" Visible="false" EnableViewState="false" />
                    </div>

                    <div class="mb-2">
                        <label for="StatusDropDown" class="form-label">Status</label>
                        <asp:DropDownList ID="StatusDropDown" runat="server" ClientIDMode="Static" CssClass="form-select">
                            <asp:ListItem Text="Active – customers can use it" Value="0" />
                            <asp:ListItem Text="Inactive – paused" Value="1" />
                            <asp:ListItem Text="Deleted" Value="2" />
                        </asp:DropDownList>
                    </div>

                    <!-- ---------- Rules ---------- -->
                    <div class="sec">Rules</div>

                    <div class="mb-3">
                        <label class="form-label">Who can use it</label>
                        <div class="seg3">
                            <input type="radio" name="cpWho" id="wAll" value="0" <%= If(WhoDropDown.SelectedValue = "0", "checked", "") %> /><label for="wAll">Everyone</label>
                            <input type="radio" name="cpWho" id="wFirst" value="1" <%= If(WhoDropDown.SelectedValue = "1", "checked", "") %> /><label for="wFirst">First order</label>
                            <input type="radio" name="cpWho" id="wBack" value="2" <%= If(WhoDropDown.SelectedValue = "2", "checked", "") %> /><label for="wBack">Past buyers</label>
                        </div>
                        <asp:DropDownList ID="WhoDropDown" runat="server" ClientIDMode="Static" CssClass="d-none">
                            <asp:ListItem Text="Everyone" Value="0" />
                            <asp:ListItem Text="First order only" Value="1" />
                            <asp:ListItem Text="Past buyers only" Value="2" />
                        </asp:DropDownList>
                        <div class="hint" id="whoHint">Anyone can use this code.</div>
                    </div>

                    <div class="mb-3 two">
                        <div>
                            <label for="MinItemsTextBox" class="form-label">Min. bobbleheads</label>
                            <asp:TextBox ID="MinItemsTextBox" runat="server" ClientIDMode="Static" MaxLength="2" CssClass="form-control" inputmode="numeric" placeholder="Any"></asp:TextBox>
                        </div>
                        <div>
                            <label for="MinAmountTextBox" class="form-label">Min. cart value</label>
                            <div class="in-grp">
                                <span class="pre">₹</span>
                                <asp:TextBox ID="MinAmountTextBox" runat="server" ClientIDMode="Static" MaxLength="9" CssClass="form-control" inputmode="decimal" placeholder="Any"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="mb-3 two">
                        <div>
                            <label for="ValidFromTextBox" class="form-label">Starts</label>
                            <asp:TextBox ID="ValidFromTextBox" runat="server" ClientIDMode="Static" TextMode="Date" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div>
                            <label for="ValidToTextBox" class="form-label">Ends (incl.)</label>
                            <asp:TextBox ID="ValidToTextBox" runat="server" ClientIDMode="Static" TextMode="Date" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="hint mb-3" style="margin-top:-8px;">Leave dates empty for no limit.</div>

                    <div class="mb-3">
                        <div class="sw">
                            <asp:CheckBox ID="ShowOnCartCheckBox" runat="server" ClientIDMode="Static" />
                            <label for="ShowOnCartCheckBox">Offer it on the cart page
                                <small>Shown as a one-tap offer when the cart qualifies. Best saving wins.</small>
                            </label>
                        </div>
                        <div class="mt-2" id="pitchRow">
                            <label for="PitchTextBox" class="form-label">Offer line</label>
                            <asp:TextBox ID="PitchTextBox" runat="server" ClientIDMode="Static" MaxLength="150" CssClass="form-control" placeholder="e.g. on your first bobblehead"></asp:TextBox>
                            <div class="hint">Shown after the amount: "₹100 off <i>on your first bobblehead</i>".</div>
                        </div>
                    </div>
                    <asp:Label ID="RulesErrorLabel" runat="server" CssClass="val d-block mb-2" Visible="false" EnableViewState="false" />

                    <div class="cp-ticket" aria-hidden="true">
                        <div class="t1">Preview</div>
                        <div class="t2" id="pvCode">YOURCODE</div>
                        <div class="t3" id="pvOff">₹0 off your order</div>
                        <div class="t4" id="pvRules"></div>
                    </div>

                    <asp:Button ID="SaveButton" runat="server" CssClass="btn btn-ib w-100" Text="Create coupon" ValidationGroup="CouponGrp" />
                    <asp:LinkButton ID="CancelButton" runat="server" CssClass="d-block text-center mt-2 text-muted" CausesValidation="false" Visible="false">Cancel editing</asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <script>
        (function () {
            var code = $('#CouponCodeTextBox'), val = $('#ValueTextBox'), type = $('#TypeDropDown'), who = $('#WhoDropDown');
            var whoHints = {
                '0': 'Anyone can use this code.',
                '1': 'Only for someone with no earlier paid order (checked by phone or email).',
                '2': 'Only for someone who has ordered before (checked by phone or email).'
            };

            // ---------- Live preview ----------
            function inr(n) { return '₹' + Number(n).toLocaleString('en-IN'); }
            function preview() {
                var pct = type.val() === '1', v = parseFloat(val.val()) || 0;
                var pitch = ($('#PitchTextBox').val() || '').trim();
                $('#valPre').text(pct ? '%' : '₹');
                $('#pvCode').text((code.val() || 'YOURCODE').toUpperCase());
                $('#pvOff').text((pct ? (v + '% off') : (inr(v) + ' off')) + ' ' + (pitch || 'your order'));

                var r = [];
                var w = who.val();
                if (w === '1') r.push('First order only');
                if (w === '2') r.push('Past buyers only');
                var mi = parseInt($('#MinItemsTextBox').val(), 10);
                if (mi > 1) r.push(mi + '+ bobbleheads');
                var ma = parseFloat($('#MinAmountTextBox').val());
                if (ma > 0) r.push('cart ' + inr(ma) + '+');
                var to = $('#ValidToTextBox').val();
                if (to) r.push('till ' + new Date(to + 'T00:00:00').toLocaleDateString('en-IN', { day: 'numeric', month: 'short', year: 'numeric' }));
                if ($('#ShowOnCartCheckBox').is(':checked')) r.push('shown on cart');
                $('#pvRules').text(r.join(' · '));

                $('#whoHint').text(whoHints[w] || whoHints['0']);
                $('#pitchRow').toggle($('#ShowOnCartCheckBox').is(':checked'));
            }
            $('input[name=cpType]').on('change', function () { type.val(this.value); preview(); });
            $('input[name=cpWho]').on('change', function () { who.val(this.value); preview(); });
            code.on('input', function () { var p = this.selectionStart; this.value = this.value.toUpperCase(); this.setSelectionRange(p, p); preview(); });
            $('#ValueTextBox, #MinItemsTextBox, #MinAmountTextBox, #PitchTextBox, #ValidToTextBox').on('input change', preview);
            $('#ShowOnCartCheckBox').on('change', preview);
            $('#MinItemsTextBox').on('input', function () { this.value = this.value.replace(/\D/g, ''); });
            preview();

            // ---------- Random code ----------
            $('#genBtn').on('click', function () {
                var a = 'ABCDEFGHJKLMNPQRSTUVWXYZ23456789', s = 'IB';
                for (var i = 0; i < 6; i++) s += a.charAt(Math.floor(Math.random() * a.length));
                code.val(s); preview(); code.focus();
            });

            // ---------- Copy code ----------
            $('.cp-copy').on('click', function () {
                var b = $(this), t = b.data('copy');
                (navigator.clipboard ? navigator.clipboard.writeText(t) : Promise.reject()).then(function () {
                    b.html('<i class="fa fa-check"></i>'); setTimeout(function () { b.html('<i class="fa fa-clone"></i>'); }, 1200);
                }, function () { prompt('Copy this code:', t); });
            });

            // ---------- Filters ----------
            var rows = $('#cpBody tr'), f = 'live';
            try { f = sessionStorage.getItem('ibCpFilter') || 'live'; } catch (e) { }
            ['live', '0', '1', '2'].forEach(function (k) {
                $('[data-c="' + k + '"]').text(rows.filter(function () { var s = '' + $(this).data('status'); return k === 'live' ? s !== '2' : s === k; }).length);
            });
            function apply() {
                var q = ($('#cpSearch').val() || '').toLowerCase().trim(), shown = 0;
                rows.each(function () {
                    var s = '' + $(this).data('status');
                    var ok = (f === 'live' ? s !== '2' : s === f) && ('' + $(this).data('code')).indexOf(q) > -1;
                    $(this).toggle(ok); if (ok) shown++;
                });
                $('#cpTabs button').removeClass('on').filter('[data-f="' + f + '"]').addClass('on');
                $('#cpNone').toggleClass('d-none', shown > 0);
                try { sessionStorage.setItem('ibCpFilter', f); } catch (e) { }
            }
            $('#cpTabs button').on('click', function () { f = '' + $(this).data('f'); apply(); });
            $('#cpSearch').on('input', apply);
            apply();

            // ---------- "New coupon" button clears the form ----------
            window.ibNewCoupon = function () {
                if ($('#<%= EditIdHidden.ClientID %>').val() !== '0') { location.href = 'managecoupons.aspx'; return; }
                code.focus();
            };
        })();
    </script>
</asp:Content>