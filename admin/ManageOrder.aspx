<%@ Page Title="Order" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="ManageOrder.aspx.vb" Inherits="IndiaBobbles.ManageOrder" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%-- Page styles live here so the page looks right even if admin/style.css is old or cached --%>
    <style>
        .mo-head { display: flex; justify-content: space-between; align-items: flex-start; gap: 14px; flex-wrap: wrap; margin-bottom: 16px; }
        .mo-head h1 { font-size: 1.6rem; font-weight: 800; color: #330B3F; margin: 0; display: flex; align-items: center; gap: 10px; flex-wrap: wrap; }
        .mo-head .meta { color: #6b5f72; font-size: .9rem; margin-top: 4px; }
        .mo-head .btns { display: flex; gap: 8px; flex-wrap: wrap; }
        .mo-head .btns .btn { white-space: nowrap; }

        .os { display: inline-block; font-size: .78rem; font-weight: 800; padding: 4px 11px; border-radius: 20px; vertical-align: middle; }
        .os-1 { background: #f1edf3; color: #6b5f72; }
        .os-2 { background: #e0f2fe; color: #0369a1; }
        .os-3, .os-4 { background: #fff4cc; color: #92400e; }
        .os-5 { background: #ede9fe; color: #5b21b6; }
        .os-6 { background: #e7f6ec; color: #157a3c; }
        .os-7, .os-8 { background: #fdecee; color: #b4232f; }

        .mo-track { display: flex; background: #fff; border: 1px solid #ece6ef; border-radius: 12px; padding: 16px 10px; margin-bottom: 18px; }
        .mo-track .st { flex: 1; text-align: center; position: relative; font-size: .8rem; font-weight: 700; color: #b4a6bd; }
        .mo-track .st::before { content: ""; position: absolute; top: 15px; left: -50%; width: 100%; height: 3px; background: #ece6ef; z-index: 0; }
        .mo-track .st:first-child::before { display: none; }
        .mo-track .dot { position: relative; z-index: 1; width: 32px; height: 32px; border-radius: 50%; background: #ece6ef; color: #fff; display: flex; align-items: center; justify-content: center; margin: 0 auto 6px; }
        .mo-track .st.done { color: #330B3F; }
        .mo-track .st.done .dot { background: #330B3F; color: #ffc107; }
        .mo-track .st.done::before { background: #330B3F; }
        .mo-track .st.now .dot { box-shadow: 0 0 0 4px #ffc107; }
        .mo-track.bad { background: #fdecee; border-color: #f5c2c7; color: #b4232f; justify-content: center; font-weight: 700; }

        .mo-items .it { display: flex; gap: 14px; align-items: center; padding: 12px 0; border-bottom: 1px solid #f1edf3; }
        .mo-items .it:last-child { border-bottom: 0; }
        .mo-items .img { width: 64px; height: 64px; border-radius: 10px; background: #f6f4f8; flex-shrink: 0; overflow: hidden; display: flex; align-items: center; justify-content: center; }
        .mo-items .img img { max-width: 100%; max-height: 100%; object-fit: contain; }
        .mo-items .nm { font-weight: 700; color: #1f1724; }
        .mo-items .cd { font-size: .78rem; color: #6b5f72; font-family: ui-monospace, Consolas, monospace; }
        .mo-items .qty { color: #6b5f72; font-size: .88rem; white-space: nowrap; }
        .mo-items .amt { margin-left: auto; font-weight: 800; white-space: nowrap; }
        .mo-sum { border-top: 2px solid #f1edf3; margin-top: 6px; padding-top: 10px; font-size: .92rem; }
        .mo-sum .r { display: flex; justify-content: space-between; padding: 3px 0; color: #3e3445; }
        .mo-sum .r.minus span:last-child { color: #157a3c; }
        .mo-sum .r.tot { border-top: 1px dashed #ddd5e2; margin-top: 6px; padding-top: 8px; font-size: 1.15rem; font-weight: 800; color: #330B3F; }

        .mo-kv { display: grid; grid-template-columns: 130px 1fr; gap: 6px 12px; font-size: .9rem; margin: 0; }
        .mo-kv dt { color: #6b5f72; font-weight: 600; }
        .mo-kv dd { margin: 0; overflow-wrap: anywhere; }
        .mo-kv .mono { font-family: ui-monospace, Consolas, monospace; font-size: .85rem; }
        .mo-addr { font-size: .95rem; line-height: 1.55; color: #1f1724; }
        .mo-addr .who { font-weight: 800; }
        .mo-copy { border: 0; background: #f1edf3; color: #330B3F; border-radius: 7px; font-size: .78rem; font-weight: 700; padding: 4px 10px; }
        .mo-copy:hover { background: #fff4cc; }
        .mo-contact a { display: flex; align-items: center; gap: 8px; padding: 6px 0; text-decoration: none; color: #1f1724; font-size: .92rem; }
        .mo-contact a i { width: 18px; text-align: center; color: #6b5f72; }
        .mo-contact a.wa i { color: #128c7e; }
        .mo-contact a:hover { color: #330B3F; text-decoration: underline; }
        .mo-muted { color: #b4a6bd; font-style: italic; }
        .mo-pre { white-space: pre-wrap; font-size: .85rem; background: #f6f4f8; border-radius: 8px; padding: 8px 10px; margin: 0; max-height: 160px; overflow: auto; }

        .mo-upd .quick { display: flex; gap: 6px; flex-wrap: wrap; margin-bottom: 12px; }
        .mo-upd .quick button { border: 1px solid #ddd5e2; background: #fff; color: #330B3F; border-radius: 20px; font-size: .8rem; font-weight: 700; padding: 4px 12px; }
        .mo-upd .quick button:hover { background: #fff4cc; border-color: #ffc107; }
        .mo-upd .trk-link { display: inline-block; margin-top: 6px; font-size: .82rem; font-weight: 700; }
        .mo-danger { border-top: 1px solid #f1edf3; margin-top: 18px; padding-top: 14px; text-align: center; }
        .mo-danger .btn { color: #b4232f; font-weight: 700; font-size: .85rem; }
        .mo-danger .btn:hover { background: #fdecee; }

        /* Packing slip – only visible when printing */
        .mo-slip { display: none; }
        @media print {
            body * { visibility: hidden !important; }
            .mo-slip, .mo-slip * { visibility: visible !important; }
            .mo-slip { display: block; position: absolute; left: 0; top: 0; width: 100%; font-family: Arial, sans-serif; color: #000; }
            .mo-slip .box { border: 2px solid #000; border-radius: 8px; padding: 16px; margin-bottom: 14px; }
            .mo-slip h2 { font-size: 14px; margin: 0 0 6px; text-transform: uppercase; letter-spacing: 1px; }
            .mo-slip .to { font-size: 20px; line-height: 1.4; }
            .mo-slip table { width: 100%; border-collapse: collapse; font-size: 13px; }
            .mo-slip td, .mo-slip th { border-bottom: 1px solid #999; padding: 6px 4px; text-align: left; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">

    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:indiabobblesConnectionString %>" SelectCommand="SELECT * FROM [OrderItem] WHERE ([OrderID] = @OrderID)">
        <SelectParameters>
            <asp:QueryStringParameter Name="OrderID" QueryStringField="orderid" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:Panel ID="NotFoundPanel" runat="server" Visible="false" CssClass="adm-panel text-center py-5">
        <i class="fa fa-question-circle fa-3x text-muted"></i>
        <h1 class="h4 mt-3">Order not found</h1>
        <p class="text-muted">Check the order number and try again.</p>
        <a runat="server" href="~/admin/orders.aspx" class="btn btn-ib">Back to orders</a>
    </asp:Panel>

    <asp:PlaceHolder ID="OrderPanel" runat="server">
    <% If Ord IsNot Nothing Then %>

        <a runat="server" href="~/admin/orders.aspx" class="adm-back"><i class="fa fa-angle-left"></i> All orders</a>

        <!-- ============ Header ============ -->
        <div class="mo-head">
            <div>
                <h1>Order #<%: Ord.ID %> <span class="os os-<%= StatusNum %>"><%: StatusName(StatusNum) %></span></h1>
                <div class="meta">
                    Placed <%: D(Ord.DateCreated) %> · <%= ItemQty %> item<%= If(ItemQty = 1, "", "s") %> · <b><%: M(Ord.Total) %></b>
                    <%= If(String.IsNullOrWhiteSpace(Convert.ToString(Ord.PaymentMode)), "", " · " & HttpUtility.HtmlEncode(Convert.ToString(Ord.PaymentMode))) %>
                </div>
            </div>
            <div class="btns">
                <asp:HyperLink ID="ReceiptLink" Target="_blank" CssClass="btn btn-ib-o btn-sm" runat="server"><i class="fa fa-file-text-o"></i> Receipt</asp:HyperLink>
                <button type="button" class="btn btn-ib-o btn-sm" onclick="window.print()"><i class="fa fa-print"></i> Packing slip</button>
                <% If Not String.IsNullOrWhiteSpace(Ord.Email) Then %>
                <a class="btn btn-ib-o btn-sm" target="_blank" href="../sendmail.aspx?email=<%: HttpUtility.UrlEncode(Ord.Email) %>&name=<%: HttpUtility.UrlEncode(Convert.ToString(Ord.Name)) %>"><i class="fa fa-envelope-o"></i> Email</a>
                <% End If %>
                <% If WaNumber <> "" Then %>
                <a class="btn btn-ib-o btn-sm" target="_blank" rel="noopener" href="https://wa.me/<%= WaNumber %>?text=<%: HttpUtility.UrlEncode("Hi " & FirstName & ", this is India Bobbles about your order #" & Ord.ID & ".") %>"><i class="fa fa-whatsapp"></i> WhatsApp</a>
                <% End If %>
            </div>
        </div>

        <asp:Label ID="MessageLabel" runat="server" CssClass="adm-flash" Visible="false" EnableViewState="false" />

        <!-- ============ Progress ============ -->
        <% If StatusNum = 7 OrElse StatusNum = 8 Then %>
            <div class="mo-track bad"><i class="fa fa-exclamation-circle me-2"></i> This order is marked <%: StatusName(StatusNum) %>.</div>
        <% Else %>
            <div class="mo-track">
                <div class="st <%= StepCss(1) %>"><div class="dot"><i class="fa fa-shopping-cart"></i></div>Placed</div>
                <div class="st <%= StepCss(2) %>"><div class="dot"><i class="fa fa-inr"></i></div>Paid</div>
                <div class="st <%= StepCss(3) %>"><div class="dot"><i class="fa fa-truck"></i></div>Shipped</div>
                <div class="st <%= StepCss(4) %>"><div class="dot"><i class="fa fa-check"></i></div>Complete</div>
            </div>
        <% End If %>

        <div class="row g-3">
            <!-- ============ Left ============ -->
            <div class="col-xl-8">

                <!-- Items -->
                <div class="adm-panel">
                    <h2><i class="fa fa-cube"></i> Items</h2>
                    <div class="mo-items">
                        <asp:Repeater ID="ItemsRepeater" runat="server" DataSourceID="SqlDataSource2">
                            <ItemTemplate>
                                <div class="it">
                                    <div class="img"><%# If(String.IsNullOrWhiteSpace(Convert.ToString(Eval("ProductImg"))), "<i class=""fa fa-image text-muted""></i>", "<img src=""" & HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("ProductImg"))) & """ alt="""" />") %></div>
                                    <div>
                                        <div class="nm"><%#: Eval("ProductName") %></div>
                                        <div class="cd"><%#: Eval("ProductCode") %></div>
                                    </div>
                                    <div class="qty ms-auto"><%# Eval("Quantity") %> × <%# M(Eval("Price")) %></div>
                                    <div class="amt" style="min-width:90px;text-align:right;"><%# M(Eval("Amount")) %></div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <div class="mo-sum">
                        <div class="r"><span>Subtotal</span><span><%: M(Ord.Amount) %></span></div>
                        <% If N(Ord.Discount) > 0 Then %><div class="r minus"><span>Discount<%: If(String.IsNullOrWhiteSpace(Convert.ToString(Ord.Coupon)), "", " (" & Convert.ToString(Ord.Coupon) & ")") %></span><span>− <%: M(Ord.Discount) %></span></div><% End If %>
                        <div class="r"><span>Shipping<%: If(String.IsNullOrWhiteSpace(Convert.ToString(Ord.ShippingService)), "", " · " & Convert.ToString(Ord.ShippingService)) %></span><span><%: If(N(Ord.ShippingPrice) = 0, "Free", M(Ord.ShippingPrice)) %></span></div>
                        <% If N(Ord.COD) > 0 Then %><div class="r"><span>COD charge</span><span><%: M(Ord.COD) %></span></div><% End If %>
                        <% If N(Ord.Tax) > 0 Then %><div class="r"><span>Tax (<%: N(Ord.TaxPercentage).ToString("0.##") %>%)</span><span><%: M(Ord.Tax) %></span></div><% End If %>
                        <div class="r tot"><span>Total</span><span><%: M(Ord.Total) %></span></div>
                    </div>
                </div>

                <div class="row g-3">
                    <!-- Ship to -->
                    <div class="col-md-6">
                        <div class="adm-panel h-100 mb-0">
                            <div class="d-flex justify-content-between align-items-start">
                                <h2><i class="fa fa-map-marker"></i> Ship to</h2>
                                <button type="button" class="mo-copy" data-copy="#shipAddr"><i class="fa fa-clone"></i> Copy</button>
                            </div>
                            <div class="mo-addr" id="shipAddr">
                                <% If ShipLines = "" Then %><span class="mo-muted">No shipping address</span><% Else %>
                                <div class="who"><%: Ord.Name %></div><%= ShipLines %><% If Not String.IsNullOrWhiteSpace(Ord.Phone) Then %><br />Phone: <%: Ord.Phone %><% End If %>
                                <% End If %>
                            </div>
                            <hr />
                            <h2 style="font-size:.9rem;"><i class="fa fa-file-text-o"></i> Billing address</h2>
                            <div class="mo-addr" style="font-size:.88rem;">
                                <%= If(BillLines = "", "<span class=""mo-muted"">Not entered</span>", If(BillLines = ShipLines, "<span class=""text-muted"">Same as shipping</span>", BillLines)) %>
                            </div>
                        </div>
                    </div>

                    <!-- Customer + payment -->
                    <div class="col-md-6">
                        <div class="adm-panel mb-3">
                            <h2><i class="fa fa-user"></i> Customer</h2>
                            <div class="mo-contact">
                                <div class="fw-bold mb-1"><%: Ord.Name %>
                                    <% If N(Ord.MemberID) > 0 Then %><span class="badge-ib ins ms-1" style="font-size:.7rem;">Member #<%: Ord.MemberID %></span><% End If %>
                                </div>
                                <% If Not String.IsNullOrWhiteSpace(Ord.Email) Then %><a href="mailto:<%: Ord.Email %>"><i class="fa fa-envelope-o"></i> <%: Ord.Email %></a><% End If %>
                                <% If Not String.IsNullOrWhiteSpace(Ord.Phone) Then %><a href="tel:+<%= WaNumber %>"><i class="fa fa-phone"></i> <%: Ord.Phone %></a><% End If %>
                                <% If WaNumber <> "" Then %><a class="wa" target="_blank" rel="noopener" href="https://wa.me/<%= WaNumber %>"><i class="fa fa-whatsapp"></i> Chat on WhatsApp</a><% End If %>
                                <a href="orders.aspx?q=<%: HttpUtility.UrlEncode(If(String.IsNullOrWhiteSpace(Ord.Email), Convert.ToString(Ord.Phone), Ord.Email)) %>"><i class="fa fa-history"></i> Other orders by this customer</a>
                            </div>
                        </div>
                        <div class="adm-panel mb-0">
                            <h2><i class="fa fa-credit-card"></i> Payment</h2>
                            <dl class="mo-kv">
                                <dt>Mode</dt><dd><%= OrDash(Ord.PaymentMode) %></dd>
                                <dt>Transaction</dt><dd class="mono"><%= OrDash(Ord.TransactionCode) %></dd>
                                <dt>Paid on</dt><dd><%: D(Ord.TransactionDate) %></dd>
                                <dt>Coupon</dt><dd><%= OrDash(Ord.Coupon) %></dd>
                                <dt>Last updated</dt><dd><%: D(Ord.DateModified) %></dd>
                            </dl>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ============ Right: update ============ -->
            <div class="col-xl-4">
                <div class="adm-sticky">
                    <div class="adm-panel mo-upd">
                        <h2><i class="fa fa-pencil"></i> Update order</h2>

                        <div class="quick">
                            <button type="button" data-st="5" data-focus="track"><i class="fa fa-truck"></i> Mark shipped</button>
                            <button type="button" data-st="6"><i class="fa fa-check"></i> Mark complete</button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="StatusDropDown">Status</label>
                            <asp:DropDownList ID="StatusDropDown" ClientIDMode="Static" CssClass="form-select" runat="server">
                                <asp:ListItem Text="New / unpaid" Value="1" />
                                <asp:ListItem Text="Processing" Value="2" />
                                <asp:ListItem Text="Card paid" Value="3" />
                                <asp:ListItem Text="Cash on delivery" Value="4" />
                                <asp:ListItem Text="Shipped" Value="5" />
                                <asp:ListItem Text="Complete" Value="6" />
                                <asp:ListItem Text="Refund" Value="7" />
                                <asp:ListItem Text="Deleted" Value="8" />
                            </asp:DropDownList>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="ShippingServiceTextBox">Courier</label>
                            <asp:TextBox ID="ShippingServiceTextBox" ClientIDMode="Static" CssClass="form-control" MaxLength="100" runat="server" list="courierList" placeholder="Delhivery"></asp:TextBox>
                            <datalist id="courierList">
                                <option value="Delhivery"></option>
                                <option value="India Post"></option>
                                <option value="Blue Dart"></option>
                                <option value="DTDC"></option>
                                <option value="Xpressbees"></option>
                                <option value="Ekart"></option>
                                <option value="Shiprocket"></option>
                            </datalist>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="TrackTextBox">Tracking code</label>
                            <asp:TextBox ID="TrackTextBox" ClientIDMode="Static" CssClass="form-control" MaxLength="20" runat="server" placeholder="AWB / consignment number" style="font-family:ui-monospace,Consolas,monospace;"></asp:TextBox>
                            <a class="trk-link d-none" id="trkLink" target="_blank" rel="noopener"><i class="fa fa-external-link"></i> Track this parcel</a>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="ShippingNotesTextBox">Shipping notes</label>
                            <asp:TextBox ID="ShippingNotesTextBox" ClientIDMode="Static" MaxLength="1000" CssClass="form-control" TextMode="MultiLine" Rows="3" runat="server" placeholder="e.g. Dispatched 9 Oct, gift wrapped"></asp:TextBox>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="DetailTextBox">Transaction detail</label>
                            <asp:TextBox ID="DetailTextBox" ClientIDMode="Static" CssClass="form-control" MaxLength="2000" TextMode="MultiLine" Rows="3" runat="server" style="font-size:.85rem;"></asp:TextBox>
                        </div>

                        <asp:Button ID="SubmitButton" CssClass="btn btn-ib w-100" runat="server" Text="Save changes" OnClick="SubmitButton_Click" OnClientClick="return ibCheckSave();" />

                        <div class="mo-danger">
                            <asp:Button ID="DeleteButton" CssClass="btn btn-link" runat="server" Text="Delete this order" CausesValidation="false"
                                OnClientClick="return confirm('Delete this order? Once deleted you won\'t be able to recover it.');" OnClick="DeleteButton_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- ============ Packing slip (print only) ============ -->
        <div class="mo-slip">
            <div class="box">
                <h2>Ship to</h2>
                <div class="to"><b><%: Ord.Name %></b><br /><%= ShipLines %><% If Not String.IsNullOrWhiteSpace(Ord.Phone) Then %><br />Phone: <%: Ord.Phone %><% End If %></div>
            </div>
            <div class="box">
                <h2>From</h2>
                India Bobbles · H104, Ajnara Daffodil, Sector 137, Noida, Uttar Pradesh 201305 · indiabobbles.com
            </div>
            <div class="box">
                <h2>Order #<%: Ord.ID %> · <%: D(Ord.DateCreated) %><%= If(StatusNum = 4, " · CASH ON DELIVERY: " & HttpUtility.HtmlEncode(M(Ord.Total)), "") %></h2>
                <table>
                    <tr><th>Item</th><th>Code</th><th>Qty</th></tr>
                    <asp:Repeater ID="SlipRepeater" runat="server" DataSourceID="SqlDataSource2">
                        <ItemTemplate><tr><td><%#: Eval("ProductName") %></td><td><%#: Eval("ProductCode") %></td><td><%# Eval("Quantity") %></td></tr></ItemTemplate>
                    </asp:Repeater>
                </table>
                <p style="margin:10px 0 0;font-size:12px;">Fragile – hand-painted collectible. Handle with care.</p>
            </div>
        </div>

        <script>
            (function () {
                var st = $('#StatusDropDown'), trk = $('#TrackTextBox'), svc = $('#ShippingServiceTextBox'), orig = st.val();

                // Quick status buttons
                $('.mo-upd .quick button').on('click', function () {
                    st.val('' + $(this).data('st'));
                    if ($(this).data('focus') === 'track') trk.focus();
                });

                // Tracking link for common couriers
                function trackUrl() {
                    var c = (svc.val() || '').toLowerCase(), t = $.trim(trk.val());
                    if (!t) return '';
                    if (c.indexOf('delhivery') > -1) return 'https://www.delhivery.com/track/package/' + encodeURIComponent(t);
                    if (c.indexOf('india post') > -1) return 'https://www.indiapost.gov.in/_layouts/15/dop.portal.tracking/trackconsignment.aspx';
                    if (c.indexOf('blue dart') > -1 || c.indexOf('bluedart') > -1) return 'https://www.bluedart.com/tracking';
                    if (c.indexOf('dtdc') > -1) return 'https://www.dtdc.in/tracking.asp';
                    return 'https://www.google.com/search?q=' + encodeURIComponent((svc.val() || '') + ' tracking ' + t);
                }
                function updLink() { var u = trackUrl(); $('#trkLink').toggleClass('d-none', !u).attr('href', u || '#'); }
                trk.add(svc).on('input', updLink); updLink();

                // Warn before saving "Shipped" without a tracking code
                window.ibCheckSave = function () {
                    if (st.val() === '5' && !$.trim(trk.val()))
                        return confirm('You are marking this order Shipped without a tracking code. Save anyway?');
                    if (st.val() === '8' && orig !== '8')
                        return confirm('Set this order to Deleted?');
                    return true;
                };

                // Copy shipping address
                $('.mo-copy').on('click', function () {
                    var b = $(this), t = $($(this).data('copy'))[0].innerText.trim();
                    (navigator.clipboard ? navigator.clipboard.writeText(t) : Promise.reject()).then(function () {
                        b.html('<i class="fa fa-check"></i> Copied'); setTimeout(function () { b.html('<i class="fa fa-clone"></i> Copy'); }, 1500);
                    }, function () { prompt('Copy the address:', t); });
                });
            })();
        </script>

    <% End If %>
    </asp:PlaceHolder>
</asp:Content>