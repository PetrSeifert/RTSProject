using System;
using System.Collections.Generic;
using UnityEngine;

public enum AttackingState
{
    ReachTarget, AttackTarget
}

public class AttackAction : UnitAction
{
    readonly IDamageable target;
    readonly IAttacking attackingUnit;
    Dictionary<AttackingState, Action> functionPerState;
    float timeFromLastAttack;

    public AttackAction(Unit unit, IDamageable target) : base(unit)
    {
        this.target = target;
        targetTransform = (target as Component).transform;
        attackingUnit = unit;
        attackingUnit.AttackingState = AttackingState.ReachTarget;
        functionPerState = new Dictionary<AttackingState, Action>
        {
            {AttackingState.ReachTarget, NavReachTarget},
            {AttackingState.AttackTarget, AttackTarget},
        };
    }
    
    public override void Update()
    {
        if (attackingUnit == null || attackingUnit.Equals(null))
        {
            NavReachTarget();
            return;
        }
        functionPerState[attackingUnit.AttackingState]();
    }

    void NavReachTarget()
    {
        if (target == null || target.Equals(null)) //Todo: Find new target
        {
            Deactivate();
            return;
        }
        unit.NavigateToTarget(targetTransform.position, unit.AttackRange);
        if (!IsTargetInRange()) return;
            
        timeFromLastAttack = attackingUnit.ReloadTime;
        attackingUnit.AttackingState = AttackingState.AttackTarget;
    }

    void AttackTarget()
    {
        if (target == null || target.Equals(null))//Todo: Find new target
        {
            Deactivate();
            return;
        }

        if (target.IsDying)
        {
            Deactivate();
            return;
        }

        if (!IsTargetInRange())
        {
            attackingUnit.AttackingState = AttackingState.ReachTarget;
            return;
        }
        
        timeFromLastAttack += Time.deltaTime;
        if (timeFromLastAttack < unit.ReloadTime) return;
        timeFromLastAttack = 0;
        unit.Attack(target);
    }

    bool IsTargetInRange()
    {
        Vector3 offset = targetTransform.position - unit.transform.position;
        return Vector3.SqrMagnitude(offset) - Vector3.SqrMagnitude(targetTransform.lossyScale) <= attackingUnit.AttackRange * attackingUnit.AttackRange;
    }
}
