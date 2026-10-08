<%@ Page Title="Products" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="Products.aspx.vb" Inherits="IndiaBobbles.Products" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">
    <asp:SqlDataSource ID="ProductsDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:indiabobblesConnectionString %>" SelectCommand="SELECT P.ID, P.Name, P.MRP, P.SalePrice, P.CreateDate, P.ModifyDate,
CASE WHEN P.Status = 0 THEN 'Active' WHEN P.Status = 1 THEN 'Inactive' WHEN P.Status = 2 THEN 'Deleted' ELSE '' END AS Status,
P.ThumbPath, P.OutofStock, STRING_AGG(CT.DisplayName,',') as Tags
FROM     CategoryTag AS CT INNER JOIN
                  ProductTag AS PT ON CT.ID = PT.TagID RIGHT OUTER JOIN
                  Product AS P ON PT.ProductID = P.ID
				  GROUP BY P.ID, P.Name, P.MRP, P.SalePrice, P.CreateDate, P.ModifyDate, P.ThumbPath, P.OutofStock, P.Status"></asp:SqlDataSource>

    <!-- ============ Page heading ============ -->
    <div class="adm-head">
        <div>
            <h1>Products</h1>
            <p>Manage your catalogue, stock and prices.</p>
        </div>
        <a runat="server" href="~/admin/manageproduct.aspx" class="btn btn-ib"><i class="fa fa-plus"></i> Create product</a>
    </div>

    <asp:Label ID="MessageLabel" runat="server" CssClass="adm-flash" Visible="false" EnableViewState="false" />

    <!-- ============ Stats (filled by script) ============ -->
    <div class="adm-stats">
        <button type="button" class="stat" data-filter-stock="all"><span class="n" id="stTotal">–</span><span class="l">Total products</span></button>
        <button type="button" class="stat ok" data-filter-stock="in"><span class="n" id="stIn">–</span><span class="l">In stock</span></button>
        <button type="button" class="stat warn" data-filter-stock="out"><span class="n" id="stOut">–</span><span class="l">Out of stock</span></button>
        <button type="button" class="stat sale" data-filter-stock="sale"><span class="n" id="stSale">–</span><span class="l">On sale</span></button>
    </div>

    <!-- ============ Toolbar ============ -->
    <div class="adm-toolbar">
        <div class="search">
            <i class="fa fa-search"></i>
            <input type="search" id="fSearch" placeholder="Search name or tag…" aria-label="Search products" />
        </div>
        <select id="fStock" class="form-select" aria-label="Stock filter">
            <option value="all">All stock</option>
            <option value="in">In stock</option>
            <option value="out">Out of stock</option>
            <option value="sale">On sale</option>
        </select>
        <select id="fStatus" class="form-select" aria-label="Status filter">
            <option value="">All statuses</option>
            <option value="Active">Active</option>
            <option value="Inactive">Inactive</option>
            <option value="Deleted">Deleted</option>
        </select>
        <select id="fTag" class="form-select" aria-label="Tag filter">
            <option value="">All tags</option>
        </select>
        <span class="count" id="fCount"></span>
    </div>

    <!-- ============ Grid ============ -->
    <div class="adm-card table-responsive">
        <asp:GridView ID="ProductsGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
            CssClass="table adm-table align-middle mb-0" GridLines="None" DataKeyNames="ID" DataSourceID="ProductsDataSource"
            PageSize="100" EnableSortingAndPagingCallbacks="False" EmptyDataText="No products yet. Click “Create product” to add one.">
            <Columns>
                <asp:TemplateField HeaderText="Product" SortExpression="Name">
                    <ItemTemplate>
                        <div class="prod">
                            <%# If(String.IsNullOrEmpty(Convert.ToString(Eval("ThumbPath"))),
                                   "<span class=""thumb empty""><i class=""fa fa-image""></i></span>",
                                   "<img class=""thumb"" src=""" & HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("ThumbPath"))) & """ alt="""" loading=""lazy"" />") %>
                            <div>
                                <a class="pname" href='<%# "manageproduct.aspx?id=" & Eval("ID") %>'><%#: Eval("Name") %></a>
                                <div class="pid">ID <%# Eval("ID") %></div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Price" SortExpression="SalePrice">
                    <ItemTemplate>
                        <div class="price">₹<%# Convert.ToDecimal(Eval("SalePrice")).ToString("#,##0.##", New System.Globalization.CultureInfo("en-IN")) %></div>
                        <%# If(Convert.ToDecimal(Eval("MRP")) > Convert.ToDecimal(Eval("SalePrice")),
                               "<div class=""mrp""><s>₹" & Convert.ToDecimal(Eval("MRP")).ToString("#,##0.##", New System.Globalization.CultureInfo("en-IN")) & "</s> <span class=""off"">" &
                               Math.Round((Convert.ToDecimal(Eval("MRP")) - Convert.ToDecimal(Eval("SalePrice"))) * 100 / Convert.ToDecimal(Eval("MRP"))) & "% off</span></div>", "") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Stock" SortExpression="OutofStock">
                    <ItemTemplate>
                        <%# If(Convert.ToBoolean(Eval("OutofStock")),
                               "<span class=""badge-ib oos"">Out of stock</span>",
                               "<span class=""badge-ib ins"">In stock</span>") %>
                        <asp:LinkButton runat="server" CssClass="toggle" CommandName="ToggleStock" CommandArgument='<%# Eval("ID") %>'
                            ToolTip="Switch stock status"
                            OnClientClick="return confirm('Change stock status for this product?');"
                            Text='<%# If(Convert.ToBoolean(Eval("OutofStock")), "Mark in stock", "Mark out of stock") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Status" SortExpression="Status">
                    <ItemTemplate>
                        <span class='<%# "badge-ib st-" & Convert.ToString(Eval("Status")).ToLower() %>' data-status='<%#: Eval("Status") %>'><%#: Eval("Status") %></span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Tags" SortExpression="Tags">
                    <ItemTemplate>
                        <span class="tags" data-tags='<%#: Eval("Tags") %>'></span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Updated" SortExpression="ModifyDate">
                    <ItemTemplate>
                        <span class="date"><%# If(IsDBNull(Eval("ModifyDate")), Convert.ToDateTime(Eval("CreateDate")).ToString("d MMM yyyy"), Convert.ToDateTime(Eval("ModifyDate")).ToString("d MMM yyyy")) %></span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="" ItemStyle-CssClass="actions">
                    <ItemTemplate>
                        <a class="act primary" href='<%# "manageproduct.aspx?id=" & Eval("ID") %>' title="Edit"><i class="fa fa-pencil"></i><span> Edit</span></a>
                        <a class="act" href='<%# "productdetail.aspx?id=" & Eval("ID") %>' title="Detail"><i class="fa fa-info-circle"></i></a>
                        <a class="act" href='<%# ResolveUrl("~/product/" & Eval("ID") & "/" & IndiaBobbles.Utility.Slugify(Convert.ToString(Eval("Name")))) %>' target="_blank" rel="noopener" title="View on website"><i class="fa fa-external-link"></i></a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <PagerSettings Position="Bottom" Mode="NumericFirstLast" />
            <PagerStyle CssClass="adm-pager" HorizontalAlign="Center" />
        </asp:GridView>
        <div class="adm-empty d-none" id="fNone"><i class="fa fa-search"></i> No products match your filters.</div>
    </div>

    <script>
        (function () {
            var rows = Array.prototype.slice.call(document.querySelectorAll('#<%= ProductsGridView.ClientID %> tr')).filter(function (r) { return r.querySelector('.prod'); });
            var tagSet = {};

            // Tag chips + collect info per row
            rows.forEach(function (r) {
                var t = r.querySelector('.tags'), tags = (t.getAttribute('data-tags') || '').split(',').map(function (x) { return x.trim(); }).filter(Boolean);
                t.innerHTML = tags.map(function (x) { tagSet[x] = 1; return '<span class="chip">' + x.replace(/</g, '&lt;') + '</span>'; }).join('') || '<span class="text-muted small">–</span>';
                r._name = r.querySelector('.pname').textContent.toLowerCase();
                r._tags = tags;
                r._out = !!r.querySelector('.oos');
                r._sale = !!r.querySelector('.off');
                r._status = r.querySelector('[data-status]').getAttribute('data-status');
            });

            Object.keys(tagSet).sort().forEach(function (t) { $('#fTag').append($('<option>').val(t).text(t)); });

            // Stats
            $('#stTotal').text(rows.length);
            $('#stOut').text(rows.filter(function (r) { return r._out; }).length);
            $('#stIn').text(rows.filter(function (r) { return !r._out; }).length);
            $('#stSale').text(rows.filter(function (r) { return r._sale; }).length);

            function apply() {
                var q = $('#fSearch').val().trim().toLowerCase(), stock = $('#fStock').val(), st = $('#fStatus').val(), tag = $('#fTag').val(), shown = 0;
                rows.forEach(function (r) {
                    var ok = (!q || r._name.indexOf(q) > -1 || r._tags.join(' ').toLowerCase().indexOf(q) > -1)
                        && (stock === 'all' || (stock === 'in' && !r._out) || (stock === 'out' && r._out) || (stock === 'sale' && r._sale))
                        && (!st || r._status === st)
                        && (!tag || r._tags.indexOf(tag) > -1);
                    r.style.display = ok ? '' : 'none';
                    if (ok) shown++;
                });
                $('#fCount').text(shown + ' of ' + rows.length);
                $('#fNone').toggleClass('d-none', shown > 0 || rows.length === 0);
                $('.adm-stats .stat').removeClass('on').filter('[data-filter-stock="' + stock + '"]').addClass('on');
                try { sessionStorage.setItem('ibAdmProdFilter', JSON.stringify({ q: q, stock: stock, st: st, tag: tag })); } catch (e) { }
            }

            // Restore last filters
            try {
                var f = JSON.parse(sessionStorage.getItem('ibAdmProdFilter') || 'null');
                if (f) { $('#fSearch').val(f.q); $('#fStock').val(f.stock); $('#fStatus').val(f.st); $('#fTag').val(f.tag); }
            } catch (e) { }

            $('#fSearch').on('input', apply);
            $('#fStock, #fStatus, #fTag').on('change', apply);
            $('.adm-stats .stat').on('click', function () { $('#fStock').val($(this).data('filter-stock')); apply(); });
            apply();

            // Keyboard shortcut: "/" focuses search
            document.addEventListener('keydown', function (e) {
                if (e.key === '/' && document.activeElement.tagName !== 'INPUT') { e.preventDefault(); $('#fSearch').focus(); }
            });
        })();
    </script>
</asp:Content>