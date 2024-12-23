Imports System.Drawing.Drawing2D
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Drawing.Text
Imports System.IO

Public Class CaptchaManager
    Private ReadOnly iHeight As Integer = 70
    Private ReadOnly iWidth As Integer = 210
    Private ReadOnly oRandom As Random = New Random()
    Private ReadOnly aBackgroundNoiseColor As Integer() = New Integer() {150, 150, 150}
    Private ReadOnly aTextColor As Integer() = New Integer() {0, 0, 0}
    Private ReadOnly aFontEmSizes As Integer() = New Integer() {27, 15, 23, 20, 30}
    Private ReadOnly aFontNames As String() = New String() {"Comic Sans MS", "Arial", "Times New Roman", "Georgia", "Verdana", "Geneva"}
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification:="<Pending>")>
    ReadOnly aFontStyles As FontStyle() = New FontStyle() {FontStyle.Bold, FontStyle.Italic, FontStyle.Regular, FontStyle.Strikeout, FontStyle.Underline}
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification:="<Pending>")>
    ReadOnly aHatchStyles As HatchStyle() = New HatchStyle() {HatchStyle.BackwardDiagonal, HatchStyle.Cross, HatchStyle.DashedDownwardDiagonal, HatchStyle.DashedHorizontal, HatchStyle.DashedUpwardDiagonal, HatchStyle.DashedVertical, HatchStyle.DiagonalBrick, HatchStyle.DiagonalCross, HatchStyle.Divot, HatchStyle.DottedDiamond, HatchStyle.DottedGrid, HatchStyle.ForwardDiagonal, HatchStyle.Horizontal, HatchStyle.HorizontalBrick, HatchStyle.LargeCheckerBoard, HatchStyle.LargeConfetti, HatchStyle.LargeGrid, HatchStyle.LightDownwardDiagonal, HatchStyle.LightHorizontal, HatchStyle.LightUpwardDiagonal, HatchStyle.LightVertical, HatchStyle.Max, HatchStyle.Min, HatchStyle.NarrowHorizontal, HatchStyle.NarrowVertical, HatchStyle.OutlinedDiamond, HatchStyle.Plaid, HatchStyle.Shingle, HatchStyle.SmallCheckerBoard, HatchStyle.SmallConfetti, HatchStyle.SmallGrid, HatchStyle.SolidDiamond, HatchStyle.Sphere, HatchStyle.Trellis, HatchStyle.Vertical, HatchStyle.Wave, HatchStyle.Weave, HatchStyle.WideDownwardDiagonal, HatchStyle.WideUpwardDiagonal, HatchStyle.ZigZag}
    Private sCaptchaText As String = ""

    Private ReadOnly _context As New indiabobblesEntities
    Public Property CaptchaImage As String = String.Empty

    Public Sub New(ByVal context As indiabobblesEntities)
        _context = context
    End Sub

    Public Function GenerateCaptcha() As Captcha
        Dim val As String = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8)
        Dim captcha As New Captcha() With {
            .Value = val,
            .CreateDate = DateTime.UtcNow,
            .Id = Guid.NewGuid()
        }
        sCaptchaText = val
        Draw()
        _context.Captchas.Add(captcha)
        _context.SaveChanges()
        Return captcha
    End Function

    Public Function IsValid(ByVal id As Guid, ByVal value As String) As Boolean
        Return _context.Captchas.Any(Function(t) t.Id = id AndAlso t.Value = value)
    End Function

    Private Sub Draw()
        Dim oOutputBitmap As Bitmap = New Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb)
        Dim oGraphics As Graphics = Graphics.FromImage(oOutputBitmap)
        oGraphics.TextRenderingHint = TextRenderingHint.AntiAlias
        Dim oRectangleF = New RectangleF(0, 0, iWidth, iHeight)
        Dim oBrush = New HatchBrush(aHatchStyles(oRandom.[Next](aHatchStyles.Length - 1)), Color.FromArgb((oRandom.[Next](100, 255)), (oRandom.[Next](100, 255)), (oRandom.[Next](100, 255))), Color.White)
        oGraphics.FillRectangle(oBrush, oRectangleF)
        Dim oMatrix As System.Drawing.Drawing2D.Matrix = New System.Drawing.Drawing2D.Matrix()
        Dim i As Integer = 0

        For i = 0 To sCaptchaText.Length - 1
            oMatrix.Reset()
            Dim iChars As Integer = sCaptchaText.Length
            Dim x As Integer = iWidth / (iChars + 1) * i
            Dim y As Integer = iHeight / 2
            oMatrix.RotateAt(oRandom.[Next](-20, 20), New PointF(x, y))
            oGraphics.Transform = oMatrix
            oGraphics.DrawString(sCaptchaText.Substring(i, 1), New Font(aFontNames(oRandom.[Next](aFontNames.Length - 1)), aFontEmSizes(oRandom.[Next](aFontEmSizes.Length - 1)), aFontStyles(oRandom.[Next](aFontStyles.Length - 1))), New SolidBrush(Color.FromArgb(oRandom.[Next](0, 100), oRandom.[Next](0, 100), oRandom.[Next](0, 100))), x, oRandom.[Next](0, iHeight / 2))
            oGraphics.ResetTransform()
        Next

        Dim oMemoryStream As MemoryStream = New MemoryStream()
        oOutputBitmap.Save(oMemoryStream, System.Drawing.Imaging.ImageFormat.Png)
        Dim oBytes As Byte() = oMemoryStream.GetBuffer()
        CaptchaImage = "data:image/png;base64," & Convert.ToBase64String(oBytes, 0, oBytes.Length)
        oOutputBitmap.Dispose()
        oMemoryStream.Close()
    End Sub

    Public Function [Get](ByVal id As Guid) As Captcha
        Return _context.Captchas.FirstOrDefault(Function(c) c.Id = id)
    End Function

    Public Sub RemoveOld(ByVal Optional hours As Integer = 1)
        Dim dt As DateTime = DateTime.UtcNow.AddHours(hours * -1)
        Dim query = _context.Captchas.Where(Function(c) c.CreateDate > dt)
        _context.Captchas.RemoveRange(query)
        _context.SaveChanges()
    End Sub
End Class
