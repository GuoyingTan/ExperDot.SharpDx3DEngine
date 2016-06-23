Imports SharpDX
Imports SharpDX.Direct3D12
Imports SharpDX.DXGI
Imports System.Threading
Imports Device = SharpDX.Direct3D12.Device
Imports Resource = SharpDX.Direct3D12.Resource
Public Class DepthBufferCube
    Implements IDisposable

    Private Const FrameCount As Integer = 3
    Private Const NumCubes As Integer = 1000

    Private viewport As ViewportF
    Private scissorRect As Rectangle
    ' Pipeline objects.
    Private swapChain As SwapChain3
    Private device As Device
    Private renderTargets As Resource() = New Resource(FrameCount - 1) {}
    Private depthTarget As Resource
    Private commandAllocator As CommandAllocator
    Private bundleAllocator As CommandAllocator
    Private commandQueue As CommandQueue
    Private rootSignature As RootSignature
    Private renderTargetViewHeap As DescriptorHeap
    Private depthStencilViewHeap As DescriptorHeap
    Private pipelineState As PipelineState
    Private commandList As GraphicsCommandList
    Private bundle As GraphicsCommandList
    Private rtvDescriptorSize As Integer

    ' App resources.
    Private vertexBuffer As Resource
    Private vertexBufferView As VertexBufferView
    Private indexBuffer As Resource
    Private indexBufferView As IndexBufferView
    'Constant Buffer
    Private constantBufferViewHeap As DescriptorHeap
    Private constantBuffer As Resource
    Private constantBufferDescriptorSize As Integer
    ' Synchronization objects.
    Private frameIndex As Integer
    Private fenceEvent As AutoResetEvent

    Private fence As Fence
    Private fenceValue As Integer
    '
    Private screenPanel As SwapChainPanel
    Private Const SwapBufferCount As Integer = 3
    Public MouseVec As New Vector3

    Private tempSingle As Single
    Public Camera As New CameraSpace
    ''' <summary>
    ''' 初始化
    ''' </summary>
    ''' <param name="scPanel">The form</param>
    Public Sub Initialize(scPanel As SwapChainPanel)
        screenPanel = scPanel
        LoadPipeline(scPanel)
        LoadAssets()
    End Sub
    Public Sub SetMouseVec(x As Single, y As Single, z As Single)
        MouseVec += New Vector3(x, y, z)
        If MouseVec.Length > 5 Then
            MouseVec = Vector3.Normalize(MouseVec) * 5
        End If
    End Sub
    ''' <summary>
    ''' 更新
    ''' </summary>
    Public Sub Update()

        Dim matrices() As CustomMath.Transform = Camera.GetTransforms.ToArray
        If matrices.Length > 0 Then
            Dim pointer As IntPtr = constantBuffer.Map(0)
            Utilities.Write(pointer, matrices, 0, matrices.Length)
            constantBuffer.Unmap(0)
        End If
    End Sub
    ''' <summary>
    ''' 渲染
    ''' </summary>
    Public Sub Render()
        ' Record all the commands we need to render the scene into the command list.
        PopulateCommandList()
        ' Execute the command list.
        commandQueue.ExecuteCommandList(commandList)
        ' Present the frame.
        swapChain.Present(1, 0)
        WaitForPreviousFrame()
    End Sub
    ''' <summary>
    ''' 释放对象
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Wait for the GPU to be done with all resources.
        WaitForPreviousFrame()
        'release all resources
        For Each target In renderTargets
            target.Dispose()
        Next
        commandAllocator.Dispose()
        bundleAllocator.Dispose()
        commandQueue.Dispose()
        rootSignature.Dispose()
        renderTargetViewHeap.Dispose()
        pipelineState.Dispose()
        commandList.Dispose()
        bundle.Dispose()
        vertexBuffer.Dispose()
        fence.Dispose()
        swapChain.Dispose()
        device.Dispose()
    End Sub
    ''' <summary>
    ''' 加载管线
    ''' </summary>
    ''' <param name="scPanel"></param>
    Private Sub LoadPipeline(scPanel As SwapChainPanel)
        Dim width As Integer = CInt(scPanel.ActualWidth)
        Dim height As Integer = CInt(scPanel.ActualHeight)

        viewport.Width = width
        viewport.Height = height
        viewport.MaxDepth = 1.0F

        scissorRect.Right = width
        scissorRect.Bottom = height

#If DEBUG Then
        ' Enable the D3D12 debug layer.
        If True Then
            DebugInterface.[Get]().EnableDebugLayer()
        End If
