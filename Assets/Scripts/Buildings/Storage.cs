using UnityEngine;

public class Storage : Building, IStoring
{
    [Header("Storage")]
    [SerializeField] int storageSpace;

    public int StorageSpace { get; set; }

    protected override void Awake()
    {
        base.Awake();
        StorageSpace = storageSpace;
        onBuilt.AddListener(AddStorageSpaceToFaction);
        onBuiltDestroy.AddListener(RemoveStorageSpaceFromFaction);
    }

    void OnDestroy()
    {
        onBuilt.RemoveListener(AddStorageSpaceToFaction);
        onBuiltDestroy.RemoveListener(RemoveStorageSpaceFromFaction);
    }

    public void AddStorageSpaceToFaction()
    {
        faction.storageSpace += StorageSpace;
    }

    public void RemoveStorageSpaceFromFaction()
    {
        faction.storageSpace -= StorageSpace;
    }
}