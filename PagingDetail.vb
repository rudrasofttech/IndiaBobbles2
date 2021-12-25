Public Class PagingDetail
    Private _currentPage As Integer = 0
    Public Property CurrentPage() As Integer
        Get
            Return _currentPage
        End Get
        Set(ByVal value As Integer)
            _currentPage = value
        End Set
    End Property

    Private _totalRecord As Integer = 0
    Public Property TotalRecord() As Integer
        Get
            Return _totalRecord
        End Get
        Set(ByVal value As Integer)
            _totalRecord = value
        End Set
    End Property

    Private _pageSize As Integer = 30
    Public Property PageSize() As Integer
        Get
            Return _pageSize
        End Get
        Set(ByVal value As Integer)
            _pageSize = value
        End Set
    End Property
End Class
