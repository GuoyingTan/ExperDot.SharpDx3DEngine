Imports SharpDX
Imports SharpDx3DLibrary

Public Class WorldBase
    Implements IWorld
    Public Property RigidBodys As New List(Of IRigidBody)
    Public Property ModelMatrix As Matrix() Implements IWorld.ModelMatrix

    ''' <summary>
    ''' 更新当前世界所有物体的模型矩阵
    ''' </summary>
    Public Sub Update()
        Dim mList As New List(Of Matrix)
        CalcMatrix(RigidBodys, New Vector3(1, 1, 1), New Quaternion(0, 0, 0, 1), Vector3.Zero, mList)
        ModelMatrix = MList.ToArray
    End Sub
    ''' <summary>
    ''' 计算指定物体List（包括它的子物体）的顶点变换矩阵
    ''' </summary>
    Private Sub CalcMatrix(bodys As List(Of IRigidBody), scale As Vector3, qua As Quaternion, loc As Vector3, ByRef mList As List(Of Matrix))
        For Each SubBody In bodys
            If SubBody.Visible Then
                SubBody.Update()
                Dim s = MultiScale(SubBody.Scale, scale) '当前对象的比例
                Dim q = Quaternion.Normalize(qua * SubBody.Qua) '当前对象的旋转
                Dim p As Vector3
                If SubBody.Parent Is Nothing Then
                    p = loc + SubBody.Location '父对象为空，则位置为绝对位置
                Else
                    p = loc + (Matrix.Translation(SubBody.Location) * Matrix.RotationQuaternion(q)).TranslationVector '旋转变换后的位置
                End If
                Dim tempWorld As Matrix = Matrix.Scaling(MultiScale(s, New Vector3(1, 1, 1))) * Matrix.RotationQuaternion(q) * Matrix.Translation(p)
                mList.Add(tempWorld)
                CalcMatrix(SubBody.Children, s, q, p, mList)
            End If
        Next
    End Sub
    ''' <summary>
    ''' 相乘指定的两个缩放
    ''' </summary>
    Private Function MultiScale(v1, v2) As Vector3
        Return New Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z)
    End Function
End Class
