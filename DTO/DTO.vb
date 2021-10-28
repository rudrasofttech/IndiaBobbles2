Imports System.ComponentModel.DataAnnotations

Public Class LoginDTO
    Private _email As String = String.Empty
    <Required>
    <EmailAddress>
    Public Property Email() As String
        Get
            Return _email
        End Get
        Set(ByVal value As String)
            _email = value
        End Set
    End Property

    Private _password As String = String.Empty
    <Required>
    Public Property Password() As String
        Get
            Return _password
        End Get
        Set(ByVal value As String)
            _password = value
        End Set
    End Property
End Class
