Imports SharpDX
''' <summary>
''' 表示一个三维世界
''' </summary>
Public Interface IWorld
    ''' <summary>
    ''' 所有模型的顶点变换矩阵
    ''' </summary>
    ''' <returns></returns>
    Property ModelMatrix As Matrix()
End Interface
