using UnityEngine;

public enum ActionType
{
    Attack, Build, Gather, Move, Trading
}

public abstract class UnitAction
{
    public bool prioritySet;
    
    protected Unit unit;
    protected Transform targetTransform;

    protected UnitAction(Unit unit)
    {
        this.unit = unit;
        unit.action?.Deactivate();//If unit has other action active, deactivate it
        unit.faction.idleVillagers.Remove(unit);
        if (unit.faction.localPlayerControlled)
        {
            EventManager.Instance.onIdleVillagersCountChanged.Invoke();
        }
    }

    public virtual void Deactivate()
    {
        unit.action = null;
        if (unit is Villager)
        {
            unit.faction.idleVillagers.Add(unit);
            if (unit.faction.localPlayerControlled)
            {
                EventManager.Instance.onIdleVillagersCountChanged.Invoke();
            }
        }
    }
    
    /// <summary>
    /// Isn't MonoBehaviour Update().
    /// Should be called by unit.
    /// </summary>
    public abstract void Update();
}
