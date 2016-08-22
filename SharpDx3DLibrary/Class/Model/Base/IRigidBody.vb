Imports SharpDX
''' <summary>
''' 表示一个可包含若干子对象的刚体
''' </summary>
Public Interface IRigidBody
    ''' <summary>
    ''' 子物体
    ''' </summary>
    ''' <returns></returns>
    Property Children As List(Of IRigidBody)
    ''' <summary>
    ''' 父物体
    ''' </summary>
    ''' <returns></returns>
    Property Parent As IRigidBody
    ''' <summary>
    ''' 位置
    ''' </summary>
    ''' <returns></returns>
    Property Location As Vector3
    ''' <summary>
    ''' 缩放
    ''' </summary>
    ''' <returns></returns>
    Property Scale As Vector3
    ''' <summary>
    ''' 旋转
    ''' </summary>
    ''' <returns></returns>
    Property Qua As Quaternion
    ''' <summary>
    ''' 可见性
    ''' </summary>
    ''' <returns></returns>
    Property Visible As Boolean
    Sub Update()
End Interface
