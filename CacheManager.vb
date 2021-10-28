Public Class CacheManager
    Public Shared Sub Remove(ByVal key As String)
        HttpContext.Current.Cache.Remove(key)
    End Sub

    Public Shared Function [Get](Of T)(ByVal key As String) As T
        Try
            Return CType(HttpContext.Current.Cache.[Get](key), T)
        Catch
            Return Nothing
        End Try
    End Function

    Public Shared Sub Add(ByVal key As String, ByVal value As Object, ByVal expiry As DateTime)
        HttpContext.Current.Cache.Add(key, value, Nothing, expiry, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, Nothing)
    End Sub

    Public Shared Sub AddSliding(ByVal key As String, ByVal value As Object, ByVal waitInMinutes As Integer)
        HttpContext.Current.Cache.Insert(key, value, Nothing, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(waitInMinutes))
    End Sub

    Public Shared Property Category As List(Of Category)
        Get

            If HttpContext.Current.Cache("Category") Is Nothing Then
                Return New List(Of Category)()
            Else
                Return CType(HttpContext.Current.Cache("Category"), List(Of Category))
            End If
        End Get
        Set(ByVal value As List(Of Category))
            HttpContext.Current.Cache("Category") = value
        End Set
    End Property
End Class
