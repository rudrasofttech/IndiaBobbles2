Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.IO
Imports System.Text.RegularExpressions
Imports IndiaBobbles

' Admin › Drive – a small file explorer for the site's image folder.
' Browse folders, upload images (validated), create / rename / delete folders and files.
Public Class ViewDrive
    Inherits AdminPage

    Private Const MaxBytes As Integer = 5 * 1024 * 1024            ' 5 MB per image
    Private Shared ReadOnly AllowedExt As String() = {".jpg", ".jpeg", ".png", ".gif", ".webp"}
    Private Shared ReadOnly ImageExt As String() = {".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".bmp", ".ico"}

    Private RootPath As String       ' physical path of the drive root
    Private RootUrl As String        ' public URL of the drive root
    Protected Property FolderPath As String = ""   ' current folder, relative, e.g. "Blog-Photos/Diwali"

    ' ------------------------------------------------------------------
    '  Page
    ' ------------------------------------------------------------------
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        RootPath = Path.GetFullPath(Server.MapPath(Utility.SiteDriveFolderPath)).TrimEnd("\"c, "/"c)
        RootUrl = Utility.SiteURL.TrimEnd("/"c) & "/" & Utility.SiteDriveFolderName.Trim("/"c)
        If Not Directory.Exists(RootPath) Then Directory.CreateDirectory(RootPath)

        Dim requested As String = CleanRel(Request.QueryString("folderpath"))
        Try
            If Not Directory.Exists(ResolvePath(requested)) Then Throw New DirectoryNotFoundException()
            FolderPath = requested
        Catch
            FolderPath = ""
            If requested <> "" Then ShowMessage("That folder doesn't exist any more – showing the top folder.", True)
        End Try
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        BindItems()
        BindBreadcrumb()
    End Sub

    ' ------------------------------------------------------------------
    '  Safe paths – nothing can escape the drive folder
    ' ------------------------------------------------------------------
    Private Shared Function CleanRel(p As String) As String
        If String.IsNullOrWhiteSpace(p) Then Return ""
        Dim parts = p.Replace("\", "/").Split({"/"c}, StringSplitOptions.RemoveEmptyEntries) _
                     .Select(Function(s) s.Trim()).Where(Function(s) s <> "" AndAlso s <> "." AndAlso s <> "..")
        Return String.Join("/", parts)
    End Function

    Private Function ResolvePath(rel As String) As String
        Dim full As String = Path.GetFullPath(Path.Combine(RootPath, CleanRel(rel).Replace("/", "\")))
        If Not (full.Equals(RootPath, StringComparison.OrdinalIgnoreCase) OrElse
                full.StartsWith(RootPath & "\", StringComparison.OrdinalIgnoreCase)) Then
            Throw New UnauthorizedAccessException("Path is outside the drive.")
        End If
        Return full
    End Function

    Private Function Join(folder As String, name As String) As String
        Return If(folder = "", name, folder & "/" & name)
    End Function

    Private Function UrlFor(rel As String) As String
        Return RootUrl & "/" & String.Join("/", rel.Split("/"c).Select(Function(s) Uri.EscapeDataString(s)))
    End Function

    Protected Function FolderLink(rel As Object) As String
        Return "viewdrive.aspx?folderpath=" & String.Join("/", Convert.ToString(rel).Split("/"c).Select(Function(s) HttpUtility.UrlEncode(s)))
    End Function

    ' ------------------------------------------------------------------
    '  Listing
    ' ------------------------------------------------------------------
    Private Sub BindItems()
        Dim dir As String = ResolvePath(FolderPath)

        Dim folders = New DirectoryInfo(dir).GetDirectories() _
            .Where(Function(d) (d.Attributes And FileAttributes.Hidden) = 0) _
            .OrderBy(Function(d) d.Name) _
            .Select(Function(d) New With {
                .Name = d.Name,
                .Rel = Join(FolderPath, d.Name),
                .Count = SafeCount(d),
                .Modified = d.LastWriteTime}).ToList()

        Dim files = New DirectoryInfo(dir).GetFiles() _
            .Where(Function(f) (f.Attributes And FileAttributes.Hidden) = 0) _
            .OrderByDescending(Function(f) f.LastWriteTime) _
            .Select(Function(f) New With {
                .Name = f.Name,
                .Rel = Join(FolderPath, f.Name),
                .Url = UrlFor(Join(FolderPath, f.Name)),
                .Size = f.Length,
                .SizeText = NiceSize(f.Length),
                .Modified = f.LastWriteTime,
                .IsImage = ImageExt.Contains(f.Extension.ToLowerInvariant()),
                .Ext = f.Extension.TrimStart("."c).ToUpperInvariant()}).ToList()

        FolderRepeater.DataSource = folders
        FolderRepeater.DataBind()
        FileRepeater.DataSource = files
        FileRepeater.DataBind()
        EmptyPanel.Visible = folders.Count = 0 AndAlso files.Count = 0
        SummaryLiteral.Text = String.Format("{0} folder{1} · {2} file{3} · {4}",
            folders.Count, If(folders.Count = 1, "", "s"), files.Count, If(files.Count = 1, "", "s"), NiceSize(files.Sum(Function(f) f.Size)))
    End Sub

    Private Shared Function SafeCount(d As DirectoryInfo) As Integer
        Try
            Return d.EnumerateFileSystemInfos().Count()
        Catch
            Return 0
        End Try
    End Function

    Private Sub BindBreadcrumb()
        Dim sb As New Text.StringBuilder()
        sb.Append("<a href=""viewdrive.aspx"" class=""crumb""><i class=""fa fa-hdd-o""></i> Drive</a>")
        Dim acc As String = ""
        For Each part In FolderPath.Split({"/"c}, StringSplitOptions.RemoveEmptyEntries)
            acc = Join(acc, part)
            sb.Append("<i class=""fa fa-angle-right sep""></i>")
            sb.AppendFormat("<a href=""{0}"" class=""crumb"">{1}</a>", HttpUtility.HtmlAttributeEncode(FolderLink(acc)), HttpUtility.HtmlEncode(part))
        Next
        BreadcrumbLiteral.Text = sb.ToString()

        Dim parent As String = If(FolderPath.Contains("/"), FolderPath.Substring(0, FolderPath.LastIndexOf("/"c)), "")
        UpLink.NavigateUrl = If(parent = "", "viewdrive.aspx", FolderLink(parent))
        UpLink.Visible = FolderPath <> ""
    End Sub

    Private Shared Function NiceSize(bytes As Long) As String
        If bytes < 1024 Then Return bytes & " B"
        If bytes < 1024 * 1024 Then Return (bytes / 1024).ToString("0") & " KB"
        Return (bytes / 1024 / 1024).ToString("0.0") & " MB"
    End Function

    ' ------------------------------------------------------------------
    '  Upload (images only)
    ' ------------------------------------------------------------------
    Protected Sub UploadButton_Click(sender As Object, e As EventArgs) Handles UploadButton.Click
        If Not UploadInput.HasFiles Then
            ShowMessage("Choose one or more images to upload.", True)
            Return
        End If

        Dim dir As String = ResolvePath(FolderPath)
        Dim ok As New List(Of String), bad As New List(Of String)

        For Each f As HttpPostedFile In UploadInput.PostedFiles
            If f Is Nothing OrElse f.ContentLength = 0 Then Continue For
            Dim original As String = Path.GetFileName(f.FileName)
            Dim ext As String = Path.GetExtension(original).ToLowerInvariant()

            If Not AllowedExt.Contains(ext) Then
                bad.Add(original & " – only JPG, PNG, GIF or WebP images are allowed") : Continue For
            End If
            If f.ContentLength > MaxBytes Then
                bad.Add(original & " – larger than 5 MB") : Continue For
            End If
            If Not f.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) OrElse Not LooksLikeImage(f.InputStream, ext) Then
                bad.Add(original & " – not a real image file") : Continue For
            End If

            Dim name As String = UniqueName(dir, SafeFileName(Path.GetFileNameWithoutExtension(original)), If(ext = ".jpeg", ".jpg", ext))
            f.SaveAs(Path.Combine(dir, name))
            ok.Add(name)
        Next

        Dim msg As String = ""
        If ok.Count > 0 Then msg = ok.Count & " image" & If(ok.Count = 1, "", "s") & " uploaded."
        If bad.Count > 0 Then msg &= If(msg = "", "", " ") & "Skipped: " & String.Join("; ", bad) & "."
        ShowMessage(msg, ok.Count = 0)
        If ok.Count > 0 Then JustUploadedHidden.Value = String.Join("|", ok)
    End Sub

    ''' <summary>Checks the file's first bytes, so a renamed .exe or script can't pose as an image.</summary>
    Private Shared Function LooksLikeImage(s As Stream, ext As String) As Boolean
        Dim head(11) As Byte
        Dim n As Integer = s.Read(head, 0, 12)
        s.Position = 0
        If n < 4 Then Return False
        Select Case ext
            Case ".jpg", ".jpeg"
                Return head(0) = &HFF AndAlso head(1) = &HD8 AndAlso head(2) = &HFF
            Case ".png"
                Return head(0) = &H89 AndAlso head(1) = &H50 AndAlso head(2) = &H4E AndAlso head(3) = &H47
            Case ".gif"
                Return head(0) = &H47 AndAlso head(1) = &H49 AndAlso head(2) = &H46 AndAlso head(3) = &H38
            Case ".webp"
                Return n >= 12 AndAlso head(0) = &H52 AndAlso head(1) = &H49 AndAlso head(2) = &H46 AndAlso head(3) = &H46 AndAlso
                       head(8) = &H57 AndAlso head(9) = &H45 AndAlso head(10) = &H42 AndAlso head(11) = &H50
        End Select
        Return False
    End Function

    ''' <summary>"My Photo (1)!" → "My-Photo-1"</summary>
    Private Shared Function SafeFileName(s As String) As String
        Dim t As String = Regex.Replace(If(s, "").Trim(), "[^A-Za-z0-9_-]+", "-").Trim("-"c)
        If t.Length > 80 Then t = t.Substring(0, 80).Trim("-"c)
        Return If(t = "", "image-" & DateTime.Now.ToString("yyyyMMdd-HHmmss"), t)
    End Function

    Private Shared Function UniqueName(dir As String, baseName As String, ext As String) As String
        Dim name As String = baseName & ext
        Dim i As Integer = 1
        While File.Exists(Path.Combine(dir, name)) OrElse Directory.Exists(Path.Combine(dir, name))
            name = baseName & "-" & i & ext
            i += 1
        End While
        Return name
    End Function

    ' ------------------------------------------------------------------
    '  New folder
    ' ------------------------------------------------------------------
    Protected Sub CreateFolderButton_Click(sender As Object, e As EventArgs) Handles CreateFolderButton.Click
        Dim name As String = FolderNameFrom(NewFolderTextBox.Text)
        If name = "" Then
            ShowMessage("Folder names can use letters, numbers, spaces, hyphens and underscores (up to 60 characters).", True)
            Return
        End If
        Dim full As String = ResolvePath(Join(FolderPath, name))
        If Directory.Exists(full) OrElse File.Exists(full) Then
            ShowMessage("A folder called """ & name & """ already exists here.", True)
            Return
        End If
        Directory.CreateDirectory(full)
        NewFolderTextBox.Text = ""
        ShowMessage("Folder """ & name & """ created.", False)
    End Sub

    ''' <summary>"Diwali offers 2026" → "Diwali-offers-2026"; "" if not allowed.</summary>
    Private Shared Function FolderNameFrom(input As String) As String
        Dim t As String = Regex.Replace(If(input, "").Trim(), "\s+", "-")
        If Not Regex.IsMatch(t, "^[A-Za-z0-9][A-Za-z0-9_-]{0,59}$") Then Return ""
        Return t
    End Function

    ' ------------------------------------------------------------------
    '  Rename / delete (target comes from the selected item)
    ' ------------------------------------------------------------------
    Protected Sub RenameButton_Click(sender As Object, e As EventArgs) Handles RenameButton.Click
        Try
            Dim rel As String = CleanRel(TargetHidden.Value)
            If Not rel.StartsWith(If(FolderPath = "", "", FolderPath & "/")) Then Throw New UnauthorizedAccessException()
            Dim full As String = ResolvePath(rel)
            Dim dir As String = Path.GetDirectoryName(full)

            If Directory.Exists(full) Then
                Dim name As String = FolderNameFrom(NewNameHidden.Value)
                If name = "" Then ShowMessage("That folder name isn't allowed.", True) : Return
                Dim dest As String = Path.Combine(dir, name)
                If Directory.Exists(dest) OrElse File.Exists(dest) Then ShowMessage("Something called """ & name & """ already exists here.", True) : Return
                Directory.Move(full, dest)
                ShowMessage("Folder renamed to """ & name & """. Links to images inside it have changed.", False)
            ElseIf File.Exists(full) Then
                Dim ext As String = Path.GetExtension(full)
                Dim name As String = SafeFileName(Path.GetFileNameWithoutExtension(NewNameHidden.Value.Trim())) & ext   ' extension can't be changed
                Dim dest As String = Path.Combine(dir, name)
                If dest.Equals(full, StringComparison.OrdinalIgnoreCase) Then Return
                If File.Exists(dest) OrElse Directory.Exists(dest) Then ShowMessage("Something called """ & name & """ already exists here.", True) : Return
                File.Move(full, dest)
                ShowMessage("Renamed to """ & name & """. Update any page that used the old link.", False)
            End If
        Catch ex As Exception
            ShowMessage("Couldn't rename: " & ex.Message, True)
        End Try
    End Sub

    Protected Sub DeleteButton_Click(sender As Object, e As EventArgs) Handles DeleteButton.Click
        Try
            Dim rel As String = CleanRel(TargetHidden.Value)
            If rel = "" OrElse Not rel.StartsWith(If(FolderPath = "", "", FolderPath & "/")) Then Throw New UnauthorizedAccessException()
            Dim full As String = ResolvePath(rel)

            If Directory.Exists(full) Then
                If Directory.EnumerateFileSystemEntries(full).Any() Then
                    ShowMessage("That folder isn't empty. Delete or move what's inside it first.", True)
                    Return
                End If
                Directory.Delete(full)
                ShowMessage("Folder deleted.", False)
            ElseIf File.Exists(full) Then
                File.Delete(full)
                ShowMessage("""" & Path.GetFileName(full) & """ deleted.", False)
            End If
        Catch ex As Exception
            ShowMessage("Couldn't delete: " & ex.Message, True)
        End Try
    End Sub

    Private Sub ShowMessage(text As String, isError As Boolean)
        FlashLabel.Text = If(isError, "<i class=""fa fa-exclamation-circle""></i> ", "<i class=""fa fa-check""></i> ") & HttpUtility.HtmlEncode(text)
        FlashLabel.CssClass = If(isError, "flash err", "flash ok")
        FlashLabel.Visible = True
    End Sub
End Class