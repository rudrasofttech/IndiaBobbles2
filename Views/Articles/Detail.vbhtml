@ModelType IndiaBobbles.Post
@Code
    Dim shareLink As String = ""
    Dim readMins As Integer = 1
    Dim tags As New List(Of String)()
    Dim bodyHtml As String = ""
    Dim inr = New System.Globalization.CultureInfo("en-IN")
    Dim siteUrl As String = IndiaBobbles.Utility.SiteURL
    Dim updated As DateTime? = Nothing
    Dim RX = System.Text.RegularExpressions.RegexOptions.IgnoreCase

    If Model IsNot Nothing Then
        ViewData("Title") = Model.Title
        ViewBag.CanonicalUrl = siteUrl & "/blog/" & Model.URL.ToLower()
        shareLink = ViewBag.CanonicalUrl

        ' Same clean-up as before
        bodyHtml = If(Model.Article, "").Replace("<datasource name=""ShareDataSource"" />", "").Replace("<datasource name=""DisqusDataSource"" />", "").Replace(System.Environment.NewLine, "<br />")

        ' Images: add alt text where missing (Google can't read comics), lazy-load all but the first
        Dim altText As String = System.Web.HttpUtility.HtmlAttributeEncode(Model.Title & " – India Bobbles comic")
        bodyHtml = System.Text.RegularExpressions.Regex.Replace(bodyHtml, "<img(?![^>]*\balt\s*=\s*[""'][^""']+)", "<img alt=""" & altText & """ ", RX)
        Dim imgNo As Integer = 0
        bodyHtml = System.Text.RegularExpressions.Regex.Replace(bodyHtml, "<img(?![^>]*\bloading=)",
            Function(m)
                imgNo += 1
                Return If(imgNo = 1, "<img decoding=""async"" ", "<img loading=""lazy"" decoding=""async"" ")
            End Function, RX)
        ' Old http:// links and images
        bodyHtml = bodyHtml.Replace("src=""http://", "src=""https://").Replace("href=""http://www.indiabobbles", "href=""https://www.indiabobbles")

        ' Reading time (about 200 words per minute)
        Dim plain As String = System.Text.RegularExpressions.Regex.Replace(bodyHtml, "<[^>]+>", " ")
        Dim words As Integer = plain.Split(New Char() {" "c, ControlChars.Tab, ControlChars.Lf, ControlChars.Cr}, StringSplitOptions.RemoveEmptyEntries).Length
        readMins = Math.Max(1, CInt(Math.Ceiling(words / 200.0)))

        ' Tags (comma separated, without stray full stops or duplicates)
        If Not String.IsNullOrWhiteSpace(Model.Tag) Then
            tags = Model.Tag.Split(","c).Select(Function(t) t.Trim().Trim("."c).Trim()).Where(Function(t) t <> "").Distinct(StringComparer.OrdinalIgnoreCase).ToList()
        End If

        ' "Updated" badge when a post was edited well after publishing (works whether DateModified is nullable or not)
        Dim dm As Object = Model.DateModified
        If dm IsNot Nothing AndAlso CDate(dm) > Model.DateCreated.AddDays(30) Then updated = CDate(dm)
    Else
        ViewData("Title") = "Post not found"
    End If

    Dim currentPostUrl As String = If(Model IsNot Nothing, Model.URL, "")

    ' ---------- Other posts: previous / next, related (shared tags), latest ----------
    Dim allPosts = IndiaBobbles.Utility.GetLatestPosts(500).OrderByDescending(Function(x) x.DateCreated).ToList()
    Dim idx As Integer = allPosts.FindIndex(Function(x) x.URL = currentPostUrl)
    Dim newerPost = If(idx > 0, allPosts(idx - 1), Nothing)
    Dim olderPost = If(idx >= 0 AndAlso idx < allPosts.Count - 1, allPosts(idx + 1), Nothing)

    Dim tagSet As New HashSet(Of String)(tags.Select(Function(t) t.ToLowerInvariant()))
    Dim related = allPosts.Where(Function(x) x.URL <> currentPostUrl AndAlso Not String.IsNullOrWhiteSpace(x.Tag)) _
        .Select(Function(x) New With {.P = x, .Score = x.Tag.Split(","c).Select(Function(t) t.Trim().Trim("."c).ToLowerInvariant()).Count(Function(t) tagSet.Contains(t))}) _
        .Where(Function(x) x.Score > 0) _
        .OrderByDescending(Function(x) x.Score).ThenByDescending(Function(x) x.P.DateCreated) _
        .Select(Function(x) x.P).Take(3).ToList()
    Dim relatedUrls As New HashSet(Of String)(related.Select(Function(x) x.URL))
    Dim lateststories = allPosts.Where(Function(x) x.URL <> currentPostUrl AndAlso Not relatedUrls.Contains(x.URL)).Take(5).ToList()

    ' ---------- Products that appear in this post (name found in title / tags / text) ----------
    Dim featured As New List(Of IndiaBobbles.Product)()
    If Model IsNot Nothing Then
        Dim hay As String = (Model.Title & " " & Model.Tag & " " & System.Text.RegularExpressions.Regex.Replace(bodyHtml, "<[^>]+>", " ")).ToLowerInvariant().Replace("kalaam", "kalam")
        Dim generic As String() = {"bobblehead", "bobble", "head", "figurine", "singh", "mahatma", "the", "india", "indian", "sample", "product"}
        Using db As New IndiaBobbles.indiabobblesEntities()
            For Each p In db.Products.Where(Function(x) x.Status = 0).ToList()
                Dim stem As String = System.Text.RegularExpressions.Regex.Replace(p.Name.ToLowerInvariant(), "\b(bobble\s*head|bobblehead|figurine)\b", "").Trim()
                Dim keys = stem.Split(" "c).Where(Function(w) w.Length >= 4 AndAlso Not generic.Contains(w)).ToList()
                If keys.Count > 0 AndAlso (hay.Contains(stem) OrElse keys.Any(Function(k) System.Text.RegularExpressions.Regex.IsMatch(hay, "\b" & System.Text.RegularExpressions.Regex.Escape(k) & "\b"))) Then
                    featured.Add(p)
                End If
            Next
        End Using
        featured = featured.OrderBy(Function(p) p.OutofStock).Take(2).ToList()
    End If

    Dim shareEnc As String = Uri.EscapeDataString(shareLink)
    Dim titleEnc As String = If(Model IsNot Nothing, Uri.EscapeDataString(Model.Title), "")
    Dim imgAbs As String = If(Model IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(Model.OGImage), If(Model.OGImage.StartsWith("//"), "https:" & Model.OGImage, Model.OGImage), "")
    Dim writer As String = If(Model Is Nothing OrElse String.IsNullOrWhiteSpace(Model.WriterName), "Team India Bobbles", Model.WriterName)

    ' ---------- Structured data (BlogPosting + breadcrumbs) ----------
    Dim ldJson As String = ""
    If Model IsNot Nothing Then
        Dim posting As New Dictionary(Of String, Object) From {
            {"@context", "https://schema.org"}, {"@type", "BlogPosting"},
            {"headline", Model.Title}, {"description", If(Model.Description, "")},
            {"image", imgAbs},
            {"datePublished", Model.DateCreated.ToString("yyyy-MM-dd")},
            {"dateModified", If(updated, Model.DateCreated).ToString("yyyy-MM-dd")},
            {"author", New Dictionary(Of String, Object) From {{"@type", "Organization"}, {"name", writer}}},
            {"publisher", New Dictionary(Of String, Object) From {{"@type", "Organization"}, {"name", "India Bobbles"},
                {"logo", New Dictionary(Of String, Object) From {{"@type", "ImageObject"}, {"url", siteUrl & "/theme/khichdi/img/ib-logo.png"}}}}},
            {"mainEntityOfPage", shareLink}, {"keywords", String.Join(", ", tags)}}
        Dim crumbs As New Dictionary(Of String, Object) From {
            {"@context", "https://schema.org"}, {"@type", "BreadcrumbList"},
            {"itemListElement", New Object() {
                New Dictionary(Of String, Object) From {{"@type", "ListItem"}, {"position", 1}, {"name", "Home"}, {"item", siteUrl & "/"}},
                New Dictionary(Of String, Object) From {{"@type", "ListItem"}, {"position", 2}, {"name", "Blog"}, {"item", siteUrl & "/blog"}},
                New Dictionary(Of String, Object) From {{"@type", "ListItem"}, {"position", 3}, {"name", Model.Title}, {"item", shareLink}}}}}
        ldJson = Newtonsoft.Json.JsonConvert.SerializeObject(New Object() {posting, crumbs}).Replace("</", "<\/")
    End If
End Code

<style>
    /* Article extras – kept in the view so the page works even if site.css is cached */
    .ib-post .crumb .cur {
        color: var(--ib-mut);
    }

    .ib-post a.chip {
        text-decoration: none;
    }

        .ib-post a.chip:hover {
            background: var(--ib-y);
        }

    .ib-post .upd {
        background: var(--ib-ok-bg);
        color: var(--ib-ok);
        font-weight: 700;
        font-size: .78rem;
        padding: 2px 8px;
        border-radius: 12px;
    }

    /* Slide shows inside old articles: round arrows on the image */
    .ib-post #article .carousel {
        position: relative;
        border-radius: 12px;
        overflow: hidden;
    }

    .ib-post #article .ib-arrow {
        position: absolute !important;
        top: 50% !important;
        bottom: auto !important;
        transform: translateY(-50%);
        width: 44px !important;
        height: 44px;
        border-radius: 50%;
        background: rgba(51,11,63,.85) !important;
        color: #fff !important;
        display: flex !important;
        align-items: center;
        justify-content: center;
        font-size: 0 !important;
        opacity: 1 !important;
        z-index: 5;
        text-decoration: none;
        border: 0;
        text-shadow: none;
        background-image: none !important;
    }

        .ib-post #article .ib-arrow > * {
            display: none !important;
        }

        .ib-post #article .ib-arrow.ib-prev {
            left: 12px !important;
            right: auto !important;
        }

        .ib-post #article .ib-arrow.ib-next {
            right: 12px !important;
            left: auto !important;
        }

        .ib-post #article .ib-arrow::before {
            font: normal 1.4rem/1 FontAwesome;
        }

        .ib-post #article .ib-arrow.ib-prev::before {
            content: "\f104";
        }

        .ib-post #article .ib-arrow.ib-next::before {
            content: "\f105";
        }

        .ib-post #article .ib-arrow:hover {
            background: var(--ib-y) !important;
            color: var(--ib-p) !important;
        }

    .ib-post #article img {
        max-width: 100%;
        height: auto;
    }
    /* Products in this story */
    .ib-feat {
        background: var(--ib-y);
        border-radius: var(--ib-r-lg);
        padding: 22px;
        color: var(--ib-p);
    }

        .ib-feat h3 {
            font-weight: 800;
            font-size: 1.3rem;
            margin: 0 0 14px;
        }

        .ib-feat .pc {
            display: flex;
            gap: 14px;
            align-items: center;
            background: #fff;
            border-radius: var(--ib-r);
            padding: 12px;
            height: 100%;
        }

            .ib-feat .pc img {
                width: 92px;
                height: 92px;
                object-fit: cover;
                border-radius: 10px;
                background: var(--ib-bg);
                flex-shrink: 0;
            }

            .ib-feat .pc .n {
                font-weight: 800;
                color: var(--ib-ink);
                line-height: 1.25;
            }

            .ib-feat .pc .pr {
                margin: 4px 0 8px;
            }

                .ib-feat .pc .pr b {
                    font-size: 1.15rem;
                }

                .ib-feat .pc .pr s {
                    color: var(--ib-mut);
                    font-size: .85rem;
                    margin-left: 4px;
                }

            .ib-feat .pc .off {
                background: var(--ib-sale);
                color: #fff;
                font-size: .7rem;
                font-weight: 800;
                padding: 2px 6px;
                border-radius: 5px;
                margin-left: 4px;
            }

            .ib-feat .pc .btn {
                color: #fff;
                font-weight: 700;
                border-radius: 8px;
                padding: .4rem .9rem;
                font-size: .88rem;
            }

                .ib-feat .pc .btn:hover {
                    background: var(--ib-p2);
                    color: white;
                }

        .ib-feat .custom {
            margin: 14px 0 0;
            font-size: .92rem;
        }

            .ib-feat .custom a {
                color: var(--ib-p);
                font-weight: 800;
            }

    /* Author, previous / next, related */
    .ib-author {
        display: flex;
        gap: 14px;
        align-items: center;
        border: 1px solid var(--ib-line);
        border-radius: var(--ib-r);
        padding: 16px 18px;
    }

        .ib-author .logo {
            width: 56px;
            height: 56px;
            border-radius: 50%;
            background: var(--ib-p);
            flex-shrink: 0;
            display: flex;
            align-items: center;
            justify-content: center;
            overflow: hidden;
        }

            .ib-author .logo img {
                width: 48px;
            }

        .ib-author p {
            margin: 0;
            color: var(--ib-mut);
            font-size: .9rem;
        }

    .ib-pn {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 12px;
    }

        .ib-pn a {
            display: flex;
            gap: 12px;
            align-items: center;
            border: 1px solid var(--ib-line);
            border-radius: var(--ib-r);
            padding: 10px;
            text-decoration: none;
            color: var(--ib-ink);
            background: #fff;
        }

            .ib-pn a:hover {
                border-color: var(--ib-p);
            }

            .ib-pn a.next {
                flex-direction: row-reverse;
                text-align: right;
            }

        .ib-pn img {
            width: 64px;
            height: 64px;
            object-fit: cover;
            border-radius: 8px;
            flex-shrink: 0;
            background: var(--ib-bg);
        }

        .ib-pn small {
            display: block;
            color: var(--ib-mut);
            font-weight: 700;
            font-size: .72rem;
            text-transform: uppercase;
            letter-spacing: .06em;
        }

        .ib-pn .t {
            font-weight: 700;
            line-height: 1.3;
            display: -webkit-box;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
            overflow: hidden;
        }

    .ib-rel h3 {
        font-weight: 800;
        color: var(--ib-p);
        font-size: 1.3rem;
    }

    .ib-rel a {
        display: block;
        text-decoration: none;
        color: var(--ib-ink);
        border: 1px solid var(--ib-line);
        border-radius: var(--ib-r);
        overflow: hidden;
        height: 100%;
        background: #fff;
        transition: transform .15s, box-shadow .15s;
    }

        .ib-rel a:hover {
            transform: translateY(-3px);
            box-shadow: var(--ib-shadow-hover);
        }

    .ib-rel .pic {
        aspect-ratio: 16 / 10;
        background: var(--ib-bg);
    }

        .ib-rel .pic img {
            width: 100%;
            height: 100%;
            object-fit: cover;
        }

    .ib-rel .t {
        padding: 10px 12px;
        font-weight: 700;
        font-size: .95rem;
        line-height: 1.3;
    }

    /* Comments load on request */
    .ib-cmt {
        border: 1px dashed var(--ib-line);
        border-radius: var(--ib-r);
        padding: 20px;
        text-align: center;
    }

        .ib-cmt .btn {
            border: 2px solid var(--ib-p);
            color: var(--ib-p);
            font-weight: 700;
            border-radius: 30px;
            padding: .55rem 1.4rem;
            background: #fff;
        }

            .ib-cmt .btn:hover {
                background: var(--ib-p);
                color: #fff;
            }

    @@media (max-width: 575.98px) {
        .ib-pn {
            grid-template-columns: 1fr;
        }

        .ib-feat .pc img {
            width: 72px;
            height: 72px;
        }
    }
</style>

@section scripts
    <script>
        function ibCopyLink(btn) {
            var link = '@Html.Raw(System.Web.HttpUtility.JavaScriptStringEncode(shareLink))';
            if (navigator.clipboard) {
                navigator.clipboard.writeText(link).then(function () {
                    $(btn).attr('title', 'Link copied!').find('i').attr('class', 'fa fa-check');
                    setTimeout(function () { $(btn).attr('title', 'Copy link').find('i').attr('class', 'fa fa-link'); }, 2000);
                });
            } else {
                window.prompt('Copy this link:', link);
            }
        }

        // Comments: Disqus is heavy, so load it only when asked (or when someone arrives via a #comments link)
        function ibLoadComments() {
            if (window.__ibDisqus) return;
            window.__ibDisqus = true;
            $('#cmtBox').hide();
            window.disqus_config = function () {
                this.page.url = '@Html.Raw(System.Web.HttpUtility.JavaScriptStringEncode(shareLink))';
                this.page.identifier = '@Html.Raw(System.Web.HttpUtility.JavaScriptStringEncode(currentPostUrl))';
            };
            var d = document.createElement('script');
            d.src = 'https://indiabobbles.disqus.com/embed.js';
            d.setAttribute('data-timestamp', +new Date());
            (document.head || document.body).appendChild(d);
        }
        if (/#(comments|disqus_thread|comment-)/.test(location.hash)) ibLoadComments();

        // Slide shows in old articles (Bootstrap 3 markup): convert for Bootstrap 5, round arrows, hide arrows for one image
        $('#article .carousel').each(function (i) {
            var c = $(this);
            if (!c.attr('id')) c.attr('id', 'ibSlides' + i);
            c.find('.item').addClass('carousel-item');
            var items = c.find('.carousel-item');
            var prev = c.find('.left.carousel-control, .carousel-control-prev, [data-slide="prev"], [data-bs-slide="prev"]');
            var next = c.find('.right.carousel-control, .carousel-control-next, [data-slide="next"], [data-bs-slide="next"]');
            if (items.length <= 1) { prev.add(next).hide(); c.find('.carousel-indicators').hide(); return; }
            if (!items.filter('.active').length) items.first().addClass('active');
            prev.addClass('ib-arrow ib-prev').attr({ 'data-bs-slide': 'prev', 'data-bs-target': '#' + c.attr('id'), 'aria-label': 'Previous picture', role: 'button' });
            next.addClass('ib-arrow ib-next').attr({ 'data-bs-slide': 'next', 'data-bs-target': '#' + c.attr('id'), 'aria-label': 'Next picture', role: 'button' });
            if (window.bootstrap && bootstrap.Carousel) bootstrap.Carousel.getOrCreateInstance(this, { interval: false });
        });
    </script>
End Section

@If ldJson <> "" Then
    @<script type="application/ld+json">@Html.Raw(ldJson)</script>
End If

<div class="ib-post container py-4">
    <div class="row g-5">

        <!-- ============ Article ============ -->
        <div class="col-lg-8">
            @If Model IsNot Nothing Then
                @<article>
                    <nav class="crumb mb-2" aria-label="Breadcrumb"><a href="~/">Home</a> &rsaquo; <a href="~/blog">Blog</a> &rsaquo; <span class="cur">@(If(Model.Title.Length > 40, Model.Title.Substring(0, 40) & "…", Model.Title))</span></nav>

                    @For Each t In tags
                        @<span class="chip">@t</span>
                    Next

                    <h1>@Model.Title</h1>

                    <div class="d-flex justify-content-between align-items-center flex-wrap gap-3 pb-3 mb-4" style="border-bottom:1px solid #ece6ef;">
                        <div class="meta">
                            <span><span class="av">@writer.Substring(0, 1).ToUpper()</span>@writer</span>
                            <span><i class="fa fa-calendar-o" aria-hidden="true"></i> <time datetime="@Model.DateCreated.ToString("yyyy-MM-dd")">@Model.DateCreated.ToString("d MMMM yyyy")</time></span>
                            @If updated.HasValue Then
                                @<span class="upd">Updated @updated.Value.ToString("MMM yyyy")</span>
                            End If
                            <span><i class="fa fa-clock-o" aria-hidden="true"></i> @readMins min read</span>
                        </div>
                        <div class="share">
                            <a class="wa" href="https://wa.me/?text=@titleEnc%20@shareEnc" target="_blank" rel="noopener" title="Share on WhatsApp"><i class="fa fa-whatsapp" aria-hidden="true"></i></a>
                            <a href="https://www.facebook.com/sharer/sharer.php?u=@shareEnc" target="_blank" rel="noopener" title="Share on Facebook"><i class="fa fa-facebook" aria-hidden="true"></i></a>
                            <a href="https://twitter.com/intent/tweet?url=@shareEnc&amp;text=@titleEnc" target="_blank" rel="noopener" title="Share on X"><b style="font-family:Arial,sans-serif;font-size:.95rem;">𝕏</b></a>
                            @If imgAbs <> "" Then
                                @<a href="https://pinterest.com/pin/create/button/?url=@shareEnc&amp;media=@Uri.EscapeDataString(imgAbs)&amp;description=@titleEnc" target="_blank" rel="noopener" title="Save to Pinterest"><i class="fa fa-pinterest-p" aria-hidden="true"></i></a>
                            End If
                            <button type="button" onclick="ibCopyLink(this)" title="Copy link"><i class="fa fa-link" aria-hidden="true"></i></button>
                        </div>
                    </div>

                    <div id="article">
                        @Html.Raw(bodyHtml)
                    </div>

                    <!-- ============ Products in this story (or the general promo) ============ -->
                    @If featured.Count > 0 Then
                        @<div class="ib-feat my-5">
                            <h3>🎭 Take @(If(featured.Count = 1, "this character", "these characters")) home</h3>
                            <div class="row g-3">
                                @For Each p In featured
                                    Dim pUrl As String = Url.Content("~/product/" & p.ID & "/" & IndiaBobbles.Utility.Slugify(p.Name))
                                    Dim onSale As Boolean = p.MRP > p.SalePrice AndAlso p.MRP > 0
                                    @<div class="@(If(featured.Count = 1, "col-12", "col-md-6"))">
                                        <div class="pc">
                                            <a href="@pUrl"><img src="@p.ThumbPath" alt="@p.Name" loading="lazy" /></a>
                                            <div>
                                                <a href="@pUrl" class="n text-decoration-none">@p.Name</a>
                                                <div class="pr">
                                                    <b>₹@(p.SalePrice.ToString("#,##0", inr))</b>
                                                    @If onSale Then
                                                        @<s>₹@(p.MRP.ToString("#,##0", inr))</s>
                                                        @<span class="off">@(Math.Round((p.MRP - p.SalePrice) * 100 / p.MRP))% OFF</span>
                                                    End If
                                                </div>
                                                @If p.OutofStock Then
                                                    @<a class="btn" href="@pUrl">Notify me</a>
                                                Else
                                                    @<a class="btn" href="@pUrl">Buy now <i class="fa fa-arrow-right" aria-hidden="true"></i></a>
                                                End If
                                            </div>
                                        </div>
                                    </div>
                                Next
                            </div>
                            <p class="custom"><a href="~/order-custom-bobbleheads">Get your own custom bobblehead →</a></p>
                        </div>
                    Else
                        @<div class="promo d-flex justify-content-between align-items-center flex-wrap gap-3 my-5">
                            <div>
                                <h3>🎭 Meet the characters</h3>
                                <p class="mb-0">Bring Daku Sambhar Singh, Chhote Bachchan and friends home, hand-painted and shipped free.</p>
                            </div>
                            <a class="btn btn-ibp" href="~/tag/collectibles">Shop bobbleheads <i class="fa fa-arrow-right" aria-hidden="true"></i></a>
                        </div>
                    End If

                    <!-- ============ Author ============ -->
                    @*<div class="ib-author mb-4">
                            <span class="logo"><img src="~/theme/khichdi/img/ib-logo.png" alt="" /></span>
                            <div>
                                <b style="color:#330B3F;">@writer</b>
                                <p>India Bobbles makes hand-painted Bollywood and Indian bobbleheads – and the comics they star in. Made in Noida, shipped across India.</p>
                            </div>
                        </div>*@

                    <!-- ============ Previous / next ============ -->
                    @If newerPost IsNot Nothing OrElse olderPost IsNot Nothing Then
                        @<nav class="ib-pn mb-5" aria-label="More stories">
                            @If olderPost IsNot Nothing Then
                                @<a href="@Url.Content("~/blog/" & olderPost.URL)" rel="prev">
                                    @If Not String.IsNullOrEmpty(olderPost.OGImage) Then
                                        @<img src="@olderPost.OGImage" alt="" loading="lazy" />
                                    End If
                                    <div><small>← Previous story</small><span class="t">@olderPost.Title</span></div>
                                </a>
                            Else
                                @<span></span>
                            End If
                            @If newerPost IsNot Nothing Then
                                @<a class="next" href="@Url.Content("~/blog/" & newerPost.URL)" rel="next">
                                    @If Not String.IsNullOrEmpty(newerPost.OGImage) Then
                                        @<img src="@newerPost.OGImage" alt="" loading="lazy" />
                                    End If
                                    <div><small>Next story →</small><span class="t">@newerPost.Title</span></div>
                                </a>
                            End If
                        </nav>
                    End If

                    <!-- ============ Related (shares a tag / character) ============ -->
                    @If related.Count > 0 Then
                        @<section class="ib-rel mb-5">
                            <h3 class="mb-3">More stories like this</h3>
                            <div class="row g-3">
                                @For Each r In related
                                    @<div class="col-sm-4">
                                        <a href="@Url.Content("~/blog/" & r.URL)">
                                            <div class="pic">
                                                @If Not String.IsNullOrEmpty(r.OGImage) Then
                                                    @<img src="@r.OGImage" alt="" loading="lazy" />
                                                End If
                                            </div>
                                            <div class="t">@r.Title</div>
                                        </a>
                                    </div>
                                Next
                            </div>
                        </section>
                    End If

                    <!-- ============ Comments (load on request) ============ -->
                    <h4 class="fw-bold mb-3" style="color:#330B3F;" id="comments">Comments</h4>
                    <div class="ib-cmt" id="cmtBox">
                        <p class="mb-3 text-muted">Laughed? Got a better caption? Tell us.</p>
                        <button type="button" class="btn" onclick="ibLoadComments()"><i class="fa fa-comments-o" aria-hidden="true"></i> Show comments</button>
                    </div>
                    <div id="disqus_thread"></div>
                </article>
            Else
                @<div class="text-center py-5">
                    <h1>Post not found</h1>
                    <p class="text-muted">This post may have moved. Try our latest stories below or on the blog.</p>
                    <a href="~/blog" class="btn btn-ibp">Go to the blog</a>
                </div>
            End If
        </div>

        <!-- ============ Sidebar ============ -->
        <div class="col-lg-4">
            <aside class="side">
                <h4>Latest from the blog</h4>
                @For Each m In lateststories
                    @<a class="mini" href="@Url.Content("~/blog/" & m.URL)">
                        <span class="th">
                            @If Not String.IsNullOrEmpty(m.OGImage) Then
                                @<img src="@m.OGImage" alt="" loading="lazy" />
                            End If
                        </span>
                        <span>@m.Title</span>
                    </a>
                Next
                <a href="~/blog" class="d-inline-block mt-3 fw-bold text-decoration-none" style="color:#330B3F;">All posts <i class="fa fa-arrow-right" aria-hidden="true"></i></a>

                <div class="side-shop">
                    <h5 class="fw-bold mb-2">Hand-painted bobbleheads</h5>
                    <p>Bollywood legends and Indian icons. Free shipping across India.</p>
                    <a class="btn w-100" href="~/tag/collectibles">Shop now</a>
                </div>
            </aside>
        </div>
    </div>
</div>