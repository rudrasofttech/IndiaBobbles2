Public Class AdminPage
    Inherits System.Web.UI.Page

    Public Property CurrentUser As Member
    Private ReadOnly db As New indiabobblesEntities

    Public Sub New()
        Mode = String.Empty
    End Sub

    Public Property Mode As String
    Public Property TargetID As Integer

    Public Function ForbidUserAccess(ParamArray ValidUserTypes As MemberTypeType()) As Boolean
        Dim utype As MemberTypeType = CType(CurrentUser.UserType, MemberTypeType)

        For i As Integer = 0 To ValidUserTypes.Length - 1

            If ValidUserTypes(i) = utype Then
                Return False
            End If
        Next

        Return True
    End Function

    Protected Overrides Sub OnInit(ByVal e As EventArgs)
        MyBase.OnInit(e)

        If Not Request.IsAuthenticated Then
            Response.Redirect("~/account/login")
        Else
            CurrentUser = db.Members.FirstOrDefault(Function(m) m.Email = Page.User.Identity.Name)
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("mode")) Then
            Mode = Request.QueryString("mode").ToString()
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
            TargetID = Integer.Parse(Request.QueryString("id").ToString())
        End If
    End Sub
End Class
