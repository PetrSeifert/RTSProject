using System.Collections.Generic;
using UnityEngine;

public class CreateTradersAction : DecisionAction
{
    public CreateTradersAction(EconomyAI economyAI) : base(economyAI){}
    
    protected override void Action()
    {
        for (int i = 0; i < economyAI.faction.maxTradersAmount - economyAI.faction.tradersAmount; i++)
        {
            economyAI.mainHallUnitSpawner.AddUnitToQueue(Globals.Instance.unitPerType[UnitType.Trader]);
        }
    }

    protected override void Transition(){}
}

public class CreateVillagerAction : DecisionAction
{
    public CreateVillagerAction(EconomyAI economyAI) : base(economyAI) { }

    protected override void Action()
    {
        if (economyAI.faction.unitsSpace - economyAI.faction.unitsSpaceOccupied + economyAI.faction.tradersAmount - economyAI.faction.maxTradersAmount < 1) 
            return; //Todo: Create query for checking space        
        economyAI.mainHallUnitSpawner.AddUnitToQueue(Globals.Instance.unitPerType[UnitType.Villager]);
    }

    protected override void Transition() { }
}

public class BuyResourcesForMoneyAction : DecisionAction
{
    public BuyResourcesForMoneyAction(EconomyAI economyAI) : base(economyAI) { }

    protected override void Action()
    {
        foreach (ResourceType resourceTypeToBuy in economyAI.neededResources.Keys)
        {
            if (economyAI.neededResources[resourceTypeToBuy] == 0) continue;
            economyAI.faction.resourceStorage[resourceTypeToBuy] += economyAI.neededResources[resourceTypeToBuy];
            economyAI.faction.resourceStorage[ResourceType.Money] -= economyAI.neededResources[resourceTypeToBuy] * Globals.Instance.costPerResourceType[resourceTypeToBuy];
        }
    }

    protected override void Transition() { }
}

public class SendVillagersToGatherAction : DecisionAction
{
    public SendVillagersToGatherAction(EconomyAI economyAI) : base(economyAI) {}

    protected override void Action()
    {
        for (int i = 0; i < economyAI.faction.idleVillagers.Count; i++)
        {
            ResourceType mostNeededResourceType = ResourceType.Food;
            foreach (ResourceType resourceType in economyAI.neededResources.Keys)
            {
                if (resourceType == ResourceType.Money) continue;
                if (economyAI.faction.resourcesPerMinute[resourceType] / economyAI.neededResources[resourceType] <
                    economyAI.faction.resourcesPerMinute[mostNeededResourceType] / economyAI.neededResources[mostNeededResourceType])
                    mostNeededResourceType = resourceType;
            }
            if (economyAI.neededResources[mostNeededResourceType] == 0) return;

            ResourceSource nearestResourceSource = null;

            List<ResourceSource> neededResourceSources = new();

            if (mostNeededResourceType == ResourceType.Food)
                neededResourceSources = Globals.Instance.foodResourceSources;
            else if (mostNeededResourceType == ResourceType.Wood)
                neededResourceSources = Globals.Instance.woodResourceSources;
            else if (mostNeededResourceType == ResourceType.Stone)
                neededResourceSources = Globals.Instance.stoneResourceSources;

            foreach (ResourceSource neededResourceSource in neededResourceSources)
            {
                if (neededResourceSource.resourceType != mostNeededResourceType) continue;
                if (!nearestResourceSource) nearestResourceSource = neededResourceSource;
                else if (Vector3.SqrMagnitude(neededResourceSource.transform.position -
                                              economyAI.faction.idleVillagers[i].transform.position) <
                         Vector3.SqrMagnitude(nearestResourceSource.transform.position -
                                              economyAI.faction.idleVillagers[i].transform.position))
                {
                    nearestResourceSource = neededResourceSource;
                }
                
            }
            if (nearestResourceSource)
                economyAI.faction.idleVillagers[i].SetAction(new GatheringAction(economyAI.faction.idleVillagers[i], nearestResourceSource));
        }
    }

    protected override void Transition() { }
}