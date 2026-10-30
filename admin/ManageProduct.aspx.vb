Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Data.Entity.Validation
Imports System.Linq

Public Class ManageProduct
    Inherits AdminPage
    Private ReadOnly db As New indiabobblesEntities

    ' Where uploaded product photos are stored (inside your existing Drive folder)
    Private Const PhotoFolder As String = "~/drive/Product-Photos/"
    Private Const MaxPhotoBytes As Integer = 5 * 1024 * 1024
    Private Shared ReadOnly AllowedExt As String() = {".jpg", ".jpeg", ".png", ".webp"}
    ' Must not be longer than the ProductPhoto.AltText column (also ALT_MAX in ManageProduct.aspx)
    Private Const AltMaxLength As Integer = 250

    Private Shared ReadOnly AllowedTags As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        "p", "br", "b", "strong", "i", "em", "u", "ul", "ol", "li", "blockquote", "h2", "h3", "h4", "a", "img"
    }

    Private Shared ReadOnly DangerousBlockTags As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        "script", "style", "iframe", "object", "embed", "form", "input", "button", "select", "textarea", "meta", "link", "svg", "math"
    }

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack And Not Page.IsCallback Then
            LoadData()
            If Session("ProductPhotoWarning") IsNot Nothing Then
                PhotoErrorLabel.Text = Server.HtmlEncode(CStr(Session("ProductPhotoWarning")))
                PhotoErrorLabel.Visible = True
                Session.Remove("ProductPhotoWarning")
            End If
        End If
    End Sub

    Private Sub LoadData()
        If TargetID <> 0 Then
            HeadingLiteral.Text = "Edit Product"
            Dim product = db.Products.FirstOrDefault(Function(m) m.ID = TargetID)
            If product IsNot Nothing Then
                NameTextBox.Text = product.Name
                DescTextBox.Text = product.Description
                MRPTextBox.Text = product.MRP.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
                SaleTextBox.Text = product.SalePrice.ToString("0.##", Globalization.CultureInfo.InvariantCulture)
                DimensionTextBox.Text = product.Dimension
                ColorTextBox.Text = product.Color
                WeightTextBox.Text = product.Weight
                MaterialTextBox.Text = product.Material
                ManufacturerTextBox.Text = product.Manufacturer
                CareTextBox.Text = product.CareInstructions
                RecommendAgeTextBox.Text = product.RecommendedAge
                CountryOriginTextBox.Text = product.CountryofOrigin
                FragileCheckBox.Checked = product.Fragile
                HandmadeCheckBox.Checked = product.Handmade
                OutofStockCheckBox.Checked = product.OutofStock
                ShippingTimeTextBox.Text = product.ShippingTime
                StatusDropDown.SelectedValue = product.Status
                ThumbPathTextBox.Text = product.ThumbPath
                ProductCodeTextBox.Text = product.ProductCode
                ' Gallery photos, in display order: one per line, "path<TAB>alt text"
                PhotosHidden.Value = String.Join(vbLf, product.ProductPhotoes.OrderBy(Function(p) p.Sequence) _
                                                       .Select(Function(p) p.ImagePath & vbTab & CleanAlt(p.AltText)))
            End If
        End If
    End Sub

    Protected Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        Page.Validate()
        If Not Page.IsValid Then
            Return
        End If

        ' ---------- 1. Save any newly uploaded photo files ----------
        ' Each entry: Key = image path, Value = alt text
        Dim uploaded As New List(Of KeyValuePair(Of String, String))
        Dim photoErrors As New List(Of String)
        ' Alt text typed for the new files: one line per file, same order as PostedFiles
        Dim pendingAlts As String() = NormalizeLines(PendingAltHidden.Value).Split({vbLf}, StringSplitOptions.None)
        If PhotoUpload.HasFiles Then
            Dim folder As String = Server.MapPath(PhotoFolder)
            If Not Directory.Exists(folder) Then Directory.CreateDirectory(folder)

            For idx As Integer = 0 To PhotoUpload.PostedFiles.Count - 1
                Dim f As HttpPostedFile = PhotoUpload.PostedFiles(idx)
                If f Is Nothing OrElse f.ContentLength = 0 Then Continue For
                Dim ext As String = Path.GetExtension(f.FileName).ToLowerInvariant()
                If Not AllowedExt.Contains(ext) OrElse Not f.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) Then
                    photoErrors.Add(Path.GetFileName(f.FileName) & " is not a supported image")
                    Continue For
                End If
                If f.ContentLength > MaxPhotoBytes Then
                    photoErrors.Add(Path.GetFileName(f.FileName) & " is larger than 5 MB")
                    Continue For
                End If

                ' Safe, unique file name: product-name-slug + short random id
                Dim slug As String = IndiaBobbles.Utility.Slugify(NameTextBox.Text.Trim())
                If String.IsNullOrEmpty(slug) Then slug = "product"
                If slug.Length > 40 Then slug = slug.Substring(0, 40)
                Dim fileName As String = slug & "-" & Guid.NewGuid().ToString("N").Substring(0, 8) & ext
                f.SaveAs(Path.Combine(folder, fileName))
                Dim alt As String = If(idx < pendingAlts.Length, CleanAlt(pendingAlts(idx)), "")
                uploaded.Add(New KeyValuePair(Of String, String)(VirtualPathUtility.ToAbsolute(PhotoFolder) & fileName, alt))
            Next
        End If

        ' ---------- 2. Product fields ----------
        Dim product As New Product()
        If TargetID <> 0 Then
            product = db.Products.FirstOrDefault(Function(m) m.ID = TargetID)
        End If

        Dim sanitizedDescription As String = SanitizeHtmlAllowBasic(DescTextBox.Text)

        ' If sanitization removes everything, keep save-safe plain text instead of failing DB validation
        If String.IsNullOrWhiteSpace(Regex.Replace(sanitizedDescription, "<[^>]+>", "").Trim()) Then
            sanitizedDescription = HttpUtility.HtmlEncode(DescTextBox.Text.Trim())
        End If

        product.Name = NameTextBox.Text.Trim()
        product.Description = sanitizedDescription
        product.MRP = Decimal.Parse(MRPTextBox.Text.Trim(), Globalization.CultureInfo.InvariantCulture)
        product.SalePrice = Decimal.Parse(SaleTextBox.Text.Trim(), Globalization.CultureInfo.InvariantCulture)
        product.Dimension = DimensionTextBox.Text.Trim()
        product.Color = ColorTextBox.Text.Trim()
        product.Weight = WeightTextBox.Text.Trim()
        product.Material = MaterialTextBox.Text.Trim()
        product.Manufacturer = ManufacturerTextBox.Text.Trim()
        product.CareInstructions = CareTextBox.Text.Trim()
        product.RecommendedAge = RecommendAgeTextBox.Text.Trim()
        product.CountryofOrigin = CountryOriginTextBox.Text.Trim()
        product.Fragile = FragileCheckBox.Checked
        product.Handmade = HandmadeCheckBox.Checked
        product.OutofStock = OutofStockCheckBox.Checked
        product.ShippingTime = ShippingTimeTextBox.Text.Trim()
        product.Status = Byte.Parse(StatusDropDown.SelectedValue)
        product.ThumbPath = ThumbPathTextBox.Text.Trim()
        product.ProductCode = ProductCodeTextBox.Text.Trim()
        ' Required legacy fields often missed in this page:
        If String.IsNullOrWhiteSpace(product.URL) Then
            product.URL = IndiaBobbles.Utility.Slugify(product.Name)
        End If

        ' ---------- 3. Gallery: final ordered list = kept/reordered + new uploads ----------
        ' Hidden field: one photo per line, "path<TAB>alt text" (lines without a TAB are path only)
        Dim items As New List(Of KeyValuePair(Of String, String))
        For Each line As String In NormalizeLines(PhotosHidden.Value).Split({vbLf}, StringSplitOptions.RemoveEmptyEntries)
            Dim t As Integer = line.IndexOf(vbTab)
            Dim photoPath As String = If(t < 0, line, line.Substring(0, t)).Trim()
            Dim photoAlt As String = If(t < 0, "", line.Substring(t + 1))
            If photoPath <> "" Then items.Add(New KeyValuePair(Of String, String)(photoPath, CleanAlt(photoAlt)))
        Next
        items.AddRange(uploaded)
        Dim paths As List(Of String) = items.Select(Function(x) x.Key).ToList()

        Dim existing As List(Of ProductPhoto) = product.ProductPhotoes.ToList()
        Dim used As New HashSet(Of ProductPhoto)

        For i As Integer = 0 To items.Count - 1
            Dim p As String = items(i).Key
            Dim altText As String = If(items(i).Value = "", String.Empty, items(i).Value)
            Dim match As ProductPhoto = existing.FirstOrDefault(Function(x) x.ImagePath = p AndAlso Not used.Contains(x))
            If match IsNot Nothing Then
                match.Sequence = i + 1
                match.AltText = altText
                used.Add(match)
            Else
                Dim np As New ProductPhoto With {.ImagePath = p, .Sequence = i + 1, .AltText = altText}
                product.ProductPhotoes.Add(np)
                used.Add(np)
            End If
        Next

        ' Remove photos the admin deleted from the gallery (files stay in Drive)
        For Each old As ProductPhoto In existing
            If Not used.Contains(old) Then db.ProductPhotoes.Remove(old)
        Next

        ' No thumbnail chosen? Use the first gallery photo.
        If String.IsNullOrWhiteSpace(product.ThumbPath) AndAlso paths.Count > 0 Then
            product.ThumbPath = paths(0)
        End If

        product.ModifyDate = DateTime.Now
        product.ModifiedBy = If(CurrentUser IsNot Nothing, CType(CurrentUser.ID, Long?), Nothing)

        If TargetID = 0 Then
            product.CreateDate = DateTime.Now
            If CurrentUser Is Nothing Then
                PhotoErrorLabel.Text = "Unable to detect current admin user. Please sign in again."
                PhotoErrorLabel.Visible = True
                Return
            End If
            product.CreatedBy = CurrentUser.ID
            db.Products.Add(product)
        End If

        Try
            db.SaveChanges()
        Catch ex As DbEntityValidationException
            Dim details = String.Join(" | ",
                ex.EntityValidationErrors.SelectMany(Function(x) x.ValidationErrors).
                    Select(Function(v) v.PropertyName & ": " & v.ErrorMessage))
            PhotoErrorLabel.Text = Server.HtmlEncode("Product could not be saved. " & details)
            PhotoErrorLabel.Visible = True
            Return
        End Try

        If photoErrors.Count > 0 Then
            ' Product saved, but some files were skipped – reopen the editor and say which
            Session("ProductPhotoWarning") = "Product saved, but these photos were skipped: " & String.Join("; ", photoErrors)
            Response.Redirect("~/admin/manageproduct.aspx?id=" & product.ID)
        End If
        Response.Redirect("~/admin/productdetail.aspx?id=" & product.ID)
    End Sub

    ' Browsers may send line breaks in hidden fields as CRLF – turn them all into LF
    Private Shared Function NormalizeLines(s As String) As String
        Return If(s, "").Replace(vbCrLf, vbLf).Replace(vbCr, vbLf)
    End Function

    ' Alt text: plain text on one line, no HTML, limited length
    Private Shared Function CleanAlt(s As String) As String
        If String.IsNullOrWhiteSpace(s) Then Return ""
        Dim t As String = Regex.Replace(s, "<[^>]*>", "")
        t = Regex.Replace(t, "\s+", " ").Trim()
        If t.Length > AltMaxLength Then t = t.Substring(0, AltMaxLength).Trim()
        Return t
    End Function

    Private Shared Function SanitizeHtmlAllowBasic(input As String) As String
        If String.IsNullOrWhiteSpace(input) Then Return ""

        Dim html As String = input.Trim()

        ' Remove null chars and HTML comments
        html = html.Replace(ChrW(0), "")
        html = Regex.Replace(html, "<!--[\s\S]*?-->", "", RegexOptions.IgnoreCase)

        ' Remove dangerous blocks completely (open+content+close)
        For Each t In DangerousBlockTags
            html = Regex.Replace(html, "<" & t & "\b[^>]*>[\s\S]*?</" & t & "\s*>", "", RegexOptions.IgnoreCase)
            html = Regex.Replace(html, "<" & t & "\b[^>]*?/?>", "", RegexOptions.IgnoreCase)
        Next

        ' Remove any tag not in whitelist
        html = Regex.Replace(
            html,
            "</?(?!p\b|br\b|b\b|strong\b|i\b|em\b|u\b|ul\b|ol\b|li\b|blockquote\b|h2\b|h3\b|h4\b|a\b|img\b)[a-z0-9]+\b[^>]*>",
            "",
            RegexOptions.IgnoreCase
        )

        ' Sanitize <a ...>
        html = Regex.Replace(html, "<a\b[^>]*>", Function(m)
                                                     Dim href As String = GetAttr(m.Value, "href")
                                                     Dim title As String = GetAttr(m.Value, "title")
                                                     Dim target As String = GetAttr(m.Value, "target")
                                                     Dim rel As String = GetAttr(m.Value, "rel")

                                                     If Not IsSafeUrl(href, allowRelative:=True) Then href = "#"
                                                     If String.IsNullOrWhiteSpace(target) Then target = "_blank"
                                                     If String.IsNullOrWhiteSpace(rel) Then rel = "noopener nofollow"

                                                     Dim sb As New Text.StringBuilder()
                                                     sb.Append("<a href=""").Append(HttpUtility.HtmlAttributeEncode(href)).Append("""")
                                                     sb.Append(" target=""").Append(HttpUtility.HtmlAttributeEncode(target)).Append("""")
                                                     sb.Append(" rel=""").Append(HttpUtility.HtmlAttributeEncode(rel)).Append("""")
                                                     If Not String.IsNullOrWhiteSpace(title) Then
                                                         sb.Append(" title=""").Append(HttpUtility.HtmlAttributeEncode(title)).Append("""")
                                                     End If
                                                     sb.Append(">")
                                                     Return sb.ToString()
                                                 End Function,
            RegexOptions.IgnoreCase
        )

        ' Sanitize <img ...>
        html = Regex.Replace(html, "<img\b[^>]*>", Function(m)
                                                       Dim src As String = GetAttr(m.Value, "src")
                                                       Dim alt As String = GetAttr(m.Value, "alt")
                                                       Dim title As String = GetAttr(m.Value, "title")

                                                       If Not IsSafeUrl(src, allowRelative:=True) Then Return ""

                                                       Dim sb As New Text.StringBuilder()
                                                       sb.Append("<img src=""").Append(HttpUtility.HtmlAttributeEncode(src)).Append("""")
                                                       sb.Append(" alt=""").Append(HttpUtility.HtmlAttributeEncode(If(alt, ""))).Append("""")
                                                       If Not String.IsNullOrWhiteSpace(title) Then
                                                           sb.Append(" title=""").Append(HttpUtility.HtmlAttributeEncode(title)).Append("""")
                                                       End If
                                                       sb.Append(" loading=""lazy"" />")
                                                       Return sb.ToString()
                                                   End Function,
            RegexOptions.IgnoreCase
        )

        Return html
    End Function

    Private Shared Function GetAttr(tag As String, attr As String) As String
        Dim m = Regex.Match(tag, "\b" & Regex.Escape(attr) & "\s*=\s*(""([^""]*)""|'([^']*)'|([^\s>]+))", RegexOptions.IgnoreCase)
        If Not m.Success Then Return ""
        If m.Groups(2).Success Then Return m.Groups(2).Value
        If m.Groups(3).Success Then Return m.Groups(3).Value
        If m.Groups(4).Success Then Return m.Groups(4).Value
        Return ""
    End Function

    Private Shared Function IsSafeUrl(url As String, allowRelative As Boolean) As Boolean
        If String.IsNullOrWhiteSpace(url) Then Return False
        Dim u As String = url.Trim()

        If u.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase) Then Return False
        If u.StartsWith("data:", StringComparison.OrdinalIgnoreCase) Then Return False
        If u.StartsWith("vbscript:", StringComparison.OrdinalIgnoreCase) Then Return False

        If allowRelative AndAlso (u.StartsWith("/") OrElse u.StartsWith("./") OrElse u.StartsWith("../") OrElse u.StartsWith("#")) Then
            Return True
        End If

        Return u.StartsWith("http://", StringComparison.OrdinalIgnoreCase) OrElse
               u.StartsWith("https://", StringComparison.OrdinalIgnoreCase) OrElse
               u.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) OrElse
               u.StartsWith("tel:", StringComparison.OrdinalIgnoreCase)
    End Function
End Class