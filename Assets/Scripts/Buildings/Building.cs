using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum BuildingType
{
    Storage, Market
}

[Serializable] public class BuildingTypeHashSet : SerializableHashSet<BuildingType> {}

public class Building : RTSFactionEntity, IDamageable
{
    public BuildingTypeHashSet buildingTypeSet;
    public float buildTime;
    public bool built;
    [HideInInspector] public bool beingDestroyed;
    
    [Header("Events")]
    public UnityEvent onBuilt;
    public UnityEvent onBuiltDestroy;
    public UnityEvent onDestroy;
    
    KeyValuePair<Villager, int> buildersAmountPair;

    public int Health { get; set; }
    public bool IsDying { get; set; }

    protected override void Awake()
    {
        base.Awake();
        terrainLayerMask = LayerMask.GetMask("Terrain");
        BuildingManager.Instance.AddBuilding(faction.owner, this);
        onBuilt.AddListener(SetBuildingColor);
        if (!faction.localPlayerControlled && !faction.neutralFaction)
        {
            faction.GetComponent<EconomyAI>().buildings.Add(this);
        }
    }
    
    protected override void Start()
    {
        base.Start();
        SetBuildingColor();
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 200f,
            terrainLayerMask);
        transform.SetPositionAndRotation(new Vector3(hitInfo.point.x, hitInfo.point.y + meshFilter.mesh.bounds.size.y * 0.5f * transform.lossyScale.y, hitInfo.point.z), Quaternion.FromToRotation(transform.up, hitInfo.normal));
        AstarPath.active.UpdateGraphs(new Bounds(transform.position, meshFilter.mesh.bounds.size));
    }

    protected virtual void Update()
    {
        if (built || !buildersAmountPair.Key) return;
        HandleBuilding();
    }

    public void AddBuilder(Villager villager)
    {
        buildersAmountPair = !buildersAmountPair.Key ? new KeyValuePair<Villager, int>(villager, 1) : new KeyValuePair<Villager, int>(villager, buildersAmountPair.Value + 1);
    }

    void HandleBuilding()
    {
        buildTime -= buildersAmountPair.Key.buildSpeedRate * buildersAmountPair.Value * Time.deltaTime;
        buildersAmountPair = new KeyValuePair<Villager, int>(buildersAmountPair.Key, 0);
        if (buildTime > 0) return;
        onBuilt.Invoke();
        built = true;
        SetBuildingColor();
    }
    
    void SetBuildingColor()
    {
        GetComponent<Renderer>().material.color = built ? faction.color : new Color(1f, 0.62f, 0f);
    }

    public void StartDestroying()
    {
        if (built) onBuiltDestroy.Invoke();
        onDestroy.Invoke();
        beingDestroyed = true;
        //Todo:Play destroy animation and destroy
    }

    public virtual void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health > 0) return;
        Die();
    }

    void Die()
    {
        IsDying = true;
        Destroy(gameObject);
    }
}
