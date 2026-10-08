@ModelType List(Of IndiaBobbles.Article)
@Code
    ViewData("Title") = "Blog – Comics, Stories & Bobblehead News | India Bobbles"
    ViewBag.CanonicalUrl = IndiaBobbles.Utility.SiteURL & "/blog"

    Dim posts = If(Model, New List(Of IndiaBobbles.Article)())
    Dim featured = If(posts.Count > 0, posts(0), Nothing)
    Dim pageSize As Integer = 12
End Code


@section scripts
    <script>
        (function () {
            var PAGE = @pageSize;
            var shown = PAGE;
            var cards = $('#ibPosts .ib-post');
            var promo = $('#ibPosts .ib-promo');
            var btn = $('#ibLoadMore');
            var search = $('#ibSearch');
            var none = $('#ibNone');

            function render() {
                var q = search.val().trim().toLowerCase();
                var visible = 0;
                cards.each(function (i) {
                    var match = q === '' || String($(this).attr('data-title')).indexOf(q) > -1;
                    var show = match && (q !== '' || i < shown);
                    $(this).toggleClass('d-none', !show);
                    if (match) visible++;
                });
                promo.toggleClass('d-none', q !== '');
                $('#ibFeatured').toggleClass('d-none', q !== '');
                btn.toggleClass('d-none', q !== '' || shown >= cards.length);
                none.toggleClass('d-none', visible > 0);
            }

            btn.on('click', function () { shown += PAGE; render(); });
            search.on('input', render);
            render();
        })();

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

<div class="ib-blog container py-4">

    <!-- Heading + search -->
    <div class="hero d-flex justify-content-between align-items-center flex-wrap gap-3 mb-4">
        <div>
            <h1>The India Bobbles Blog</h1>
            <p>Comics, filmy stories and bobblehead news from Daku Sambhar Singh and friends.</p>
        </div>
        <div class="search">
            <i class="fa fa-search" aria-hidden="true"></i>
            <input type="search" id="ibSearch" placeholder="Search posts…" aria-label="Search blog posts" />
        </div>
    </div>

    @If featured IsNot Nothing Then
        @<a class="feat mb-5" id="ibFeatured" href="@Url.Content("~/blog/" & featured.URL)">
            <div class="pic">
                @If Not String.IsNullOrEmpty(featured.OGImage) Then
                    @<img src="@featured.OGImage" alt="@featured.Title" />
                Else
                    @<div class="noimg">IB</div>
                End If
            </div>
            <div class="txt">
                <span class="tag align-self-start">Latest post</span>
                <h2>@featured.Title</h2>
                <span class="more">Read the story <i class="fa fa-arrow-right" aria-hidden="true"></i></span>
            </div>
        </a>
    End If

    <!-- Posts -->
    <div class="row row-cols-1 row-cols-sm-2 row-cols-lg-3 g-4" id="ibPosts">
        @For i As Integer = 1 To posts.Count - 1
            Dim m = posts(i)
            Dim postLink As String = Url.Content("~/blog/" & m.URL)
            Dim titleKey As String = If(m.Title, "").ToLower()

            @<div class="col ib-post" data-title="@titleKey">
                <a class="post" href="@postLink">
                    <div class="pic">
                        @If Not String.IsNullOrEmpty(m.OGImage) Then
                            @<img src="@m.OGImage" alt="@m.Title" loading="lazy" />
                        Else
                            @<div class="noimg">IB</div>
                        End If
                    </div>
                    <div class="body">
                        <h3>@m.Title</h3>
                        <span class="more">Read more <i class="fa fa-arrow-right" aria-hidden="true"></i></span>
                    </div>
                </a>
            </div>

            @* After the first 5 posts, show a shop card *@
            If i = 5 Then
                @<div class="col ib-promo">
                    <a class="promo" href="~/tag/collectibles">
                        <span class="e">🎭</span>
                        <h3>Meet the characters</h3>
                        <p>Love the comics? Bring Daku Sambhar Singh, Chhote Bachchan and friends home, hand-painted and shipped free.</p>
                        <span class="btn">Shop bobbleheads <i class="fa fa-arrow-right" aria-hidden="true"></i></span>
                    </a>
                </div>
            End If
        Next
    </div>

    <p id="ibNone" class="text-center text-muted py-5 d-none">No posts match your search.</p>

    <div class="text-center my-5">
        <button type="button" id="ibLoadMore" class="btn btn-load">Load more posts</button>
    </div>

    <!-- Newsletter -->
    @*<div class="news d-flex justify-content-between align-items-center flex-wrap gap-3 mb-5" id="ibNews">
        <div>
            <h3>Never miss a comic</h3>
            <p>New stories, launches and members-only offers, straight to your inbox.</p>
        </div>
        <div>
            @Html.AntiForgeryToken()
            <div class="d-flex flex-wrap gap-2">
                <input type="email" id="subEmail" placeholder="Your email address" maxlength="250" aria-label="Email address" />
                <input type="text" id="subPhone" style="display:none;" tabindex="-1" autocomplete="off" />
                <button type="button" id="subBtn" class="btn btn-iby" onclick="ibSubscribe()">Subscribe</button>
            </div>
            <div id="subMsg" class="small mt-2"></div>
        </div>
    </div>*@
</div>
