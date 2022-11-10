using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum FactionType
{
    Neutral, Green, Blue, Yellow, Red
}

[Serializable] public class FactionTypeHashSet : SerializableHashSet<FactionType> {}

public class Faction : MonoBehaviour
{
    public FactionType owner;
    public Color color;
    public SelectionController selectionController;
    public UnitController unitController;
    public BuildingPlacer buildingPlacer;
    public bool localPlayerControlled;
    public bool neutralFaction;
    public Transform unitsHolder;
    public Transform buildingsHolder;
    public int maxTradersAmount;
    public int maxUnitsSpace;

    [SerializeField] Building mainHallPrefab;
    [SerializeField] Unit villagerPrefab;
    [SerializeField] DictionaryAmountPerResource defaultResources; //Default resources that player receives when game starts

    [SerializeField] int defaultUnitsAmount;

    [HideInInspector] public List<Unit> idleVillagers = new();

    [HideInInspector] public int storageSpace; //Not implemented
    [HideInInspector] public int unitsSpace;
    [HideInInspector] public int housingSpace;
    [HideInInspector] public int unitsSpaceOccupied;
    [HideInInspector] public int tradersAmount;

    public Dictionary<ResourceType, int> resourceStorage = new();
    public Dictionary<ResourceType, float> resourcesPerMinute = new();
    public Dictionary<ResourceType, int> workersPerResource = new();

    float timeFromLastGoldEarning;

    int terrainLayerMask;
    int noTerrainLayerMask;

    void Awake()
    {
        terrainLayerMask = LayerMask.GetMask("Terrain");
        noTerrainLayerMask =~ LayerMask.GetMask("Terrain");
        if (localPlayerControlled)
        {
            UI_Manager.Instance.playerFaction = this;
            GameManager.Instance.mainCamera = Camera.main;
            GameManager.Instance.cameraTransform = Camera.main.transform;
        }

        resourcesPerMinute = new Dictionary<ResourceType, float>
        {
            {ResourceType.Money, 0},
            {ResourceType.Food, 0},
            {ResourceType.Wood, 0},
            {ResourceType.Stone, 0},
        };
        
        workersPerResource = new Dictionary<ResourceType, int>
        {
            {ResourceType.Money, 0},
            {ResourceType.Food, 0},
            {ResourceType.Wood, 0},
            {ResourceType.Stone, 0},
        };
        
        EventManager.Instance.onHousingSpaceChanged.AddListener(UpdateUnitsSpace);
    }

    void Start()
    {
        if (neutralFaction) return;
        GameManager.Instance.aliveFactionsByFactionType.Add(owner, this);
        SpawnMainHall();
        SpawnDefaultVillagers();
        SetDefaultResources();
        EventManager.Instance.onResourcesAmountChanged.Invoke();
    }

    void Update()
    {
        if (neutralFaction) return;
        EarnGoldEverySecond();
        CalculateGatheringSpeed();
    }

    void EarnGoldEverySecond()
    {
        timeFromLastGoldEarning += Time.deltaTime;
        if (timeFromLastGoldEarning < 1) return;
        resourceStorage[ResourceType.Money]++;
        EventManager.Instance.onResourcesAmountChanged.Invoke();
        timeFromLastGoldEarning--;
    }

    void SpawnMainHall()
    {
        Physics.Raycast(transform.position + Vector3.up * 50, Vector3.down, out RaycastHit hitInfo, 150, terrainLayerMask);
        mainHallPrefab.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y + mainHallPrefab.transform.localScale.y / 2, hitInfo.point.z);
        buildingPlacer.PlaceBuilding(mainHallPrefab.transform, mainHallPrefab);
    }

    void SpawnDefaultVillagers()
    {
        Vector3 unitSpawnPosition = transform.position + new Vector3(10, 50, 0);
        for (int i = 0; i < defaultUnitsAmount; i++)
        {
            bool validPlacement = false;
            while (!validPlacement)
            {
                unitSpawnPosition += new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
                Physics.Raycast(unitSpawnPosition, Vector3.down, out RaycastHit hitInfo, 200, noTerrainLayerMask);
                if (hitInfo.collider != null) continue;
                Physics.Raycast(unitSpawnPosition, Vector3.down, out hitInfo, 200, terrainLayerMask);
                if (Vector3.Angle(hitInfo.normal, Vector3.up) <= 26)
                    validPlacement = true;
            }
            if (localPlayerControlled)
                Debug.Log(unitsSpaceOccupied);
            unitsSpaceOccupied++;
            GameObject unitObject = Instantiate(villagerPrefab.gameObject, unitSpawnPosition, Quaternion.identity, unitsHolder);
            Unit spawnedUnit = unitObject.GetComponent<Unit>();
            spawnedUnit.faction = this;
            spawnedUnit.SetAction(new MoveToPointAction(spawnedUnit, unitSpawnPosition));
        }
    }

    void SetDefaultResources()
    {
        foreach (ResourceType resourceType in defaultResources.Keys.ToList())
        {
            if (defaultResources[resourceType] > storageSpace)
                defaultResources[resourceType] = storageSpace;
            resourceStorage.Add(resourceType, defaultResources[resourceType]);
        }
    }

    void UpdateUnitsSpace()
    {
        unitsSpace = housingSpace;
        
        if (unitsSpace > maxUnitsSpace)
            unitsSpace = maxUnitsSpace;
    }

    void CalculateGatheringSpeed()
    {
        foreach (ResourceType resourceType in workersPerResource.Keys)
        {
            resourcesPerMinute[resourceType] = workersPerResource[resourceType] * 60 / Globals.Instance.resourceSourcePerType[resourceType].timeToGather;
            if (resourceType == ResourceType.Money)
                resourcesPerMinute[resourceType] += 60;
        }
    }

    public void StoreResources(Dictionary<ResourceType, int> resources)
    {
        foreach (ResourceType resourceType in resources.Keys.ToList())
            resourceStorage[resourceType] += resources[resourceType];

        EventManager.Instance.onResourcesAmountChanged.Invoke();
    }

    public void UseResources(DictionaryAmountPerResource resources)
    {
        foreach (ResourceType resourceType in resources.Keys)
            resourceStorage[resourceType] -= resources[resourceType];

        EventManager.Instance.onResourcesAmountChanged.Invoke();
    }

    public bool HasEnoughResources(DictionaryAmountPerResource resources)
    {
        foreach (ResourceType resourceType in resources.Keys)
            if (resourceStorage[resourceType] < resources[resourceType]) return false;

        return true;
    }
}
