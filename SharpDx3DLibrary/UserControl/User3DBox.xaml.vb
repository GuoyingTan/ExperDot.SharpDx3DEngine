' The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
Imports SharpDX
Public NotInheritable Class User3DBox
    Inherits UserControl
    Public MyDepthBufferCube As DepthBufferCube
    Public Sub New()
        Me.InitializeComponent()
        Dim TargetRendering = Sub(sender As Object, e As Object)
                                  UpdateAndRender()
                              End Sub
        AddHandler Me.Loaded, Sub(o, e)
                                  MyDepthBufferCube = New DepthBufferCube()
                                  MyDepthBufferCube.Initialize(ScPanel)
                                  AddHandler CompositionTarget.Rendering, TargetRendering
                              End Sub
        AddHandler Me.Unloaded, Sub(o, e)
                                    RemoveHandler CompositionTarget.Rendering, TargetRendering
                                    If MyDepthBufferCube IsNot Nothing Then
                                        MyDepthBufferCube.Dispose()
                                        MyDepthBufferCube = Nothing
                                    End If
                                End Sub
    End Sub
    Private Sub UpdateAndRender()
        If MyDepthBufferCube IsNot Nothing Then
            MyDepthBufferCube.Update()
            MyDepthBufferCube.Render()
        End If
    End Sub

    Dim isMouseDown As Boolean
    Dim OldPoint As New Windows.Foundation.Point
    Private Sub User3DBox_PointerMoved(sender As Object, e As PointerRoutedEventArgs) Handles Me.PointerMoved
        If isMouseDown Then
            Dim x = (e.GetCurrentPoint(Me).Position.X - OldPoint.X) / 10
            Dim y = (e.GetCurrentPoint(Me).Position.Y - OldPoint.Y) / 10
            If x = 0 And y = 0 Then Return
            MyDepthBufferCube.Camera.EyeOfView += New Vector3(x, y, 0)
            MyDepthBufferCube.Camera.TargetOfView += New Vector3(x, y, 0)
            OldPoint = e.GetCurrentPoint(Me).Position
        End If
    End Sub

    Private Sub User3DBox_PointerPressed(sender As Object, e As PointerRoutedEventArgs) Handles Me.PointerPressed
        isMouseDown = True
        OldPoint = e.GetCurrentPoint(Me).Position
    End Sub

    Private Sub User3DBox_PointerReleased(sender As Object, e As PointerRoutedEventArgs) Handles Me.PointerReleased
        isMouseDown = False
    End Sub

    Private Sub User3DBox_PointerWheelChanged(sender As Object, e As PointerRoutedEventArgs) Handles Me.PointerWheelChanged
        Dim d As Double = e.GetCurrentPoint(Me).Properties.MouseWheelDelta / 10
        MyDepthBufferCube.Camera.EyeOfView += New Vector3(0, 0, d)
        MyDepthBufferCube.Camera.TargetOfView += New Vector3(0, 0, d)
    End Sub
End Class
