Imports SharpDX
''' <summary>
''' 表示骨骼结点
''' </summary>
Public Class Bone
    Inherits RigidBodyBase
    Public Overrides Property Qua As Quaternion
        Set(value As Quaternion)
            If IsNewQua Then
                IsNewQua = False
                sQua = value
                sQua.Invert()
                sQua.Normalize()
            End If
            mQua = value
        End Set
        Get
            Return Quaternion.Normalize(sQua * mQua)
        End Get
    End Property
    ''' <summary>
    ''' 绝对坐标
    ''' </summary>
    Public AbsoluteLoc As Vector3
    ''' <summary>
    ''' 相对坐标
    ''' </summary>
    Public RelativeLoc As Vector3
    ''' <summary>
    ''' 父骨骼
    ''' </summary>
    Public ParentBone As Bone
    ''' <summary>
    ''' 骨骼相对旋转
    ''' </summary>
    Public BoneQua As New Quaternion(0, 0, 0, 1)
    ''' <summary>
    ''' 子骨骼
    ''' </summary>
    Public ChildrenBone As New List(Of Bone)
    ''' <summary>
    ''' 索引
    ''' </summary>
    Public Index As Integer
    Private mQua As New Quaternion(0, 0, 0, 1)
    Private sQua As New Quaternion(0, 0, 0, 1)
    Private IsNewQua As Boolean = True
    Public Sub New(loc As Vector3, scale As Vector3)
        Me.RelativeLoc = loc * 10
        Me.Scale = scale
    End Sub
End Class
