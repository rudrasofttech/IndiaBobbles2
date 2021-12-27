Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports IndiaBobbles

Public Class Emails
    Inherits AdminPage
    Private dc As New indiabobblesEntities()

    Private Sub Bind(ByVal pageIndex As Integer)
        Try
            Dim query = dc.EmailMessages.Where(Function(m) True)

            If KeywordTextBox.Text.Trim() <> "" Then
                query = query.Where(Function(m) m.Subject.Contains(KeywordTextBox.Text.Trim()) Or m.Message.Contains(KeywordTextBox.Text.Trim()))
            End If

            If TypeDropDown.SelectedValue <> "" Then
                query = query.Where(Function(m) m.EmailType = Byte.Parse(TypeDropDown.SelectedValue.Trim()))
            End If

            If GroupDropDown.SelectedValue <> "" Then

                query = query.Where(Function(m) m.EmailGroup = GroupDropDown.SelectedValue.Trim())
            End If

            If SentDropDown.SelectedValue = "1" Then
                query = query.Where(Function(m) m.IsSent)
            End If
            If SentDropDown.SelectedValue = "0" Then
                query = query.Where(Function(m) Not m.IsSent)
            End If

            If ReadDropDown.SelectedValue = "1" Then
                query = query.Where(Function(m) m.IsRead)
            End If
            If ReadDropDown.SelectedValue = "0" Then
                query = query.Where(Function(m) Not m.IsRead)
            End If

            query = query.OrderByDescending(Function(m) m.CreateDate)
            EmailGrid.DataSource = query.ToList()
            EmailGrid.PageIndex = pageIndex
            EmailGrid.DataBind()

        Catch ex As Exception
            Trace.Write("Unable to fetch email records.")
            Trace.Write(ex.Message)
            Trace.Write(ex.StackTrace)
        End Try
    End Sub

    Protected Sub SubmitButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        Bind(0)
    End Sub

    Protected Sub EmailGrid_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs)
        Bind(e.NewPageIndex)
    End Sub

    Protected Sub DeleteButton_Click(ByVal sender As Object, ByVal e As EventArgs)

        For Each row As GridViewRow In EmailGrid.Rows
            If row.RowIndex > -1 Then
                Dim ck As CheckBox = TryCast(row.FindControl("cbSelect"), CheckBox)
                If ck.Checked Then
                    Dim lt As Literal = TryCast(row.FindControl("EmailIDLt"), Literal)
                    Dim em = dc.EmailMessages.SingleOrDefault(Function(m) m.ID.ToString() = lt.Text)

                    If em IsNot Nothing Then
                        dc.EmailMessages.Remove(em)
                    End If
                End If
            End If
        Next
        dc.SaveChanges()
        Bind(EmailGrid.PageIndex)
    End Sub

    Private Sub Emails_Load(sender As Object, e As EventArgs) Handles Me.Load
        If ForbidUserAccess(MemberTypeType.Admin, MemberTypeType.Editor) Then
            Response.Redirect("default.aspx")
        End If

        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            Bind(0)
        End If
    End Sub
End Class