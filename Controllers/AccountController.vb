Imports System.Threading.Tasks
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
                    user.Status = GeneralStatusType.Active
                    db.SaveChanges()
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
                    Dim r As New Random(0)
                    Dim password As String = String.Format("{0}{1}{2}{3}{4}{5}{7}", r.Next(0, 9), r.Next(0, 9), r.Next(0, 9), r.Next(0, 9), r.Next(0, 9), r.Next(0, 9), r.Next(0, 9), r.Next(0, 9))
                    Dim m As New Member With {
                        .MemberName = dto.Name,
                        .Email = dto.Email,
                        .Password = password,
                        .Createdate = DateTime.UtcNow,
                        .Mobile = dto.Mobile,
                        .Newsletter = dto.Newsletter,
                        .UserType = MemberTypeType.Member
                    }
                    db.Members.Add(m)
                    db.SaveChanges()

                    Dim body As String = String.Format("Dear {0},<br/><br/>You are now a registered member of IndiaBobbles.<br/><br/>Your one time password is <strong>{1}</strong>.<br/><br/>", m.MemberName, password)
                    Dim eman As New EmailManager()
                    eman.SendMail(Utility.NewsletterEmail, m.Email, Utility.AdminName,
                                  m.MemberName, body, "Registration Successfull",
                                  EmailMessageType.Communication, "Registration")

                    Return Redirect("~/account/login")
                End If
            Catch ex As Exception
                ViewBag.Error = ex.Message
                Return View(dto)
            End Try
        End Function

        Function Logout() As ActionResult
            FormsAuthentication.SignOut()
            Return Redirect("~")
        End Function

        Function GenerateOTP() As ActionResult
            Return View(New OTPDTO())
        End Function

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Function GenerateOTP(ByVal dto As OTPDTO) As ActionResult
            If Not ModelState.IsValid Then
                ViewBag.Error = "Please check your input."
                Return View(dto)
            End If

            Try
                Dim user = db.Members.FirstOrDefault(Function(m) m.Email = dto.Email)
                If user Is Nothing Then
                    ViewBag.Error = "This email address is not in our records. Please register before "
                    Return View(dto)
                Else
                    Dim r As New Random()
                    Dim password As String = r.Next(100000, 999999).ToString()
                    user.Password = password
                    db.SaveChanges()

                    Dim body As String = String.Format("Dear {0},<br/><br/>Your one time password is <strong>{1}</strong>.<br/><br/>", user.MemberName, password)
                    Dim eman As New EmailManager()
                    eman.SendMail(Utility.NewsletterEmail, user.Email, Utility.AdminName,
                                  user.MemberName, body, "India Bobbles OTP",
                                  EmailMessageType.Communication, "OTP")
                    ViewBag.Success = String.Format("We have sent OTP to you registered email address. Please check you mail box for an email from {0}", user.Email)
                    Return View(dto)
                End If
            Catch ex As Exception
                ViewBag.Error = ex.Message
                Return View(dto)
            End Try
        End Function

        <Authorize>
        Function ManageProfile() As ActionResult
            Dim u = db.Members.FirstOrDefault(Function(m) m.Email = User.Identity.Name)
            Return View(u)
        End Function

        <Authorize>
        <HttpPost>
        <ValidateAntiForgeryToken>
        Function ManageProfile(ByVal m As Member) As ActionResult
            If Not ModelState.IsValid Then
                Return View(m)
            End If
            Dim u = db.Members.FirstOrDefault(Function(t) t.Email = User.Identity.Name)
            u.Newsletter = m.Newsletter
            u.MemberName = m.MemberName
            u.LastName = m.LastName
            u.DOB = m.DOB
            u.Country = m.Country
            u.Mobile = m.Mobile
            u.Gender = m.Gender
            db.SaveChanges()
            Return Redirect("~/account/manageprofile")
        End Function

    End Class
End Namespace