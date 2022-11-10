using System;
using System.Collections.Generic;
using UnityEngine;

public enum TradingState
{
    ReachMarket, FindNearestStorage, ReachNearestStorage, StoreResources
}

public class TradingAction : UnitAction
{
    readonly Building market;
    readonly ITrading tradingUnit;
    Building nearestStorage;
    Dictionary<TradingState, Action> functionPerState;
    Vector3 lastPosition;

    public TradingAction(Unit unit, Building market) : base(unit)
    {
        tradingUnit = unit as ITrading;
        tradingUnit.TradingState = TradingState.ReachMarket;
        tradingUnit.SqrTraveledDistance = 0;
        this.market = market;
        lastPosition = unit.transform.position;
        functionPerState = new Dictionary<TradingState, Action>
        {
            {TradingState.ReachMarket, NavReachMarket},
            {TradingState.FindNearestStorage, FindNearestStorage},
            {TradingState.ReachNearestStorage, NavReachNearestStorage},
            {TradingState.StoreResources, StoreGolds}
        };
        targetTransform = market.gameObject.transform;
    }
    
    public override void Update()
    {
        functionPerState[tradingUnit.TradingState]();
    }

    void NavReachMarket()
    {
        if (unit.richAI.destination != market.transform.position) unit.NavigateToTarget(market.transform.position, market.GetComponent<MeshFilter>().mesh.bounds.extents.x * market.transform.lossyScale.x * 1.4f + unit.GetComponentInChildren<MeshFilter>().mesh.bounds.extents.x);
        targetTransform = market.gameObject.transform;
        if (tradingUnit == null) return;
        tradingUnit.SqrTraveledDistance += Vector3.SqrMagnitude(lastPosition - unit.transform.position);
        if (unit.richAI.pathPending) return;
        if (!(unit.richAI.reachedEndOfPath)) return;
        tradingUnit.CollectGolds();
        tradingUnit.TradingState = TradingState.FindNearestStorage;
        targetTransform = null;
    }

    void FindNearestStorage()
    {
        nearestStorage = BuildingManager.Instance.GetRequesterNearestBuilding(unit.faction.owner, unit.transform.position, BuildingType.Storage);
        targetTransform = nearestStorage.gameObject.transform;
        tradingUnit.TradingState = TradingState.ReachNearestStorage;
    }

    void NavReachNearestStorage()
    {
        if (unit.richAI.destination != nearestStorage.transform.position) unit.NavigateToTarget(nearestStorage.transform.position, nearestStorage.GetComponent<MeshFilter>().mesh.bounds.extents.x * nearestStorage.transform.lossyScale.x * 1.35f + unit.GetComponentInChildren<MeshFilter>().mesh.bounds.extents.x);
        if (unit.richAI.pathPending) return;
        if (!unit.richAI.reachedEndOfPath) return;
        tradingUnit.TradingState = TradingState.StoreResources;
        targetTransform = null;
    }

    void StoreGolds()
    {
        GameManager.Instance.aliveFactionsByFactionType[unit.faction.owner].StoreResources(new Dictionary<ResourceType, int>{
            {ResourceType.Money, tradingUnit.GoldAmount}});
        tradingUnit.SqrTraveledDistance = 0;
        tradingUnit.GoldAmount = 0;
        tradingUnit.TradingState = TradingState.ReachMarket;
    }
}
