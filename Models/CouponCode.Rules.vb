Imports System.Globalization
Imports System.Linq
Imports System.Web

' ------------------------------------------------------------------------------------------
'  CouponCode – everything needed to offer, check and price a coupon.
'  This is the hand-written half of the EF partial class (the generated half has the columns).
'  Keep it in the same project/namespace as the generated CouponCode.vb.
'
'  Rules per coupon (set in Admin › Coupon Codes, or with SQL):
'    MinItems        – bobbleheads needed in the cart (0 = any)
'    MinAmount       – cart value needed (0 = any)
'    FirstOrderOnly  – only for someone with NO paid order yet (checked by phone OR email)
'    ReturningOnly   – only for someone WITH a paid order already (checked by phone OR email)
'    ShowOnCart      – offer it automatically on the cart page
'    Pitch           – short line shown with it, e.g. "on your first bobblehead"
'    ValidFrom/To    – optional dates (empty = no limit)
' ------------------------------------------------------------------------------------------
Partial Public Class CouponCode

    Private Shared ReadOnly Inr As New CultureInfo("en-IN")
    Private Const CacheKey As String = "ib-cart-offers"

    ' ======================== Per-coupon ========================

    ''' <summary>The code as customers type it: trimmed, in capitals.</summary>
    Public ReadOnly Property Code As String
        Get
            Return If(Name, "").Trim().ToUpperInvariant()
        End Get
    End Property

    ''' <summary>Active and within its dates.</summary>
    Public ReadOnly Property IsLive As Boolean
        Get
            Dim now As Date = DateTime.Now
            Return Status = 0 AndAlso
                   (Not ValidFrom.HasValue OrElse ValidFrom.Value <= now) AndAlso
                   (Not ValidTo.HasValue OrElse ValidTo.Value >= now)
        End Get
    End Property

    ''' <summary>Discount this coupon gives on a subtotal.</summary>
    Public Function DiscountOn(subtotal As Decimal) As Decimal
        If subtotal <= 0 Then Return 0D
        If IsPercent Then Return Math.Round(subtotal * Value / 100D, 0)
        Return Math.Min(Value, subtotal)
    End Function

    ''' <summary>"₹100 off" or "15% off"</summary>
    Public Function Label() As String
        If IsPercent Then Return Value.ToString("0.##", Inr) & "% off"
        Return "₹" & Value.ToString("#,##0.##", Inr) & " off"
    End Function

    ''' <summary>Does the cart meet the item / amount minimums?</summary>
    Public Function FitsCart(itemCount As Integer, subtotal As Decimal) As Boolean
        Return itemCount >= Math.Max(MinItems, 1) AndAlso subtotal >= MinAmount
    End Function

    ''' <summary>
    ''' Can this cart use the coupon? Returns Nothing if yes, otherwise a friendly message.
    ''' phone / email are the customer's details on the order (either may be empty).
    ''' </summary>
    Public Function CheckFor(itemCount As Integer, subtotal As Decimal, phone As String, Optional email As String = Nothing) As String
        If Status = 1 Then Return String.Format("{0} is paused at the moment.", Code)
        If Status <> 0 Then Return String.Format("Sorry, ""{0}"" isn't a valid code.", Code)
        Dim now As Date = DateTime.Now
        If ValidFrom.HasValue AndAlso ValidFrom.Value > now Then Return String.Format("{0} starts on {1}.", Code, ValidFrom.Value.ToString("d MMM yyyy", Inr))
        If ValidTo.HasValue AndAlso ValidTo.Value < now Then Return String.Format("{0} expired on {1}.", Code, ValidTo.Value.ToString("d MMM yyyy", Inr))

        If MinItems > 1 AndAlso itemCount < MinItems Then
            Dim more As Integer = MinItems - itemCount
            Return String.Format("{0} works on {1} or more bobbleheads. Add {2} more to use it.", Code, MinItems, more)
        End If

        If MinAmount > 0 AndAlso subtotal < MinAmount Then
            Return String.Format(Inr, "{0} works on orders of ₹{1:#,##0} or more. Add ₹{2:#,##0} more to use it.", Code, MinAmount, MinAmount - subtotal)
        End If

        If FirstOrderOnly OrElse ReturningOnly Then
            Dim bought As Boolean? = IsPastBuyer(phone, email)

            If FirstOrderOnly Then
                If Not bought.HasValue Then Return String.Format("Enter your WhatsApp number to use {0}.", Code)
                If bought.Value Then Return String.Format("{0} is only for first orders, and you've ordered with us before. Thank you for coming back!", Code)
            End If

            If ReturningOnly Then
                If Not bought.HasValue Then Return String.Format("Enter the WhatsApp number or email you ordered with before to use {0}.", Code)
                If Not bought.Value Then Return String.Format("{0} is a thank-you for our past customers. We couldn't find an earlier order with this number or email.", Code)
            End If
        End If

        Return Nothing
    End Function

    ' ======================== Shared helpers ========================

    ''' <summary>Finds a coupon by code (any case, ignores spaces around it). Deleted coupons are not returned.</summary>
    Public Shared Function Find(code As String) As CouponCode
        Dim c As String = If(code, "").Trim().ToUpper()
        If c = "" Then Return Nothing
        Try
            Using db As New indiabobblesEntities()
                Return db.CouponCodes.Where(Function(x) x.Status <> 2 AndAlso x.Name.Trim().ToUpper() = c) _
                                     .OrderBy(Function(x) x.Status).ThenByDescending(Function(x) x.Id) _
                                     .FirstOrDefault()
            End Using
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>Live coupons marked "Show on cart". Cached for 5 minutes.</summary>
    Public Shared Function CartOffers() As List(Of CouponCode)
        Dim cached = TryCast(HttpRuntime.Cache(CacheKey), List(Of CouponCode))
        If cached Is Nothing Then
            cached = New List(Of CouponCode)()
            Try
                Using db As New indiabobblesEntities()
                    cached = db.CouponCodes.Where(Function(x) x.Status = 0 AndAlso x.ShowOnCart).ToList()
                End Using
            Catch
                ' Never break the cart because of offers
            End Try
            'HttpRuntime.Cache.Insert(CacheKey, cached, Nothing, DateTime.UtcNow.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration)
        End If
        Return cached.Where(Function(x) x.IsLive).ToList()     ' dates are checked every time, not cached
    End Function

    ''' <summary>Call after saving a coupon in admin so the cart shows the change straight away.</summary>
    Public Shared Sub ClearCache()
        HttpRuntime.Cache.Remove(CacheKey)
    End Sub

    ''' <summary>
    ''' Best offer this cart can use right now (biggest saving).
    ''' First-order offers are hidden from known past buyers; returning-only offers are shown only to known past buyers.
    ''' </summary>
    Public Shared Function Best(offers As List(Of CouponCode), itemCount As Integer, subtotal As Decimal, Optional phone As String = Nothing, Optional email As String = Nothing) As CouponCode
        Dim fits = offers.Where(Function(o) o.FitsCart(itemCount, subtotal)).ToList()
        If fits.Count = 0 Then Return Nothing

        If fits.Any(Function(o) o.FirstOrderOnly OrElse o.ReturningOnly) Then
            Dim bought As Boolean? = IsPastBuyer(phone, email)
            Dim isBuyer As Boolean = bought.HasValue AndAlso bought.Value
            fits = fits.Where(Function(o) Not (o.FirstOrderOnly AndAlso isBuyer) AndAlso Not (o.ReturningOnly AndAlso Not isBuyer)).ToList()
        End If

        Return fits.OrderByDescending(Function(o) o.DiscountOn(subtotal)).FirstOrDefault()
    End Function

    ''' <summary>The next offer the customer can unlock by adding more bobbleheads (for the "add 1 more" nudge).</summary>
    Public Shared Function NextTier(offers As List(Of CouponCode), itemCount As Integer, subtotal As Decimal, Optional phone As String = Nothing, Optional email As String = Nothing) As CouponCode
        If itemCount <= 0 Then Return Nothing
        Dim current = Best(offers, itemCount, subtotal, phone, email)
        Dim currentSave As Decimal = If(current Is Nothing, 0D, current.DiscountOn(subtotal))
        Dim perItem As Decimal = subtotal / itemCount
        Return offers.Where(Function(o) o.MinItems > itemCount AndAlso Not o.FirstOrderOnly AndAlso Not o.ReturningOnly) _
                     .Where(Function(o) perItem * o.MinItems >= o.MinAmount AndAlso o.DiscountOn(perItem * o.MinItems) > currentSave) _
                     .OrderBy(Function(o) o.MinItems) _
                     .FirstOrDefault()
    End Function

    ''' <summary>
    ''' Has this customer bought before? Checked by phone (last 10 digits) OR email.
    ''' Returns Nothing when there is no usable phone or email to check.
    ''' </summary>
    Public Shared Function IsPastBuyer(phone As String, email As String) As Boolean?
        Dim digits As String = Last10Digits(phone)
        Dim mail As String = If(email, "").Trim().ToLowerInvariant()
        If mail <> "" AndAlso Not mail.Contains("@") Then mail = ""
        If digits Is Nothing AndAlso mail = "" Then Return Nothing

        ' Cache per request – the cart page may ask more than once
        Dim key As String = "ib-buyer|" & If(digits, "") & "|" & mail
        Dim ctx = HttpContext.Current
        If ctx IsNot Nothing AndAlso ctx.Items.Contains(key) Then Return CBool(ctx.Items(key))

        Dim result As Boolean = False
        Try
            Using db As New indiabobblesEntities()
                result = db.Database.SqlQuery(Of Integer)(
                    "SELECT COUNT(*) FROM [Order] WHERE Status IN (3,4,5,6) AND (" &
                    " (@p0 <> '' AND RIGHT(REPLACE(REPLACE(REPLACE(ISNULL(Phone,''),' ',''),'-',''),'+',''), 10) = @p0)" &
                    " OR (@p1 <> '' AND LOWER(LTRIM(RTRIM(ISNULL(Email,'')))) = @p1))",
                    If(digits, ""), mail).Single() > 0
            End Using
        Catch
            result = False     ' if the check fails, treat as a new customer
        End Try

        If ctx IsNot Nothing Then ctx.Items(key) = result
        Return result
    End Function

    ' ======================== Private ========================

    Private Shared Function Last10Digits(phone As String) As String
        Dim d As String = New String(If(phone, "").Where(Function(ch) Char.IsDigit(ch)).ToArray())
        If d.Length > 10 Then d = d.Substring(d.Length - 10)
        Return If(d.Length = 10, d, Nothing)
    End Function

End Class