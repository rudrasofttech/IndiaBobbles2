﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated from a template.
'
'     Manual changes to this file may cause unexpected behavior in your application.
'     Manual changes to this file will be overwritten if the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Data.Entity
Imports System.Data.Entity.Infrastructure

Partial Public Class indiabobblesEntities
    Inherits DbContext

    Public Sub New()
        MyBase.New("name=indiabobblesEntities")
    End Sub

    Protected Overrides Sub OnModelCreating(modelBuilder As DbModelBuilder)
        Throw New UnintentionalCodeFirstException()
    End Sub

    Public Overridable Property Categories() As DbSet(Of Category)
    Public Overridable Property CouponCodes() As DbSet(Of CouponCode)
    Public Overridable Property CustomDataSources() As DbSet(Of CustomDataSource)
    Public Overridable Property CustomPages() As DbSet(Of CustomPage)
    Public Overridable Property EmailMessages() As DbSet(Of EmailMessage)
    Public Overridable Property Members() As DbSet(Of Member)
    Public Overridable Property Orders() As DbSet(Of Order)
    Public Overridable Property OrderItems() As DbSet(Of OrderItem)
    Public Overridable Property PageComments() As DbSet(Of PageComment)
    Public Overridable Property Posts() As DbSet(Of Post)
    Public Overridable Property TopStories() As DbSet(Of TopStory)
    Public Overridable Property WebsiteSettings() As DbSet(Of WebsiteSetting)
    Public Overridable Property MemberStatus() As DbSet(Of MemberStatu)
    Public Overridable Property PostStatus1() As DbSet(Of PostStatus)
    Public Overridable Property UserInRoles() As DbSet(Of UserInRole)
    Public Overridable Property UserRoles() As DbSet(Of UserRole)
    Public Overridable Property CategoryTags() As DbSet(Of CategoryTag)
    Public Overridable Property Links() As DbSet(Of Link)
    Public Overridable Property Products() As DbSet(Of Product)
    Public Overridable Property ProductPhotoes() As DbSet(Of ProductPhoto)
    Public Overridable Property ProductTags() As DbSet(Of ProductTag)
    Public Overridable Property Captchas() As DbSet(Of Captcha)

End Class
