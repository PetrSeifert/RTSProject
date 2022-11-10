using UnityEngine.Events;

public class EventManager : PersistentSingleton<EventManager>
{
    public UnityEvent onResourcesAmountChanged;
    public UnityEvent<RTSFactionEntity[]> onRTSFactionEntitiesSelected;
    public UnityEvent<RTSFactionEntity[]> onRTSFactionEntitiesDeselected;
    public UnityEvent onSelectionCleared;
    public UnityEvent onTerrainGenerated;
    public UnityEvent onStorageBuilt;
    public UnityEvent onHousingSpaceChanged;
    public UnityEvent onUnitsSpawningQueueChanged;
    public UnityEvent onIdleVillagersCountChanged;

    protected override void Awake()
    {
        base.Awake();
        onResourcesAmountChanged = new UnityEvent();
        onStorageBuilt = new UnityEvent();
        onHousingSpaceChanged = new UnityEvent();
        onRTSFactionEntitiesSelected = new UnityEvent<RTSFactionEntity[]>();
        onRTSFactionEntitiesDeselected = new UnityEvent<RTSFactionEntity[]>();
        onSelectionCleared = new UnityEvent();
        onTerrainGenerated = new UnityEvent();
        onUnitsSpawningQueueChanged = new UnityEvent();
        onIdleVillagersCountChanged = new UnityEvent();
    }

    void OnDestroy()
    {
        onResourcesAmountChanged.RemoveAllListeners();
        onRTSFactionEntitiesDeselected.RemoveAllListeners();
        onRTSFactionEntitiesSelected.RemoveAllListeners();
        onSelectionCleared.RemoveAllListeners();
        onTerrainGenerated.RemoveAllListeners();
        onStorageBuilt.RemoveAllListeners();
        onHousingSpaceChanged.RemoveAllListeners();
        onUnitsSpawningQueueChanged.RemoveAllListeners();
        onIdleVillagersCountChanged.RemoveAllListeners();
    }
}
