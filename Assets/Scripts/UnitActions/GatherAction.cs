using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GatheringState
{
    ReachSource, GatherFromSource, FindNearestStorage, ReachNearestStorage, StoreResources, ReachLastSourcePosition
}

public class GatheringAction : UnitAction
{
    ResourceSource resourceSource;
    readonly IGathering gatheringUnit;
    ResourceType lastResourceSourceType;
    Vector3 lastResourceSourcePosition;
    Building nearestStorage;
    float gatheringTime;
    Dictionary<GatheringState, Action> functionPerState;

    int resourceSourceLayerMask;

    public GatheringAction(Unit unit, ResourceSource resourceSource) : base(unit)
    {
        gatheringUnit = unit as IGathering;
        gatheringUnit.GatheringState = GatheringState.ReachSource;
        this.resourceSource = resourceSource;
        lastResourceSourceType = resourceSource.resourceType;
        lastResourceSourcePosition = resourceSource.transform.position;
        functionPerState = new Dictionary<GatheringState, Action>
        {
            {GatheringState.ReachSource, NavReachSource},
            {GatheringState.GatherFromSource, GatherFromSource},
            {GatheringState.FindNearestStorage, FindNearestStorage},
            {GatheringState.ReachNearestStorage, NavReachNearestStorage},
            {GatheringState.StoreResources, StoreResources},
            {GatheringState.ReachLastSourcePosition, NavReachLastSourcePosition}
        };
        targetTransform = resourceSource.transform;
        unit.faction.workersPerResource[resourceSource.resourceType]++;
        resourceSourceLayerMask = LayerMask.GetMask("ResourceSource");
    }

    public override void Update()
    {
        if (gatheringUnit == null)
        {
            NavReachSource();
            return;
        }
        functionPerState[gatheringUnit.GatheringState]();
    }

    void NavReachSource()
    {
        if (!resourceSource)
        {
            gatheringUnit.GatheringState = GatheringState.ReachLastSourcePosition;
            return;
        }
        if (unit.richAI.destination != resourceSource.transform.position) unit.NavigateToTarget(resourceSource.transform.position, resourceSource.GetComponent<MeshFilter>().mesh.bounds.extents.x + unit.GetComponentInChildren<MeshFilter>().mesh.bounds.extents.x);
        targetTransform = resourceSource.gameObject.transform;
        if (unit.richAI.pathPending) return;
        if (!unit.richAI.reachedEndOfPath) return;
        gatheringUnit.GatheringState = GatheringState.GatherFromSource;
        targetTransform = null;
    }
    
    void NavReachLastSourcePosition()
    {
        if (unit.richAI.destination != lastResourceSourcePosition) unit.NavigateToTarget(lastResourceSourcePosition, 0.1f);
        if (unit.richAI.pathPending) return;
        if (!unit.richAI.reachedEndOfPath) return;
        resourceSource = FindNewSourceInRange();
        if (!resourceSource)
        {
            
            Deactivate();
            return;
        }
        gatheringUnit.GatheringState = GatheringState.ReachSource;
        targetTransform = resourceSource.transform;
        lastResourceSourcePosition = targetTransform.position;
        lastResourceSourceType = resourceSource.resourceType;
    }

    void GatherFromSource()
    {
        if (!resourceSource)
        {
            gatheringUnit.GatheringState = GatheringState.FindNearestStorage;
            gatheringTime = 0;
            return;
        }
        gatheringTime += Time.deltaTime;
        if (gatheringTime < resourceSource.timeToGather) return;
        Dictionary<ResourceType, int> gatheredResource = resourceSource.GetResource();
        int deltaToFullInventory = gatheringUnit.UsedInventorySpace - gatheringUnit.InventorySpace;
        if (deltaToFullInventory + gatheredResource.First().Value >= 0)
        {
            gatheringUnit.ResourcesInventory[gatheredResource.First().Key] += -deltaToFullInventory;
            gatheringUnit.UsedInventorySpace += -deltaToFullInventory;
            gatheringUnit.GatheringState = GatheringState.FindNearestStorage;
            gatheringTime = 0;
            return;
        }
        gatheringUnit.ResourcesInventory[gatheredResource.First().Key] += gatheredResource.First().Value;
        gatheringUnit.UsedInventorySpace += gatheredResource.First().Value;
        gatheringTime -= resourceSource.timeToGather;
    }

    void FindNearestStorage()
    {
        nearestStorage = BuildingManager.Instance.GetRequesterNearestBuilding(unit.faction.owner, lastResourceSourcePosition, BuildingType.Storage);
        targetTransform = nearestStorage.gameObject.transform;
        gatheringUnit.GatheringState = GatheringState.ReachNearestStorage;
    }

    void NavReachNearestStorage()
    {
        if (unit.richAI.destination != nearestStorage.transform.position) unit.NavigateToTarget(nearestStorage.transform.position, nearestStorage.GetComponent<MeshFilter>().mesh.bounds.extents.x * nearestStorage.transform.lossyScale.x * 1.35f + unit.GetComponentInChildren<MeshFilter>().mesh.bounds.extents.x);
        if (unit.richAI.pathPending) return;
        if (!unit.richAI.reachedEndOfPath) return;
        gatheringUnit.GatheringState = GatheringState.StoreResources;
        targetTransform = null;
    }

    void StoreResources()
    {
        GameManager.Instance.aliveFactionsByFactionType[unit.faction.owner].StoreResources(gatheringUnit.ResourcesInventory);
        gatheringUnit.ClearResourcesInventory();
        gatheringUnit.GatheringState = GatheringState.ReachSource;
    }

    ResourceSource FindNewSourceInRange()
    {
        Collider[] results = Physics.OverlapBox(unit.transform.position, new Vector3(10, 50, 10), Quaternion.identity,
            resourceSourceLayerMask);
        foreach (Collider result in results)
        {
            ResourceSource newResourceSource = result.GetComponent<ResourceSource>();
            if (newResourceSource.resourceType == lastResourceSourceType) return newResourceSource;
        }

        return null;
    }

    public override void Deactivate()
    {
        base.Deactivate();
        unit.faction.workersPerResource[lastResourceSourceType]--;
    }
}