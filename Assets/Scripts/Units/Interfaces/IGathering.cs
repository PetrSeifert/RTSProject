using System.Collections.Generic;


public interface IGathering
{
    int InventorySpace { get; set; } 
    public Dictionary<ResourceType, int> ResourcesInventory { get; set; }
    GatheringState GatheringState { get; set; }
    int UsedInventorySpace { get; set; }

    void ClearResourcesInventory();
}
