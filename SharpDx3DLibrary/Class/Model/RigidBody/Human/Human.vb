Imports SharpDX
Public Class Human
    Inherits RigidBodyBase
    Public RootBone As Bone
    Public Sub New()
        CreateBone()
        CalcBone(RootBone)
    End Sub
    Public Sub UpdateBone(qua As Quaternion, index As Integer)
        qua.Normalize()
        DirectCast(Children(index), Bone).Qua = qua
        CalcBone(DirectCast(Children(index), Bone).ParentBone)
    End Sub
    Private Sub CalcBone(parent As Bone)
        For Each SubBone As Bone In parent.ChildrenBone
            SubBone.BoneQua = Quaternion.Normalize(Me.Qua * SubBone.Qua)
            Dim tempLoc = (Matrix.Translation(SubBone.RaletiveLoc) * Matrix.RotationQuaternion(SubBone.BoneQua)).TranslationVector
            SubBone.AbsoluteLoc = parent.AbsoluteLoc + tempLoc
            SubBone.Location = parent.AbsoluteLoc + tempLoc / 2
            CalcBone(SubBone)
        Next
    End Sub
    Dim BoneIndexArr() As BoneIndex = {
                                       New BoneIndex(New Vector3(0, 0, 0), New Vector3(1, 1, 1), 0, New Integer() {1, 12, 16}),'腰部0
                                       New BoneIndex(New Vector3(0, 5, 0), New Vector3(2.5, 5, 1), 0, New Integer() {2, 4, 8}),'胸部1
                                       New BoneIndex(New Vector3(0, 1, 0), New Vector3(0.7, 1, 1), 1, New Integer() {3}),'颈部2
                                       New BoneIndex(New Vector3(0, 1.5, 0), New Vector3(1.3, 1.5, 1), 2, New Integer() {}),'头部3
                                       New BoneIndex(New Vector3(-2, 0, 0), New Vector3(2, 1, 1), 1, New Integer() {5}),'左肩4
                                       New BoneIndex(New Vector3(0, -2.5, 0), New Vector3(1, 2.5, 1), 4, New Integer() {6}),'左上臂5
                                       New BoneIndex(New Vector3(0, -2.5, 0), New Vector3(1, 2.5, 1), 5, New Integer() {7}),'左小臂6
                                       New BoneIndex(New Vector3(0, -1, 0), New Vector3(1, 1, 1), 6, New Integer() {}),'左手7
                                       New BoneIndex(New Vector3(2, 0, 0), New Vector3(2, 1, 1), 1, New Integer() {9}),'右肩8
                                       New BoneIndex(New Vector3(0, -2.5, 0), New Vector3(1, 2.5, 1), 8, New Integer() {10}),'右上臂9
                                       New BoneIndex(New Vector3(0, -2.5, 0), New Vector3(1, 2.5, 1), 9, New Integer() {11}),'右小臂10
                                       New BoneIndex(New Vector3(0, -1, 0), New Vector3(1, 1, 1), 10, New Integer() {}),'右手11
                                       New BoneIndex(New Vector3(-0.8, 0, 0), New Vector3(0.8, 1, 1), 0, New Integer() {13}),'左骻12
                                       New BoneIndex(New Vector3(0, -4, 0), New Vector3(1, 4, 1), 12, New Integer() {14}),'左大腿13
                                       New BoneIndex(New Vector3(0, -4, 0), New Vector3(1, 4, 1), 13, New Integer() {15}),'左小腿14
                                       New BoneIndex(New Vector3(0, -1, 0), New Vector3(1, 1, 1), 14, New Integer() {}),'左脚15
                                       New BoneIndex(New Vector3(0.8, 0, 0), New Vector3(0.8, 1, 1), 0, New Integer() {17}),'右骻16
                                       New BoneIndex(New Vector3(0, -4, 0), New Vector3(1, 4, 1), 16, New Integer() {18}),'右大腿17
                                       New BoneIndex(New Vector3(0, -4, 0), New Vector3(1, 4, 1), 17, New Integer() {19}),'右小腿18
                                       New BoneIndex(New Vector3(0, -1, 0), New Vector3(1, 1, 1), 18, New Integer() {})'右脚19
                                       }
    Private Sub CreateBone()
        For i = 0 To BoneIndexArr.Count - 1
            Children.Add(New Bone(BoneIndexArr(i).Loc, BoneIndexArr(i).Scale))
            DirectCast(Children(i), Bone).Index = i
        Next
        For i = 0 To BoneIndexArr.Count - 1
            DirectCast(Children(i), Bone).ParentBone = Children(BoneIndexArr(i).ParentIndex)
            For Each SubIndex In BoneIndexArr(i).ChildIndexArr
                DirectCast(Children(i), Bone).ChildrenBone.Add(Children(SubIndex))
            Next
        Next
        RootBone = DirectCast(Children(0), Bone)
        RootBone.Parent = RootBone
    End Sub
End Class
