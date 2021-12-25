Public Class Message
    Inherits System.Web.UI.UserControl
    Public Property Indicate As AlertType

    Public Property Heading As String
        Get
            Return HeadingLit.Text
        End Get
        Set(ByVal value As String)
            HeadingLit.Text = value
        End Set
    End Property

    Public Property Text As String
        Get
            Return TextLit.Text
        End Get
        Set(ByVal value As String)
            TextLit.Text = value
        End Set
    End Property

    Public ReadOnly Property Block As String
        Get

            If Indicate = AlertType.[Error] Then
                Return "alert-danger"
            ElseIf Indicate = AlertType.Success Then
                Return "alert-success"
            ElseIf Indicate = AlertType.Warning Then
                Return "alert-warning"
            ElseIf Indicate = AlertType.Info Then
                Return "alert-info"
            Else
                Return ""
            End If
        End Get
    End Property

    Public Property HideClose As Boolean

End Class