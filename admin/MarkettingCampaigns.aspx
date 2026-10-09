<%@ Page Title="Marketting Campaigns" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="MarkettingCampaigns.aspx.vb" Inherits="IndiaBobbles.BulkEmail" ValidateRequest="false" %>
<%@ Import Namespace="System.Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">

    <!-- ============ Heading ============ -->
    <div class="adm-head">
        <div>
            <h1>Marketting Campaign</h1>
            <p>Write one email and send it to members, past buyers or a pasted list.</p>
        </div>
        <a runat="server" href="~/admin/emails.aspx" class="btn btn-ib-o"><i class="fa fa-envelope"></i> Sent emails</a>
    </div>

    <asp:Panel ID="StatusPanel" runat="server" Visible="false" CssClass="alert" role="alert">
        <asp:Literal ID="StatusLiteral" runat="server"></asp:Literal>
    </asp:Panel>

    <asp:ValidationSummary runat="server" ValidationGroup="BulkMailGrp" CssClass="adm-error" HeaderText="Please fix these before sending:" DisplayMode="BulletList" />

    <div class="row g-3">
        <!-- ============ Left: compose ============ -->
        <div class="col-xl-8">

            <!-- 1. Recipients -->
            <div class="adm-panel">
                <div class="be-head">
                    <h2><span class="be-step">1</span> Who should get it?</h2>
                    <div class="be-src">
                        <button type="button" class="btn btn-ib-o btn-sm" data-bs-toggle="modal" data-bs-target="#memberPickerModal">
                            <i class="fa fa-users"></i> Members <span class="be-n"><asp:Literal ID="MemberCountLiteral" runat="server"></asp:Literal></span></button>
                        <button type="button" class="btn btn-ib-o btn-sm" data-bs-toggle="modal" data-bs-target="#orderPickerModal">
                            <i class="fa fa-shopping-bag"></i> Past buyers <span class="be-n"><asp:Literal ID="OrderCountLiteral" runat="server"></asp:Literal></span></button>
                    </div>
                </div>

                <asp:TextBox ID="RecipientsTextBox" runat="server" CssClass="form-control be-rcpt" TextMode="MultiLine" Rows="8"
                    placeholder='Use JSON lines, e.g.&#10;{"Email":"priya@example.com","Name":"Priya","OrderTotal":"1299","OrderDate":"09 Oct 2026"}'></asp:TextBox>

                <div class="hint">One JSON object per line (or JSON array). Any property can be used in template tokens like {{Name}}, {{OrderTotal}}, {{DOB}}, etc.</div>
            </div>

            <!-- 2. Content -->
            <div class="adm-panel">
                <h2><span class="be-step">2</span> What do you want to say?</h2>

                <div class="mb-3">
                    <div class="d-flex justify-content-between">
                        <label for="<%= SubjectTextBox.ClientID %>" class="form-label">Subject <span class="req">*</span></label>
                        <span class="be-cc" id="subjCount">0 / 60</span>
                    </div>
                    <asp:TextBox ID="SubjectTextBox" runat="server" CssClass="form-control" MaxLength="200" placeholder="e.g. New Bollywood bobbleheads are here 🎬"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="SubjectReqVal" runat="server" ControlToValidate="SubjectTextBox" ValidationGroup="BulkMailGrp" CssClass="val" ErrorMessage="Subject is required." Display="Dynamic"></asp:RequiredFieldValidator>
                    <div class="hint">Keep it under ~60 characters so it isn't cut off on phones.</div>
                </div>

                <div class="be-tabs" role="tablist">
                    <button type="button" class="btn btn-sm btn-secondary on" data-tab="write"><i class="fa fa-pencil"></i> Write</button>
                    <button type="button" class="btn btn-sm btn-secondary" data-tab="preview"><i class="fa fa-eye"></i> Preview</button>
                </div>

                <div id="tabWrite">
                    <div class="be-tools" aria-label="Formatting">
                        <button type="button" data-ins="b" title="Bold"><i class="fa fa-bold"></i></button>
                        <button type="button" data-ins="i" title="Italic"><i class="fa fa-italic"></i></button>
                        <button type="button" data-ins="h" title="Heading"><i class="fa fa-header"></i></button>
                        <button type="button" data-ins="p" title="Paragraph"><i class="fa fa-paragraph"></i></button>
                        <button type="button" data-ins="ul" title="Bullet list"><i class="fa fa-list-ul"></i></button>
                        <span class="sep"></span>
                        <button type="button" data-ins="a" title="Link"><i class="fa fa-link"></i></button>
                        <button type="button" data-ins="img" title="Image"><i class="fa fa-image"></i></button>
                        <button type="button" data-ins="btn" title="Button"><i class="fa fa-hand-pointer-o"></i> Button</button>
                        <span class="sep"></span>
                        <button type="button" data-ins="tpl" title="Insert a starter layout"><i class="fa fa-file-text-o"></i> Starter template</button>
                    </div>
                    <asp:TextBox ID="MessageTextBox" runat="server" CssClass="form-control be-msg" TextMode="MultiLine" Rows="14"
                        placeholder="Write your message. HTML is allowed – use the buttons above for formatting."></asp:TextBox>
                    <asp:RequiredFieldValidator ID="MessageReqVal" runat="server" ControlToValidate="MessageTextBox" ValidationGroup="BulkMailGrp" CssClass="val" ErrorMessage="Message is required." Display="Dynamic"></asp:RequiredFieldValidator>
                    <div class="hint">Plain text works too – blank lines become paragraphs in the preview. Your draft is saved in this browser as you type.</div>
                </div>

                <div id="tabPreview" class="d-none">
                    <div class="be-inbox">
                        <div class="from"><b>India Bobbles</b> <span>&lt;indiabobbles@rudrasofttech.com&gt;</span></div>
                        <div class="subj" id="pvSubj">(no subject)</div>
                    </div>
                    <div class="be-device">
                        <button type="button" class="on" data-w="100%"><i class="fa fa-desktop"></i></button>
                        <button type="button" data-w="380px"><i class="fa fa-mobile"></i></button>
                    </div>
                    <div class="be-frame-wrap"><iframe id="pvFrame" title="Email preview" sandbox=""></iframe></div>
                </div>
            </div>
        </div>

        <!-- ============ Right: summary + send ============ -->
        <div class="col-xl-4">
            <div class="adm-sticky">
                <div class="adm-panel">
                    <h2><span class="be-step">3</span> Review &amp; send</h2>

                    <dl class="be-sum">
                        <dt>To</dt><dd><b id="sumTo">0</b> recipients</dd>
                        <dt>Subject</dt><dd id="sumSubj" class="text-muted">Not set</dd>
                        <dt>Message</dt><dd id="sumMsg" class="text-muted">Empty</dd>
                    </dl>

                    <div class="mb-3">
                        <label for="<%= EmailGroupTextBox.ClientID %>" class="form-label">Email group</label>
                        <asp:TextBox ID="EmailGroupTextBox" runat="server" CssClass="form-control" MaxLength="100" Text="ManualBulk" list="groupList"></asp:TextBox>
                        <datalist id="groupList">
                            <option value="ManualBulk"></option>
                            <option value="Newsletter"></option>
                            <option value="NewArrivals"></option>
                            <option value="Offer"></option>
                            <option value="ReEngagement"></option>
                        </datalist>
                        <asp:RequiredFieldValidator ID="EmailGroupReqVal" runat="server" ControlToValidate="EmailGroupTextBox" ValidationGroup="BulkMailGrp" CssClass="val" ErrorMessage="Email group is required." Display="Dynamic"></asp:RequiredFieldValidator>
                        <div class="hint">A label to find this campaign later in Sent emails, e.g. <i>Diwali2026</i>.</div>
                    </div>

                    <ul class="be-check" id="checklist">
                        <li data-k="to"><i class="fa"></i> At least one valid recipient</li>
                        <li data-k="bad"><i class="fa"></i> No invalid addresses</li>
                        <li data-k="subj"><i class="fa"></i> Subject written</li>
                        <li data-k="msg"><i class="fa"></i> Message written</li>
                        <li data-k="unsub"><i class="fa"></i> Mentions how to unsubscribe</li>
                    </ul>

                    <asp:Button ID="SubmitButton" runat="server" CssClass="btn btn-ib w-100 be-send" Text="Send email" ValidationGroup="BulkMailGrp"
                        OnClick="SubmitButton_Click" OnClientClick="if (!ibConfirmSend()) return false;" />
                    <div class="hint text-center mt-2">You'll be asked to confirm before anything is sent.</div>
                </div>

                <div class="adm-panel be-tips">
                    <h2><i class="fa fa-lightbulb-o"></i> Tips</h2>
                    <ul>
                        <li>Send yourself a test first: put only your own address in the list.</li>
                        <li>Only email people who've bought or signed up, and include an unsubscribe line.</li>
                        <li>One clear button (“Shop now”) gets more clicks than many links.</li>
                    </ul>
                </div>
            </div>
        </div>
    </div>

    <!-- ============ Members modal ============ -->
    <div class="modal fade be-modal" id="memberPickerModal" tabindex="-1" aria-labelledby="memberPickerTitle" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 id="memberPickerTitle" class="modal-title"><i class="fa fa-users"></i> Select members</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="be-mbar">
                        <div class="search"><i class="fa fa-search"></i>
                            <input type="search" id="memberSearchText" placeholder="Search name or email…" aria-label="Search members" />
                        </div>
                        <button type="button" id="memberSelectAllVisibleBtn" class="btn btn-sm btn-ib-o">Select shown</button>
                        <button type="button" id="memberClearVisibleBtn" class="btn btn-sm btn-light">Clear shown</button>
                    </div>
                    <table class="table adm-table be-pick mb-0">
                        <thead>
    <tr>
        <th style="width: 44px;"></th>
        <th>Name</th>
        <th>Email</th>
        <th>DOB</th>
        <th>Mobile</th>
        <th>Country</th>
        <th>Created</th>
    </tr>
