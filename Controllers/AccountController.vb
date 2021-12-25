Imports System.Web.Mvc


Namespace Controllers
    Public Class AccountController
        Inherits Controller

        Private ReadOnly db As New indiabobblesEntities

        ' GET: Account/Login
        Function Login() As ActionResult
            Return View(New LoginDTO())
        End Function

        ' POST: Account/Login
        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Function Login(ByVal dto As LoginDTO) As ActionResult
            If Not ModelState.IsValid Then
                ViewBag.Error = "Invalid input"
            End If

            Try
                Dim user = db.Members.FirstOrDefault(Function(m) m.Email = dto.Email And m.Password = dto.Password)
                If user IsNot Nothing Then
                    FormsAuthentication.SetAuthCookie(user.Email, True)
                    If user.UserType = CByte(MemberTypeType.Admin) Then
                        Return Redirect("~/admin/orders.aspx")
                    Else
                        Return Redirect("~")
                    End If
                Else
                    ViewBag.Error = "Invalid Credentials"
                    Return View(dto)
                End If
            Catch ex As Exception
                ViewBag.Error = ex.Message
                Return View(dto)
            End Try
        End Function

        Function Register() As ActionResult
            Return View(New RegisterDTO())
        End Function

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Function Register(ByVal dto As RegisterDTO) As ActionResult
            If Not ModelState.IsValid Then
                ViewBag.Error = "Please check your input."
                Return View(dto)
            End If

            Try
                Dim user = db.Members.FirstOrDefault(Function(m) m.Email = dto.Email)
                If user IsNot Nothing Then
                    ViewBag.Error = "This email address is already in our records."
                    Return View(dto)
                Else
                    Dim m As New Member()
                    m.MemberName = dto.Name
                    m.Email = dto.Email
                    m.Password = dto.Password
                    m.Createdate = DateTime.UtcNow
                    m.Mobile = dto.Mobile
                    m.Newsletter = dto.Newsletter
                    m.UserType = MemberTypeType.Member
                    db.Members.Add(m)
                    db.SaveChanges()

                    Dim body As String = String.Format("Dear {0},<br/><br/>You are now a registered member of IndiaBobbles.<br/><br/>Please click this link to <a href=""https://www.indiabobbles.com/account/activate/{1}"">activate your account</a>.<br/><br/>", m.MemberName, m.Email)
                    Dim eman As New EmailManager()
                    eman.SendMail(Utility.NewsletterEmail, m.Email, Utility.AdminName,
                                  m.MemberName, body, "Registration Successfull",
                                  EmailMessageType.Communication, "Registration")


                End If
            Catch ex As Exception
                ViewBag.Error = ex.Message
                Return View(dto)
            End Try


        End Function

    End Class
End Namespace