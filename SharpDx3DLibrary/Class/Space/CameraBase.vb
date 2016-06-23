Imports SharpDX

Public MustInherit Class CameraBase
    Public Property EyeOfView As Vector3
        Get
            Return mEye
        End Get
        Set(value As Vector3)
            mEye = value
            ChangeView()
        End Set
    End Property
    Public Property TargetOfView As Vector3
        Get
            Return mTarget
        End Get
        Set(value As Vector3)
            mTarget = value
            ChangeView()
        End Set
    End Property
    Protected View As Matrix = Matrix.LookAtLH(New Vector3(0, 50, 200), New Vector3(), Vector3.UnitY)
    Private mEye As Vector3
    Private mTarget As Vector3
    Private Sub ChangeView()
        View = Matrix.LookAtLH(mEye, mTarget, Vector3.UnitY)
    End Sub
End Class
