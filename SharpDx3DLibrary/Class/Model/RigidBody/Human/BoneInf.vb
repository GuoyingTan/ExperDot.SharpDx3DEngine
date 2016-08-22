Imports SharpDX
''' <summary>
''' 表示一个用于描述骨骼信息的对象
''' </summary>
Public Class BoneInf
    Public Loc As Vector3
    Public Scale As Vector3
    Public ParentIndex As Integer
    Public ChildIndexArr() As Integer
    Public Sub New(l As Vector3, s As Vector3, p As Integer, c As Integer())
        Loc = New Vector3(l.Z, l.Y, l.X)
        Scale = New Vector3(s.Z, s.Y, s.X)
        ParentIndex = p
        ChildIndexArr = c
    End Sub
End Class
