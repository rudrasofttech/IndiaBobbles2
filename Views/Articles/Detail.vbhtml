@ModelType IndiaBobbles.Post
@Code
    Dim shareLink As String = ""
    Dim readMins As Integer = 1
    Dim tags As New List(Of String)()
    Dim bodyHtml As String = ""

    If Model IsNot Nothing Then
        ViewData("Title") = Model.Title
        ViewBag.CanonicalUrl = IndiaBobbles.Utility.SiteURL & "/blog/" & Model.URL.ToLower()
        shareLink = ViewBag.CanonicalUrl

        ' Same clean-up as before
        bodyHtml = If(Model.Article, "").Replace("<datasource name=""ShareDataSource"" />", "").Replace("<datasource name=""DisqusDataSource"" />", "").Replace(System.Environment.NewLine, "<br />")

        ' Reading time (about 200 words per minute)
        Dim plain As String = System.Text.RegularExpressions.Regex.Replace(bodyHtml, "<[^>]+>", " ")
        Dim words As Integer = plain.Split(New Char() {" "c, ControlChars.Tab, ControlChars.Lf, ControlChars.Cr}, StringSplitOptions.RemoveEmptyEntries).Length
        readMins = Math.Max(1, CInt(Math.Ceiling(words / 200.0)))

        ' Tags (comma separated)
        If Not String.IsNullOrWhiteSpace(Model.Tag) Then
            tags = Model.Tag.Split(","c).Select(Function(t) t.Trim()).Where(Function(t) t <> "").ToList()
        End If
    Else
        ViewData("Title") = "Post not found"
    End If

    Dim currentPostUrl As String = If(Model IsNot Nothing, Model.URL, "")
    Dim lateststories = IndiaBobbles.Utility.GetLatestPosts(10).Where(Function(x) x.URL <> currentPostUrl).Take(6).ToList()
    Dim shareEnc As String = Uri.EscapeDataString(shareLink)
    Dim titleEnc As String = If(Model IsNot Nothing, Uri.EscapeDataString(Model.Title), "")
End Code

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

        function ibSubscribe() {
            var email = $('#subEmail').val().trim();
            var msg = $('#subMsg');
            var b = $('#subBtn');
            if (!/^[^\s@@]+@@[^\s@@]+\.[^\s@@]+$/.test(email)) {
                msg.text('Please enter a valid email address.').css('color', '#ffb3b3');
                return;
            }
            b.prop('disabled', true).text('Subscribing...');
            $.ajax({
                url: '@Url.Action("Subscribe", "Home")',
                type: 'POST',
                data: {
                    email: email,
                    honeypot: $('#subPhone').val(),
                    __RequestVerificationToken: $('#ibNews input[name="__RequestVerificationToken"]').val()
                },
                success: function (r) {
                    if (r.success) {
                        $('#subEmail').val('');
                        msg.text(r.message || 'Thank you! You are subscribed.').css('color', '#ffc107');
                        b.text('Subscribed');
                    } else {
                        msg.text(r.message || 'Something went wrong.').css('color', '#ffb3b3');
                        b.prop('disabled', false).text('Subscribe');
                    }
                },
                error: function () {
                    msg.text('Something went wrong. Please try again.').css('color', '#ffb3b3');
                    b.prop('disabled', false).text('Subscribe');
                }
            });
        }
    </script>
End Section

<div class="ib-post container py-4">
    <div class="row g-5">

        <!-- ============ Article ============ -->
        <div class="col-lg-8">
            @If Model IsNot Nothing Then
                @<article>
                    <nav class="crumb mb-2"><a href="~/">Home</a> &rsaquo; <a href="~/blog">Blog</a></nav>

                    @For Each t In tags
                        @<span class="chip">@t</span>
                    Next

                    <h1>@Model.Title</h1>

                    <div class="d-flex justify-content-between align-items-center flex-wrap gap-3 pb-3 mb-4" style="border-bottom:1px solid #ece6ef;">
                        <div class="meta">
                            <span><span class="av">@(If(String.IsNullOrEmpty(Model.WriterName), "IB", Model.WriterName.Substring(0, 1).ToUpper()))</span>@(If(String.IsNullOrEmpty(Model.WriterName), "India Bobbles", Model.WriterName))</span>
                            <span><i class="fa fa-calendar-o" aria-hidden="true"></i> @(Model.DateCreated.ToString("d MMMM yyyy"))</span>
                            <span><i class="fa fa-clock-o" aria-hidden="true"></i> @readMins min read</span>
                        </div>
                        <div class="share">
                            <a class="wa" href="https://wa.me/?text=@titleEnc%20@shareEnc" target="_blank" rel="noopener" title="Share on WhatsApp"><i class="fa fa-whatsapp" aria-hidden="true"></i></a>
                            <a href="https://www.facebook.com/sharer/sharer.php?u=@shareEnc" target="_blank" rel="noopener" title="Share on Facebook"><i class="fa fa-facebook" aria-hidden="true"></i></a>
                            <a href="https://twitter.com/intent/tweet?url=@shareEnc&amp;text=@titleEnc" target="_blank" rel="noopener" title="Share on X"><i class="fa fa-twitter" aria-hidden="true"></i></a>
                            <button type="button" onclick="ibCopyLink(this)" title="Copy link"><i class="fa fa-link" aria-hidden="true"></i></button>
                        </div>
                    </div>

                    <div id="article">
                        @Html.Raw(bodyHtml)
                    </div>

                    <!-- Shop promo -->
                    <div class="promo d-flex justify-content-between align-items-center flex-wrap gap-3 my-5">
                        <div>
                            <h3>🎭 Meet the characters</h3>
                            <p class="mb-0">Bring Daku Sambhar Singh, Chhote Bachchan and friends home, hand-painted and shipped free.</p>
                        </div>
                        <a class="btn btn-ibp" href="~/tag/collectibles">Shop bobbleheads <i class="fa fa-arrow-right" aria-hidden="true"></i></a>
                    </div>

                    <!-- Newsletter -->
                    @*<div class="news mb-5" id="ibNews">
                        <h4 class="fw-bold mb-1">Enjoyed this? Never miss a comic.</h4>
                        <p class="mb-3">New stories, launches and members-only offers, straight to your inbox.</p>
                        @Html.AntiForgeryToken()
                        <div class="d-flex flex-wrap gap-2">
                            <input type="email" id="subEmail" placeholder="Your email address" maxlength="250" aria-label="Email address" />
                            <input type="text" id="subPhone" style="display:none;" tabindex="-1" autocomplete="off" />
                            <button type="button" id="subBtn" class="btn btn-iby" onclick="ibSubscribe()">Subscribe</button>
                        </div>
                        <div id="subMsg" class="small mt-2"></div>
                    </div>*@

                    <!-- Comments -->
                    <h4 class="fw-bold mb-3" style="color:#330B3F;">Comments</h4>
                    <div id="disqus_thread"></div>
                    <script type="text/javascript">
                        var disqus_shortname = 'indiabobbles';
                        (function () {
                            var dsq = document.createElement('script'); dsq.type = 'text/javascript'; dsq.async = true;
                            dsq.src = '//' + disqus_shortname + '.disqus.com/embed.js';
                            (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);
                        })();
                    </script>
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