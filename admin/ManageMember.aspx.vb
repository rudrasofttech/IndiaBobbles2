Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports IndiaBobbles
Public Class ManageMember
    Inherits AdminPage
    Private ReadOnly dc As New indiabobblesEntities()
    Private Sub ManageMember_Load(sender As Object, e As EventArgs) Handles Me.Load
        If ForbidUserAccess(MemberTypeType.Admin) Then
            Response.Redirect("default.aspx")
        End If

        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            PopulateYear()
            PopulateDays()
            If Mode = "edit" Then
                Bind()
            Else
                Response.Redirect("members.aspx")
            End If
        End If
    End Sub

    Public Sub PopulateYear()
        YearDropDown.Items.Clear()
        Dim year As Integer = DateTime.Now.Year - 13
        For i As Integer = (year - 80) To year
            YearDropDown.Items.Add(New ListItem(i.ToString(), i.ToString()))
        Next
    End Sub

    Public Sub PopulateDays()
        DateDropDown.Items.Clear()
        For i As Integer = 1 To DateTime.DaysInMonth(Integer.Parse(YearDropDown.SelectedValue), Integer.Parse(MonthDropDown.SelectedValue))
            DateDropDown.Items.Add(New ListItem(i.ToString(), i.ToString()))
        Next
    End Sub

    Private Sub Bind()
        Try
            Dim m As Member = dc.Members.FirstOrDefault(Function(item) item.ID = TargetID)
            EmailTextBox.Text = m.Email
            NameTextBox.Text = m.MemberName
            StatusDropDown.SelectedValue = m.Status.ToString()
            MemberTypeDropDown.SelectedValue = m.UserType.ToString()
            NewsletterCheckBox.Checked = m.Newsletter
            LastNameTextBox.Text = m.LastName
            If m.DOB.HasValue Then
                YearDropDown.SelectedValue = m.DOB.Value.Year.ToString()
                MonthDropDown.SelectedValue = m.DOB.Value.Month.ToString()
                PopulateDays()
                DateDropDown.SelectedValue = m.DOB.Value.Date.ToString()
            End If

            CountryDropDown.SelectedValue = m.Country
            AltEmailTextBox.Text = m.AlternateEmail
            AltEmail2TextBox.Text = m.AlternateEmail2
            MobileTextBox.Text = m.Mobile
            PhoneTextBox.Text = m.Phone
            AddressTextBox.Text = m.Address

            If Not String.IsNullOrEmpty(m.Gender) Then
                GenderDropDown.SelectedValue = m.Gender
            End If

        Catch ex As Exception
            Response.Redirect("members.aspx")
            Trace.Write("Unable to load member details.")
            Trace.Write(ex.Message)
            Trace.Write(ex.StackTrace)
        End Try
    End Sub

    Protected Sub SubmitButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        Page.Validate("MemberGrp")
        If Not Page.IsValid Then Return
        Try
            Dim dob As DateTime = DateTime.Parse(String.Format("{0}-{1}-{2}", YearDropDown.SelectedValue, MonthDropDown.SelectedValue, DateDropDown.SelectedValue))
            Dim m As Member = (From u In dc.Members Where u.ID = TargetID Select u).SingleOrDefault()
            m.MemberName = NameTextBox.Text.Trim()
            m.Status = Byte.Parse(StatusDropDown.SelectedValue)
            m.UserType = Byte.Parse(MemberTypeDropDown.SelectedValue)
            m.Newsletter = NewsletterCheckBox.Checked
            m.DOB = dob
            m.Country = CountryDropDown.SelectedValue
            m.AlternateEmail = AltEmailTextBox.Text.Trim()
            m.AlternateEmail2 = AltEmail2TextBox.Text.Trim()
            m.Mobile = MobileTextBox.Text.Trim()
            m.Phone = PhoneTextBox.Text.Trim()
            m.Address = AddressTextBox.Text.Trim()
            m.LastName = LastNameTextBox.Text.Trim()
            m.ModifiedBy = CurrentUser.ID
            m.ModifyDate = DateTime.Now
            m.Gender = Char.Parse(GenderDropDown.SelectedValue)
            dc.SaveChanges()
            Response.Redirect("members.aspx")
        Catch ex As Exception
            Trace.Write("Unable to load member details.")
            Trace.Write(ex.Message)
            Trace.Write(ex.StackTrace)
        End Try
    End Sub

    Protected Sub YearDropDown_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        PopulateDays()
    End Sub

    Protected Sub MonthDropDown_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        PopulateDays()
    End Sub
End Class