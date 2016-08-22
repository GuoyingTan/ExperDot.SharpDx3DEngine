Imports SharpDX
''' <summary>
''' 表示一个人类骨骼系统模型
''' </summary>
Public Class Human
    Inherits RigidBodyBase
    Public RootBone As Bone
    Dim BoneInfArr() As BoneInf = {
                                       New BoneInf(New Vector3(0, 0, 0), New Vector3(1, 1, 1), 0, New Integer() {1, 12, 16}),'腰部0
                                       New BoneInf(New Vector3(0, 5, 0), New Vector3(2.5, 5, 1), 0, New Integer() {2, 4, 8}),'胸部1
                                       New BoneInf(New Vector3(0, 1, 0), New Vector3(0.7, 1, 1), 1, New Integer() {3}),'颈部2
                                       New BoneInf(New Vector3(0, 1.5, 0), New Vector3(1.3, 1.5, 1), 2, New Integer() {}),'头部3
                                       New BoneInf(New Vector3(-2, 0, 0), New Vector3(2, 1, 1), 1, New Integer() {5}),'左肩4
                                       New BoneInf(New Vector3(0, -2.5, 0), New Vector3(1, 2.5, 1), 4, New Integer() {6}),'左上臂5
                                       New BoneInf(New Vector3(0, -2.5, 0), New Vector3(1, 2.5, 1), 5, New Integer() {7}),'左小臂6
                                       New BoneInf(New Vector3(0, -1, 0), New Vector3(1, 1, 1), 6, New Integer() {}),'左手7
                                       New BoneInf(New Vector3(2, 0, 0), New Vector3(2, 1, 1), 1, New Integer() {9}),'右肩8
                                       New BoneInf(New Vector3(0, -2.5, 0), New Vector3(1, 2.5, 1), 8, New Integer() {10}),'右上臂9
                                       New BoneInf(New Vector3(0, -2.5, 0), New Vector3(1, 2.5, 1), 9, New Integer() {11}),'右小臂10
                                       New BoneInf(New Vector3(0, -1, 0), New Vector3(1, 1, 1), 10, New Integer() {}),'右手11
                                       New BoneInf(New Vector3(-0.8, 0, 0), New Vector3(0.8, 1, 1), 0, New Integer() {13}),'左骻12
                                       New BoneInf(New Vector3(0, -4, 0), New Vector3(1, 4, 1), 12, New Integer() {14}),'左大腿13
                                       New BoneInf(New Vector3(0, -4, 0), New Vector3(1, 4, 1), 13, New Integer() {15}),'左小腿14
                                       New BoneInf(New Vector3(0, -1, 0), New Vector3(1, 1, 1), 14, New Integer() {}),'左脚15
                                       New BoneInf(New Vector3(0.8, 0, 0), New Vector3(0.8, 1, 1), 0, New Integer() {17}),'右骻16
                                       New BoneInf(New Vector3(0, -4, 0), New Vector3(1, 4, 1), 16, New Integer() {18}),'右大腿17
                                       New BoneInf(New Vector3(0, -4, 0), New Vector3(1, 4, 1), 17, New Integer() {19}),'右小腿18
                                       New BoneInf(New Vector3(0, -1, 0), New Vector3(1, 1, 1), 18, New Integer() {})'右脚19
                                       }
    Public Sub New()
        CreateBody()
        CalcBone(RootBone)
    End Sub
    ''' <summary>
    ''' 更新指定索引的骨骼
    ''' </summary>
    ''' <param name="qua">旋转</param>
    ''' <param name="index">骨骼索引</param>
    Public Sub UpdateBone(qua As Quaternion, index As Integer)
        qua.Normalize()
        DirectCast(Children(index), Bone).Qua = qua
        CalcBone(DirectCast(Children(index), Bone).ParentBone)
    End Sub
    ''' <summary>
    ''' 更新所有子骨骼
    ''' </summary>
    ''' <param name="parent"></param>
    Private Sub CalcBone(parent As Bone)
        For Each SubBone As Bone In parent.ChildrenBone
            SubBone.BoneQua = Quaternion.Normalize(Me.Qua * SubBone.Qua)
            Dim tempLoc = (Matrix.Translation(SubBone.RelativeLoc) * Matrix.RotationQuaternion(SubBone.BoneQua)).TranslationVector
            SubBone.AbsoluteLoc = parent.AbsoluteLoc + tempLoc
            SubBone.Location = parent.AbsoluteLoc + tempLoc / 2
            CalcBone(SubBone)
        Next
    End Sub
    ''' <summary>
    ''' 创建人物身体的所有骨骼
    ''' </summary>
    Private Sub CreateBody()
        For i = 0 To BoneInfArr.Count - 1
            Children.Add(New Bone(BoneInfArr(i).Loc, BoneInfArr(i).Scale))
            DirectCast(Children(i), Bone).Index = i
        Next
        For i = 0 To BoneInfArr.Count - 1
            DirectCast(Children(i), Bone).ParentBone = Children(BoneInfArr(i).ParentIndex)
            For Each SubIndex In BoneInfArr(i).ChildIndexArr
                DirectCast(Children(i), Bone).ChildrenBone.Add(Children(SubIndex))
            Next
        Next
        RootBone = DirectCast(Children(0), Bone)
        RootBone.Parent = RootBone
    End Sub
End Class
