using System.Collections.Generic;
using UnityEngine;

public class Villager : Unit, IGathering
{
    [Header("Building")]
    public float buildSpeedRate;
    [HideInInspector] public BuildingState buildingState;
    
    [Header("Gathering")]
    [SerializeField] int inventorySpace;
    
    public int InventorySpace { get => inventorySpace;
        set => inventorySpace = value; }
    public Dictionary<ResourceType, int> ResourcesInventory { get; set; } = new();
    public int UsedInventorySpace { get; set; }
    public GatheringState GatheringState { get; set; }

    protected override void Awake()
    {
        base.Awake();
        foreach (ResourceType resourceType in GameManager.Instance.resourceTypes)
        {
            ResourcesInventory.Add(resourceType, 0);
        }
    }

    protected override void Die()
    {
        base.Die();
        faction.idleVillagers.Remove(this);
        if (faction.localPlayerControlled)
            EventManager.Instance.onIdleVillagersCountChanged.Invoke();
    }

    public void ClearResourcesInventory()
    {
        foreach (ResourceType resourceType in GameManager.Instance.resourceTypes)
        {
            ResourcesInventory[resourceType] = 0;
        }
        UsedInventorySpace = 0;
    }
}
