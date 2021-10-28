Public Class CookieWorker
    Public Const OrderIdKey As String = "indiabobblesorder"

    Public Shared Sub SetCookie(ByVal cname As String, ByVal key As String, ByVal value As String, ByVal expiry As DateTime)
        Dim cookie As HttpCookie = New HttpCookie(cname)
        cookie(key) = value
        cookie.Expires = expiry
        HttpContext.Current.Response.Cookies.Add(cookie)
    End Sub

    Public Shared Sub RemoveCookie(ByVal cname As String)
        HttpContext.Current.Response.Cookies(cname).Expires = DateTime.Now.AddDays(-1)
    End Sub

    Public Shared Function GetCookie(ByVal cname As String, ByVal key As String) As String
        Dim result As String = String.Empty

        If HttpContext.Current.Request.Cookies(cname) IsNot Nothing Then

            If HttpContext.Current.Request.Cookies(cname)(key) IsNot Nothing Then
                result = HttpContext.Current.Request.Cookies(cname)(key)
            End If
        End If

        Return result
    End Function
End Class