</thead>
<tbody id="memberTableBody">
    <asp:Repeater ID="MemberRepeater" runat="server">
        <ItemTemplate>
            <tr class="member-row" data-search='<%# HttpUtility.HtmlAttributeEncode((Convert.ToString(Eval("MemberName")) & " " & Convert.ToString(Eval("Email")) & " " & Convert.ToString(Eval("Mobile")) & " " & Convert.ToString(Eval("Country"))).ToLower()) %>'>
                <td>
                    <input type="checkbox" class="form-check-input member-email-check" aria-label="Select"
                        data-email="<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("Email"))) %>"
                        data-name="<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("MemberName"))) %>"
                        data-membername="<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("MemberName"))) %>"
                        data-dob="<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("DOB"))) %>"
                        data-mobile="<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("Mobile"))) %>"
                        data-country="<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("Country"))) %>"
                        data-createdate="<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("Createdate"))) %>" />
                </td>
                <td class="fw-semibold"><%#: Eval("MemberName") %></td>
                <td class="text-muted"><%#: Eval("Email") %></td>
                <td><%#: Eval("DOB") %></td>
                <td><%#: Eval("Mobile") %></td>
                <td><%#: Eval("Country") %></td>
                <td><%#: Eval("Createdate") %></td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</tbody>
                    </table>
                    <div class="adm-empty d-none" data-none="member"><i class="fa fa-search"></i> No members match.</div>
                </div>
                <div class="modal-footer">
                    <span class="me-auto text-muted small"><b data-sel="member">0</b> selected</span>
                    <button type="button" class="btn btn-light" data-bs-dismiss="modal">Cancel</button>
                    <button id="addSelectedMembersBtn" type="button" class="btn btn-ib" data-bs-dismiss="modal"><i class="fa fa-plus"></i> Add selected</button>
                </div>
            </div>
        </div>
    </div>

    <!-- ============ Past buyers (orders) modal ============ -->
    <div class="modal fade be-modal" id="orderPickerModal" tabindex="-1" aria-labelledby="orderPickerTitle" aria-hidden="true">
        <div class="modal-dialog modal-xl modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 id="orderPickerTitle" class="modal-title"><i class="fa fa-shopping-bag"></i> Select past buyers</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="be-filter">
                        <label class="form-label small mb-1" for="<%= OrderProductFilterTextBox.ClientID %>">Only people who bought…</label>
                        <div class="d-flex gap-2 flex-wrap">
                            <asp:TextBox ID="OrderProductFilterTextBox" runat="server" CssClass="form-control flex-grow-1" placeholder="Product names or codes, comma separated – e.g. Shah Rukh, Cricket"></asp:TextBox>
                            <asp:Button ID="ApplyOrderFilterButton" runat="server" Text="Apply filter" CssClass="btn btn-ib btn-sm be-reopen" CausesValidation="false" OnClick="ApplyOrderFilterButton_Click" />
                            <asp:Button ID="ClearOrderFilterButton" runat="server" Text="Clear" CssClass="btn btn-light btn-sm be-reopen" CausesValidation="false" OnClick="ClearOrderFilterButton_Click" />
                        </div>
                    </div>
                    <div class="be-mbar">
                        <div class="search"><i class="fa fa-search"></i>
                            <input type="search" id="orderSearchText" placeholder="Search name, email or product…" aria-label="Search buyers" />
                        </div>
                        <button type="button" id="orderSelectAllVisibleBtn" class="btn btn-sm btn-ib-o">Select shown</button>
                        <button type="button" id="orderClearVisibleBtn" class="btn btn-sm btn-light">Clear shown</button>
                    </div>
                    <table class="table adm-table be-pick mb-0">
                        <thead>
