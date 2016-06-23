Imports SharpDX
Imports SharpDx3DLibrary

Public Class Ground
    Inherits RigidBodyBase
    'Public rQua As Quaternion
    'Public Overrides Property Qua As Quaternion
    ''    Set(value As Quaternion)
    ''        If IsNewQua Then
    ''            IsNewQua = False
    ''            rQua = value
    ''            rQua.Invert()
    ''        End If
    ''        mQua = value
    ''    End Set
    ''    Get
    ''        Return rQua * mQua
    ''    End Get
    ''End Property
    'Private mQua As Quaternion
    'Private IsNewQua As Boolean = True

    Public Sub New()
        MyBase.New
        OnCreate()
    End Sub
    Private Sub OnCreate()
        Children.Clear()
        Dim dis As Single = 11
        Dim rk As Integer = 3
        Dim rp As Single = ((rk - 1) / 2) * dis
        Location = New Vector3(0, -rp, 0)
        For i = 0 To rk - 1
            For j = 0 To rk - 1
                For k = 0 To rk - 1
                    Dim pLoc = New Vector3(rp, rp, rp)
                    Dim rLoc = New Vector3(i * dis, j * dis, k * dis) - pLoc
                    Children.Add(New GroundBlock With {.Parent = Me, .Location = rLoc, .Scale = New Vector3(1, 1, 1)})
                Next
            Next
        Next
        Update()
    End Sub
    Public Sub UpdateQua(qua As Quaternion)
        Me.Qua = qua
        Me.Qua.Normalize()
    End Sub
    Public Sub UpdateLoc(Acce As Vector3)
        Dim tempAcce = (Matrix.Translation(Acce) * Matrix.RotationQuaternion(Me.Qua)).TranslationVector
        Me.Acceleration = tempAcce
        Me.Move()
        If Me.Location.Length > 800 Then
            Me.Location = Vector3.Zero
        End If
    End Sub
End Class
