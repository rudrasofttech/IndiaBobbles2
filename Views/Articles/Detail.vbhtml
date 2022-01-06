@ModelType IndiaBobbles.Post
@Code
    If Model IsNot Nothing Then
        ViewData("Title") = Model.Title
    Else
        ViewData("Title") = ""
    End If
    Dim lateststories = IndiaBobbles.Utility.GetLatestPosts(10)
End Code
<div class="container bg-white pt-2 pb-2">
    <div class="row">
        <div class="col-md-9" id="article">
            @If Model IsNot Nothing Then
                @<h1>@Model.Title</h1>
                @<div>@Model.WriterName | @Model.DateCreated</div>
                @<div>@Model.Tag</div>

                @Html.Raw(Model.Article.Replace("<datasource name=""ShareDataSource"" />", "").Replace("<datasource name=""DisqusDataSource"" />", ""))

                @<div id="disqus_thread"></div>
                @<script type="text/javascript">
                     var disqus_shortname = 'indiabobbles';
                     (function () {
                         var dsq = document.createElement('script'); dsq.type = 'text/javascript'; dsq.async = true;
                         dsq.src = '//' + disqus_shortname + '.disqus.com/embed.js';
                         (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);
                     })();
                </script>
            End If
        </div>
        <div class="col-md-3">
            <h3 style="font-family:'Spongeboy Me Bob';color:#330B41;text-align:center">Blog's Latest</h3>
            @For Each m In lateststories
                @<div Class="card">
                    <div style="max-height:200px;overflow-y:hidden;">
                        <img src="@m.OGImage" Class="card-img-top" alt="" />
                    </div>
                    <div Class="card-body">
                        <h5 Class="card-title">@m.Title</h5>
                        <a href="@Url.Content("~/blog/" & m.URL)" class="btn btn-outline-primary">Read More</a>
                    </div>
                </div>
            Next
        </div>
    </div>
</div>

@Section head
    <style>
        #article img{
            max-width:100%;
            height:auto;
            text-align:center;
        }
    </style>
End Section


