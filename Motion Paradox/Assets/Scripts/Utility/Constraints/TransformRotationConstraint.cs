using UnityEngine;

[AddComponentMenu("Transform Constraints/Transform Rotation Constraint", 1)]
public sealed class TransformRotationConstraint : TransformConstraintBase
{
	protected override void ConstrainGlobal()
	{
		transform.rotation = Quaternion.Euler(sourceObject.eulerAngles + offset);
	}

	protected override void ConstrainLocal()
	{
		transform.localRotation = Quaternion.Euler(sourceObject.localEulerAngles + offset);
	}
}