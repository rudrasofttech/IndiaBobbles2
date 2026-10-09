<%@ Page Title="Article" Language="vb" ValidateRequest="false" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="ManageArticle.aspx.vb" Inherits="IndiaBobbles.ManageArticle" %>

<%@ Register Src="~/admin/controls/Message.ascx" TagPrefix="uc1" TagName="Message" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">
    <asp:SqlDataSource ID="CategorySource" runat="server" CacheExpirationPolicy="Sliding"
        ConnectionString="<%$ ConnectionStrings:indiabobblesConnectionString %>" DataSourceMode="DataReader"
        SelectCommand="SELECT ID, Name FROM Category WHERE Status = 0 ORDER BY Name"></asp:SqlDataSource>

    <div class="ma">
        <!-- ============ Heading ============ -->
        <a href="articles.aspx" class="adm-back"><i class="fa fa-angle-left"></i> All articles</a>
        <div class="adm-head">
            <div>
                <h1><asp:Literal ID="HeadingLit" runat="server">New article</asp:Literal></h1>
                <p>Write in HTML or plain text – check the Preview tab before publishing.</p>
            </div>
            <asp:HyperLink ID="ViewLink" runat="server" Visible="false" Target="_blank" CssClass="btn btn-ib-o"><i class="fa fa-external-link"></i> View on website</asp:HyperLink>
        </div>

        <uc1:Message ID="message1" Visible="false" runat="server" />
        <asp:ValidationSummary runat="server" ValidationGroup="VideoGrp" CssClass="adm-error" HeaderText="Please fix these before saving:" DisplayMode="BulletList" />

        <div class="row g-3">
            <!-- ============ Main column ============ -->
            <div class="col-xl-8">
                <div class="adm-panel">
                    <div class="mb-3">
                        <div class="lbl-row">
                            <label class="form-label" for="TitleTextBox">Title <span class="text-danger">*</span></label>
                            <span class="cc" id="ccTitle"></span>
                        </div>
                        <asp:TextBox CssClass="form-control title-in" ID="TitleTextBox" ClientIDMode="Static" MaxLength="250" runat="server" placeholder="e.g. 10 Reasons to Collect Bollywood Bobbleheads" />
                        <asp:RequiredFieldValidator ID="TitleReqVal" ValidationGroup="VideoGrp" ControlToValidate="TitleTextBox" runat="server"
                            ErrorMessage="Title is required" CssClass="validate" Display="Dynamic" SetFocusOnError="True" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="URLTextBox">Web address <span class="text-danger">*</span></label>
                        <div class="url-row" id="urlRow">
                            <span class="pre"><%= IndiaBobbles.Utility.SiteURL %>/blog/</span>
                            <asp:TextBox CssClass="form-control" ID="URLTextBox" ClientIDMode="Static" MaxLength="250" runat="server" placeholder="filled-in-from-the-title" />
                        </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ValidationGroup="VideoGrp" ControlToValidate="URLTextBox" runat="server"
                            ErrorMessage="Web address is required" CssClass="validate" Display="Dynamic" SetFocusOnError="True" />
                        <asp:CustomValidator ID="CustomValidator1" runat="server" ValidationGroup="VideoGrp" ControlToValidate="URLTextBox"
                            ErrorMessage="Another article already uses this web address – change the title or edit the address." CssClass="validate"
                            Display="Dynamic" OnServerValidate="CustomValidator1_ServerValidate" SetFocusOnError="True" />
                        <div class="hint" id="urlHint">Filled in from the title. Lowercase letters, numbers and hyphens.</div>
                    </div>

                    <div class="mb-0">
                        <div class="lbl-row">
                            <label class="form-label" for="DescTextBox">Short description <span class="text-danger">*</span></label>
                            <span class="cc" id="ccDesc"></span>
                        </div>
                        <asp:TextBox CssClass="form-control" ID="DescTextBox" ClientIDMode="Static" TextMode="MultiLine" Rows="3" runat="server"
                            placeholder="One or two sentences shown in the blog list and in Google results." />
                        <asp:RequiredFieldValidator ID="DescReqVal" ValidationGroup="VideoGrp" ControlToValidate="DescTextBox" runat="server"
                            ErrorMessage="Short description is required" CssClass="validate" Display="Dynamic" SetFocusOnError="True" />
                    </div>
                </div>

                <!-- Body -->
                <div class="adm-panel">
                    <div class="d-flex justify-content-between align-items-center flex-wrap gap-2">
                        <div class="ma-tabs mb-0" style="border:0;">
                            <button type="button" class="on" data-tab="write"><i class="fa fa-pencil"></i> Write</button>
                            <button type="button" data-tab="preview"><i class="fa fa-eye"></i> Preview</button>
                        </div>
                        <span class="cc" id="ccBody"></span>
                    </div>

                    <div id="tabWrite">
                        <div class="ma-tools" aria-label="Formatting">
                            <button type="button" data-ins="h2" title="Heading">H2</button>
                            <button type="button" data-ins="h3" title="Sub-heading">H3</button>
                            <button type="button" data-ins="p" title="Paragraph"><i class="fa fa-paragraph"></i></button>
                            <button type="button" data-ins="b" title="Bold"><i class="fa fa-bold"></i></button>
                            <button type="button" data-ins="i" title="Italic"><i class="fa fa-italic"></i></button>
                            <button type="button" data-ins="ul" title="Bullet list"><i class="fa fa-list-ul"></i></button>
                            <button type="button" data-ins="quote" title="Quote"><i class="fa fa-quote-left"></i></button>
                            <span class="sep"></span>
                            <button type="button" data-ins="a" title="Link"><i class="fa fa-link"></i></button>
                            <button type="button" data-ins="img" title="Image"><i class="fa fa-image"></i></button>
                            <button type="button" data-ins="product" title="Link to a product"><i class="fa fa-shopping-bag"></i> Product</button>
                            <button type="button" data-ins="hr" title="Divider">―</button>
                            <span class="sep"></span>
                            <button type="button" data-bs-toggle="modal" data-bs-target="#driveModal" title="Find an image in Drive"><i class="fa fa-folder-open"></i> Drive</button>
                        </div>
                        <asp:TextBox CssClass="form-control body-in" ID="TextTextBox" ClientIDMode="Static" TextMode="MultiLine" Rows="22" runat="server" />
                        <asp:RequiredFieldValidator ID="TextReqVal" ValidationGroup="VideoGrp" ControlToValidate="TextTextBox" runat="server"
                            ErrorMessage="Article text is required" CssClass="validate" Display="Dynamic" SetFocusOnError="True" />
                        <div class="hint">Select some text, then click a button to wrap it. Copy image addresses from Drive.</div>
                    </div>
                    <div id="tabPreview" class="d-none">
                        <iframe id="pvFrame" class="ma-frame" title="Article preview" sandbox=""></iframe>
                    </div>
                </div>
            </div>

            <!-- ============ Sidebar ============ -->
            <div class="col-xl-4">
                <div class="adm-sticky">
                    <!-- Publish -->
                    <div class="adm-panel save">
                        <h2><i class="fa fa-paper-plane"></i> Publish</h2>
                        <div class="seg mb-2" id="stSeg">
                            <input type="radio" name="maSt" id="st1" value="1" /><label for="st1">Draft</label>
                            <input type="radio" name="maSt" id="st2" value="2" /><label for="st2">Published</label>
                            <input type="radio" name="maSt" id="st3" value="3" /><label for="st3">Inactive</label>
                        </div>
                        <asp:DropDownList ID="StatusDropDown" ClientIDMode="Static" CssClass="d-none" runat="server">
                            <asp:ListItem Value="1">Draft</asp:ListItem>
                            <asp:ListItem Selected="True" Value="2">Publish</asp:ListItem>
                            <asp:ListItem Value="3">Inactive</asp:ListItem>
                        </asp:DropDownList>
                        <div class="hint mb-2" id="stHint"></div>

                        <div class="sw">
                            <asp:CheckBox ID="SitemapCheckBox" ClientIDMode="Static" Checked="true" runat="server" />
                            <label for="SitemapCheckBox">Include in sitemap</label>
                        </div>
                        <div class="sw">
                            <asp:CheckBox ID="SlideShowCheckBox" ClientIDMode="Static" runat="server" />
                            <label for="SlideShowCheckBox">Slide show</label>
                        </div>
                        <div class="sw">
                            <asp:CheckBox ID="QuestionCheckBox" ClientIDMode="Static" runat="server" />
                            <label for="QuestionCheckBox">Question</label>
                        </div>

                        <asp:Button ID="SubmitButton" ValidationGroup="VideoGrp" CssClass="btn btn-ib w-100 mt-3" runat="server" Text="Save" OnClick="SubmitButton_Click" />
                        <asp:Button ID="SaveStayButton" ValidationGroup="VideoGrp" CssClass="btn btn-ib-o btn-sm stay" runat="server" Text="Save & keep editing (Ctrl+S)" OnClick="SubmitButton_Click" ClientIDMode="Static" />
                        <div class="dirty" id="dirtyNote"><i class="fa fa-circle"></i> Unsaved changes</div>
                        <a href="articles.aspx" class="d-block text-center mt-2 text-muted small">Cancel</a>
                    </div>

                    <!-- Details -->
                    <div class="adm-panel">
                        <h2><i class="fa fa-user"></i> Details</h2>
                        <div class="mb-3">
                            <label class="form-label" for="CategoryDropDown">Category <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="CategoryDropDown" ClientIDMode="Static" CssClass="form-select" runat="server" DataSourceID="CategorySource"
                                DataTextField="Name" DataValueField="ID" AppendDataBoundItems="true">
                                <asp:ListItem Selected="True" Value="">Choose…</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="CategoryReqVal" ValidationGroup="VideoGrp" ControlToValidate="CategoryDropDown" runat="server"
                                ErrorMessage="Choose a category" CssClass="validate" Display="Dynamic" SetFocusOnError="True" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label" for="WriterTextBox">Writer name <span class="text-danger">*</span></label>
                            <asp:TextBox CssClass="form-control" ID="WriterTextBox" ClientIDMode="Static" MaxLength="250" runat="server" />
                            <asp:RequiredFieldValidator ID="WriterReqVal" ValidationGroup="VideoGrp" ControlToValidate="WriterTextBox" runat="server"
                                ErrorMessage="Writer name is required" CssClass="validate" Display="Dynamic" SetFocusOnError="True" />
                        </div>
                        <div class="mb-0">
                            <label class="form-label" for="WriterEmailTextBox">Writer email <span class="text-danger">*</span></label>
                            <asp:TextBox CssClass="form-control" ID="WriterEmailTextBox" ClientIDMode="Static" MaxLength="250" runat="server" TextMode="Email" />
                            <asp:RequiredFieldValidator ID="WriterEmailReqVal" ValidationGroup="VideoGrp" ControlToValidate="WriterEmailTextBox" runat="server"
                                ErrorMessage="Writer email is required" CssClass="validate" Display="Dynamic" SetFocusOnError="True" />
                        </div>
                    </div>

                    <!-- SEO -->
                    <div class="adm-panel">
                        <h2><i class="fa fa-google"></i> Search (SEO)</h2>
                        <div class="google mb-3" aria-hidden="true">
                            <div class="u" id="gUrl">indiabobbles.com › blog</div>
                            <div class="t" id="gTitle">Article title</div>
                            <div class="d" id="gDesc">Short description</div>
                        </div>
                        <div class="mb-3">
                            <div class="lbl-row">
                                <label class="form-label" for="MetaTitleTextBox">Search title</label>
                                <span class="cc" id="ccMeta"></span>
                            </div>
                            <asp:TextBox CssClass="form-control" ID="MetaTitleTextBox" ClientIDMode="Static" MaxLength="250" runat="server" placeholder="Leave empty to use the article title" />
                            <div class="hint">Shown as the blue link in Google. Aim for under 60 characters.</div>
                        </div>
                        <div class="mb-0">
                            <label class="form-label" for="TagTextBox">Keywords <span class="text-danger">*</span></label>
                            <asp:TextBox CssClass="form-control" ID="TagTextBox" ClientIDMode="Static" runat="server" placeholder="bobblehead, bollywood, gifts" />
                            <asp:RequiredFieldValidator ID="TagReqVal" ValidationGroup="VideoGrp" ControlToValidate="TagTextBox" runat="server"
                                ErrorMessage="Add at least one keyword" CssClass="validate" Display="Dynamic" SetFocusOnError="True" />
                            <div class="hint">Separate with commas.</div>
                        </div>
                    </div>

                    <!-- Social -->
                    <div class="adm-panel">
                        <h2><i class="fa fa-facebook"></i> Social sharing</h2>
                        <div class="fb mb-3" aria-hidden="true">
                            <div class="img" id="fbImg">No image</div>
                            <div class="b">
                                <div class="s">indiabobbles.com</div>
                                <div class="t" id="fbTitle">Article title</div>
                                <div class="d" id="fbDesc">Description</div>
                            </div>
                        </div>
                        <div class="mb-3">
                            <label class="form-label" for="FacebookImageTextBox">Share image</label>
                            <div class="d-flex gap-2">
                                <asp:TextBox CssClass="form-control" ID="FacebookImageTextBox" ClientIDMode="Static" MaxLength="250" runat="server" placeholder="https://www.indiabobbles.com/drive/…" />
                                <button type="button" class="btn btn-ib-o btn-sm" id="fbFromBody" title="Use the first image in the article"><i class="fa fa-magic"></i></button>
                            </div>
                            <div class="hint">Best size 1200 × 630. <i class="fa fa-magic"></i> uses the first image in the article.</div>
                        </div>
                        <div class="mb-0">
                            <label class="form-label" for="FacebookDescTextBox">Share description</label>
                            <asp:TextBox CssClass="form-control" ID="FacebookDescTextBox" ClientIDMode="Static" MaxLength="250" runat="server" placeholder="Leave empty to use the short description" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
        (function () {
            var $t = $('#TitleTextBox'), $u = $('#URLTextBox'), $d = $('#DescTextBox'), $b = $('#TextTextBox'),
                $m = $('#MetaTitleTextBox'), $fi = $('#FacebookImageTextBox'), $fd = $('#FacebookDescTextBox'), $st = $('#StatusDropDown');
            var isEdit = <%= If(Mode = "edit", "true", "false") %>, urlTouched = isEdit || $u.val() !== '', dirty = false;

            function slug(s) { return (s || '').toLowerCase().replace(/&/g, ' and ').replace(/['’]/g, '').replace(/[^a-z0-9]+/g, '-').replace(/^-+|-+$/g, '').substring(0, 120); }
            function cc(el, $in, max) { var n = $in.val().length; $(el).text(n + ' / ' + max).toggleClass('over', n > max); }

            // ---------- Web address ----------
            if (isEdit && $st.val() === '2') $('#urlHint').html('<i class="fa fa-exclamation-triangle"></i> This article is live – changing the address breaks links people have shared.');
            $t.on('input', function () { if (!urlTouched) $u.val(slug($t.val())); });
            $u.on('input', function () { urlTouched = $u.val() !== ''; });
            $u.on('blur', function () { $u.val(slug($u.val())); refresh(); });

            // ---------- Status segment ----------
            var hints = { '1': 'Only visible to you. Nobody can see it on the blog yet.', '2': 'Live on the blog for everyone.', '3': 'Hidden from the blog, kept for later.' };
            $('#stSeg input[value="' + $st.val() + '"]').prop('checked', true);
            function stHint() { $('#stHint').text(hints[$st.val()] || ''); }
            $('#stSeg input').on('change', function () { $st.val(this.value); stHint(); setDirty(); });
            stHint();

            // ---------- Previews ----------
            function refresh() {
                var title = $t.val().trim() || 'Article title', meta = $m.val().trim() || title, desc = $d.val().trim() || 'Short description';
                $('#gUrl').text('indiabobbles.com › blog › ' + ($u.val() || '…'));
                $('#gTitle').text(meta);
                $('#gDesc').text(desc);
                $('#fbTitle').text(title);
                $('#fbDesc').text($fd.val().trim() || desc);
                var img = $fi.val().trim();
                $('#fbImg').css('background-image', img ? 'url("' + img.replace(/"/g, '') + '")' : 'none').text(img ? '' : 'No image');
                cc('#ccTitle', $t, 70); cc('#ccMeta', $m, 60); cc('#ccDesc', $d, 160);
                var words = $('<div>').html($b.val()).text().trim().split(/\s+/).filter(Boolean).length;
                $('#ccBody').text(words + ' words · ~' + Math.max(1, Math.round(words / 200)) + ' min read');
            }
            $('.ma input, .ma textarea, .ma select').on('input change', function () { refresh(); setDirty(); });
            refresh();

            $('#fbFromBody').on('click', function () {
                var m = $b.val().match(/<img[^>]+src=["']([^"']+)["']/i);
                if (!m) { alert('No image found in the article text.'); return; }
                var src = m[1]; if (src.indexOf('//') === 0) src = 'https:' + src;
                $fi.val(src); refresh(); setDirty();
            });

            // ---------- Formatting buttons ----------
            function wrap(before, after, def) {
                var el = $b[0], s = el.selectionStart, e = el.selectionEnd, v = el.value, sel = v.substring(s, e) || def || '';
                el.value = v.substring(0, s) + before + sel + after + v.substring(e);
                el.focus(); el.selectionStart = s + before.length; el.selectionEnd = s + before.length + sel.length;
                refresh(); setDirty();
            }
            var SNIP = {
                h2: function () { wrap('<h2>', '</h2>\n', 'Heading'); },
                h3: function () { wrap('<h3>', '</h3>\n', 'Sub-heading'); },
                p: function () { wrap('<p>', '</p>\n', 'Paragraph'); },
                b: function () { wrap('<b>', '</b>', 'bold text'); },
                i: function () { wrap('<i>', '</i>', 'italic text'); },
                ul: function () { wrap('<ul>\n  <li>', '</li>\n  <li>Second point</li>\n</ul>\n', 'First point'); },
                quote: function () { wrap('<blockquote>', '</blockquote>\n', 'Quote'); },
                hr: function () { wrap('\n<hr />\n', '', ''); },
                a: function () { var u = prompt('Link address', 'https://www.indiabobbles.com/'); if (u) wrap('<a href="' + u.replace(/"/g, '') + '">', '</a>', 'link text'); },
                img: function () {
                    var u = prompt('Image address (copy it from Drive)'); if (!u) return;
                    var alt = prompt('Describe the image in a few words (helps Google and screen readers)', '') || '';
                    wrap('<img src="' + u.replace(/"/g, '') + '" alt="' + alt.replace(/"/g, '') + '" style="max-width:100%;height:auto;" />\n', '', '');
                },
                product: function () {
                    var u = prompt('Product page address', 'https://www.indiabobbles.com/product/'); if (!u) return;
                    wrap('<p><a href="' + u.replace(/"/g, '') + '" class="btn btn-warning">', '</a></p>\n', 'Buy this bobblehead');
                }
            };
            $('.ma-tools [data-ins]').on('click', function () { SNIP[$(this).data('ins')](); });

            // ---------- Preview tab ----------
            function renderPreview() {
                var body = $b.val();
                if (!/<[a-z][\s\S]*>/i.test(body)) body = body.split(/\n{2,}/).map(function (p) { return '<p>' + $('<div>').text(p).html().replace(/\n/g, '<br>') + '</p>'; }).join('');
                body = body.replace(/(src|href)=(["'])\/\//gi, '$1=$2https://');
                document.getElementById('pvFrame').srcdoc =
                    '<!doctype html><html><head><meta charset="utf-8"><style>body{margin:0;font-family:Georgia,serif;color:#1f1724;line-height:1.7;font-size:17px}' +
                    '.w{max-width:720px;margin:0 auto;padding:28px 22px}h1{font-family:Arial,sans-serif;color:#330B3F;font-size:30px;line-height:1.25}' +
                    'h2,h3,h4,h5{font-family:Arial,sans-serif;color:#330B3F}img{max-width:100%;height:auto;border-radius:8px}.lead{color:#6b5f72;font-size:19px}' +
                    'blockquote{border-left:4px solid #ffc107;margin:0;padding:4px 16px;color:#3e3445}.btn{display:inline-block;background:#ffc107;color:#330B3F;padding:10px 20px;border-radius:8px;font-weight:bold;text-decoration:none;font-family:Arial}' +
                    '.by{font-family:Arial;font-size:13px;color:#6b5f72}</style></head><body><div class="w">' +
                    '<h1>' + $('<div>').text($t.val() || 'Article title').html() + '</h1>' +
                    '<div class="by">By ' + $('<div>').text($('#WriterTextBox').val() || '…').html() + '</div>' +
                    '<p class="lead">' + $('<div>').text($d.val()).html() + '</p>' + body + '</div></body></html>';
            }
            $('.ma-tabs button').on('click', function () {
                var t = $(this).data('tab');
                $('.ma-tabs button').removeClass('on'); $(this).addClass('on');
                $('#tabWrite').toggleClass('d-none', t !== 'write'); $('#tabPreview').toggleClass('d-none', t !== 'preview');
                if (t === 'preview') renderPreview();
            });

            // ---------- Unsaved changes + Ctrl+S ----------
            function setDirty() { dirty = true; $('#dirtyNote').addClass('on'); }
            $('#<%= SubmitButton.ClientID %>, #SaveStayButton').on('click', function () { dirty = false; });
            window.addEventListener('beforeunload', function (e) { if (dirty) { e.preventDefault(); e.returnValue = ''; } });
            document.addEventListener('keydown', function (e) {
                if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 's') { e.preventDefault(); $('#SaveStayButton').click(); }
            });
        })();
    </script>
</asp:Content>