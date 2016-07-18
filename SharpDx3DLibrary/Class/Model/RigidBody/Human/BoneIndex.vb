Imports SharpDX
Public Class BoneIndex
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
