Imports SharpDX
''' <summary>
''' 表示骨骼结点
''' </summary>
Public Class Bone
    Inherits RigidBodyBase
    Public Shadows Parent As Bone
    Public Shadows Children As List(Of Bone)
    Public AbsoluteLoc As Vector3
    Public RaletiveLoc As Vector3
    Public rQua As Quaternion
    Public Overrides Property Qua As Quaternion
        Set(value As Quaternion)
            If IsNewQua Then
                IsNewQua = False
                rQua = value
                rQua.Invert()
            End If
            mQua = value
        End Set
        Get
            Return rQua * mQua
        End Get
    End Property
    Private mQua As Quaternion
    Private IsNewQua As Boolean = True
    Public Sub New(loc As Vector3, scale As Vector3)
        Me.RaletiveLoc = loc * 10
        Me.Scale = scale
        Children = New List(Of Bone)
    End Sub
End Class
