using UnityEngine;

public class MainHall : Building, IStoring
{
    [Header("Storage")] 
    [SerializeField] int storageSpace;

    [Header("House")] 
    [SerializeField] int housingSpace;
    
    UnitSpawner unitSpawner;

    public int StorageSpace { get; set; }

    protected override void Awake()
    {
        base.Awake();
        StorageSpace = storageSpace;
        AddStorageSpaceToFaction();
        AddHousingSpace();
        onBuiltDestroy.AddListener(Destroy);
        unitSpawner = GetComponent<UnitSpawner>();
        if (!faction.localPlayerControlled)
        {
            faction.GetComponent<EconomyAI>().mainHall = this;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (!built) return;
        unitSpawner.HandleUnitSpawning();
    }

    void OnDestroy()
    {
        faction.housingSpace -= housingSpace;
        onBuiltDestroy.RemoveListener(Destroy);
    }
    
    void AddHousingSpace()
    {
        faction.housingSpace += housingSpace;
        EventManager.Instance.onHousingSpaceChanged.Invoke();
    }

    public void AddStorageSpaceToFaction()
    {
        faction.storageSpace += StorageSpace;
    }

    public void RemoveStorageSpaceFromFaction()
    {
        faction.storageSpace -= StorageSpace;
    }

    void Destroy()
    {
        GameManager.Instance.DestroyFaction(faction);
    }
}
