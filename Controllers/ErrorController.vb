Imports System.Web.Mvc
Imports IndiaBobbles

' Friendly "page not found" page that still returns a real 404 status (important for Google).
Public Class ErrorController
    Inherits Controller

    ' GET /error/notfound   (also used by web.config for any missing URL)
    Function NotFound() As ActionResult
        Response.StatusCode = 404
        Response.TrySkipIisCustomErrors = True      ' stop IIS replacing this page with its own grey one
        Response.Cache.SetNoStore()

        ' The address the visitor tried (IIS passes it as ?aspxerrorpath= or in the query string after "404;")
        Dim tried As String = Request.QueryString("aspxerrorpath")
        If String.IsNullOrEmpty(tried) Then
            Dim raw As String = Request.Url.Query
            Dim i As Integer = raw.IndexOf("404;", StringComparison.Ordinal)
            If i >= 0 Then
                Try
                    tried = New Uri(Uri.UnescapeDataString(raw.Substring(i + 4))).AbsolutePath
                Catch
                End Try
            End If
        End If
        If String.IsNullOrEmpty(tried) AndAlso Not Request.Path.ToLowerInvariant().StartsWith("/error") Then tried = Request.Path
        ViewBag.TriedPath = tried

        ' Products to suggest: in stock first, sale items first, newest first
        Try
            Using db As New indiabobblesEntities()
                ViewBag.Products = db.Products.Where(Function(p) p.Status = 0) _
                    .OrderBy(Function(p) p.OutofStock) _
                    .ThenByDescending(Function(p) p.MRP - p.SalePrice) _
                    .ThenByDescending(Function(p) p.CreateDate) _
                    .Take(4).ToList()
            End Using
        Catch ex As Exception
            ViewBag.Products = New List(Of Product)()
        End Try

        Try
            ViewBag.Posts = Utility.GetLatestPosts(3)
        Catch ex As Exception
            ViewBag.Posts = Nothing
        End Try

        Return View()
    End Function
End Class