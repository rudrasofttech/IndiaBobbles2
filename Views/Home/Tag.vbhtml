@ModelType IEnumerable(Of IndiaBobbles.Product)
@Code
    ViewData("Title") = ViewBag.Tag

    Dim inr = New System.Globalization.CultureInfo("en-IN")
    Dim inv = System.Globalization.CultureInfo.InvariantCulture
    Dim products = Model.ToList()
    Dim urlHelper As New UrlHelper(ViewContext.RequestContext)
    ' Page heading from the tag, e.g. "collectibles" -> "Collectibles"
    Dim tagText As String = If(ViewBag.Tag IsNot Nothing, CStr(ViewBag.Tag).Replace("-", " ").Trim(), "")
    Dim heading As String = If(tagText = "" OrElse tagText.ToLower() = "collectibles", "Bobbleheads", inr.TextInfo.ToTitleCase(tagText.ToLower()))
End Code


@section scripts
    <script>
        (function () {
            var grid = document.getElementById('ibGrid');
            if (!grid) return;
            var items = Array.prototype.slice.call(grid.querySelectorAll('.ib-item'));
            var inStockOnly = document.getElementById('ibInStock');
            var sortSel = document.getElementById('ibSort');
            var countEl = document.getElementById('ibCount');

            function apply() {
                var sorted = items.slice();
                if (sortSel.value === 'low') sorted.sort(function (a, b) { return a.dataset.price - b.dataset.price; });
                else if (sortSel.value === 'high') sorted.sort(function (a, b) { return b.dataset.price - a.dataset.price; });
                else sorted.sort(function (a, b) { return a.dataset.index - b.dataset.index; });

                var shown = 0;
                sorted.forEach(function (el) {
                    var hide = inStockOnly.checked && el.dataset.instock === '0';
                    el.classList.toggle('d-none', hide);
                    if (!hide) shown++;
                    grid.appendChild(el);
                });
                countEl.textContent = shown + (shown === 1 ? ' bobblehead' : ' bobbleheads');
            }
            inStockOnly.addEventListener('change', apply);
            sortSel.addEventListener('change', apply);
        })();
    </script>
End Section

<div class="ib-list container py-3 fullbody">

    <!-- Heading -->
    <div class="ib-hero mb-3">
        <div>
            <h1>@heading</h1>
            <p>Bollywood legends and Indian icons, each one hand-painted by our artists.</p>
        </div>
        <span class="ib-count" id="ibCount">@products.Count @(If(products.Count = 1, "bobblehead", "bobbleheads"))</span>
    </div>

    @If products.Count = 0 Then
        @<div class="text-center py-5">
            <p class="fs-5 mb-3">No bobbleheads found here yet.</p>
            <a href="~/tag/collectibles" class="btn btn-dark">See all bobbleheads</a>
        </div>
    Else
        @<div>
            <!-- Filter + sort -->
            <div class="ib-bar d-flex justify-content-between align-items-center gap-2 flex-wrap mb-3">
                <label class="d-flex align-items-center gap-2 mb-0">
                    <input type="checkbox" id="ibInStock" class="form-check-input mt-0" /> Show in-stock only
                </label>
                <select id="ibSort" class="form-select form-select-sm" aria-label="Sort products">
                    <option value="featured">Sort: Featured</option>
                    <option value="low">Price: Low to High</option>
                    <option value="high">Price: High to Low</option>
                </select>
            </div>

            <!-- Product grid -->
            <div class="row row-cols-2 row-cols-md-3 row-cols-lg-4 g-3 g-md-4 mb-5" id="ibGrid">
                @For i As Integer = 0 To products.Count - 1
                    Dim item = products(i)
                    Dim url As String = urlHelper.Content("~/product/" & item.ID & "/" & IndiaBobbles.Utility.Slugify(item.Name))
                    Dim hasDiscount As Boolean = item.MRP > item.SalePrice
                    Dim offPct As Integer = If(hasDiscount AndAlso item.MRP > 0, CInt(Math.Round((item.MRP - item.SalePrice) * 100 / item.MRP)), 0)
                    Dim showPrice = If(hasDiscount, item.SalePrice, item.MRP)

                    @<div class="col ib-item" data-index="@i" data-price="@(showPrice.ToString("0.00", inv))" data-instock="@(If(item.OutofStock, "0", "1"))">
                        <div class="ib-card @(If(item.OutofStock, "is-oos", ""))">

                            <a class="ib-img" href="@url">
                                @If Not String.IsNullOrEmpty(item.ThumbPath) Then
                                    @<img src="@item.ThumbPath" alt="Photo of @item.Name" loading="lazy" />
                                Else
                                    @<span class="ib-noimg">@item.Name.Substring(0, 1)</span>
                                End If
                                @If hasDiscount Then
                                    @<span class="ib-off">@offPct% OFF</span>
                                End If
                                @If item.OutofStock Then
                                    @<span class="ib-oos-ov"><span>Out of stock</span></span>
                                End If
                            </a>

                            <div class="ib-body">
                                <a class="ib-name" href="@url">@item.Name</a>
                                <div class="ib-price">
                                    <b>₹@(showPrice.ToString("#,##0.##", inr))</b>
                                    @If hasDiscount Then
                                        @<s>₹@(item.MRP.ToString("#,##0.##", inr))</s>
                                    End If
                                </div>
                                <div class="ib-ship"><i class="fa fa-check" aria-hidden="true"></i> Free shipping</div>

                                <div class="ib-actions">
                                    <a class="ib-btn ib-btn-o" href="@url">View</a>
                                    @If Not item.OutofStock Then
                                        @<form method="get" action="@urlHelper.Content("~/cart/add/" & item.ID)">
                                            <button class="ib-btn ib-btn-p">Add to Cart</button>
                                        </form>
                                    Else
                                        @<a class="ib-btn ib-btn-g" href="@url" title="Get an email when it's back"><i class="fa fa-bell" aria-hidden="true"></i> Notify me</a>
                                    End If
                                </div>
                            </div>
                        </div>
                    </div>
                Next
            </div>
        </div>
    End If
</div>