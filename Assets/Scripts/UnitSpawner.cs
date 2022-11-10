using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    Queue<Unit> unitsToSpawnQueue = new();
    float currentProduceTime;
    Vector3 defaultReachPoint;
    Mesh mesh;
    Faction faction;

    void Awake()
    {
        mesh = GetComponentInChildren<MeshFilter>().mesh;
        defaultReachPoint = transform.position + transform.localScale + Vector3.right * 5;
    }

    void Start()
    {
        faction = GetComponent<RTSFactionEntity>().faction;
    }

    public void HandleUnitSpawning()
    {
        if (unitsToSpawnQueue.Count == 0)
        {
            currentProduceTime = 0;
            return;
        }

        currentProduceTime += Time.deltaTime;
        if (currentProduceTime < unitsToSpawnQueue.Peek().produceTime) return;
        Unit unitToSpawn = unitsToSpawnQueue.Dequeue();
        Unit spawnedUnit = Instantiate(unitToSpawn, transform.position + Vector3.right * mesh.bounds.size.x, Quaternion.identity, faction.unitsHolder);
        spawnedUnit.faction = faction;
        spawnedUnit.SetAction(new MoveToPointAction(spawnedUnit, defaultReachPoint));
        currentProduceTime -= spawnedUnit.produceTime;
    }

    public void AddUnitToQueue(Unit unit)
    {
        if (unit is Trader && faction.maxTradersAmount == faction.tradersAmount)
            return;

        faction.UseResources(unit.resourcesNeededToCreateMe);
        unitsToSpawnQueue.Enqueue(unit);
        faction.unitsSpaceOccupied += unit.housingSize;
        EventManager.Instance.onUnitsSpawningQueueChanged.Invoke();

        if (unit is Trader)
            faction.tradersAmount++;
    }
}
