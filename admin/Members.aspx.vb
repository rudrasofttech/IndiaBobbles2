Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports IndiaBobbles
Public Class Members
    Inherits AdminPage

    Private ReadOnly dc As New indiabobblesEntities

    Private Sub Members_Load(sender As Object, e As EventArgs) Handles Me.Load
        If ForbidUserAccess(MemberTypeType.Admin) Then
            Response.Redirect("default.aspx")
        End If



        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            Bind(0)
        End If
    End Sub

    Private Sub Bind(ByVal pageIndex As Integer)
        Try
            Dim members = dc.Members.Where(Function(t) True)
            Dim query As String = "SELECT ID, Email, Createdate, Newsletter, UserType, MemberName, Status, Password, Mobile " & " FROM Member AS M WHERE UserType <> 1 "

            If FilterTextBox.Text.Trim() <> "" Then
                members = members.Where(Function(t) t.Email.Contains(FilterTextBox.Text.Trim()))
            End If

            If StatusDropDown.SelectedValue <> "" Then
                query = String.Format("{0} AND(Status = {1})", query, StatusDropDown.SelectedValue)

                Select Case StatusDropDown.SelectedValue
                    Case "0"
                        members = members.Where(Function(t) t.Status = CByte(GeneralStatusType.Active))
                    Case "1"
                        members = members.Where(Function(t) t.Status = CByte(GeneralStatusType.Inactive))
                    Case "2"
                        members = members.Where(Function(t) t.Status = CByte(GeneralStatusType.Deleted))
                End Select
            End If

            If SubscribeList.SelectedValue <> "" Then
                query = String.Format("{0} AND(Newsletter = {1})", query, SubscribeList.SelectedValue)
                members = members.Where(Function(t) t.Newsletter)
            End If

            members = members.OrderByDescending(Function(t) t.Createdate)
            query = String.Format("{0} ORDER BY CreateDate desc ", query)
            MemberGridView.DataSource = members.ToList()
            MemberGridView.PageIndex = pageIndex
            MemberGridView.DataBind()
        Catch ex As Exception
            Trace.Write("Unable to fetch member records.")
            Trace.Write(ex.Message)
            Trace.Write(ex.StackTrace)
        End Try
    End Sub

    Protected Sub SubmitButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        Bind(0)
    End Sub

    Protected Sub MemberGridView_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs)
        Bind(e.NewPageIndex)
    End Sub

    Protected Sub MemberGridView_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        If e.Row.RowIndex > -1 Then

            Select Case e.Row.Cells(5).Text
                Case "2"
                    e.Row.Cells(5).Text = "Author"
                Case "3"
                    e.Row.Cells(5).Text = "Member"
            End Select

            Select Case e.Row.Cells(7).Text
                Case "0"
                    e.Row.Cells(7).Text = "Active"
                Case "2"
                    e.Row.Cells(7).Text = "Deleted"
                Case "1"
                    e.Row.Cells(7).Text = "Inactive"
            End Select
        End If
    End Sub

    Protected Sub MemberGridView_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)
        If e.CommandName = "Remove" Then
            Try
                Dim memberid As Long = Long.Parse(e.CommandArgument.ToString())
                Dim m As Member = dc.Members.SingleOrDefault(Function(t) t.ID = memberid)
                If m IsNot Nothing Then
                    dc.Members.Remove(m)
                    dc.SaveChanges()
                    Bind(MemberGridView.PageIndex)
                End If
            Catch ex As Exception
                message.Visible = True
                message.Text = String.Format("Error: {0}", ex.Message)
                message.Indicate = AlertType.[Error]
                message.Heading = "Oh Snap!"
            End Try
        End If
    End Sub

    Protected Sub DeleteButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        For Each row As GridViewRow In MemberGridView.Rows

            If row.RowIndex > -1 Then
                Dim ck As CheckBox = TryCast(row.FindControl("cbSelect"), CheckBox)

                If ck.Checked Then
                    Dim lt As Literal = TryCast(row.FindControl("MemberIDLt"), Literal)
                    Dim mid = Long.Parse(lt.Text)
                    Dim m As Member = dc.Members.SingleOrDefault(Function(t) t.ID = mid)
                    If m IsNot Nothing Then
                        Try
                            dc.Members.Remove(m)
                            dc.SaveChanges()
                        Catch ex As Exception
                            message.Visible = True
                            message.Text = String.Format("{0} is not delete. Error: {1}", m.UserName, ex.Message)
                            message.Indicate = AlertType.[Error]
                            message.Heading = "Oh Snap!"
                        End Try
                    End If
                End If
            End If
        Next
        Bind(MemberGridView.PageIndex)
    End Sub
End Class