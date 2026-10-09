<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ViewDrive.aspx.vb" Inherits="IndiaBobbles.ViewDrive" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="robots" content="noindex, nofollow" />
    <title>Drive – India Bobbles</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3" crossorigin="anonymous" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css" rel="stylesheet" />
    <style>
        :root { --p: #330B3F; --p2: #4d1460; --y: #ffc107; --y2: #fff4cc; --ink: #1f1724; --mut: #6b5f72; --line: #ece6ef; --bg: #f6f4f8; --ok: #157a3c; --err: #b4232f; }
        html, body { height: 100%; }
        body { margin: 0; background: var(--bg); color: var(--ink); font-size: .92rem; }
        form { height: 100%; display: flex; flex-direction: column; }

        /* Top bar */
        .dv-top { background: #fff; border-bottom: 1px solid var(--line); padding: 10px 14px; display: flex; flex-wrap: wrap; gap: 8px; align-items: center; }
        .dv-crumbs { display: flex; align-items: center; gap: 6px; flex-wrap: wrap; flex: 1 1 260px; min-width: 0; }
        .dv-crumbs .crumb { color: var(--p); font-weight: 700; text-decoration: none; padding: 3px 8px; border-radius: 6px; white-space: nowrap; }
        .dv-crumbs .crumb:hover { background: var(--y2); }
        .dv-crumbs .crumb:last-child { color: var(--ink); }
        .dv-crumbs .sep { color: #b4a6bd; font-size: .8rem; }
        .dv-up { color: var(--mut) !important; border: 1px solid var(--line); border-radius: 8px; width: 32px; height: 32px; display: inline-flex; align-items: center; justify-content: center; text-decoration: none; }
        .dv-up:hover { background: var(--bg); }
        .dv-tools { display: flex; gap: 6px; align-items: center; flex-wrap: wrap; }
        .dv-search { position: relative; }
        .dv-search i { position: absolute; left: 10px; top: 50%; transform: translateY(-50%); color: var(--mut); font-size: .8rem; }
        .dv-search input { border: 1px solid #ddd5e2; border-radius: 8px; padding: 5px 10px 5px 28px; width: 170px; font-size: .85rem; }
        .dv-tools select { border: 1px solid #ddd5e2; border-radius: 8px; padding: 5px 8px; font-size: .85rem; background: #fff; }
        .dv-tools input:focus, .dv-tools select:focus { outline: 0; border-color: var(--p); box-shadow: 0 0 0 .18rem rgba(51,11,63,.12); }
        .dv-seg { display: inline-flex; border: 1px solid #ddd5e2; border-radius: 8px; overflow: hidden; }
        .dv-seg button { border: 0; background: #fff; color: var(--mut); width: 32px; height: 30px; }
        .dv-seg button.on { background: var(--p); color: #fff; }
        .btn-ib { background: var(--p); color: #fff !important; border: 0; font-weight: 700; border-radius: 8px; }
        .btn-ib:hover { background: var(--p2); }
        .btn-ib-o { background: #fff; color: var(--p) !important; border: 1px solid var(--p); font-weight: 700; border-radius: 8px; }
        .btn-ib-o:hover { background: var(--y2); }

        .dv-newfolder { display: none; background: var(--y2); padding: 8px 14px; gap: 8px; align-items: center; flex-wrap: wrap; }
        .dv-newfolder.show { display: flex; }
        .dv-newfolder input { border: 1px solid #e6cf7a; border-radius: 8px; padding: 5px 10px; min-width: 220px; }

        .flash { display: block; margin: 10px 14px 0; padding: 8px 12px; border-radius: 8px; font-weight: 600; font-size: .85rem; }
        .flash.ok { background: #e7f6ec; color: var(--ok); }
        .flash.err { background: #fdecee; color: var(--err); }
        #clientFlash:empty { display: none; }

        /* Body */
        .dv-body { flex: 1; display: flex; min-height: 0; }
        .dv-main { flex: 1; overflow: auto; padding: 14px; position: relative; }
        .dv-summary { font-size: .78rem; color: var(--mut); margin-bottom: 10px; }

        .dv-items.grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(128px, 1fr)); gap: 10px; }
        .dv-items.grid .it { background: #fff; border: 2px solid transparent; border-radius: 12px; padding: 8px; cursor: pointer; text-align: center; user-select: none; position: relative; box-shadow: 0 1px 2px rgba(51,11,63,.06); }
        .dv-items.grid .it:hover { border-color: #ddd5e2; }
        .dv-items .it.sel { border-color: var(--p) !important; box-shadow: 0 0 0 3px var(--y); }
        .dv-items.grid .thumb { aspect-ratio: 1 / 1; border-radius: 8px; background: var(--bg) center / contain no-repeat; display: flex; align-items: center; justify-content: center; overflow: hidden; }
        .dv-items.grid .thumb img { width: 100%; height: 100%; object-fit: cover; }
        .dv-items.grid .thumb .fa-folder { font-size: 3.4rem; color: #f5b400; }
        .dv-items.grid .thumb .fa-file-o { font-size: 2.4rem; color: #b4a6bd; }
        .dv-items.grid .nm { margin-top: 6px; font-size: .78rem; font-weight: 600; line-height: 1.25; word-break: break-all; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; }
        .dv-items.grid .meta { font-size: .7rem; color: var(--mut); }
        .dv-items .new { position: absolute; top: 6px; left: 6px; background: var(--y); color: var(--p); font-size: .62rem; font-weight: 800; border-radius: 8px; padding: 1px 6px; }

        .dv-items.list { background: #fff; border-radius: 12px; border: 1px solid var(--line); overflow: hidden; }
        .dv-items.list .it { display: flex; align-items: center; gap: 10px; padding: 6px 12px; border-bottom: 1px solid #f3eff5; cursor: pointer; border-left: 3px solid transparent; }
        .dv-items.list .it:hover { background: #fcfbfd; }
        .dv-items.list .it.sel { background: var(--y2); border-left-color: var(--p); box-shadow: none; }
        .dv-items.list .thumb { width: 36px; height: 36px; border-radius: 6px; background: var(--bg); flex-shrink: 0; display: flex; align-items: center; justify-content: center; overflow: hidden; }
        .dv-items.list .thumb img { width: 100%; height: 100%; object-fit: cover; }
        .dv-items.list .thumb .fa-folder { color: #f5b400; font-size: 1.4rem; }
        .dv-items.list .nm { flex: 1; font-weight: 600; min-width: 0; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
        .dv-items.list .meta { font-size: .78rem; color: var(--mut); white-space: nowrap; }

        .dv-empty { text-align: center; color: var(--mut); padding: 50px 20px; }
        .dv-empty i { font-size: 3rem; color: #d3c8da; display: block; margin-bottom: 10px; }

        /* Details pane */
        .dv-side { width: 290px; flex-shrink: 0; background: #fff; border-left: 1px solid var(--line); overflow: auto; padding: 14px; }
        .dv-side { position: relative; }
        .dv-close { position: absolute; top: 8px; right: 8px; width: 30px; height: 30px; border: 0; border-radius: 8px; background: var(--bg); color: var(--mut); z-index: 2; }
        .dv-close:hover { background: var(--y2); color: var(--p); }
        .dv-side h3 { padding-right: 34px; }
        @media (min-width: 821px) { .dv-body.side-off .dv-side { display: none; } }
        .dv-side .none { color: var(--mut); text-align: center; padding: 30px 6px; font-size: .85rem; }
        .dv-side .none i { font-size: 2.2rem; color: #d3c8da; display: block; margin-bottom: 8px; }
        .dv-prev { aspect-ratio: 4 / 3; border-radius: 10px; background: repeating-conic-gradient(#f1edf3 0% 25%, #fff 0% 50%) 50% / 16px 16px; display: flex; align-items: center; justify-content: center; overflow: hidden; margin-bottom: 10px; }
        .dv-prev img { max-width: 100%; max-height: 100%; }
        .dv-prev .fa { font-size: 4rem; color: #f5b400; }
        .dv-side h3 { font-size: .95rem; font-weight: 800; word-break: break-all; margin: 0 0 8px; }
        .dv-kv { display: grid; grid-template-columns: 74px 1fr; gap: 3px 8px; font-size: .8rem; margin-bottom: 10px; }
        .dv-kv dt { color: var(--mut); font-weight: 600; }
        .dv-kv dd { margin: 0; }
        .dv-url { display: flex; margin-bottom: 8px; }
        .dv-url input { flex: 1; min-width: 0; border: 1px solid #ddd5e2; border-radius: 8px 0 0 8px; padding: 5px 8px; font-size: .75rem; font-family: ui-monospace, Consolas, monospace; background: var(--bg); }
        .dv-url button { border: 1px solid var(--p); background: var(--p); color: #fff; border-radius: 0 8px 8px 0; padding: 0 10px; font-size: .8rem; }
        .dv-acts { display: grid; grid-template-columns: 1fr 1fr; gap: 6px; }
        .dv-acts .btn { font-size: .8rem; padding: 6px 4px; }
        .dv-acts .full { grid-column: 1 / -1; }
        .dv-acts .danger { color: var(--err) !important; border-color: #f0c4c9; }
        .dv-acts .danger:hover { background: #fdecee; }
        .dv-use { display: none; }
        .in-frame .dv-use { display: block; }
        .dv-warn { font-size: .74rem; color: #92400e; background: #fff7e6; border-radius: 6px; padding: 6px 8px; margin-top: 8px; }

        /* Drop zone + busy */
        .dv-drop { position: fixed; inset: 0; background: rgba(51,11,63,.82); color: #fff; display: none; align-items: center; justify-content: center; flex-direction: column; z-index: 50; text-align: center; font-weight: 700; font-size: 1.2rem; }
        .dv-drop i { font-size: 3.5rem; color: var(--y); margin-bottom: 10px; }
        .dv-drop.show, .dv-busy.show { display: flex; }
        .dv-drop small { font-weight: 400; font-size: .85rem; opacity: .85; margin-top: 4px; }
        .dv-busy { position: fixed; inset: 0; background: rgba(255,255,255,.8); display: none; align-items: center; justify-content: center; z-index: 60; font-weight: 700; color: var(--p); gap: 10px; }

        @media (max-width: 820px) {
            .dv-side { position: fixed !important; left: 0; right: 0; bottom: 0; width: auto; max-height: 55%; border-left: 0; border-top: 1px solid var(--line); box-shadow: 0 -8px 24px rgba(0,0,0,.12); border-radius: 16px 16px 0 0; display: none; z-index: 40; }
            .dv-side.has { display: block; }
            .dv-search input { width: 120px; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- ============ Top bar ============ -->
        <div class="dv-top">
            <asp:HyperLink ID="UpLink" runat="server" CssClass="dv-up" ToolTip="Up one folder (Backspace)"><i class="fa fa-level-up"></i></asp:HyperLink>
            <div class="dv-crumbs"><asp:Literal ID="BreadcrumbLiteral" runat="server" /></div>
            <div class="dv-tools">
                <div class="dv-search"><i class="fa fa-search"></i><input type="search" id="dvSearch" placeholder="Filter this folder…" aria-label="Filter" /></div>
                <select id="dvSort" aria-label="Sort">
                    <option value="new">Newest first</option>
                    <option value="name">Name A–Z</option>
                    <option value="size">Largest first</option>
                </select>
                <div class="dv-seg" role="group" aria-label="View">
                    <button type="button" data-view="grid" title="Thumbnails"><i class="fa fa-th-large"></i></button>
                    <button type="button" data-view="list" title="List"><i class="fa fa-list"></i></button>
                </div>
                <div class="dv-seg" role="group" aria-label="Details panel">
                    <button type="button" id="sideToggle" title="Show / hide the details panel (I)"><i class="fa fa-columns"></i></button>
                </div>
                <button type="button" class="btn btn-ib-o btn-sm" id="newFolderBtn"><i class="fa fa-folder-o"></i> New folder</button>
                <button type="button" class="btn btn-ib btn-sm" id="uploadBtn"><i class="fa fa-upload"></i> Upload images</button>
            </div>
        </div>

        <!-- New folder -->
        <asp:Panel ID="NewFolderPanel" runat="server" CssClass="dv-newfolder" DefaultButton="CreateFolderButton" ClientIDMode="Static">
            <i class="fa fa-folder" style="color:#f5b400;font-size:1.2rem;"></i>
            <asp:TextBox ID="NewFolderTextBox" runat="server" ClientIDMode="Static" MaxLength="60" placeholder="Folder name, e.g. Diwali-2026" autocomplete="off" />
            <asp:Button ID="CreateFolderButton" runat="server" Text="Create" CssClass="btn btn-ib btn-sm" ClientIDMode="Static" />
            <button type="button" class="btn btn-light btn-sm" id="cancelFolderBtn">Cancel</button>
            <span style="font-size:.75rem;color:#92400e;">Letters, numbers, hyphens and underscores. Spaces become hyphens.</span>
        </asp:Panel>

        <asp:Label ID="FlashLabel" runat="server" Visible="false" EnableViewState="false" />
        <div id="clientFlash" class="flash err"></div>

        <!-- Hidden controls used by the toolbar / details pane -->
        <div class="d-none">
            <asp:FileUpload ID="UploadInput" runat="server" ClientIDMode="Static" AllowMultiple="true" accept="image/jpeg,image/png,image/gif,image/webp" />
            <asp:Button ID="UploadButton" runat="server" ClientIDMode="Static" Text="Upload" />
            <asp:HiddenField ID="TargetHidden" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="NewNameHidden" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="JustUploadedHidden" runat="server" ClientIDMode="Static" />
            <asp:Button ID="RenameButton" runat="server" ClientIDMode="Static" Text="Rename" />
            <asp:Button ID="DeleteButton" runat="server" ClientIDMode="Static" Text="Delete" />
        </div>

        <div class="dv-body">
            <!-- ============ Items ============ -->
            <div class="dv-main" id="dvMain">
                <div class="dv-summary"><asp:Literal ID="SummaryLiteral" runat="server" /> · drop images anywhere or paste with Ctrl+V</div>
                <div class="dv-items grid" id="dvItems">
                    <asp:Repeater ID="FolderRepeater" runat="server">
                        <ItemTemplate>
                            <div class="it" tabindex="0" data-kind="folder" data-name='<%#: Eval("Name") %>' data-rel='<%#: Eval("Rel") %>'
                                 data-href='<%#: FolderLink(Eval("Rel")) %>' data-count='<%# Eval("Count") %>' data-size="0"
                                 data-mod='<%# Convert.ToDateTime(Eval("Modified")).ToString("yyyy-MM-ddTHH:mm:ss") %>'>
                                <div class="thumb"><i class="fa fa-folder"></i></div>
                                <div class="nm"><%#: Eval("Name") %></div>
                                <div class="meta"><%# Eval("Count") %> item<%# If(Convert.ToInt32(Eval("Count")) = 1, "", "s") %></div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Repeater ID="FileRepeater" runat="server">
                        <ItemTemplate>
                            <div class="it" tabindex="0" data-kind="file" data-name='<%#: Eval("Name") %>' data-rel='<%#: Eval("Rel") %>'
                                 data-url='<%#: Eval("Url") %>' data-size='<%# Eval("Size") %>' data-sizetext='<%#: Eval("SizeText") %>'
                                 data-img='<%# If(CBool(Eval("IsImage")), "1", "0") %>' data-ext='<%#: Eval("Ext") %>'
                                 data-mod='<%# Convert.ToDateTime(Eval("Modified")).ToString("yyyy-MM-ddTHH:mm:ss") %>'>
                                <div class="thumb"><%# If(CBool(Eval("IsImage")), "<img loading=""lazy"" alt="""" src=""" & HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("Url"))) & """ />", "<i class=""fa fa-file-o""></i>") %></div>
                                <div class="nm"><%#: Eval("Name") %></div>
                                <div class="meta"><%#: Eval("SizeText") %></div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <asp:Panel ID="EmptyPanel" runat="server" CssClass="dv-empty" Visible="false">
                    <i class="fa fa-folder-open-o"></i>
                    This folder is empty.<br />Drag images here, or click <b>Upload images</b>.
                </asp:Panel>
                <div class="dv-empty d-none" id="noMatch"><i class="fa fa-search"></i>Nothing here matches your filter.</div>
            </div>

            <!-- ============ Details ============ -->
            <aside class="dv-side" id="dvSide">
                <button type="button" class="dv-close" id="sideClose" title="Close details (Esc)" aria-label="Close details"><i class="fa fa-times"></i></button>
                <div class="none" id="sideNone"><i class="fa fa-hand-pointer-o"></i>Click an image to see its link, size and options.<br /><br />Double-click a folder to open it.</div>
                <div id="sideDetail" class="d-none">
                    <div class="dv-prev" id="sPrev"></div>
                    <h3 id="sName"></h3>
                    <dl class="dv-kv" id="sMeta"></dl>
                    <div id="sFileOnly">
                        <div class="dv-url"><input type="text" id="sUrl" readonly="readonly" aria-label="Image link" /><button type="button" id="copyUrl" title="Copy link"><i class="fa fa-clone"></i></button></div>
                        <div class="dv-acts">
                            <button type="button" class="btn btn-ib full dv-use" id="useBtn"><i class="fa fa-check"></i> Use this image</button>
                            <button type="button" class="btn btn-ib-o" id="copyTag" title="Copy an &lt;img&gt; tag for articles"><i class="fa fa-code"></i> Copy &lt;img&gt;</button>
                            <a class="btn btn-ib-o" id="openFile" target="_blank" rel="noopener"><i class="fa fa-external-link"></i> Open</a>
                            <button type="button" class="btn btn-ib-o" id="renameBtn"><i class="fa fa-i-cursor"></i> Rename</button>
                            <button type="button" class="btn btn-ib-o danger" id="deleteBtn"><i class="fa fa-trash"></i> Delete</button>
                        </div>
                        <div class="dv-warn" id="sWarn"></div>
                    </div>
                    <div id="sFolderOnly" class="dv-acts">
                        <a class="btn btn-ib full" id="openFolder"><i class="fa fa-folder-open"></i> Open folder</a>
                        <button type="button" class="btn btn-ib-o" id="renameFolderBtn"><i class="fa fa-i-cursor"></i> Rename</button>
                        <button type="button" class="btn btn-ib-o danger" id="deleteFolderBtn"><i class="fa fa-trash"></i> Delete</button>
                    </div>
                </div>
            </aside>
        </div>

        <div class="dv-drop" id="dvDrop"><i class="fa fa-cloud-upload"></i>Drop images to upload<small>JPG, PNG, GIF or WebP · up to 5 MB each</small></div>
        <div class="dv-busy" id="dvBusy"><i class="fa fa-spinner fa-spin"></i> Uploading…</div>
    </form>

    <script>
        (function () {
            var MAX = 5 * 1024 * 1024, OK_EXT = /\.(jpe?g|png|gif|webp)$/i, OK_TYPE = /^image\/(jpeg|png|gif|webp)$/i;
            var inFrame = window.self !== window.top;
            if (inFrame) document.body.classList.add('in-frame');

            var items = Array.prototype.slice.call(document.querySelectorAll('#dvItems .it'));
            var box = document.getElementById('dvItems'), sel = null;
            function $(id) { return document.getElementById(id); }
            function esc(s) { var d = document.createElement('div'); d.textContent = s; return d.innerHTML; }
            function store(k, v) { try { if (v === undefined) return localStorage.getItem(k); localStorage.setItem(k, v); } catch (e) { return null; } }

            // ---------- View, sort, filter ----------
            function setView(v) {
                box.className = 'dv-items ' + v;
                document.querySelectorAll('.dv-seg button').forEach(function (b) { b.classList.toggle('on', b.getAttribute('data-view') === v); });
                store('ibDriveView', v);
            }
            document.querySelectorAll('.dv-seg button').forEach(function (b) { b.addEventListener('click', function () { setView(b.getAttribute('data-view')); }); });
            setView(store('ibDriveView') || 'grid');

            function sortItems() {
                var how = $('dvSort').value; store('ibDriveSort', how);
                var cmp = {
                    name: function (a, b) { return a.dataset.name.localeCompare(b.dataset.name, undefined, { numeric: true }); },
                    'new': function (a, b) { return b.dataset.mod.localeCompare(a.dataset.mod); },
                    size: function (a, b) { return (+b.dataset.size) - (+a.dataset.size); }
                }[how];
                var folders = items.filter(function (i) { return i.dataset.kind === 'folder'; }).sort(how === 'size' ? cmp.name : cmp);
                var files = items.filter(function (i) { return i.dataset.kind === 'file'; }).sort(cmp);
                folders.concat(files).forEach(function (i) { box.appendChild(i); });   // folders always first
            }
            $('dvSort').value = store('ibDriveSort') || 'new';
            $('dvSort').addEventListener('change', sortItems);
            sortItems();

            $('dvSearch').addEventListener('input', function () {
                var q = this.value.trim().toLowerCase(), shown = 0;
                items.forEach(function (i) { var ok = i.dataset.name.toLowerCase().indexOf(q) > -1; i.style.display = ok ? '' : 'none'; if (ok) shown++; });
                $('noMatch').classList.toggle('d-none', shown > 0 || items.length === 0);
            });

            // ---------- Selection + details ----------
            function select(it) {
                if (sel) sel.classList.remove('sel');
                sel = it;
                $('dvSide').classList.toggle('has', !!it);
                $('sideNone').classList.toggle('d-none', !!it);
                $('sideDetail').classList.toggle('d-none', !it);
                if (!it) return;
                it.classList.add('sel');
                var d = it.dataset, isFolder = d.kind === 'folder';
                $('sName').textContent = d.name;
                $('sFileOnly').classList.toggle('d-none', isFolder);
                $('sFolderOnly').classList.toggle('d-none', !isFolder);
                var mod = new Date(d.mod).toLocaleString('en-IN', { day: 'numeric', month: 'short', year: 'numeric', hour: 'numeric', minute: '2-digit' });
                if (isFolder) {
                    $('sPrev').innerHTML = '<i class="fa fa-folder"></i>';
                    $('sMeta').innerHTML = '<dt>Contains</dt><dd>' + d.count + ' item' + (d.count === '1' ? '' : 's') + '</dd><dt>Modified</dt><dd>' + mod + '</dd>';
                    $('openFolder').href = d.href;
                } else {
                    $('sPrev').innerHTML = d.img === '1' ? '<img alt="" src="' + esc(d.url) + '" />' : '<i class="fa fa-file-o" style="color:#b4a6bd"></i>';
                    $('sMeta').innerHTML = '<dt>Type</dt><dd>' + esc(d.ext) + '</dd><dt>Size</dt><dd>' + esc(d.sizetext) + '</dd><dt>Dimensions</dt><dd id="sDim">…</dd><dt>Uploaded</dt><dd>' + mod + '</dd>';
                    $('sUrl').value = d.url;
                    $('openFile').href = d.url;
                    var img = $('sPrev').querySelector('img');
                    if (img) img.onload = function () {
                        $('sDim').textContent = img.naturalWidth + ' × ' + img.naturalHeight + ' px';
                        var warn = [];
                        if (+d.size > 500 * 1024) warn.push('Over 500 KB – pages load faster with smaller images (compress at tinypng.com).');
                        if (img.naturalWidth > 2400) warn.push('Wider than 2400 px – 1200–1600 px is plenty for the website.');
                        $('sWarn').innerHTML = warn.join('<br>'); $('sWarn').style.display = warn.length ? '' : 'none';
                    }; else { $('sDim').textContent = '–'; $('sWarn').style.display = 'none'; }
                }
            }
            // ---------- Show / hide the details panel ----------
            var body = document.querySelector('.dv-body'), isPhone = window.matchMedia('(max-width: 820px)');
            function setSide(on) {
                body.classList.toggle('side-off', !on);
                $('sideToggle').classList.toggle('on', on);
                store('ibDriveSide', on ? '1' : '0');
            }
            setSide(store('ibDriveSide') !== '0');
            $('sideToggle').addEventListener('click', function () { setSide(body.classList.contains('side-off')); });
            $('sideClose').addEventListener('click', function () {
                if (isPhone.matches) select(null);      // phone: close the bottom sheet
                else setSide(false);                    // desktop: hide the panel, keep the selection
            });

            items.forEach(function (it) {
                it.addEventListener('click', function () { select(it); });
                it.addEventListener('dblclick', function () {
                    if (it.dataset.kind === 'folder') location.href = it.dataset.href;
                    else if (inFrame) use(it.dataset.url); else window.open(it.dataset.url, '_blank');
                });
            });
            select(null);

            // ---------- Copy / use ----------
            function copy(text, btn) {
                var done = function () { var h = btn.innerHTML; btn.innerHTML = '<i class="fa fa-check"></i> Copied'; setTimeout(function () { btn.innerHTML = h; }, 1300); };
                if (navigator.clipboard) navigator.clipboard.writeText(text).then(done, function () { prompt('Copy:', text); });
                else { prompt('Copy:', text); }
            }
            $('copyUrl').addEventListener('click', function () { copy($('sUrl').value, this); });
            $('copyTag').addEventListener('click', function () {
                var alt = sel.dataset.name.replace(/\.[^.]+$/, '').replace(/[-_]+/g, ' ');
                copy('<img src="' + sel.dataset.url + '" alt="' + alt + '" style="max-width:100%;height:auto;" />', this);
            });
            function use(url) { window.parent.postMessage({ type: 'ib-drive-pick', url: url }, location.origin); }
            $('useBtn').addEventListener('click', function () { if (sel) use(sel.dataset.url); });

            // ---------- Rename / delete ----------
            function rename() {
                if (!sel) return;
                var d = sel.dataset, isFolder = d.kind === 'folder';
                var ext = isFolder ? '' : (d.name.match(/\.[^.]+$/) || [''])[0];
                var base = isFolder ? d.name : d.name.slice(0, d.name.length - ext.length);
                var n = prompt((isFolder ? 'New folder name' : 'New file name (the ' + ext + ' ending stays)') + '\n\nNote: pages already using this ' + (isFolder ? 'folder' : 'image') + ' will need the new link.', base);
                if (!n || n.trim() === base) return;
                $('TargetHidden').value = d.rel; $('NewNameHidden').value = n.trim() + ext;
                $('RenameButton').click();
            }
            function del() {
                if (!sel) return;
                var d = sel.dataset;
                var msg = d.kind === 'folder' ? 'Delete the folder "' + d.name + '"? (Only empty folders can be deleted.)'
                    : 'Delete "' + d.name + '"?\n\nAny product or article using this image will show a broken picture. This cannot be undone.';
                if (!confirm(msg)) return;
                $('TargetHidden').value = d.rel;
                $('DeleteButton').click();
            }
            $('renameBtn').addEventListener('click', rename); $('renameFolderBtn').addEventListener('click', rename);
            $('deleteBtn').addEventListener('click', del); $('deleteFolderBtn').addEventListener('click', del);

            // ---------- New folder ----------
            $('newFolderBtn').addEventListener('click', function () { $('NewFolderPanel').classList.add('show'); $('NewFolderTextBox').focus(); });
            $('cancelFolderBtn').addEventListener('click', function () { $('NewFolderPanel').classList.remove('show'); $('NewFolderTextBox').value = ''; });
            $('CreateFolderButton').addEventListener('click', function (e) {
                var v = $('NewFolderTextBox').value.trim().replace(/\s+/g, '-');
                if (!/^[A-Za-z0-9][A-Za-z0-9_-]{0,59}$/.test(v)) {
                    e.preventDefault();
                    flash('Folder names can use letters, numbers, hyphens and underscores, and must start with a letter or number.');
                }
            });
            if ($('NewFolderTextBox').value) $('NewFolderPanel').classList.add('show');

            // ---------- Upload (validated before sending) ----------
            function flash(msg) { $('clientFlash').innerHTML = msg ? '<i class="fa fa-exclamation-circle"></i> ' + msg : ''; }
            function upload(fileList) {
                var good = new DataTransfer(), bad = [];
                Array.prototype.forEach.call(fileList, function (f) {
                    if (!OK_EXT.test(f.name) && !OK_TYPE.test(f.type)) bad.push(esc(f.name) + ' (not a JPG, PNG, GIF or WebP image)');
                    else if (!OK_TYPE.test(f.type)) bad.push(esc(f.name) + ' (not an image)');
                    else if (f.size > MAX) bad.push(esc(f.name) + ' (' + (f.size / 1048576).toFixed(1) + ' MB – max 5 MB)');
                    else {
                        // pasted images have no proper name
                        if (!OK_EXT.test(f.name)) f = new File([f], 'pasted-' + Date.now() + '.' + f.type.split('/')[1].replace('jpeg', 'jpg'), { type: f.type });
                        good.items.add(f);
                    }
                });
                flash(bad.length ? 'Skipped: ' + bad.join(', ') : '');
                if (!good.files.length) return;
                $('UploadInput').files = good.files;
                $('dvBusy').classList.add('show');
                $('UploadButton').click();
            }
            $('uploadBtn').addEventListener('click', function () { $('UploadInput').value = ''; $('UploadInput').click(); });
            $('UploadInput').addEventListener('change', function () { if (this.files.length && !$('dvBusy').classList.contains('show')) upload(this.files); });

            var dragDepth = 0;
            window.addEventListener('dragenter', function (e) { if (e.dataTransfer && Array.prototype.indexOf.call(e.dataTransfer.types, 'Files') > -1) { dragDepth++; $('dvDrop').classList.add('show'); } });
            window.addEventListener('dragleave', function () { if (--dragDepth <= 0) { dragDepth = 0; $('dvDrop').classList.remove('show'); } });
            window.addEventListener('dragover', function (e) { e.preventDefault(); });
            window.addEventListener('drop', function (e) {
                e.preventDefault(); dragDepth = 0; $('dvDrop').classList.remove('show');
                if (e.dataTransfer.files.length) upload(e.dataTransfer.files);
            });
            window.addEventListener('paste', function (e) {
                if (/INPUT|TEXTAREA/.test(document.activeElement.tagName)) return;
                var files = Array.prototype.map.call(e.clipboardData.items, function (i) { return i.kind === 'file' ? i.getAsFile() : null; }).filter(Boolean);
                if (files.length) { e.preventDefault(); upload(files); }
            });

            // Highlight and select what was just uploaded
            var just = ($('JustUploadedHidden').value || '').split('|').filter(Boolean);
            if (just.length) {
                var first = null;
                items.forEach(function (it) {
                    if (it.dataset.kind === 'file' && just.indexOf(it.dataset.name) > -1) {
                        var b = document.createElement('span'); b.className = 'new'; b.textContent = 'NEW'; it.appendChild(b);
                        if (!first) first = it;
                    }
                });
                if (first) { select(first); first.scrollIntoView({ block: 'nearest' }); }
                $('JustUploadedHidden').value = '';
            }

            // ---------- Keyboard ----------
            document.addEventListener('keydown', function (e) {
                if (/INPUT|TEXTAREA|SELECT/.test(document.activeElement.tagName)) return;
                var vis = items.filter(function (i) { return i.style.display !== 'none'; });
                var order = Array.prototype.slice.call(box.children).filter(function (i) { return vis.indexOf(i) > -1; });
                var idx = sel ? order.indexOf(sel) : -1;
                if (e.key === 'Enter' && sel) { sel.dispatchEvent(new MouseEvent('dblclick')); }
                else if (e.key === 'Delete' && sel) { del(); }
                else if (e.key === 'F2' && sel) { e.preventDefault(); rename(); }
                else if (e.key === 'Backspace' && $('UpLink')) { e.preventDefault(); $('UpLink').click(); }
                else if (e.key === 'ArrowRight' || e.key === 'ArrowDown') { e.preventDefault(); if (order[idx + 1]) { select(order[idx + 1]); order[idx + 1].focus(); } }
                else if (e.key === 'ArrowLeft' || e.key === 'ArrowUp') { e.preventDefault(); if (idx > 0) { select(order[idx - 1]); order[idx - 1].focus(); } }
                else if (e.key === 'Escape') { if (isPhone.matches || !sel) select(null); else setSide(false); }
                else if (e.key === 'i' || e.key === 'I') { setSide(body.classList.contains('side-off')); }
            });
        })();
    </script>
</body>
</html>