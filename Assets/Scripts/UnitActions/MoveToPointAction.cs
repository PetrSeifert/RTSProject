using UnityEngine;

public class MoveToPointAction : UnitAction
{
    Vector3 point;

    public MoveToPointAction(Unit unit,  Vector3 point) : base(unit)
    {
        this.point = point;
    }

    public override void Update()
    {
        if (unit.richAI.destination != point)
        {
            unit.NavigateToTarget(point, 0.25f);
        }
        else
        {
            if (!unit.richAI.reachedEndOfPath || unit.richAI.pathPending) return;
            Deactivate();
        }
    }
}
