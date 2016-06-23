Imports SharpDX
Public Class CameraSpace
    Inherits CameraBase
    Public Property Width As Integer = 600
    Public Property Height As Integer = 400
    Public Property World As WorldSpace
    Public Function GetTransforms() As List(Of CustomMath.Transform)
        Dim tempTransforms As New List(Of CustomMath.Transform)
        If World IsNot Nothing Then
            Dim aspect As Single = CSng(Width) / CSng(Height)
            Dim projection As Matrix = Matrix.PerspectiveFovLH(CSng(Math.PI) / 2.0F, aspect, 1, 10000)
            For Each SubMatrix In World.ModelMatrix
                tempTransforms.Add(New CustomMath.Transform() With {.WVP = SubMatrix * View * projection})
            Next
        End If
        Return tempTransforms
    End Function
End Class
