Imports SharpDX
''' <summary>
''' 表示用于视变换的摄像机
''' </summary>
Public Interface ICamera
    ''' <summary>
    ''' 获取或设置摄像机位置
    ''' </summary>
    ''' <returns></returns>
    Property Eye As Vector3
    ''' <summary>
    ''' 获取或设置目标视点位置
    ''' </summary>
    ''' <returns></returns>
    Property Target As Vector3
    ''' <summary>
    ''' 获取或设置摄像机向上方向
    ''' </summary>
    ''' <returns></returns>
    Property Up As Vector3
    ''' <summary>
    ''' 获取当前视变换矩阵
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property View As Matrix
End Interface