<tr>
    <th style="width: 44px;"></th>
    <th>Name</th>
    <th>Email</th>
    <th>Products</th>
    <th>Order Count</th>
    <th>Last Order Date</th>
    <th>Last Order Total</th>
    <th>Lifetime Total</th>
</tr>
                        </thead>
                        <tbody id="orderTableBody">
                            <asp:Repeater ID="OrderRepeater" runat="server">
                                <ItemTemplate>
                                    <tr class="order-row" data-search='<%# HttpUtility.HtmlAttributeEncode((Convert.ToString(Eval("Name")) & " " & Convert.ToString(Eval("Email")) & " " & Convert.ToString(Eval("Products")) & " " & Convert.ToString(Eval("OrderDate"))).toLower()) %>'>
                                        <td>
                                            <input type="checkbox" class="form-check-input order-email-check" aria-label="Select"
                                                data-email="<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("Email"))) %>"
                                                data-name="<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("Name"))) %>"
                                                data-products="<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("Products"))) %>"
data-ordercount="<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("OrderCount"))) %>"
data-orderdate="<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("OrderDate"))) %>"
data-ordertotal="<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("OrderTotal"))) %>"
data-lifetimeordertotal="<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("LifetimeOrderTotal"))) %>" />
                                        </td>
                                        <td class="fw-semibold"><%#: Eval("Name") %></td>
                                        <td class="text-muted"><%#: Eval("Email") %></td>
                                        <td class="be-prods"><%#: Eval("Products") %></td>
                                        <td><%#: Eval("OrderCount") %></td>
                                        <td><%#: Eval("OrderDate") %></td>
                                        <td><%#: Eval("OrderTotal") %></td>
                                        <td><%#: Eval("LifetimeOrderTotal") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <div class="adm-empty d-none" data-none="order"><i class="fa fa-search"></i> No buyers match.</div>
                </div>
                <div class="modal-footer">
                    <span class="me-auto text-muted small"><b data-sel="order">0</b> selected</span>
                    <button type="button" class="btn btn-light" data-bs-dismiss="modal">Cancel</button>
                    <button id="addSelectedOrdersBtn" type="button" class="btn btn-ib" data-bs-dismiss="modal"><i class="fa fa-plus"></i> Add selected</button>
                </div>
            </div>
        </div>
    </div>

    <script>
        (function () {
            var $rcpt = $('#<%= RecipientsTextBox.ClientID %>'),
                $subj = $('#<%= SubjectTextBox.ClientID %>'),
                $msg = $('#<%= MessageTextBox.ClientID %>'),
                $grp = $('#<%= EmailGroupTextBox.ClientID %>'),
                EMAIL = /^[^\s@<>,;]+@[^\s@<>,;]+\.[a-z]{2,}$/i,
                DRAFT = 'ibBulkDraft';

            /* ---------- Recipient parsing ---------- */
            function normalizeEmail(v) { return String(v || '').trim().toLowerCase(); }

function parseRecipientObjects(v) {
    if (!v) return [];
    var t = String(v).trim();
    if (!t) return [];

    // JSON array mode
    if (t[0] === '[' && t[t.length - 1] === ']') {
        try {
            var arr = JSON.parse(t);
            return Array.isArray(arr) ? arr.filter(function (x) { return x && typeof x === 'object'; }) : [];
        } catch (e) {
            return [];
        }
    }

    // JSON-lines mode (with legacy fallback)
    var lines = t.split(/\r?\n/).map(function (x) { return x.trim(); }).filter(Boolean);
    return lines.map(function (line) {
        if (line[0] === '{' && line[line.length - 1] === '}') {
            try { return JSON.parse(line); } catch (e) { return { __invalid: line }; }
        }
        // legacy "Name <email>"
        var m = line.match(/^(.*)<([^>]+)>$/);
        if (m) return { Name: String(m[1] || '').trim(), Email: String(m[2] || '').trim() };
        return { Email: line };
    });
}

function analyse() {
    var list = parseRecipientObjects($rcpt.val()), seen = {}, good = [], bad = [], dup = 0;

    list.forEach(function (r) {
        if (!r || typeof r !== 'object' || r.__invalid) { bad.push(JSON.stringify(r)); return; }
        var e = normalizeEmail(r.Email || r.email);
        if (!EMAIL.test(e)) { bad.push(JSON.stringify(r)); return; }
        if (seen[e]) { dup++; return; }

        seen[e] = 1;
        if (!r.Email) r.Email = e;
        if (!r.Name && r.MemberName) r.Name = r.MemberName;
        good.push(r);
    });

    return { good: good, bad: bad, dup: dup };
}

function addRecipients(sel) {
    var add = [];
    $(sel + ':checked').each(function () {
        var e = ($(this).attr('data-email') || '').trim();
        if (!e) return;

        var obj = {
            Email: e,
            Name: ($(this).attr('data-name') || '').trim()
        };

        // member fields
        if ($(this).attr('data-membername') !== undefined) obj.MemberName = ($(this).attr('data-membername') || '').trim();
        if ($(this).attr('data-dob') !== undefined) obj.DOB = ($(this).attr('data-dob') || '').trim();
        if ($(this).attr('data-mobile') !== undefined) obj.Mobile = ($(this).attr('data-mobile') || '').trim();
        if ($(this).attr('data-country') !== undefined) obj.Country = ($(this).attr('data-country') || '').trim();
        if ($(this).attr('data-createdate') !== undefined) obj.Createdate = ($(this).attr('data-createdate') || '').trim();

        // order fields
        if ($(this).attr('data-products') !== undefined) obj.Products = ($(this).attr('data-products') || '').trim();
        if ($(this).attr('data-ordercount') !== undefined) obj.OrderCount = ($(this).attr('data-ordercount') || '').trim();
        if ($(this).attr('data-orderdate') !== undefined) obj.OrderDate = ($(this).attr('data-orderdate') || '').trim();
        if ($(this).attr('data-ordertotal') !== undefined) obj.OrderTotal = ($(this).attr('data-ordertotal') || '').trim();
        if ($(this).attr('data-lifetimeordertotal') !== undefined) obj.LifetimeOrderTotal = ($(this).attr('data-lifetimeordertotal') || '').trim();

        add.push(obj);
    });

    var all = parseRecipientObjects($rcpt.val()).concat(add);
    var seen = {}, out = [];

    all.forEach(function (r) {
        if (!r || typeof r !== 'object') return;
        var e = normalizeEmail(r.Email || r.email);
        if (!e) return;
        r.Email = e;
        if (!seen[e]) {
            seen[e] = 1;
            out.push(r);
        }
    });

    $rcpt.val(out.map(function (x) { return JSON.stringify(x); }).join('\n'));
    $(sel).prop('checked', false);
    refresh();
}

/* ---------- Pickers ---------- */
            function wirePicker(kind, searchId, rowSel, allBtn, clearBtn, addBtn, chk) {
                function count() { $('[data-sel="' + kind + '"]').text($(chk + ':checked').length); }
                $(searchId).on('input', function () {
                    var t = ($(this).val() || '').toLowerCase().trim(), shown = 0;
                    $(rowSel).each(function () { var ok = ($(this).attr('data-search') || '').indexOf(t) > -1; $(this).toggle(ok); if (ok) shown++; });
                    $('[data-none="' + kind + '"]').toggleClass('d-none', shown > 0);
                });
                $(allBtn).on('click', function () { $(rowSel + ':visible ' + chk).prop('checked', true); count(); });
                $(clearBtn).on('click', function () { $(rowSel + ':visible ' + chk).prop('checked', false); count(); });
                $(rowSel).on('click', function (e) { if (e.target.tagName !== 'INPUT') { var c = $(this).find(chk); c.prop('checked', !c.prop('checked')); } count(); });
                $(addBtn).on('click', function () { addRecipients(chk); count(); });
            }
            wirePicker('member', '#memberSearchText', '#memberTableBody .member-row', '#memberSelectAllVisibleBtn', '#memberClearVisibleBtn', '#addSelectedMembersBtn', '.member-email-check');
            wirePicker('order', '#orderSearchText', '#orderTableBody .order-row', '#orderSelectAllVisibleBtn', '#orderClearVisibleBtn', '#addSelectedOrdersBtn', '.order-email-check');

            // Re-open the buyers modal after the product filter posts back
            $('.be-reopen').on('click', function () { try { sessionStorage.setItem('ibReopenOrders', '1'); } catch (e) { } });
            try {
                if (sessionStorage.getItem('ibReopenOrders')) {
                    sessionStorage.removeItem('ibReopenOrders');
                    new bootstrap.Modal(document.getElementById('orderPickerModal')).show();
                }
            } catch (e) { }

            /* ---------- Message helpers ---------- */
            function wrap(before, after, def) {
                var el = $msg[0], s = el.selectionStart, e = el.selectionEnd, v = el.value, sel = v.substring(s, e) || def || '';
                el.value = v.substring(0, s) + before + sel + after + v.substring(e);
                el.focus(); el.selectionStart = s + before.length; el.selectionEnd = s + before.length + sel.length;
                refresh();
            }
            var SNIP = {
                b: function () { wrap('<b>', '</b>', 'bold text'); },
                i: function () { wrap('<i>', '</i>', 'italic text'); },
                h: function () { wrap('<h2 style="color:#330B3F;">', '</h2>', 'Heading'); },
                p: function () { wrap('<p>', '</p>', 'Paragraph'); },
                ul: function () { wrap('<ul>\n  <li>', '</li>\n  <li>Second point</li>\n</ul>', 'First point'); },
                a: function () { var u = prompt('Link address', 'https://www.indiabobbles.com/'); if (u) wrap('<a href="' + u.replace(/"/g, '') + '" style="color:#330B3F;">', '</a>', 'link text'); },
                img: function () { var u = prompt('Image address (from Drive or https://…)'); if (u) wrap('<img src="' + u.replace(/"/g, '') + '" alt="', '" style="max-width:100%;height:auto;border-radius:8px;" />', ''); },
                btn: function () {
                    var u = prompt('Button link', 'https://www.indiabobbles.com/'); if (!u) return;
                    wrap('<p style="text-align:center;margin:24px 0;"><a href="' + u.replace(/"/g, '') + '" style="background:#ffc107;color:#330B3F;font-weight:bold;text-decoration:none;padding:12px 28px;border-radius:8px;display:inline-block;">', '</a></p>', 'Shop now');
                },
                tpl: function () {
                    if ($msg.val().trim() && !confirm('Insert the starter template at the cursor?')) return;
                    wrap('<p>Hi there,</p>\n\n<p>', '</p>\n\n<p style="text-align:center;margin:24px 0;"><a href="https://www.indiabobbles.com/" style="background:#ffc107;color:#330B3F;font-weight:bold;text-decoration:none;padding:12px 28px;border-radius:8px;display:inline-block;">Shop now</a></p>\n\n<p>Warm regards,<br />Team India Bobbles</p>\n\n<p style="font-size:12px;color:#888;">You are receiving this because you shopped or signed up at indiabobbles.com. Reply “unsubscribe” and we\'ll take you off the list.</p>',
                        'Write your main message here.');
                }
            };
            $('.be-tools [data-ins]').on('click', function () { SNIP[$(this).data('ins')](); });

            /* ---------- Preview ---------- */
            function bodyHtml() {
                var v = $msg.val().trim();
                if (!/<[a-z][\s\S]*>/i.test(v)) {   // plain text → paragraphs
                    v = v.split(/\n{2,}/).map(function (p) { return '<p>' + $('<div>').text(p).html().replace(/\n/g, '<br>') + '</p>'; }).join('');
                }
                return '<!doctype html><html><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><style>body{margin:0;background:#f6f4f8;font-family:Arial,Helvetica,sans-serif;color:#1f1724;line-height:1.6;font-size:15px}.w{max-width:600px;margin:0 auto;background:#fff;padding:24px}img{max-width:100%}</style></head><body><div class="w">' +
                    (v || '<p style="color:#999">Your message will appear here.</p>') + '</div></body></html>';
            }
            function renderPreview() {
                $('#pvSubj').text($subj.val().trim() || '(no subject)');
                document.getElementById('pvFrame').srcdoc = bodyHtml();
            }
            $('.be-tabs button').on('click', function () {
                var t = $(this).data('tab');
                $('.be-tabs button').removeClass('on'); $(this).addClass('on');
                $('#tabWrite').toggleClass('d-none', t !== 'write'); $('#tabPreview').toggleClass('d-none', t !== 'preview');
                if (t === 'preview') renderPreview();
            });
            $('.be-device button').on('click', function () {
                $('.be-device button').removeClass('on'); $(this).addClass('on');
                $('#pvFrame').css('width', $(this).data('w'));
            });

            /* ---------- Live summary + checklist ---------- */
            var last;
            function refresh() {
                var a = last = analyse(), s = $subj.val().trim(), m = $msg.val().trim();
                $('#mValid, #sumTo').text(a.good.length);
                $('#mBad').text(a.bad.length); $('#mBadWrap').toggleClass('d-none', !a.bad.length);
                $('#mDup').text(a.dup); $('#mDupWrap').toggleClass('d-none', !a.dup);
                $('#cleanBtn').toggleClass('d-none', !a.bad.length && !a.dup);
                $('#badList').toggleClass('d-none', !a.bad.length).text(a.bad.length ? 'Invalid: ' + a.bad.slice(0, 8).join(' · ') + (a.bad.length > 8 ? ' …' : '') : '');

                $('#subjCount').text(s.length + ' / 60').toggleClass('over', s.length > 60);
                $('#sumSubj').text(s || 'Not set').toggleClass('text-muted', !s);
                var words = m ? $('<div>').html(m).text().trim().split(/\s+/).filter(Boolean).length : 0;
                $('#sumMsg').text(m ? words + ' words' : 'Empty').toggleClass('text-muted', !m);

                var st = { to: a.good.length > 0, bad: a.bad.length === 0, subj: !!s, msg: !!m, unsub: /unsubscribe/i.test(m) };
                $('#checklist li').each(function () { $(this).toggleClass('done', !!st[$(this).data('k')]); });
                $('.be-send').val(a.good.length ? 'Send to ' + a.good.length + (a.good.length === 1 ? ' person' : ' people') : 'Send email');

                try { localStorage.setItem(DRAFT, JSON.stringify({ s: $subj.val(), m: $msg.val(), g: $grp.val() })); } catch (e) { }
            }
            $('#cleanBtn').on('click', function () { $rcpt.val(analyse().good.map(function (x) { return JSON.stringify(x); }).join('\n')); refresh(); });
            $('#clearRcptBtn').on('click', function () { if (!$rcpt.val() || confirm('Clear all recipients?')) { $rcpt.val(''); refresh(); } });
            $rcpt.add($subj).add($msg).add($grp).on('input', refresh);

            /* ---------- Draft restore / clear ---------- */
            var sent = $('#<%= StatusPanel.ClientID %>').hasClass('alert-success');
            try {
                if (sent) localStorage.removeItem(DRAFT);
                else if (!$subj.val() && !$msg.val()) {
                    var d = JSON.parse(localStorage.getItem(DRAFT) || 'null');
                    if (d && (d.s || d.m)) { $subj.val(d.s); $msg.val(d.m); if (d.g) $grp.val(d.g); }
                }
            } catch (e) { }
            refresh();

            /* ---------- Confirm + no double send ---------- */
            window.ibConfirmSend = function () {
                if (typeof Page_ClientValidate === 'function' && !Page_ClientValidate('BulkMailGrp')) return false;
                var a = last || analyse();
                if (!a.good.length) { alert('There are no valid email addresses to send to.'); return false; }
                var msg = 'Send "' + $subj.val().trim() + '" to ' + a.good.length + (a.good.length === 1 ? ' person' : ' people') + '?';
                if (a.bad.length) msg += '\n\n' + a.bad.length + ' invalid address(es) will be skipped or may fail – use "Clean up list" first.';
                if (!confirm(msg)) return false;
                var b = $('.be-send');
                setTimeout(function () { b.prop('disabled', true).val('Sending…'); }, 0);
                return true;
            };
        })();
    </script>
</asp:Content>