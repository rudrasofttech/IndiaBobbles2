@ModelType List(Of IndiaBobbles.Article)
@Code
    ViewData("Title") = "Blog"
End Code

<div class="container bg-white">
    <div class="row">
        <div class="col-12">
            <h1 class="text-center">Blog</h1>
            <div class="row row-cols-1 row-cols-md-3 g-4">
                @For Each m In Model
                    @<div Class="col">
                        <div Class="card">
                            <div style="max-height:200px;overflow-y:hidden;">
                                <img src="@m.OGImage" Class="card-img-top" alt=""  />
                            </div>
                            <div Class="card-body">
                                <h5 Class="card-title">@m.Title</h5>
                                <a href="@Url.Content("~/blog/" & m.URL)" class="btn btn-outline-primary">Read More</a>
                            </div>
                        </div>
                    </div>
                Next
            </div>
        </div>
    </div>
</div>
