Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports IndiaBobbles

Public Class ManageSitemap
    Inherits AdminPage

    Private ReadOnly db As New indiabobblesEntities

    Private Sub ManageSitemap_Load(sender As Object, e As EventArgs) Handles Me.Load
        If ForbidUserAccess(MemberTypeType.Admin) Then
            Response.Redirect("default.aspx")
        End If

        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            BindPreview()
        End If
    End Sub

    Private Function BuildEntries() As List(Of SitemapEntry)
        Dim baseUrl As String = String.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority)
        Dim entries As New List(Of SitemapEntry)

        ' Static pages
        Dim staticPages As New List(Of Tuple(Of String, String)) From {
            Tuple.Create("/", "monthly"),
            Tuple.Create("/about", "monthly"),
            Tuple.Create("/terms-and-conditions", "monthly"),
            Tuple.Create("/privacy-policy", "monthly"),
            Tuple.Create("/shipping-policy", "monthly"),
            Tuple.Create("/payment-options", "monthly"),
            Tuple.Create("/order-custom-bobbleheads", "monthly"),
            Tuple.Create("/collectibles", "weekly"),
            Tuple.Create("/games", "weekly"),
            Tuple.Create("/blog", "weekly")
        }

        For Each sPage In staticPages
            entries.Add(New SitemapEntry With {
                .Type = "Static",
                .Loc = baseUrl & sPage.Item1,
                .ChangeFreq = sPage.Item2,
                .Priority = "0.6"
            })
        Next

        ' Products
        For Each product In db.Products.ToList()
            entries.Add(New SitemapEntry With {
                .Type = "Product",
                .Loc = String.Format("{0}/product/{1}/{2}", baseUrl, product.ID, Utility.Slugify(product.Name)),
                .ChangeFreq = "weekly",
                .Priority = "0.8"
            })
        Next

        ' Category Tags
        For Each tag In db.CategoryTags.ToList()
            entries.Add(New SitemapEntry With {
                .Type = "Tag",
                .Loc = String.Format("{0}/tag/{1}", baseUrl, tag.UrlName),
                .ChangeFreq = "weekly",
                .Priority = "0.7"
            })
        Next

        ' Blog Posts
        For Each post In db.Posts.Where(Function(p) p.Status = PostStatusType.Publish).ToList()
            entries.Add(New SitemapEntry With {
                .Type = "Blog",
                .Loc = String.Format("{0}/blog/{1}", baseUrl, post.URL),
                .ChangeFreq = "monthly",
                .Priority = "0.5"
            })
        Next

        Return entries
    End Function

    Private Sub BindPreview()
        SitemapGridView.DataSource = BuildEntries()
        SitemapGridView.DataBind()
    End Sub

    Protected Sub GenerateButton_Click(sender As Object, e As EventArgs)
        Try
            Dim entries = BuildEntries()
            Dim sb As New StringBuilder()

            sb.AppendLine("<?xml version=""1.0"" encoding=""UTF-8""?>")
            sb.AppendLine("<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">")

            For Each entry In entries
                sb.AppendLine("  <url>")
                sb.AppendLine(String.Format("    <loc>{0}</loc>", HttpUtility.HtmlEncode(entry.Loc)))
                sb.AppendLine(String.Format("    <changefreq>{0}</changefreq>", entry.ChangeFreq))
                sb.AppendLine(String.Format("    <priority>{0}</priority>", entry.Priority))
                sb.AppendLine("  </url>")
            Next

            sb.AppendLine("</urlset>")

            Dim filePath As String = Server.MapPath("~/sitemap.xml")
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8)

            message.Visible = True
            message.Text = String.Format("sitemap.xml generated successfully with {0} entries.", entries.Count)
            message.Indicate = AlertType.Success
            message.Heading = "Done!"

            BindPreview()
        Catch ex As Exception
            message.Visible = True
            message.Text = String.Format("Error generating sitemap: {0}", ex.Message)
            message.Indicate = AlertType.[Error]
            message.Heading = "Oh Snap!"
            Trace.Write(ex.Message)
            Trace.Write(ex.StackTrace)
        End Try
    End Sub

    Public Class SitemapEntry
        Public Property Type As String
        Public Property Loc As String
        Public Property ChangeFreq As String
        Public Property Priority As String
    End Class

End Class