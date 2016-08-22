Imports SharpDX
''' <summary>
''' 表示一个具有加速度、速度的力学单元
''' </summary>
Public Interface IPhysicalUnit
    ''' <summary>
    ''' 速度
    ''' </summary>
    ''' <returns></returns>
    Property Velocity As Vector3
    ''' <summary>
    ''' 加速度
    ''' </summary>
    ''' <returns></returns>
    Property Acceleration As Vector3
    ''' <summary>
    ''' 移动
    ''' </summary>
    Sub Move()
End Interface
