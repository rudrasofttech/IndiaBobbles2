<%@ Page Title="Manage Product" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="ManageProduct.aspx.vb" Inherits="IndiaBobbles.ManageProduct" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">
    <!-- ============ Heading ============ -->
    <div class="adm-head">
        <div>
            <a runat="server" href="~/admin/products.aspx" class="adm-back"><i class="fa fa-arrow-left"></i> All products</a>
            <h1><asp:Literal ID="HeadingLiteral" runat="server">Add Product</asp:Literal></h1>
        </div>
        <div class="d-flex gap-2 flex-wrap">
            <% If Not String.IsNullOrEmpty(Request.QueryString("id")) Then %>
                <a class="btn btn-ib-o" target="_blank" rel="noopener" href="<%= ResolveUrl("~/product/" & Request.QueryString("id") & "/" & IndiaBobbles.Utility.Slugify(NameTextBox.Text)) %>"><i class="fa fa-external-link"></i> View on website</a>
            <% End If %>
        </div>
    </div>

    <asp:ValidationSummary runat="server" CssClass="adm-error" HeaderText="Please fix the highlighted fields:" DisplayMode="BulletList" />

    <div class="row g-4">

        <!-- ================= Main column ================= -->
        <div class="col-xl-8">

            <!-- Basics -->
            <div class="adm-panel">
                <h2><i class="fa fa-info-circle"></i> Basic information</h2>

                <div class="mb-3">
                    <label for="NameTextBox" class="form-label">Product name <span class="req">*</span></label>
                    <asp:TextBox ID="NameTextBox" ClientIDMode="Static" MaxLength="300" CssClass="form-control" runat="server" placeholder="e.g. Daku Sambhar Singh Bobblehead"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Product name is required" Text="Product name is required" ControlToValidate="NameTextBox" CssClass="val" Display="Dynamic"></asp:RequiredFieldValidator>
                    <div class="hint">Shown on the product page, cart and receipts. Include "Bobblehead" or "Figurine" for search.</div>
                </div>

                <div>
                    <div class="d-flex justify-content-between align-items-end">
                        <label for="DescTextBox" class="form-label">Description <span class="req">*</span></label>
                        <span class="hint" id="descCount"></span>
                    </div>
                    <asp:TextBox ID="DescTextBox" ClientIDMode="Static" Rows="14" TextMode="MultiLine" CssClass="form-control desc" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Description is required" Text="Description is required" ControlToValidate="DescTextBox" CssClass="val" Display="Dynamic"></asp:RequiredFieldValidator>
                    <div class="hint">The first ~170 characters appear as the teaser on the product page. HTML is allowed.</div>
                </div>
            </div>

            <!-- Photos -->
            <div class="adm-panel">
                <div class="d-flex justify-content-between align-items-start flex-wrap gap-2">
                    <h2 class="mb-1"><i class="fa fa-camera"></i> Photos <span class="hint fw-normal ms-1" id="photoCount"></span></h2>
                    <div class="d-flex gap-2 flex-wrap">
                        <label class="btn btn-ib btn-sm mb-0" for="PhotoUpload"><i class="fa fa-upload"></i> Upload photos</label>
                        <button type="button" class="btn btn-ib-o btn-sm" id="addUrlBtn"><i class="fa fa-link"></i> Add by URL</button>
                    </div>
                </div>
                <p class="hint mb-3">These appear in the gallery on the product page. <b>Drag to reorder</b>, the first photo is shown first. JPG, PNG or WebP, up to 5 MB each.</p>

                <asp:FileUpload ID="PhotoUpload" runat="server" ClientIDMode="Static" AllowMultiple="true" accept="image/jpeg,image/png,image/webp,image/gif" CssClass="d-none" />
                <asp:HiddenField ID="PhotosHidden" runat="server" ClientIDMode="Static" />

                <div class="url-row d-none" id="urlRow">
                    <input type="text" class="form-control" id="urlInput" placeholder="Paste image URL, e.g. /drive/theme/khichdi/img/photo.jpg" />
                    <button type="button" class="btn btn-ib btn-sm" id="urlAdd">Add</button>
                </div>

                <div class="gallery" id="gallery"></div>
                <div class="drop" id="dropZone">
                    <i class="fa fa-cloud-upload"></i>
                    <div><b>Drop photos here</b> or <label for="PhotoUpload" class="link">browse</label></div>
                    <div class="hint">New uploads are saved when you click <b>Save product</b>.</div>
                </div>
                <asp:Label ID="PhotoErrorLabel" runat="server" CssClass="val" Visible="false" EnableViewState="false" />
            </div>

            <!-- Pricing -->
            <div class="adm-panel">
                <h2><i class="fa fa-inr"></i> Pricing</h2>
                <div class="row g-3">
                    <div class="col-md-4">
                        <label for="MRPTextBox" class="form-label">MRP <span class="req">*</span></label>
                        <div class="input-group">
                            <span class="input-group-text">₹</span>
                            <asp:TextBox ID="MRPTextBox" TextMode="Number" step="any" min="0" ClientIDMode="Static" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="MRP is required" Text="MRP is required" ControlToValidate="MRPTextBox" CssClass="val" Display="Dynamic"></asp:RequiredFieldValidator>
                    </div>
                    <div class="col-md-4">
                        <label for="SaleTextBox" class="form-label">Sale price <span class="req">*</span></label>
                        <div class="input-group">
                            <span class="input-group-text">₹</span>
                            <asp:TextBox ID="SaleTextBox" TextMode="Number" step="any" min="0" ClientIDMode="Static" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Sale price is required" Text="Sale price is required" ControlToValidate="SaleTextBox" CssClass="val" Display="Dynamic"></asp:RequiredFieldValidator>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">Customer sees</label>
                        <div class="price-preview" id="pricePreview">–</div>
                    </div>
                </div>
                <div class="adm-warn d-none mt-3" id="priceWarn"><i class="fa fa-exclamation-triangle"></i> Sale price is higher than MRP. Customers will see the sale price only.</div>
                <div class="hint mt-2">Set the sale price lower than MRP to show a "% OFF" badge across the site.</div>
            </div>

            <!-- Details -->
            <div class="adm-panel">
                <h2><i class="fa fa-list-ul"></i> Product details</h2>
                <p class="hint mb-3">Shown in the "Details" tab on the product page. Leave a field empty to hide it.</p>
                <div class="row g-3">
                    <div class="col-md-6">
                        <label for="DimensionTextBox" class="form-label">Dimensions</label>
                        <asp:TextBox ID="DimensionTextBox" ClientIDMode="Static" MaxLength="50" CssClass="form-control" runat="server" placeholder='1.5" x 1.5" x 6"'></asp:TextBox>
                        <div class="hint">Length × Width × Height</div>
                    </div>
                    <div class="col-md-6">
                        <label for="WeightTextBox" class="form-label">Weight</label>
                        <asp:TextBox ID="WeightTextBox" ClientIDMode="Static" MaxLength="50" CssClass="form-control" runat="server" placeholder="e.g. 200 g"></asp:TextBox>
                    </div>
                    <div class="col-md-6">
                        <label for="MaterialTextBox" class="form-label">Material</label>
                        <asp:TextBox ID="MaterialTextBox" ClientIDMode="Static" MaxLength="50" CssClass="form-control" runat="server" list="materialList" placeholder="e.g. Polyresin"></asp:TextBox>
                        <datalist id="materialList"><option value="Polyresin"></option><option value="Resin"></option><option value="PLA"></option></datalist>
                    </div>
                    <div class="col-md-6">
                        <label for="ColorTextBox" class="form-label">Colour</label>
                        <asp:TextBox ID="ColorTextBox" ClientIDMode="Static" MaxLength="50" CssClass="form-control" runat="server" placeholder="e.g. Multicolour"></asp:TextBox>
                        <div class="hint">Write "Multicolour" if there are many prominent colours.</div>
                    </div>
                    <div class="col-md-6">
                        <label for="ManufacturerTextBox" class="form-label">Manufacturer</label>
                        <asp:TextBox ID="ManufacturerTextBox" ClientIDMode="Static" MaxLength="50" CssClass="form-control" runat="server" placeholder="India Bobbles"></asp:TextBox>
                    </div>
                    <div class="col-md-6">
                        <label for="CountryOriginTextBox" class="form-label">Country of origin</label>
                        <asp:TextBox ID="CountryOriginTextBox" ClientIDMode="Static" MaxLength="100" CssClass="form-control" runat="server" placeholder="India"></asp:TextBox>
                    </div>
                    <div class="col-md-6">
                        <label for="RecommendAgeTextBox" class="form-label">Recommended age</label>
                        <asp:TextBox ID="RecommendAgeTextBox" ClientIDMode="Static" MaxLength="50" CssClass="form-control" runat="server" placeholder="14 years and above"></asp:TextBox>
                    </div>
                    <div class="col-md-6">
                        <label for="ShippingTimeTextBox" class="form-label">Shipping time</label>
                        <asp:TextBox ID="ShippingTimeTextBox" ClientIDMode="Static" MaxLength="50" CssClass="form-control" runat="server" placeholder="2 days"></asp:TextBox>
                    </div>
                    <div class="col-12">
                        <label for="CareTextBox" class="form-label">Care instructions</label>
                        <asp:TextBox ID="CareTextBox" ClientIDMode="Static" MaxLength="1000" CssClass="form-control" runat="server" placeholder="Wipe with a clean, dry cotton cloth. Keep away from water and direct sunlight."></asp:TextBox>
                    </div>
                </div>
            </div>
        </div>

        <!-- ================= Side column ================= -->
        <div class="col-xl-4">
            <div class="adm-sticky">

                <!-- Publish -->
                <div class="adm-panel">
                    <h2><i class="fa fa-eye"></i> Visibility</h2>
                    <label for="StatusDropDown" class="form-label">Status</label>
                    <asp:DropDownList ID="StatusDropDown" CssClass="form-select" ClientIDMode="Static" runat="server">
                        <asp:ListItem Text="Active – shown on website" Value="0"></asp:ListItem>
                        <asp:ListItem Text="Inactive – hidden" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Deleted" Value="2"></asp:ListItem>
                    </asp:DropDownList>

                    <div class="switches">
                        <asp:CheckBox ID="OutofStockCheckBox" CssClass="ib-switch" Text="Out of stock" TextAlign="Right" runat="server" />
                        <div class="hint">Customers see "Out of stock" and can ask to be notified.</div>
                        <asp:CheckBox ID="FragileCheckBox" CssClass="ib-switch" Text="Fragile" TextAlign="Right" runat="server" />
                        <asp:CheckBox ID="HandmadeCheckBox" CssClass="ib-switch" Text="Handmade / hand-painted" TextAlign="Right" runat="server" />
                    </div>

                    <div class="save-row">
                        <asp:Button ID="SaveButton" runat="server" Text="Save product" CssClass="btn btn-ib w-100" CausesValidation="true" />
                        <a runat="server" href="~/admin/products.aspx" class="cancel">Cancel</a>
                    </div>
                    <div class="hint text-center mt-1">Tip: press Ctrl + S to save</div>
                </div>

                <!-- Image -->
                <div class="adm-panel">
                    <h2><i class="fa fa-image"></i> Thumbnail</h2>
                    <p class="hint mt-n2 mb-2">Used on product cards, the home page and emails.</p>
                    <div class="img-preview" id="imgPreview">
                        <img id="imgPreviewImg" alt="" />
                        <span class="empty"><i class="fa fa-image"></i><br />No image yet</span>
                    </div>
                    <label for="ThumbPathTextBox" class="form-label mt-3">Image URL</label>
                    <asp:TextBox ID="ThumbPathTextBox" ClientIDMode="Static" MaxLength="300" CssClass="form-control" runat="server" placeholder="/drive/theme/khichdi/img/..."></asp:TextBox>
                    <div class="d-flex gap-2 mt-2">
                        <button type="button" class="btn btn-ib-o flex-fill" id="useFirstBtn"><i class="fa fa-star"></i> Use first photo</button>
                        <button type="button" class="btn btn-ib-o flex-fill" data-bs-toggle="modal" data-bs-target="#driveModal"><i class="fa fa-folder-open"></i> Drive</button>
                    </div>
                    <div class="hint mt-2">Square images work best (at least 800 × 800 px). Tip: click ★ on any gallery photo to use it here. If left empty, the first gallery photo is used.</div>
                </div>
            </div>
        </div>
    </div>

    <script>
        (function () {
            var dirty = false;
            var inr = function (n) { return '₹' + Number(n).toLocaleString('en-IN', { maximumFractionDigits: 2 }); };

            // Price preview + warning
            function price() {
                var mrp = parseFloat($('#MRPTextBox').val()), sale = parseFloat($('#SaleTextBox').val());
                var out = '–';
                if (!isNaN(sale)) {
                    out = '<b>' + inr(sale) + '</b>';
                    if (!isNaN(mrp) && mrp > sale && mrp > 0) out += ' <s>' + inr(mrp) + '</s> <span class="off">' + Math.round((mrp - sale) * 100 / mrp) + '% OFF</span>';
                }
                $('#pricePreview').html(out);
                $('#priceWarn').toggleClass('d-none', !(sale > mrp));
            }
            $('#MRPTextBox, #SaleTextBox').on('input', price);
            price();

            // Image preview
            function img() {
                var v = $('#ThumbPathTextBox').val().trim();
                $('#imgPreview').toggleClass('has', !!v);
                $('#imgPreviewImg').attr('src', v || '');
            }
            $('#imgPreviewImg').on('error', function () { $('#imgPreview').removeClass('has'); });
            $('#ThumbPathTextBox').on('input', img);
            img();

            // ================= Photo gallery =================
            var photos = ($('#PhotosHidden').val() || '').split('\n').map(function (x) { return x.trim(); }).filter(Boolean);
            var pending = [];   // File objects waiting to be uploaded on Save
            var MAX = 5 * 1024 * 1024, OK = /^image\/(jpeg|png|webp|gif)$/;

            function esc(t) { return String(t).replace(/[&<>"']/g, function (c) { return { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]; }); }
            function sync() { $('#PhotosHidden').val(photos.join('\n')); }
            function syncFiles() {
                try { var dt = new DataTransfer(); pending.forEach(function (f) { dt.items.add(f); }); document.getElementById('PhotoUpload').files = dt.files; } catch (e) { }
            }
            function render() {
                var thumb = $('#ThumbPathTextBox').val().trim(), h = '';
                photos.forEach(function (p, i) {
                    h += '<div class="ph" draggable="true" data-i="' + i + '">' +
                        '<img src="' + esc(p) + '" alt="" loading="lazy" />' +
                        (i === 0 ? '<span class="tag-main">Main</span>' : '') +
                        (p === thumb ? '<span class="tag-thumb"><i class="fa fa-star"></i></span>' : '') +
                        '<div class="tools">' +
                        '<button type="button" data-act="left" title="Move left"' + (i === 0 ? ' disabled' : '') + '><i class="fa fa-chevron-left"></i></button>' +
                        '<button type="button" data-act="thumb" title="Use as thumbnail"><i class="fa fa-star"></i></button>' +
                        '<button type="button" data-act="del" title="Remove"><i class="fa fa-trash"></i></button>' +
                        '<button type="button" data-act="right" title="Move right"' + (i === photos.length - 1 ? ' disabled' : '') + '><i class="fa fa-chevron-right"></i></button>' +
                        '</div></div>';
                });
                pending.forEach(function (f, i) {
                    h += '<div class="ph pend" data-p="' + i + '"><img data-pf="' + i + '" alt="" /><span class="tag-new">New</span>' +
                        '<div class="tools"><button type="button" data-act="delp" title="Remove"><i class="fa fa-trash"></i></button></div></div>';
                });
                $('#gallery').html(h);
                pending.forEach(function (f, i) {
                    var r = new FileReader(); r.onload = function (e) { $('#gallery img[data-pf="' + i + '"]').attr('src', e.target.result); }; r.readAsDataURL(f);
                });
                var n = photos.length + pending.length;
                $('#photoCount').text(n ? '(' + n + ')' : '');
                $('#dropZone').toggleClass('small', n > 0);
                sync(); syncFiles();
            }

            function addFiles(list) {
                var bad = [];
                Array.prototype.forEach.call(list, function (f) {
                    if (!OK.test(f.type)) bad.push(f.name + ' (not an image)');
                    else if (f.size > MAX) bad.push(f.name + ' (over 5 MB)');
                    else pending.push(f);
                });
                if (bad.length) alert('Skipped:\n' + bad.join('\n'));
                render(); dirty = true;
            }

            $('#PhotoUpload').on('change', function () {
                var files = Array.prototype.slice.call(this.files).filter(function (f) { return pending.indexOf(f) === -1; });
                addFiles(files);         // keeps earlier picks; input is rebuilt from 'pending'
            });

            // Drag & drop files
            $('#dropZone').on('dragover', function (e) { e.preventDefault(); $(this).addClass('over'); })
                .on('dragleave drop', function () { $(this).removeClass('over'); })
                .on('drop', function (e) { e.preventDefault(); addFiles(e.originalEvent.dataTransfer.files); });

            // Add by URL
            $('#addUrlBtn').on('click', function () { $('#urlRow').toggleClass('d-none'); $('#urlInput').focus(); });
            $('#urlAdd').on('click', function () {
                var v = $('#urlInput').val().trim(); if (!v) return;
                photos.push(v); $('#urlInput').val(''); render(); dirty = true;
            });
            $('#urlInput').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); $('#urlAdd').click(); } });

            // Tile buttons
            $('#gallery').on('click', 'button', function () {
                var tile = $(this).closest('.ph'), act = $(this).data('act');
                if (act === 'delp') { pending.splice(+tile.data('p'), 1); render(); return; }
                var i = +tile.data('i');
                if (act === 'del') { if (confirm('Remove this photo from the gallery?')) photos.splice(i, 1); }
                if (act === 'left' && i > 0) photos.splice(i - 1, 0, photos.splice(i, 1)[0]);
                if (act === 'right' && i < photos.length - 1) photos.splice(i + 1, 0, photos.splice(i, 1)[0]);
                if (act === 'thumb') { $('#ThumbPathTextBox').val(photos[i]); img(); }
                render(); dirty = true;
            });

            // Drag to reorder saved photos
            var dragFrom = null;
            $('#gallery').on('dragstart', '.ph:not(.pend)', function (e) { dragFrom = +$(this).data('i'); $(this).addClass('dragging'); e.originalEvent.dataTransfer.effectAllowed = 'move'; })
                .on('dragend', '.ph', function () { $(this).removeClass('dragging'); $('.ph').removeClass('target'); })
                .on('dragover', '.ph:not(.pend)', function (e) { if (dragFrom === null) return; e.preventDefault(); $('.ph').removeClass('target'); $(this).addClass('target'); })
                .on('drop', '.ph:not(.pend)', function (e) {
                    if (dragFrom === null) return; e.preventDefault(); e.stopPropagation();
                    var to = +$(this).data('i'); photos.splice(to, 0, photos.splice(dragFrom, 1)[0]); dragFrom = null; render(); dirty = true;
                });

            $('#useFirstBtn').on('click', function () { if (photos[0]) { $('#ThumbPathTextBox').val(photos[0]); img(); render(); } else alert('Add a gallery photo first.'); });
            $('#ThumbPathTextBox').on('input', render);
            render();

            // Description character count
            function count() { $('#descCount').text(($('#DescTextBox').val() || '').length.toLocaleString('en-IN') + ' characters'); }
            $('#DescTextBox').on('input', count);
            count();

            // Unsaved changes warning + Ctrl/Cmd+S
            $('#mainform').on('input change', 'input, textarea, select', function () { dirty = true; });
            $('#<%= SaveButton.ClientID %>').on('click', function () { dirty = false; });
            window.addEventListener('beforeunload', function (e) { if (dirty) { e.preventDefault(); e.returnValue = ''; } });
            document.addEventListener('keydown', function (e) {
                if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 's') { e.preventDefault(); $('#<%= SaveButton.ClientID %>').click(); }
            });
        })();
    </script>
</asp:Content>