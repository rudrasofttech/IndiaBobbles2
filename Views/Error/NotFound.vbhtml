@Imports IndiaBobbles
@Code
    ViewData("Title") = "Page not found"
    ViewBag.NoIndex = True

    Dim inr = New System.Globalization.CultureInfo("en-IN")
    Dim products As List(Of Product) = If(TryCast(ViewBag.Products, List(Of Product)), New List(Of Product)())
    Dim posts As List(Of Post) = If(TryCast(ViewBag.Posts, List(Of Post)), New List(Of Post)())
    Dim tried As String = Convert.ToString(ViewBag.TriedPath)
End Code

' ...

@If posts.Count > 0 Then
    @<div class="sec-h">
        <div>
            <h2>Or catch up on the comics</h2>
            <p>The latest adventures of the India Bobbles gang.</p>
        </div>
        <a href="~/blog">All stories <i class="fa fa-arrow-right" aria-hidden="true"></i></a>
    </div>
    @<div class="row g-3">
        @For Each m In posts
            @<div class="col-md-4">
                <a class="post" href="@Url.Content("~/blog/" & m.URL)">
                    <span class="th">
                        @If Not String.IsNullOrEmpty(m.OGImage) Then
                            @<img src="@m.OGImage" alt="" loading="lazy" />
                        End If
                    </span>
                    <span>@m.Title</span>
                </a>
            </div>
        Next
    </div>
End If