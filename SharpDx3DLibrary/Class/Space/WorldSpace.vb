Imports SharpDX
Public Class WorldSpace
    Public MyHuman As Human
    ''' <summary>
    ''' 当前世界所有的模型
    ''' </summary>
    Public RigidBodys As New List(Of IRigidBody)
    ''' <summary>
    ''' 当前世界所有物体的模型矩阵
    ''' </summary>
    Public ModelMatrix() As Matrix
    Private MList As New List(Of Matrix)
    Public Sub New()
        'RigidBodys.Add(New Human)
        'MyHuman = RigidBodys(0)
        'MyHuman.Visible = False
        For i = 0 To 0
            RigidBodys.Add(New Ground With {.Location = New Vector3((i) * 50, 0, 0)})
        Next
        Update()
    End Sub
    ''' <summary>
    ''' 更新当前世界所有物体的模型矩阵
    ''' </summary>
    Public Sub Update()
        MList.Clear()
        CalcMatrix(RigidBodys, New Vector3(1, 1, 1), New Quaternion(0, 0, 0, 1), Vector3.Zero)
        ModelMatrix = MList.ToArray
    End Sub

    Public Sub CalcMatrix(bodys As List(Of IRigidBody), scale As Vector3, qua As Quaternion, loc As Vector3)
        For Each SubBody In bodys
            If SubBody.Visible Then
                Dim s = MultiScale(SubBody.Scale, scale)
                Dim q = qua * SubBody.Qua
                Dim p As Vector3
                If SubBody.Parent Is Nothing Then
                    p = loc + SubBody.Location
                Else
                    p = loc + (Matrix.Translation(SubBody.Location) * Matrix.RotationQuaternion(q)).TranslationVector
                End If

                Dim tempWorld As Matrix = Matrix.Scaling(s) * Matrix.RotationQuaternion(q) * Matrix.Translation(p)
                MList.Add(tempWorld)
                CalcMatrix(SubBody.Children, s, q, p)
            End If
        Next
    End Sub
    Public Function MultiScale(v1, v2) As Vector3
        Return New Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z)
    End Function
End Class
