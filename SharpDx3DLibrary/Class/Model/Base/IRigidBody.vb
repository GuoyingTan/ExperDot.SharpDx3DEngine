Imports SharpDX
Public Interface IRigidBody
    Property Children As List(Of IRigidBody)
    Property Parent As IRigidBody
    Property Location As Vector3
    Property Scale As Vector3
    Property Qua As Quaternion
    Property Visible As Boolean

    Sub Update()
End Interface
