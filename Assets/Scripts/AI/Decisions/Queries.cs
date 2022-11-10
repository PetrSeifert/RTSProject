using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasMaxTradersQuery : DecisionQuery
{
    public HasMaxTradersQuery(EconomyAI economyAI) : base(economyAI){}

    protected override void PositiveTransition(){}

    protected override void NegativeTransition()
    {
        new HasResourcesForTradersQuery(economyAI);
    }

    protected override bool Test() => economyAI.faction.maxTradersAmount == economyAI.faction.tradersAmount;
}

public class HasResourcesForTradersQuery : DecisionQuery
{
    public HasResourcesForTradersQuery(EconomyAI economyAI) : base(economyAI){}

    protected override void PositiveTransition()
    {
        new CreateTradersAction(economyAI);
    }

    protected override void NegativeTransition()
    {
        foreach (ResourceType resourceType in Globals.Instance.unitPerType[UnitType.Trader]
                     .resourcesNeededToCreateMe.Keys)
        {
            economyAI.neededResources[resourceType] = 0;
            if (Globals.Instance.unitPerType[UnitType.Trader].resourcesNeededToCreateMe[resourceType] *
                (economyAI.faction.maxTradersAmount - economyAI.faction.tradersAmount) >
                economyAI.faction.resourceStorage[resourceType])
                economyAI.neededResources[resourceType] = Globals.Instance.unitPerType[UnitType.Trader]
                                                              .resourcesNeededToCreateMe[resourceType] *
                                                          (economyAI.faction.maxTradersAmount -
                                                          economyAI.faction.tradersAmount) -
                                                          economyAI.faction.resourceStorage[resourceType];
        }

        new WillGatheringTakeOverHalfMinuteQuery(economyAI);
    }

    protected override bool Test()
    {
        foreach (ResourceType resourceType in Globals.Instance.unitPerType[UnitType.Trader]
                     .resourcesNeededToCreateMe.Keys)
        {
            if (Globals.Instance.unitPerType[UnitType.Trader].resourcesNeededToCreateMe[resourceType] *
                (economyAI.faction.maxTradersAmount - economyAI.faction.tradersAmount) <= economyAI.faction.resourceStorage[resourceType])
                continue;
            return false;
        }

        return true;
    }
}

public class HasResourcesForVillagersQuery : DecisionQuery
{
    public HasResourcesForVillagersQuery(EconomyAI economyAI) : base(economyAI) { }

    protected override void PositiveTransition()
    {
        new CreateVillagerAction(economyAI);
    }

    protected override void NegativeTransition()
    {
        if (economyAI.cantCreateVillagers)
            return;

        foreach (ResourceType resourceType in Globals.Instance.unitPerType[UnitType.Villager]
                     .resourcesNeededToCreateMe.Keys)
        {
            economyAI.neededResources[resourceType] = 0;
            if (Globals.Instance.unitPerType[UnitType.Villager].resourcesNeededToCreateMe[resourceType] >
                economyAI.faction.resourceStorage[resourceType])
                economyAI.neededResources[resourceType] = Globals.Instance.unitPerType[UnitType.Villager]
                                                            .resourcesNeededToCreateMe[resourceType] -
                                                          economyAI.faction.resourceStorage[resourceType];
        }
        economyAI.cantCreateVillagers = true;

        new WillGatheringTakeOverHalfMinuteQuery(economyAI);
    }

    protected override bool Test()
    {
        foreach (ResourceType resourceType in Globals.Instance.unitPerType[UnitType.Villager]
                     .resourcesNeededToCreateMe.Keys)
        {
            if (Globals.Instance.unitPerType[UnitType.Villager].resourcesNeededToCreateMe[resourceType] <= economyAI.faction.resourceStorage[resourceType])
                continue;
            return false;
        }

        return true;
    }
}

public class WillGatheringTakeOverHalfMinuteQuery : DecisionQuery
{
    public WillGatheringTakeOverHalfMinuteQuery(EconomyAI economyAI) : base(economyAI){}

    protected override void PositiveTransition()
    {
        new CanBuyResourcesForMoneyQuery(economyAI);
    }

    protected override void NegativeTransition(){}

    protected override bool Test()
    {
        foreach (ResourceType resourceType in economyAI.neededResources.Keys)
        {
            if (resourceType == ResourceType.Money) continue;
            if (economyAI.faction.resourcesPerMinute[resourceType] / 2 < economyAI.neededResources[resourceType])
                return true;
        }

        return false;
    }
}

public class HasIdleVillagersQuery : DecisionQuery
{
    public HasIdleVillagersQuery(EconomyAI economyAI) : base(economyAI){}

    protected override void PositiveTransition()
    {
        new SendVillagersToGatherAction(economyAI);
    }

    protected override void NegativeTransition()
    {
        new HasResourcesForVillagersQuery(economyAI);
    }

    protected override bool Test() => economyAI.faction.idleVillagers.Count != 0;
}

public class CanBuyResourcesForMoneyQuery : DecisionQuery
{
    public CanBuyResourcesForMoneyQuery(EconomyAI economyAI) : base(economyAI) { }

    protected override void PositiveTransition()
    {
        new BuyResourcesForMoneyAction(economyAI);
    }

    protected override void NegativeTransition()
    {
        new HasIdleVillagersQuery(economyAI);
    }

    protected override bool Test()
    {
        if (economyAI.neededResources[ResourceType.Money] != 0) return false;
        int currentMoney = economyAI.faction.resourceStorage[ResourceType.Money];
        foreach (ResourceType neededResourceType in economyAI.neededResources.Keys)
        {
            if (neededResourceType == ResourceType.Money) continue;
            if (currentMoney < economyAI.neededResources[neededResourceType] * Globals.Instance.costPerResourceType[neededResourceType]) return false;
            currentMoney -= economyAI.neededResources[neededResourceType] * Globals.Instance.costPerResourceType[neededResourceType];
        }
        return true;
    }
}



