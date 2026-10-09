<%@ Page Title="Articles" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="Articles.aspx.vb" Inherits="IndiaBobbles.Articles" %>

<%@ Register Src="~/admin/controls/Message.ascx" TagPrefix="uc1" TagName="Message" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">

    <asp:SqlDataSource ID="ArticleSource" runat="server" ConnectionString="<%$ ConnectionStrings:indiabobblesConnectionString %>"
        SelectCommand="SELECT P.ID, P.Title, P.DateCreated, P.WriterName, P.Viewed, C.Name AS Category, PS.Name AS Status, P.Status AS StatusID, P.URL, P.Sitemap FROM Category AS C INNER JOIN Post AS P ON C.ID = P.Category INNER JOIN PostStatus AS PS ON P.Status = PS.ID ORDER BY P.ID DESC">
    </asp:SqlDataSource>

    <!-- ============ Heading ============ -->
    <div class="adm-head">
        <div>
            <h1>Articles</h1>
            <p>Blog posts, guides and game pages on indiabobbles.com/blog.</p>
        </div>
        <a href="ManageArticle.aspx?mode=add" class="btn btn-ib"><i class="fa fa-plus"></i> New article</a>
    </div>

    <uc1:Message ID="message1" Visible="false" EnableViewState="false" runat="server" />
    <asp:Label ID="MessageLabel" runat="server" CssClass="adm-flash" Visible="false" EnableViewState="false" />

    <!-- ============ Stats (filled by script, click to filter) ============ -->
    <div class="ar-stats">
        <button type="button" class="ar-stat" data-st=""><span class="n" id="sAll">–</span><span class="l">All articles</span></button>
        <button type="button" class="ar-stat pub" data-st="publish"><span class="n" id="sPub">–</span><span class="l">Published</span></button>
        <button type="button" class="ar-stat dft" data-st="draft"><span class="n" id="sDft">–</span><span class="l">Drafts</span></button>
        <button type="button" class="ar-stat" data-st="inactive"><span class="n" id="sIna">–</span><span class="l">Inactive</span></button>
        <button type="button" class="ar-stat" data-st="nositemap"><span class="n" id="sNoMap">–</span><span class="l">Live but not in sitemap</span></button>
    </div>

    <!-- ============ Toolbar ============ -->
    <div class="ar-bar">
        <div class="search">
            <i class="fa fa-search"></i>
            <input type="search" id="arSearch" placeholder="Search title or writer…" aria-label="Search articles" />
        </div>
        <select id="arStatus" class="form-select form-select-sm" aria-label="Status">
            <option value="">All statuses</option>
            <option value="publish">Published</option>
            <option value="draft">Drafts</option>
            <option value="inactive">Inactive</option>
            <option value="nositemap">Live, not in sitemap</option>
        </select>
        <select id="arCat" class="form-select form-select-sm" aria-label="Category">
            <option value="">All categories</option>
        </select>
        <span class="count" id="arCount"></span>
    </div>

    <!-- ============ Grid ============ -->
    <div class="adm-card table-responsive">
        <asp:GridView ID="ArticleGridView" runat="server" AllowPaging="True" AllowSorting="True" PageSize="200"
            AutoGenerateColumns="False" CssClass="table adm-table ar-table align-middle mb-0"
            DataKeyNames="ID" DataSourceID="ArticleSource" GridLines="None"
            OnRowCommand="ArticleGridView_RowCommand">
            <EmptyDataTemplate>
                <div class="adm-empty"><i class="fa fa-file-text-o fa-2x d-block mb-2"></i>No articles yet. Click “New article” to write one.</div>
            </EmptyDataTemplate>
            <Columns>
                <asp:TemplateField HeaderText="Article" SortExpression="Title">
                    <ItemTemplate>
                        <div class="ar-row" data-status='<%#: Convert.ToString(Eval("Status")).ToLower() %>' data-cat='<%#: Eval("Category") %>'
                             data-map='<%# If(Convert.ToBoolean(If(IsDBNull(Eval("Sitemap")), False, Eval("Sitemap"))), "1", "0") %>'
                             data-search='<%#: (Convert.ToString(Eval("Title")) & " " & Convert.ToString(Eval("WriterName"))).ToLower() %>'>
                            <a class="ar-title" href='<%# "managearticle.aspx?id=" & Eval("ID") & "&mode=edit" %>'><%#: Eval("Title") %></a>
                            <div class="ar-meta">#<%# Eval("ID") %> · by <%#: Eval("WriterName") %> · <span class="url">/blog/<%#: Eval("URL") %></span></div>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Category" SortExpression="Category">
                    <ItemTemplate><span class="ar-cat"><%#: Eval("Category") %></span></ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Status" SortExpression="Status">
                    <ItemTemplate><span class='<%# "as as-" & Convert.ToString(Eval("Status")).ToLower() %>'><%#: If(Convert.ToString(Eval("Status")) = "Publish", "Published", Eval("Status")) %></span></ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Sitemap" SortExpression="Sitemap">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CausesValidation="False" CommandName="ToggleSitemap" CommandArgument='<%# Eval("ID") %>'
                            CssClass='<%# "ar-sm " & If(Convert.ToBoolean(If(IsDBNull(Eval("Sitemap")), False, Eval("Sitemap"))), "on", "off") %>'
                            ToolTip="Click to include / exclude from the Google sitemap"
                            Text='<%# If(Convert.ToBoolean(If(IsDBNull(Eval("Sitemap")), False, Eval("Sitemap"))), "<i class=""fa fa-check""></i> Yes", "<i class=""fa fa-minus""></i> No") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Created" SortExpression="DateCreated">
                    <ItemTemplate><span class="ar-date"><%# Convert.ToDateTime(Eval("DateCreated")).ToString("d MMM yyyy") %></span></ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField ItemStyle-CssClass="text-end text-nowrap">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CausesValidation="False" CommandName="TogglePublish" CommandArgument='<%# Eval("ID") %>'
                            CssClass='<%# If(Convert.ToString(Eval("Status")) = "Publish", "ar-act unpub", "ar-act pub") %>'
                            ToolTip='<%# If(Convert.ToString(Eval("Status")) = "Publish", "Move back to draft", "Make this article live") %>'
                            OnClientClick='<%# If(Convert.ToString(Eval("Status")) = "Publish", "return confirm(""Unpublish this article? It will go back to Draft."");", "") %>'
                            Text='<%# If(Convert.ToString(Eval("Status")) = "Publish", "Unpublish", "<i class=""fa fa-globe""></i>&nbsp;Publish") %>' />
                        <a class="ar-act" href='<%# "managearticle.aspx?id=" & Eval("ID") & "&mode=edit" %>' title="Edit"><i class="fa fa-pencil"></i></a>
                        <a class="ar-act" href='<%#: "https://www.indiabobbles.com/blog/" & Convert.ToString(Eval("URL")) & "?preview=true" %>' target="_blank" rel="noopener" title="Preview"><i class="fa fa-eye"></i></a>
                        <asp:LinkButton runat="server" CssClass="ar-act del" CommandArgument='<%# Eval("ID") %>' CausesValidation="False" CommandName="DeleteCommand" ToolTip="Delete"
                            OnClientClick="return confirm('Delete this article permanently? This cannot be undone. Tip: set it to Draft instead if you might need it later.');"><i class="fa fa-trash"></i></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <PagerSettings Mode="NumericFirstLast" Position="Bottom" />
            <PagerStyle CssClass="adm-pager" HorizontalAlign="Center" />
        </asp:GridView>
        <div class="adm-empty d-none" id="arNone"><i class="fa fa-search"></i> No articles match your filters.</div>
    </div>

    <script>
        (function () {
            var rows = $('.ar-row').map(function () { return $(this).closest('tr')[0]; }).get(), cats = {};
            rows.forEach(function (tr) {
                var d = $(tr).find('.ar-row');
                tr._st = d.data('status'); tr._cat = '' + d.data('cat'); tr._map = '' + d.data('map') === '1'; tr._q = '' + d.data('search');
                cats[tr._cat] = 1;
            });
            Object.keys(cats).sort().forEach(function (c) { $('#arCat').append($('<option>').val(c).text(c)); });

            function n(fn) { return rows.filter(fn).length; }
            $('#sAll').text(rows.length);
            $('#sPub').text(n(function (r) { return r._st === 'publish'; }));
            $('#sDft').text(n(function (r) { return r._st === 'draft'; }));
            $('#sIna').text(n(function (r) { return r._st === 'inactive'; }));
            $('#sNoMap').text(n(function (r) { return r._st === 'publish' && !r._map; }));

            function apply() {
                var q = $('#arSearch').val().trim().toLowerCase(), st = $('#arStatus').val(), cat = $('#arCat').val(), shown = 0;
                rows.forEach(function (r) {
                    var ok = (!q || r._q.indexOf(q) > -1) && (!cat || r._cat === cat) &&
                        (!st || (st === 'nositemap' ? (r._st === 'publish' && !r._map) : r._st === st));
                    r.style.display = ok ? '' : 'none'; if (ok) shown++;
                });
                $('#arCount').text(shown + ' of ' + rows.length);
                $('#arNone').toggleClass('d-none', shown > 0 || rows.length === 0);
                $('.ar-stat').removeClass('on').filter('[data-st="' + st + '"]').addClass('on');
                try { sessionStorage.setItem('ibArtFilter', JSON.stringify({ q: q, st: st, cat: cat })); } catch (e) { }
            }
            try {
                var f = JSON.parse(sessionStorage.getItem('ibArtFilter') || 'null');
                if (f) { $('#arSearch').val(f.q); $('#arStatus').val(f.st); $('#arCat').val(f.cat); }
            } catch (e) { }
            $('#arSearch').on('input', apply);
            $('#arStatus, #arCat').on('change', apply);
            $('.ar-stat').on('click', function () { $('#arStatus').val('' + $(this).data('st')); apply(); });
            apply();

            document.addEventListener('keydown', function (e) {
                if (e.key === '/' && !/INPUT|TEXTAREA|SELECT/.test(document.activeElement.tagName)) { e.preventDefault(); $('#arSearch').focus(); }
            });
        })();
    </script>
</asp:Content>