Imports SharpDX
''' <summary>
''' 表示一个三维世界
''' </summary>
Public Interface IWorld
    ''' <summary>
    ''' 模型顶点变换矩阵的数组
    ''' </summary>
    ''' <returns></returns>
    Property ModelMatrix As Matrix()
    ''' <summary>
    ''' 更新模型顶点变换矩阵
    ''' </summary>
    Sub Update()
End Interface
