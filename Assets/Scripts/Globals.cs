using System.Collections.Generic;
using UnityEngine;

public class Globals : PersistentSingleton<Globals>
{
    [SerializeField] ResourceSource[] resourceSources;
    [SerializeField] Unit[] units;

    public Dictionary<ResourceType, ResourceSource> resourceSourcePerType = new();
    public Dictionary<UnitType, Unit> unitPerType = new(); 
    public Dictionary<ResourceType, int> costPerResourceType;

    [HideInInspector] public List<ResourceSource> foodResourceSources;
    [HideInInspector] public List<ResourceSource> woodResourceSources;
    [HideInInspector] public List<ResourceSource> stoneResourceSources;

    protected override void Awake()
    {
        base.Awake();
        foreach (ResourceSource resourceSource in resourceSources)
        {
            resourceSourcePerType.Add(resourceSource.resourceType, resourceSource);
        }

        foreach (Unit unit in units)
        {
            unitPerType.Add(unit.type, unit);
        }

        costPerResourceType = new()
        {
            { ResourceType.Food, 2},
            { ResourceType.Wood, 1 },
            { ResourceType.Stone, 3 }
        };
    }
}
