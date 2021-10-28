Imports System.IO
Imports System.Xml.Serialization

Public Class Utility
    Public Shared ReadOnly Property SiteLogo As String
        Get
            Return GetSiteSetting("SiteLogo")
        End Get
    End Property

    Public Shared ReadOnly Property ContactEmail As String
        Get
            Return GetSiteSetting("ContactEmail")
        End Get
    End Property

    Public Shared ReadOnly Property Fax As String
        Get
            Return GetSiteSetting("Fax")
        End Get
    End Property

    Public Shared ReadOnly Property Phone As String
        Get
            Return GetSiteSetting("Phone")
        End Get
    End Property

    Public Shared ReadOnly Property Address As String
        Get
            Return GetSiteSetting("Address")
        End Get
    End Property

    Public Shared ReadOnly Property SiteName As String
        Get
            Return GetSiteSetting("SiteName")
        End Get
    End Property

    Public Shared ReadOnly Property SiteURL As String
        Get
            Return GetSiteSetting("SiteURL")
        End Get
    End Property

    Public Shared ReadOnly Property UniversalPassword As String
        Get
            Return GetSiteSetting("UniversalPassword")
        End Get
    End Property

    Public Shared ReadOnly Property NewsletterEmail As String
        Get
            Return GetSiteSetting("NewsletterEmail")
        End Get
    End Property

    Public Shared ReadOnly Property AdminName As String
        Get
            Return GetSiteSetting("AdminName")
        End Get
    End Property

    Public Shared ReadOnly Property SiteTitle As String
        Get
            Return GetSiteSetting("SiteTitle")
        End Get
    End Property

    Public Shared Function NewsletterDesign() As String
        Return GetSiteSetting("NewsletterDesign")
    End Function

    Public Shared Function ImageFormat() As String
        Return ".bmp,.dds,.dng,.gif,.jpg,.png,.psd,.psd,.pspimage,.tga,.thm,.tif,.yuv,.ai,.eps,.ps,.svg"
    End Function

    Public Shared Function VideoFormat() As String
        Return ".3g2,.3gp,.asf,.asx,.flv,.mov,.mp4,.mpg,.rm,.srt,.swf,.vob,.wmv"
    End Function

    Public Shared Function TextFormat() As String
        Return ".doc,.docx,.log,.msg,.odt,.pages,.rtf,.tex,.txt,.wpd,.wps"
    End Function

    Public Shared Function CompresssedFormat() As String
        Return ".7z,.cbr,.deb,.gz,.pkg,.rar,.rpm,.sit,.sitx,.tar.gz,.zip,.zipx"
    End Function

    Public Shared ReadOnly Property ArticleFolder As String
        Get
            Return "~/sitedata/Article"
        End Get
    End Property

    Public Shared ReadOnly Property CustomPageFolder As String
        Get
            Return "~/sitedata/CustomPage"
        End Get
    End Property

    Public Shared ReadOnly Property SiteDriveFolderPath As String
        Get
            Return String.Format("~/{0}", SiteDriveFolderName)
        End Get
    End Property

    Public Shared ReadOnly Property SiteDriveFolderName As String
        Get
            Return "drive"
        End Get
    End Property

    Public Shared Function Deserialize(Of T)(ByVal obj As String) As T
        Dim SerializerObj As XmlSerializer = New XmlSerializer(GetType(T))
        Dim LoadedObj As T = CType(SerializerObj.Deserialize(New StringReader(obj)), T)
        Return LoadedObj
    End Function

    Public Shared Function Serialize(Of T)(ByVal obj As T) As String
        Dim SerializerObj As XmlSerializer = New XmlSerializer(GetType(T))
        Dim WriteFileStream As TextWriter = New StringWriter()
        SerializerObj.Serialize(WriteFileStream, obj)
        Return WriteFileStream.ToString()
    End Function

    Public Shared Function GetArticleCount() As Integer
        If CacheManager.[Get](Of Integer?)("ArticleCount") Is Nothing Then
            Dim dc As New indiabobblesEntities
            CacheManager.Add("ArticleCount", dc.Posts.Count(Function(t) t.Status <> PostStatusType.Inactive), Date.Now.AddMinutes(10))

        End If

        Return CacheManager.[Get](Of Integer?)("ArticleCount").Value
    End Function

    Public Shared Function GetLatestPosts(take As Integer) As List(Of Post)
        Dim dc As New indiabobblesEntities
        Return dc.Posts.Where(Function(t) t.Category1.Name <> "Games").OrderByDescending(Function(t) t.DateCreated).Take(take).ToList()
    End Function

    Public Shared Function GetOrderCount() As Integer
        If CacheManager.[Get](Of Integer?)("OrderCount") Is Nothing Then
            Dim dc As New indiabobblesEntities
            CacheManager.Add("OrderCount", dc.Orders.Count(Function(t) t.Status <> OrderStatusType.[New]), Date.Now.AddMinutes(2))

        End If

        Return CacheManager.[Get](Of Integer?)("OrderCount").Value
    End Function

    Public Shared Function GetNewOrderCount() As Integer
        If CacheManager.[Get](Of Integer?)("NewOrderCount") Is Nothing Then
            Dim dc As New indiabobblesEntities

            CacheManager.Add("NewOrderCount", dc.Orders.Count(Function(t) t.Status = OrderStatusType.Process), Date.Now.AddMinutes(2))

        End If

        Return CacheManager.[Get](Of Integer?)("NewOrderCount").Value
    End Function

    Public Shared Function GetMemberCount() As Integer
        If CacheManager.[Get](Of Integer?)("MemberCount") Is Nothing Then
            Dim dc As New indiabobblesEntities

            CacheManager.Add("MemberCount", dc.Members.Count(Function(t) t.Status = CByte(GeneralStatusType.Active)), DateTime.Now.AddMinutes(50))

        End If

        Return CacheManager.[Get](Of Integer?)("MemberCount").Value
    End Function

    Public Shared Function CategoryList() As List(Of Category)
        If CacheManager.Category.Count = 0 Then
            Dim dc As New indiabobblesEntities
            CacheManager.Category = dc.Categories.ToList()
        End If

        Return CacheManager.Category
    End Function

    Public Shared Function GetSiteSetting(ByVal keyname As String) As String
        If CacheManager.[Get](Of String)(keyname) Is Nothing Then
            Dim dc As New indiabobblesEntities
            CacheManager.Add(keyname, (From t In dc.WebsiteSettings Where t.KeyName = keyname Select t.KeyValue).SingleOrDefault(), DateTime.Now.AddDays(7))
        End If

        Return CacheManager.[Get](Of String)(keyname)
    End Function

    Public Shared Function RemoveAccent(ByVal txt As String) As String
        Dim bytes As Byte() = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt)
        Return System.Text.Encoding.ASCII.GetString(bytes)
    End Function

    Public Shared Function Slugify(ByVal phrase As String) As String
        Dim str As String = RemoveAccent(phrase).ToLower()
        str = System.Text.RegularExpressions.Regex.Replace(str, "[^a-z0-9/\s-]", "")
        str = System.Text.RegularExpressions.Regex.Replace(str, "\s+", " ").Trim()
        str = System.Text.RegularExpressions.Regex.Replace(str, "\s", "-")
        Return str
    End Function

    Public Shared Function GenerateBlogArticleURL(ByVal a As Article, ByVal root As String) As String
        Return String.Format("{1}/blog/{0}", a.URL, root)
    End Function

    Public Shared Function GetCategoryArticleCount(ByVal c As Category) As Integer
        If CacheManager.[Get](Of Integer?)(String.Format("CategoryArticleCount{0}", c.ID)) Is Nothing Then
            Dim dc As New indiabobblesEntities
            CacheManager.Add(String.Format("CategoryArticleCount{0}", c.ID), (From t In dc.Posts Where t.Status = CByte(PostStatusType.Publish) AndAlso t.Category = c.ID Select t).Count(), DateTime.Now.AddMinutes(10))
        End If

        Return CacheManager.[Get](Of Integer?)(String.Format("CategoryArticleCount{0}", c.ID)).Value
    End Function

    Public Shared Function ValidateEmail(ByVal email As String) As Boolean
        Dim pattern As String = "\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
        Dim regex As Regex = New Regex(pattern, RegexOptions.IgnoreCase)
        Return regex.IsMatch(email)
    End Function

    Public Shared Function ValidateRequired(ByVal input As String) As Boolean
        If input.Trim() = String.Empty Then
            Return False
        Else
            Return True
        End If
    End Function
End Class
