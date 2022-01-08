Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports IndiaBobbles

Imports System.IO
Imports System.Data

Public Class ViewDrive
    Inherits AdminPage

    Public FolderList As List(Of String) = New List(Of String)()
    Public CurrentFolder As RDirectoryItem = New RDirectoryItem()
    Public Property FolderPath As String
    Private DM As DriveManager

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            DM = New DriveManager(CurrentUser, Server.MapPath(Utility.SiteDriveFolderPath), String.Format("{0}/{1}", Utility.SiteURL, Utility.SiteDriveFolderName))
            DM.ItemDeletable = True

            If Request.QueryString("folderpath") IsNot Nothing Then
                FolderPath = Request.QueryString("folderpath").ToString().Trim()
            Else
                FolderPath = String.Empty
            End If

            FolderList = FolderPath.Split("/"c).ToList()
            CurrentFolder = DM.GetFolderName(FolderPath)
            FolderTableRepeater.DataSource = DM.GetDirectoryItemList(FolderPath)
            FolderTableRepeater.DataBind()
            FileItemRepeater.DataSource = DM.GetFileItemList(FolderPath)
            FileItemRepeater.DataBind()
        Catch ex As Exception
            message4.Text = String.Format("Unable to process request. Error - {0}", ex.Message)
            message4.Visible = True
            message4.Indicate = AlertType.[Error]
        End Try
    End Sub

End Class