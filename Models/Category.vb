'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated from a template.
'
'     Manual changes to this file may cause unexpected behavior in your application.
'     Manual changes to this file will be overwritten if the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic

Partial Public Class Category
    Public Property ID As Integer
    Public Property Name As String
    Public Property UrlName As String
    Public Property Parent As Nullable(Of Integer)
    Public Property Status As Byte

    Public Overridable Property Category1 As ICollection(Of Category) = New HashSet(Of Category)
    Public Overridable Property Category2 As Category
    Public Overridable Property Posts As ICollection(Of Post) = New HashSet(Of Post)

End Class
