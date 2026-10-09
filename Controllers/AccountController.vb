Imports System.Threading.Tasks
Imports System.Web.Mvc


Namespace Controllers
    Public Class AccountController
        Inherits Controller

        Private ReadOnly db As indiabobblesEntities
        Private ReadOnly _captchaManager As CaptchaManager
        Public Property CaptchaKey As Guid
        Public Property CaptchaImage As String

        Public Sub New()
            db = New indiabobblesEntities()
            _captchaManager = New CaptchaManager(db)
        End Sub

        Private Sub LoadCaptcha()
            _captchaManager.RemoveOld()
            Dim c = _captchaManager.GenerateCaptcha()
            CaptchaKey = c.Id
            CaptchaImage = _captchaManager.CaptchaImage
        End Sub

        Private Sub LoadHighlights()
            Dim hl = db.CategoryTags.FirstOrDefault(Function(m) m.UrlName = "highlight")
            If hl IsNot Nothing Then
                ViewBag.Highlights = db.ProductTags.Where(Function(m) m.TagID = hl.ID).Select(Function(m) m.Product).ToList()
            Else
                ViewBag.Highlights = New List(Of Product)()
            End If
        End Sub

        ' GET: Account/Login
        Function Login() As ActionResult
            Return RedirectPermanent("~/account/otplogin")
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
            LoadCaptcha()
            Dim model As New RegisterDTO With {
                .CaptchaImage = CaptchaImage,
                .CaptchaKey = CaptchaKey
            }
            Return View(model)
        End Function

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Function Register(ByVal dto As RegisterDTO) As ActionResult
            If Not ModelState.IsValid Then
                ViewBag.Error = "Please check your input."
                LoadCaptcha()
                dto.CaptchaValue = String.Empty
                dto.CaptchaImage = CaptchaImage
                dto.CaptchaKey = CaptchaKey
                Return View(dto)
            End If
            If Not _captchaManager.IsValid(dto.CaptchaKey, dto.CaptchaValue) Then

                ViewBag.Error = "Invalid Captcha"
                LoadCaptcha()
                dto.CaptchaValue = String.Empty
                dto.CaptchaImage = CaptchaImage
                dto.CaptchaKey = CaptchaKey
                Return View(dto)
            End If

            Try
                Dim user = db.Members.FirstOrDefault(Function(m) m.Email = dto.Email)
                If user IsNot Nothing Then
                    ViewBag.Error = "This email address is already in our records."
                    LoadCaptcha()
                    dto.CaptchaValue = String.Empty
                    dto.CaptchaImage = CaptchaImage
                    dto.CaptchaKey = CaptchaKey
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
                                  m.MemberName, body, "Registration Successful",
                                  EmailMessageType.Communication, "Registration")

                    Return Redirect("~/account/otplogin")
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


        Function Unsubscribe(Optional ByVal email As String = "") As ActionResult
            LoadHighlights()
            Dim model As New UnsubscribeDTO With {
                .email = If(email, String.Empty).Trim()
            }
            Return View(model)
        End Function

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Function Unsubscribe(ByVal dto As UnsubscribeDTO) As ActionResult
            LoadHighlights()

            If Not ModelState.IsValid Then
                ViewBag.Error = "Please enter a valid email address."
                Return View(dto)
            End If

            Try
                Dim normalizedEmail = dto.Email.Trim().ToLowerInvariant()
                Dim alreadyUnsubscribed = db.UnsubscribedEmails.Any(Function(t) t.Email.ToLower() = normalizedEmail)

                If alreadyUnsubscribed Then
                    ViewBag.Success = "This email is already unsubscribed."
                    dto.Email = normalizedEmail
                    Return View(dto)
                End If

                Dim item As New UnsubscribedEmail With {
                    .ID = Guid.NewGuid(),
                    .Email = normalizedEmail,
                    .CreateDate = DateTime.UtcNow
                }

                db.UnsubscribedEmails.Add(item)
                db.SaveChanges()

                ViewBag.Success = "You have been unsubscribed successfully."
                dto.Email = normalizedEmail
                Return View(dto)
            Catch ex As Exception
                ViewBag.Error = ex.Message
                Return View(dto)
            End Try
        End Function

        Function OtpLogin() As ActionResult
            Return View(New OtpLoginDTO())
        End Function

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Function OtpLogin(ByVal dto As OtpLoginDTO, ByVal command As String) As ActionResult
            If String.Equals(command, "send", StringComparison.OrdinalIgnoreCase) Then
                ModelState.Remove("OTP")

                If Not ModelState.IsValid Then
                    ViewBag.Error = "Please check your input."
                    dto.StepNo = 1
                    Return View(dto)
                End If

                Try
                    Dim user = db.Members.FirstOrDefault(Function(m) m.Email = dto.Email)
                    If user Is Nothing Then
                        ViewBag.Error = "This email address is not in our records. Please register before login."
                        dto.StepNo = 1
                        Return View(dto)
                    End If

                    Dim r As New Random()
                    Dim password As String = r.Next(100000, 999999).ToString()
                    user.Password = password
                    db.SaveChanges()

                    Dim subject As String = password & " is your India Bobbles login code"
                    Dim body As String = BuildOtpEmail(user.MemberName, password)
                    Dim eman As New EmailManager()
                    eman.SendMail(Utility.NewsletterEmail, user.Email, Utility.AdminName,
                                  user.MemberName, body, subject,
                                  EmailMessageType.Communication, "OTP")

                    ViewBag.Success = String.Format("OTP sent to {0}. Please check your mailbox.", user.Email)
                    dto.StepNo = 2
                    Return View(dto)
                Catch ex As Exception
                    ViewBag.Error = ex.Message
                    dto.StepNo = 1
                    Return View(dto)
                End Try
            End If

            dto.StepNo = 2
            If Not ModelState.IsValid Then
                ViewBag.Error = "Please enter valid OTP."
                Return View(dto)
            End If

            Try
                Dim user = db.Members.FirstOrDefault(Function(m) m.Email = dto.Email And m.Password = dto.OTP)
                If user Is Nothing Then
                    ViewBag.Error = "Invalid OTP."
                    Return View(dto)
                End If

                user.Status = GeneralStatusType.Active
                db.SaveChanges()
                FormsAuthentication.SetAuthCookie(user.Email, True)

                If user.UserType = CByte(MemberTypeType.Admin) Then
                    Return Redirect("~/admin/orders.aspx")
                Else
                    Return Redirect("~")
                End If
            Catch ex As Exception
                ViewBag.Error = ex.Message
                Return View(dto)
            End Try
        End Function

        ''' <summary>Branded, mobile-friendly OTP login email (inline styles for email clients).</summary>
        Public Shared Function BuildOtpEmail(memberName As String, otp As String, Optional validMinutes As Integer = 10) As String
            Dim enc = Function(s As String) System.Net.WebUtility.HtmlEncode(If(s, ""))
            Dim firstName As String = If(String.IsNullOrWhiteSpace(memberName), "there", memberName.Trim().Split(" "c)(0))
            Dim code As String = enc(otp)
            ' Spaced-out digits are easier to read: 4 8 2 9 1 7
            Dim spaced As String = String.Join("&nbsp;", code.ToCharArray().Select(Function(c) c.ToString()))

            Dim sb As New System.Text.StringBuilder()
            sb.Append("<div style=""margin:0;padding:0;background:#f4f1f6;font-family:Arial,Helvetica,sans-serif;"">")
            ' Preview text shown in the inbox list
            sb.AppendFormat("<div style=""display:none;max-height:0;overflow:hidden;"">{0} is your India Bobbles login code. It expires in {1} minutes.</div>", code, validMinutes)
            sb.Append("<table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f4f1f6;""><tr><td align=""center"" style=""padding:24px 12px;"">")
            sb.Append("<table role=""presentation"" width=""480"" cellpadding=""0"" cellspacing=""0"" style=""width:100%;max-width:480px;background:#ffffff;border-radius:14px;overflow:hidden;"">")

            ' Header
            sb.Append("<tr><td align=""center"" style=""background:#ffc107;padding:16px 24px;"">")
            sb.Append("<a href=""https://www.indiabobbles.com""><img src=""https://www.indiabobbles.com/theme/khichdi/img/ib-logo.png"" alt=""India Bobbles"" height=""40"" style=""display:block;border:0;height:40px;""></a>")
            sb.Append("</td></tr>")

            ' Body
            sb.Append("<tr><td style=""padding:30px 28px 10px;color:#1f1724;font-size:15px;line-height:1.6;"">")
            sb.AppendFormat("<div style=""font-size:20px;font-weight:bold;color:#330B3F;margin-bottom:8px;"">Hi {0},</div>", enc(firstName))
            sb.Append("Use this code to log in to your India Bobbles account:")
            sb.Append("</td></tr>")

            ' Code box
            sb.Append("<tr><td align=""center"" style=""padding:14px 28px 6px;"">")
            sb.AppendFormat("<div style=""display:inline-block;background:#330B3F;color:#ffc107;font-size:32px;font-weight:bold;letter-spacing:4px;padding:16px 28px;border-radius:12px;font-family:'Courier New',Courier,monospace;"">{0}</div>", spaced)
            sb.AppendFormat("<div style=""font-size:13px;color:#6b5f72;margin-top:10px;"">This code expires in <b>{0} minutes</b> and can be used only once.</div>", validMinutes)
            sb.Append("</td></tr>")

            ' Safety note
            sb.Append("<tr><td style=""padding:20px 28px 26px;"">")
            sb.Append("<table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#fff4cc;border-radius:10px;""><tr><td style=""padding:12px 14px;font-size:13px;color:#3e3445;line-height:1.55;"">")
            sb.Append("&#128274; <b>Never share this code.</b> India Bobbles will never ask for it by phone, WhatsApp or email. ")
            sb.Append("If you didn't try to log in, you can ignore this email. Your account is safe.")
            sb.Append("</td></tr></table>")
            sb.Append("</td></tr>")

            ' Footer
            sb.Append("<tr><td align=""center"" style=""background:#330B3F;padding:16px 24px;color:#cbb8d4;font-size:12px;line-height:1.7;"">")
            sb.Append("<b style=""color:#ffc107;"">India Bobbles</b> · Hand-painted Bollywood &amp; Indian bobbleheads<br>")
            sb.Append("<a href=""https://www.indiabobbles.com"" style=""color:#ffc107;text-decoration:none;"">indiabobbles.com</a> · ")
            sb.Append("<a href=""mailto:indiabobbles@rudrasofttech.com"" style=""color:#ffc107;text-decoration:none;"">indiabobbles@rudrasofttech.com</a>")
            sb.Append("</td></tr>")

            sb.Append("</table></td></tr></table></div>")
            Return sb.ToString()
        End Function
    End Class
End Namespace