#End If

        Using factory = New Factory4()
            device = New Device(factory.GetWarpAdapter(), SharpDX.Direct3D.FeatureLevel.Level_11_0)
            ' Describe and create the command queue.
            Dim queueDesc As New CommandQueueDescription(CommandListType.Direct)
            commandQueue = device.CreateCommandQueue(queueDesc)
            Dim swapChainDescription1 = New SwapChainDescription1() With {
                .AlphaMode = AlphaMode.Unspecified,
                .BufferCount = SwapBufferCount,
                .Usage = Usage.RenderTargetOutput,
                .SwapEffect = SwapEffect.FlipSequential,
                .SampleDescription = New SampleDescription(1, 0),
                .Format = Format.R8G8B8A8_UNorm,
                .Width = width,
                .Height = height
            }
            Using sc1 = New SwapChain1(factory, commandQueue, swapChainDescription1)
                swapChain = sc1.QueryInterface(Of SwapChain3)()
                Using comPtr = New ComObject(scPanel)
                    Using native = comPtr.QueryInterface(Of ISwapChainPanelNative)()
                        native.SwapChain = swapChain
                    End Using
                End Using
            End Using
            frameIndex = swapChain.CurrentBackBufferIndex
        End Using

        ' Create descriptor heaps.
        ' Describe and create a render target view (RTV) descriptor heap.
        Dim rtvHeapDesc As New DescriptorHeapDescription() With {
            .DescriptorCount = FrameCount,
            .Flags = DescriptorHeapFlags.None,
            .Type = DescriptorHeapType.RenderTargetView
        }
        renderTargetViewHeap = device.CreateDescriptorHeap(rtvHeapDesc)
        rtvDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView)
        ' Create frame resources.
        Dim rtvHandle As CpuDescriptorHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart
        For n As Integer = 0 To FrameCount - 1
            renderTargets(n) = swapChain.GetBackBuffer(Of Resource)(n)
            device.CreateRenderTargetView(renderTargets(n), Nothing, rtvHandle)
            rtvHandle += rtvDescriptorSize
        Next
        'create depth buffer;
        Dim dsvHeapDesc As New DescriptorHeapDescription() With {
            .DescriptorCount = FrameCount,
            .Flags = DescriptorHeapFlags.None,
            .Type = DescriptorHeapType.DepthStencilView
        }
        depthStencilViewHeap = device.CreateDescriptorHeap(dsvHeapDesc)
        Dim dsvHandle As CpuDescriptorHandle = depthStencilViewHeap.CPUDescriptorHandleForHeapStart

        Dim depthOptimizedClearValue As New ClearValue() With {
            .Format = Format.D32_Float,
            .DepthStencil = New DepthStencilValue() With {
                .Depth = 1.0F,
                .Stencil = 0
            }
        }

        depthTarget = device.CreateCommittedResource(New HeapProperties(HeapType.[Default]), HeapFlags.None, New ResourceDescription(ResourceDimension.Texture2D, 0, width, height, 1, 0,
            Format.D32_Float, 1, 0, TextureLayout.Unknown, ResourceFlags.AllowDepthStencil), ResourceStates.DepthWrite, depthOptimizedClearValue)
        Dim depthView = New DepthStencilViewDescription() With {
            .Format = Format.D32_Float,
            .Dimension = DepthStencilViewDimension.Texture2D,
            .Flags = DepthStencilViewFlags.None
        }
        'bind depth buffer
        device.CreateDepthStencilView(depthTarget, Nothing, dsvHandle)
        commandAllocator = device.CreateCommandAllocator(CommandListType.Direct)
        bundleAllocator = device.CreateCommandAllocator(CommandListType.Bundle)
    End Sub
    ''' <summary>
    ''' 加载资源
    ''' </summary>
    Private Sub LoadAssets()
        Dim ranges As DescriptorRange() = New DescriptorRange() {New DescriptorRange() With {
            .RangeType = DescriptorRangeType.ConstantBufferView,
            .BaseShaderRegister = 0,
            .DescriptorCount = 1
        }}
        Dim parameter As New RootParameter(ShaderVisibility.Vertex, ranges)
        ' Create a root signature.
        Dim rootSignatureDesc As New RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout, New RootParameter() {parameter})
        rootSignature = device.CreateRootSignature(rootSignatureDesc.Serialize())

        ' Create the pipeline state, which includes compiling and loading shaders.

#If DEBUG Then
        Dim vertexShader = New ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile("shaders.hlsl", "VSMain", "vs_5_0", SharpDX.D3DCompiler.ShaderFlags.Debug))
#Else
			Dim vertexShader = New ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile("shaders.hlsl", "VSMain", "vs_5_0"))
#End If

#If DEBUG Then
        Dim pixelShader = New ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile("shaders.hlsl", "PSMain", "ps_5_0", SharpDX.D3DCompiler.ShaderFlags.Debug))
#Else
			Dim pixelShader = New ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile("shaders.hlsl", "PSMain", "ps_5_0"))
#End If

        ' Define the vertex input layout.
        Dim inputElementDescs As InputElement() = New InputElement() {New InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0), New InputElement("COLOR", 0, Format.R32G32B32A32_Float, 12, 0)}

        ' Describe and create the graphics pipeline state object (PSO).
        Dim psoDesc As New GraphicsPipelineStateDescription() With {
            .InputLayout = New InputLayoutDescription(inputElementDescs),
            .RootSignature = rootSignature,
            .VertexShader = vertexShader,
            .PixelShader = pixelShader,
            .RasterizerState = RasterizerStateDescription.[Default](),
            .BlendState = BlendStateDescription.[Default](),
            .DepthStencilFormat = SharpDX.DXGI.Format.D32_Float,
            .DepthStencilState = DepthStencilStateDescription.[Default](),
            .SampleMask = Integer.MaxValue,
            .PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
            .RenderTargetCount = 1,
            .Flags = PipelineStateFlags.None,
            .SampleDescription = New SharpDX.DXGI.SampleDescription(1, 0),
            .StreamOutput = New StreamOutputDescription()
        }
        psoDesc.RenderTargetFormats(0) = SharpDX.DXGI.Format.R8G8B8A8_UNorm

        pipelineState = device.CreateGraphicsPipelineState(psoDesc)
        ' Create the command list.
        commandList = device.CreateCommandList(CommandListType.Direct, commandAllocator, pipelineState)
        ' Command lists are created in the recording state, but there is nothing
        ' to record yet. The main loop expects it to be closed, so close it now.
        commandList.Close()


        ' Create the vertex buffer.
        Dim aspectRatio As Single = viewport.Width / viewport.Height
        Dim vertices As CustomMath.Vertex() = CustomMath.CreateCube(10, 10, 10)
        Dim vertexBufferSize As Integer = Utilities.SizeOf(vertices)
        ' Note: using upload heaps to transfer static data like vert buffers is not 
        ' recommended. Every time the GPU needs it, the upload heap will be marshalled 
        ' over. Please read up on Default Heap usage. An upload heap is used here for 
        ' code simplicity and because there are very few verts to actually transfer.
        vertexBuffer = device.CreateCommittedResource(New HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(vertexBufferSize), ResourceStates.GenericRead)

        ' Copy the triangle data to the vertex buffer.
        Dim pVertexDataBegin As IntPtr = vertexBuffer.Map(0)
        Utilities.Write(pVertexDataBegin, vertices, 0, vertices.Length)
        vertexBuffer.Unmap(0)

        ' Initialize the vertex buffer view.
        vertexBufferView = New VertexBufferView()
        vertexBufferView.BufferLocation = vertexBuffer.GPUVirtualAddress
        vertexBufferView.StrideInBytes = Utilities.SizeOf(Of CustomMath.Vertex)()
        vertexBufferView.SizeInBytes = vertexBufferSize


        'Create Index Buffer
        'Indices
        Dim indices As Integer() = New Integer() {0, 1, 2, 0, 2, 3,
            4, 6, 5, 4, 7, 6,
            8, 9, 10, 8, 10, 11,
            12, 14, 13, 12, 15, 14,
            16, 18, 17, 16, 19, 18,
            20, 21, 22, 20, 22, 23}
        Dim indexBufferSize As Integer = Utilities.SizeOf(indices)

        indexBuffer = device.CreateCommittedResource(New HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(indexBufferSize), ResourceStates.GenericRead)

        ' Copy the triangle data to the vertex buffer.
        Dim pIndexDataBegin As IntPtr = indexBuffer.Map(0)
        Utilities.Write(pIndexDataBegin, indices, 0, indices.Length)
        indexBuffer.Unmap(0)

        ' Initialize the index buffer view.
        indexBufferView = New IndexBufferView()
        indexBufferView.BufferLocation = indexBuffer.GPUVirtualAddress
        indexBufferView.Format = Format.R32_UInt
        indexBufferView.SizeInBytes = indexBufferSize

        'constant Buffer for each cubes
        constantBufferViewHeap = device.CreateDescriptorHeap(New DescriptorHeapDescription() With {
            .DescriptorCount = NumCubes,
            .Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
            .Flags = DescriptorHeapFlags.ShaderVisible
        })

        Dim constantBufferSize As Integer = (Utilities.SizeOf(Of CustomMath.Transform)() + 255) And Not 255
        constantBuffer = device.CreateCommittedResource(New HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(constantBufferSize * NumCubes), ResourceStates.GenericRead)
        constantBufferDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView)

        'First cube
        Dim cbvDesc As New ConstantBufferViewDescription() With {
            .BufferLocation = constantBuffer.GPUVirtualAddress,
            .SizeInBytes = constantBufferSize
        }

        Dim cbHandleHeapStart As CpuDescriptorHandle = constantBufferViewHeap.CPUDescriptorHandleForHeapStart

        For i As Integer = 0 To NumCubes - 1
            device.CreateConstantBufferView(cbvDesc, cbHandleHeapStart)
            cbvDesc.BufferLocation += Utilities.SizeOf(Of CustomMath.Transform)()
            cbHandleHeapStart += constantBufferDescriptorSize
        Next
        InitBundles()
    End Sub
    ''' <summary>
    ''' 加载Bundles
    ''' </summary>
    Private Sub InitBundles()
        ' Create and record the bundle.
        bundle = device.CreateCommandList(0, CommandListType.Bundle, bundleAllocator, pipelineState)
        bundle.SetGraphicsRootSignature(rootSignature)
        bundle.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList
        bundle.SetVertexBuffer(0, vertexBufferView)
        bundle.SetIndexBuffer(indexBufferView)
        bundle.SetDescriptorHeaps(1, New DescriptorHeap() {constantBufferViewHeap})
        'first cube
        Dim heapStart As GpuDescriptorHandle = constantBufferViewHeap.GPUDescriptorHandleForHeapStart
        For i As Integer = 0 To NumCubes - 1
            bundle.SetGraphicsRootDescriptorTable(0, heapStart)
            bundle.DrawIndexedInstanced(36, 1, 0, 0, 0)
            heapStart += constantBufferDescriptorSize
        Next
        bundle.Close()
        ' Create synchronization objects.
        If True Then
            fence = device.CreateFence(0, FenceFlags.None)
            fenceValue = 1
            ' Create an event handle to use for frame synchronization.
            fenceEvent = New AutoResetEvent(False)
        End If
    End Sub
    Private Sub PopulateCommandList()
        ' Command list allocators can only be reset when the associated 
        ' command lists have finished execution on the GPU; apps should use 
        ' fences to determine GPU execution progress.
        commandAllocator.Reset()
        ' However, when ExecuteCommandList() is called on a particular command 
        ' list, that command list can then be reset at any time and must be before 
        ' re-recording.
        commandList.Reset(commandAllocator, pipelineState)
        ' Set necessary state.
        commandList.SetGraphicsRootSignature(rootSignature)
        commandList.SetViewport(viewport)
        commandList.SetScissorRectangles(scissorRect)
        ' Indicate that the back buffer will be used as a render target.
        commandList.ResourceBarrierTransition(renderTargets(frameIndex), ResourceStates.Present, ResourceStates.RenderTarget)
        Dim rtvHandle As CpuDescriptorHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart
        rtvHandle += frameIndex * rtvDescriptorSize
        'pointer to depth stencil heap
        Dim dsvHandle As CpuDescriptorHandle = depthStencilViewHeap.CPUDescriptorHandleForHeapStart
        'set render target and depth stencil
        commandList.SetRenderTargets(1, rtvHandle, dsvHandle)
        ' Record commands.
        commandList.ClearRenderTargetView(rtvHandle, New Color4(0, 0.2F, 0.4F, 1), 0, Nothing)
        commandList.ClearDepthStencilView(dsvHandle, ClearFlags.FlagsDepth, 1, 0)
        commandList.SetDescriptorHeaps(1, New DescriptorHeap() {constantBufferViewHeap})
        commandList.ExecuteBundle(bundle)
        ' Indicate that the back buffer will now be used to present.
        commandList.ResourceBarrierTransition(renderTargets(frameIndex), ResourceStates.RenderTarget, ResourceStates.Present)
        commandList.Close()
    End Sub
    ''' <summary> 
    ''' 等待现有的CommandList执行完毕
    ''' </summary> 
    Private Sub WaitForPreviousFrame()
        ' WAITING FOR THE FRAME TO COMPLETE BEFORE CONTINUING IS NOT BEST PRACTICE. 
        ' This is code implemented as such for simplicity. 

        Dim currentFence As Integer = fenceValue
        commandQueue.Signal(fence, currentFence)
        fenceValue += 1

        ' Wait until the previous frame is finished.
        If fence.CompletedValue < currentFence Then
            fence.SetEventOnCompletion(currentFence, fenceEvent.GetSafeWaitHandle().DangerousGetHandle())
            fenceEvent.WaitOne()
        End If
        frameIndex = swapChain.CurrentBackBufferIndex
    End Sub

End Class

