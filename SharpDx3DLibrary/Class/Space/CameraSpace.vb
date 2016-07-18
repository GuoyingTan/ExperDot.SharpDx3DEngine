Imports SharpDX
''' <summary>
''' 摄像机空间
''' </summary>
Public Class CameraSpace
    Inherits CameraBase
    Public Property Width As Integer = 600
    Public Property Height As Integer = 400
    Public Property World As WorldSpace
    Public Sub New()
        Me.Eye = New Vector3(0, 0, 100)
        Me.Target = New Vector3(0, 0, 0)
        Me.Up = New Vector3(0, 1, 0)
    End Sub
    Public Function GetTransforms() As List(Of GraphicsUtilities.Transform)
        Dim tempTransforms As New List(Of GraphicsUtilities.Transform)
        If World IsNot Nothing Then
            Dim aspect As Single = CSng(Width) / CSng(Height)
            Dim projection As Matrix = Matrix.PerspectiveFovLH(CSng(Math.PI) / 2.0F, aspect, 1, 10000)
            For Each SubMatrix In World.ModelMatrix
                tempTransforms.Add(New GraphicsUtilities.Transform() With {.WVP = SubMatrix * View * projection})
            Next
        End If
        Return tempTransforms
    End Function
End Class
