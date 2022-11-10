using System.Collections.Generic;
using UnityEngine;

public class Tower : Building, IAttacking
{
    public AttackingState AttackingState { get; set; }
    public float AttackRange { get; set; }
    public float ReloadTime { get; set; }
    public int Damage { get; set; }
    
    [SerializeField] float attackRange;
    [SerializeField] float reloadTime;
    [SerializeField] int damage;

    float timeFromLastAttack;

    int unitLayerMask;

    protected override void Awake()
    {
        base.Awake();
        AttackRange = attackRange;
        ReloadTime = reloadTime;
        Damage = damage;
        timeFromLastAttack = reloadTime;
        unitLayerMask = LayerMask.GetMask("Unit");
    }

    protected override void Update()
    {
        base.Update();
        if (!built) return;
        timeFromLastAttack += Time.deltaTime;
        IDamageable target = GetWeakestTarget();
        if (timeFromLastAttack < reloadTime) return;
        if (target == null || target.Equals(null)) return;
        if (target.IsDying) return;
        Attack(target);
        timeFromLastAttack = 0;
    }

    public void Attack(IDamageable target)
    {
        target.TakeDamage(Damage);
    }
    
    IDamageable GetWeakestTarget()
    {
        List<IDamageable> targets;
        targets = GetTargetsInRange();
        if (targets.Count == 0) return null;
        IDamageable weakestTarget = targets[0];
        
        for (int i = 1; i < targets.Count; i++)
            if (targets[i].Health < weakestTarget.Health) weakestTarget = targets[i];

        return weakestTarget;
    }

    List<IDamageable> GetTargetsInRange()
    {
        Collider[] results = Physics.OverlapSphere(transform.position, AttackRange, unitLayerMask);
        List<IDamageable> targets = new();
        foreach (Collider result in results)
        {
            Unit unitInRange = result.GetComponentInParent<Unit>();
            if (unitInRange.faction == faction) continue;
            targets.Add(unitInRange);
        }

        return targets;
    }
}