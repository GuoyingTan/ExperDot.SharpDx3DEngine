Imports SharpDX
Imports SharpDx3DLibrary

Public MustInherit Class RigidBodyBase

    Implements IRigidBody
    Implements IPhysicalUnit

    Public Property Children As New List(Of IRigidBody) Implements IRigidBody.Children
    Public Property Parent As IRigidBody Implements IRigidBody.Parent
    Public Overridable Property Location As New Vector3(0, 0, 0) Implements IRigidBody.Location
    Public Overridable Property Scale As New Vector3(1, 1, 1) Implements IRigidBody.Scale
    Public Overridable Property Qua As New Quaternion(0, 0, 0, 1) Implements IRigidBody.Qua
    Public Property Visible As Boolean = True Implements IRigidBody.Visible

    Public Overridable Property Acceleration As Vector3 Implements IPhysicalUnit.Acceleration
    Public Overridable Property Velocity As Vector3 Implements IPhysicalUnit.Velocity

    Public Overridable Sub Update() Implements IRigidBody.Update

    End Sub
    Public Sub Move() Implements IPhysicalUnit.Move
        Velocity += Acceleration
        Location += Velocity
        Acceleration = Vector3.Zero
        Velocity = Vector3.Zero
    End Sub
End Class
