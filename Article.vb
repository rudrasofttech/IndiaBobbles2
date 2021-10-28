Public Class Article
    Public Property ID As Long
    Public Property Title As String
    Public Property MetaTitle As String
    Public Property OGImage As String
    Public Property OGDescription As String
    Public Property DateCreated As DateTime
    Public Property DateModified As DateTime?
    Public Property CreatedBy As Long
    Public Property CreatedByName As String
    Public Property ModifiedBy As Long?
    Public Property ModifiedByName As String
    Public Property Category As Integer
    Public Property Tag As String
    Public Property CategoryName As String
    Public Property Status As PostStatusType
    Public Property Description As String
    Public Property Text As String
    Public Property TemplateName As String
    Public Property WriterName As String
    Public Property WriterEmail As String
    Public Property Viewed As Integer
    Public Property URL As String
    Public Property Sitemap As Boolean

    Public Sub New()
        TemplateName = String.Empty
        Status = PostStatusType.Draft
        WriterEmail = String.Empty
        WriterName = String.Empty
        OGImage = String.Empty
        OGDescription = String.Empty
        ModifiedByName = String.Empty
        Viewed = 0
        URL = String.Empty
        Sitemap = True
        MetaTitle = String.Empty
    End Sub
End Class