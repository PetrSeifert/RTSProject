using System;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingState
{
    ReachBuilding, BuildBuilding
}

public class BuildingAction : UnitAction
{
    Villager villager;
    Building building;
    Dictionary<BuildingState, Action> functionPerState;

    public BuildingAction(Unit unit, Building building) : base(unit)
    {
        villager = unit as Villager;
        this.building = building;
        functionPerState = new Dictionary<BuildingState, Action>
        {
            {BuildingState.ReachBuilding, NavReachBuilding},
            {BuildingState.BuildBuilding, BuildBuilding},
        };
        if (villager) villager.buildingState = BuildingState.ReachBuilding;
        targetTransform = building.gameObject.transform;
    }
    
    public override void Update()
    {
        if (!villager)
        {
            NavReachBuilding();
            return;
        }
        functionPerState[villager.buildingState]();
    }

    void NavReachBuilding()
    {
        if (unit.richAI.destination != building.transform.position) unit.NavigateToTarget(building.transform.position, building.GetComponent<MeshFilter>().mesh.bounds.extents.x * building.transform.lossyScale.x * 1.35f + unit.GetComponentInChildren<MeshFilter>().mesh.bounds.extents.x);
        if (unit.richAI.pathPending) return;
        if (!unit.richAI.reachedEndOfPath) return;
        if (!villager) return;
        villager.buildingState = BuildingState.BuildBuilding;
        targetTransform = null;
    }

    void BuildBuilding()
    {
        if (building.built)
        {
            Deactivate();
        }
        building.AddBuilder(villager);
    }
}
