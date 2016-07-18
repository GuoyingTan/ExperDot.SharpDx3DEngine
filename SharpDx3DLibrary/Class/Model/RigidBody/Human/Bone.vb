Imports SharpDX
''' <summary>
''' 表示骨骼结点
''' </summary>
Public Class Bone
    Inherits RigidBodyBase
    Public rQua As New Quaternion(0, 0, 0, 1)
    Public Overrides Property Qua As Quaternion
        Set(value As Quaternion)
            If IsNewQua Then
                IsNewQua = False
                rQua = value
                rQua.Invert()
                rQua.Normalize()
            End If
            mQua = value
        End Set
        Get
            'Dim rV As New Vector3(0, 0, 1)
            'Dim rS As Single = Math.PI / 2
            'Dim tQ As New Quaternion(rV * Math.Cos(rS / 2), Math.Sin(rS / 2))
            Return Quaternion.Normalize(rQua * mQua)
        End Get
    End Property
    Private mQua As New Quaternion(0, 0, 0, 1)
    Private IsNewQua As Boolean = True
    Public AbsoluteLoc As Vector3
    Public RaletiveLoc As Vector3
    Public ParentBone As Bone
    Public BoneQua As New Quaternion(0, 0, 0, 1)
    Public ChildrenBone As New List(Of Bone)
    Public Index As Integer
    Public Sub New(loc As Vector3, scale As Vector3)
        Me.RaletiveLoc = loc * 10
        Me.Scale = scale
    End Sub
End Class
