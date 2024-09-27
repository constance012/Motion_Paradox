using UnityEngine;

public abstract class TransformConstraintBase : MonoBehaviour
{
	[Header("Source Object"), Space]
	[SerializeField] protected Transform sourceObject;

	[Header("Constrain Positions"), Space]
	[SerializeField] protected bool useConstraint;
	[SerializeField] protected Vector3 offset;
	[SerializeField] protected ConstrainScope scope;

	protected void LateUpdate()
	{
		if (useConstraint)
		{
			switch (scope)
			{
				case ConstrainScope.Global:
					ConstrainGlobal();
					break;
				case ConstrainScope.Local:
					ConstrainLocal();
					break;
			}
		}
	}
	
	protected abstract void ConstrainLocal();
	protected abstract void ConstrainGlobal();

	protected enum ConstrainScope
	{
		Global,
		Local
	}
}