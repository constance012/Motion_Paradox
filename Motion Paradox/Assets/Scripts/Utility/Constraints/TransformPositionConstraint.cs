using UnityEngine;

[AddComponentMenu("Transform Constraints/Transform Position Constraint", 0)]
public sealed class TransformPositionConstraint : TransformConstraintBase
{
	protected override void ConstrainGlobal()
	{
		transform.position = sourceObject.position + offset;
	}

	protected override void ConstrainLocal()
	{
		transform.localPosition = sourceObject.localPosition + offset;
	}
}