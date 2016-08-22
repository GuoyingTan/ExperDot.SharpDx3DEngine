Imports SharpDX
Public Class GraphicsUtilities
    ''' <summary>
    ''' 表示一个顶点
    ''' </summary>
    Public Structure Vertex
        Public Position As Vector3
        Public Color As Vector4
        Public Sub New(position As Vector3, color As Vector4)
            Me.Position = position
            Me.Color = color
        End Sub
    End Structure
    Public Structure Transform
        Public WVP As Matrix
        'need to reach 256 bits offset
        Public Unused1 As Matrix
        Public Unused2 As Matrix
        Public Unused3 As Matrix
    End Structure
    ''' <summary>
    ''' 返回一个指定长宽高的正六面体的顶点数组
    ''' </summary>
    Public Shared Function CreateCube(w As Single, h As Single, d As Single) As Vertex()
        w = w / 2
        h = h / 2
        d = d / 2
        Dim vertices As Vertex() = New Vertex() {
            New Vertex(New Vector3(-w, h, d), New Vector4(0, 1, 0, 1)), New Vertex(New Vector3(w, h, d), New Vector4(0, 1, 0, 1)),
            New Vertex(New Vector3(w, h, -d), New Vector4(0, 1, 0, 1)), New Vertex(New Vector3(-w, h, -d), New Vector4(0, 1, 0, 1)),
            New Vertex(New Vector3(-w, -h, d), New Vector4(1, 0, 1, 1)), New Vertex(New Vector3(w, -h, d), New Vector4(1, 0, 1, 1)),
            New Vertex(New Vector3(w, -h, -d), New Vector4(1, 0, 1, 1)), New Vertex(New Vector3(-w, -h, -d), New Vector4(1, 0, 1, 1)),
            New Vertex(New Vector3(-w, -h, d), New Vector4(1, 0, 0, 1)), New Vertex(New Vector3(-w, h, d), New Vector4(1, 0, 0, 1)),
            New Vertex(New Vector3(-w, h, -d), New Vector4(1, 0, 0, 1)), New Vertex(New Vector3(-w, -h, -d), New Vector4(1, 0, 0, 1)),
            New Vertex(New Vector3(w, -h, d), New Vector4(1, 1, 0, 1)), New Vertex(New Vector3(w, h, d), New Vector4(1, 1, 0, 1)),
            New Vertex(New Vector3(w, h, -d), New Vector4(1, 1, 0, 1)), New Vertex(New Vector3(w, -h, -d), New Vector4(1, 1, 0, 1)),
            New Vertex(New Vector3(-w, h, d), New Vector4(0, 1, 1, 1)), New Vertex(New Vector3(w, h, d), New Vector4(0, 1, 1, 1)),
            New Vertex(New Vector3(w, -h, d), New Vector4(0, 1, 1, 1)), New Vertex(New Vector3(-w, -h, d), New Vector4(0, 1, 1, 1)),
            New Vertex(New Vector3(-w, h, -d), New Vector4(0, 0, 1, 1)), New Vertex(New Vector3(w, h, -d), New Vector4(0, 0, 1, 1)),
            New Vertex(New Vector3(w, -h, -d), New Vector4(0, 0, 1, 1)), New Vertex(New Vector3(-w, -h, -d), New Vector4(0, 0, 1, 1))}
        Return vertices
    End Function
End Class
