Imports SharpDX
Imports SharpDx3DLibrary
''' <summary>
''' 摄像机空间基类
''' </summary>
Public MustInherit Class CameraBase
    Implements ICamera
    Public Overridable Property Eye As Vector3 Implements ICamera.Eye
    Public Overridable Property Target As Vector3 Implements ICamera.Target
    Public Overridable Property Up As Vector3 Implements ICamera.Up
    Public Overridable ReadOnly Property View As Matrix Implements ICamera.View
        Get
            Return Matrix.LookAtLH(Eye, Target, Up)
        End Get
    End Property
End Class
