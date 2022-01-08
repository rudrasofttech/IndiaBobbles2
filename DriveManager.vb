Imports System.IO

Public Class DriveManager
    Public CurrentMember As Member
    Private drivePath As String = String.Empty
    Private webPath As String = String.Empty
    Public Property ItemDeletable As Boolean

    Public ReadOnly Property MemberDataAbsPath As String
        Get
            Return drivePath
        End Get
    End Property

    Public ReadOnly Property MemberWebPath As String
        Get
            Return webPath
        End Get
    End Property

    Public Sub New(ByVal m As Member, ByVal dataFolderAbsolutePath As String, ByVal dataFolderWebPath As String)
        CurrentMember = m
        drivePath = dataFolderAbsolutePath
        webPath = dataFolderWebPath
    End Sub

    Public ReadOnly Property DriveExist As Boolean
        Get
            Return Directory.Exists(MemberDataAbsPath)
        End Get
    End Property

    Public Function RenameFile(ByVal filepath As String, ByVal name As String) As Boolean
        If Not DriveExist Then
            Throw New DriveDoesNotExistException()
        End If

        Dim drivepath As String = Path.Combine(MemberDataAbsPath, filepath)
        Dim fi As FileInfo = New FileInfo(drivepath)

        If fi.Exists Then
            Dim newpath As String = Path.Combine(fi.DirectoryName, name)
            File.Move(drivepath, newpath)
            Return True
        Else
            Throw New DirectoryNotFoundException()
        End If
    End Function

    Public Function RenameFolder(ByVal folderpath As String, ByVal name As String) As Boolean
        If Not DriveExist Then
            Throw New DriveDoesNotExistException()
        End If

        Dim drivepath As String = Path.Combine(MemberDataAbsPath, folderpath)
        Dim di As DirectoryInfo = New DirectoryInfo(drivepath)

        If di.Exists Then
            Dim newpath As String = Path.Combine(di.Parent.FullName, name)
            Directory.Move(drivepath, newpath)
            Return True
        Else
            Throw New DirectoryNotFoundException()
        End If
    End Function

    Public Function GetFolderName(ByVal folderPath As String) As RDirectoryItem
        If Not DriveExist Then
            Throw New DriveDoesNotExistException()
        End If

        If folderPath.Trim() <> String.Empty Then
            Dim drivepath As String = Path.Combine(MemberDataAbsPath, folderPath)
            Dim i As DirectoryInfo = New DirectoryInfo(drivepath)
            Dim rdi As RDirectoryItem = New RDirectoryItem()
            rdi.ID = Guid.NewGuid()
            rdi.CreateDate = i.CreationTime
            rdi.LastAccessDate = i.LastAccessTime
            rdi.Location = i.FullName.Replace(String.Format("{0}\", MemberDataAbsPath), String.Empty)
            rdi.ModifyDate = i.LastWriteTime
            rdi.Name = i.Name
            rdi.Contains = ""
            rdi.ThumbNail = String.Format("{0}/bootstrap/img/drive/folder-data-icon.png", Utility.SiteURL)
            Return rdi
        Else
            Return New RDirectoryItem()
        End If
    End Function

    Public Function GetFileItemList(ByVal folderpath As String) As List(Of RFileItem)
        If Not DriveExist Then
            Throw New DriveDoesNotExistException()
        End If

        Dim list As List(Of RFileItem) = New List(Of RFileItem)()
        Dim drivepath As String = Path.Combine(MemberDataAbsPath, folderpath)
        Dim di As DirectoryInfo = New DirectoryInfo(drivepath)

        If di.Exists Then

            For Each i As FileInfo In di.EnumerateFiles()
                Dim rdi As RFileItem = New RFileItem()
                rdi.ID = Guid.NewGuid()
                rdi.CreateDate = i.CreationTime
                rdi.FileType = i.Extension
                rdi.LastAccessDate = i.LastAccessTime
                rdi.Location = i.FullName.Replace(String.Format("{0}\", MemberDataAbsPath), String.Empty)
                rdi.ModifyDate = i.LastWriteTime
                rdi.Name = i.Name
                rdi.Deletable = ItemDeletable
                rdi.Editable = True
                Dim length As Long = i.Length

                If length < 1024 Then
                    rdi.Size = String.Format("{0} B", i.Length.ToString())
                ElseIf length >= 1024 AndAlso length < (1024 * 1024) Then
                    rdi.Size = String.Format("{0} KB", (i.Length / 1024).ToString())
                ElseIf length >= (1024 * 1024) AndAlso length < (1024 * 1024 * 1024) Then
                    rdi.Size = String.Format("{0} MB", ((i.Length / 1024) / 1024).ToString())
                End If

                rdi.WebPath = String.Format("{0}/{1}", MemberWebPath, rdi.Location.Replace("\", "/"))

                If Utility.ImageFormat().ToLower().IndexOf(rdi.FileType.ToLower()) > -1 Then
                    rdi.ItemType = DriveItemType.ImageFile
                    rdi.ThumbNail = rdi.WebPath
                ElseIf Utility.VideoFormat().ToLower().IndexOf(rdi.FileType.ToLower()) > -1 Then
                    rdi.ItemType = DriveItemType.VideoFile
                ElseIf Utility.TextFormat().ToLower().IndexOf(rdi.FileType.ToLower()) > -1 Then
                    rdi.ItemType = DriveItemType.TextFile
                ElseIf Utility.CompresssedFormat().ToLower().IndexOf(rdi.FileType.ToLower()) > -1 Then
                    rdi.ItemType = DriveItemType.ZipFile
                Else
                    rdi.ItemType = DriveItemType.File
                End If

                list.Add(rdi)
            Next
        End If

        Return list
    End Function

    Public Function GetDirectoryItemList(ByVal folderpath As String) As List(Of RDirectoryItem)
        If Not DriveExist Then
            Throw New DriveDoesNotExistException()
        End If

        Dim folderDeletable As Boolean = True
        Dim list As List(Of RDirectoryItem) = New List(Of RDirectoryItem)()
        Dim drivepath As String = Path.Combine(MemberDataAbsPath, folderpath)
        folderDeletable = ItemDeletable

        If folderpath = String.Empty Then
            folderDeletable = False
        End If

        Dim di As DirectoryInfo = New DirectoryInfo(drivepath)

        If di.Exists Then

            For Each i As DirectoryInfo In di.EnumerateDirectories()
                Dim rdi As RDirectoryItem = New RDirectoryItem()
                rdi.ID = Guid.NewGuid()
                rdi.CreateDate = i.CreationTime
                rdi.LastAccessDate = i.LastAccessTime
                rdi.Location = i.FullName.Replace(String.Format("{0}\", MemberDataAbsPath), String.Empty)
                rdi.ModifyDate = i.LastWriteTime
                rdi.Name = i.Name

                If i.EnumerateDirectories().Count() = 0 AndAlso i.EnumerateFiles().Count() = 0 Then
                    rdi.Deletable = True
                Else
                    rdi.Deletable = False
                End If

                rdi.Editable = True
                rdi.Contains = String.Format("{0} Folders, {1} Files", i.EnumerateDirectories().Count(), i.EnumerateFiles().Count())
                rdi.ThumbNail = String.Empty
                list.Add(rdi)
            Next
        Else
            Throw New DriveDoesNotExistException()
        End If

        Return list
    End Function

    Public Function CreateDriveFolder(ByVal dirPath As String, ByVal itype As DriveItemType) As Boolean
        If Not DriveExist Then
            Throw New DriveDoesNotExistException()
        End If

        Dim fi As DirectoryInfo = New DirectoryInfo(String.Format("{0}\{1}", MemberDataAbsPath, dirPath))
        fi.Create()
        Return fi.Exists
    End Function

    Public Function DeleteFile(ByVal folderPath As String) As Boolean
        If Not DriveExist Then
            Throw New DriveDoesNotExistException()
        End If

        Dim drivepath As String = Path.Combine(MemberDataAbsPath, folderPath)
        File.Delete(drivepath)
        Return True
    End Function

    Public Function DeleteFolder(ByVal folderPath As String) As Boolean
        If Not DriveExist Then
            Throw New DriveDoesNotExistException()
        End If

        Dim drivepath As String = Path.Combine(MemberDataAbsPath, folderPath)
        Directory.Delete(drivepath, True)
        Return True
    End Function

    Public Sub UploadFile(ByVal folderPath As String, ByVal pfile As HttpPostedFile)
        If Not DriveExist Then
            Throw New DriveDoesNotExistException()
        End If

        Dim fname As String = Path.GetFileName(pfile.FileName)
        Dim tempfilepath As String = Path.Combine(MemberDataAbsPath, folderPath, "temp", fname)
        Dim filepath As String = Path.Combine(MemberDataAbsPath, folderPath, fname)
        Dim fi As FileInfo = New FileInfo(filepath)

        If Not fi.Exists Then
            pfile.SaveAs(filepath)
        Else
            pfile.SaveAs(tempfilepath)
        End If
    End Sub
End Class

Public Class RDirectoryItem
    Public Property ID As Guid
    Public Property Name As String
    Public Property Location As String
    Public Property Contains As String
    Public Property Size As String
    Public Property CreateDate As DateTime
    Public Property ModifyDate As DateTime
    Public Property LastAccessDate As DateTime
    Public Property Deletable As Boolean
    Public Property Editable As Boolean
    Public Property ThumbNail As String
End Class

Public Class RFileItem
    Public Property ID As Guid
    Public Property Name As String
    Public Property Location As String
    Public Property Size As String
    Public Property FileType As String
    Public Property CreateDate As DateTime
    Public Property ModifyDate As DateTime
    Public Property LastAccessDate As DateTime
    Public Property Deletable As Boolean
    Public Property Editable As Boolean
    Public Property ThumbNail As String
    Public Property ItemType As DriveItemType
    Public Property WebPath As String
End Class

Public Enum DriveItemType
    Folder
    File
    TextFile
    ImageFile
    VideoFile
    ZipFile
End Enum

Public Class DriveItemDoesNotExistException
    Inherits Exception
End Class

Public Class DriveDoesNotExistException
    Inherits Exception
End Class

Public Class DuplicateDriveException
    Inherits Exception
End Class

Public Class InvalidDriveIdException
    Inherits Exception
End Class

