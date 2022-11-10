using System.Collections.Generic;
using UnityEngine;

public class EconomyAI : MonoBehaviour
{
    public MainHall mainHall;
    public List<Building> buildings = new();
    public Dictionary<ResourceType, int> neededResources = new();
    
    [HideInInspector] public Faction faction;
    [HideInInspector] public UnitSpawner mainHallUnitSpawner;
    [HideInInspector] public bool cantCreateVillagers;
    [HideInInspector] public int resourceSourceLayerMask;

    void Awake()
    {
        resourceSourceLayerMask = LayerMask.GetMask("ResourceSource");
        neededResources = new Dictionary<ResourceType, int>
        {
            {ResourceType.Money, 0},
            {ResourceType.Food, 0},
            {ResourceType.Wood, 0},
            {ResourceType.Stone, 0}
        };
        
        faction = GetComponent<Faction>();
    }

    void Start()
    {
        mainHallUnitSpawner = mainHall.GetComponent<UnitSpawner>();
    }

    void Update()
    {
        cantCreateVillagers = false;
        new HasMaxTradersQuery(this);
    }
}
