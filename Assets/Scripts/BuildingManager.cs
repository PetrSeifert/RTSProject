using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : Singleton<BuildingManager>
{
    Dictionary<FactionType, List<Building>> buildingsPerFaction = new();
    
    public Building GetRequesterNearestBuilding(FactionType requester, Vector3 position, BuildingType buildingType)
    {
        Building nearestBuilding = null;
        float sqrLowestDistance = Mathf.Infinity;
        foreach (Building building in buildingsPerFaction[requester])
        {
            if (!building.built) continue;
            if (!building.buildingTypeSet.Contains(buildingType)) continue;
            Vector3 offset = building.transform.position - position;
            float sqrDistance = Vector3.SqrMagnitude(offset);
            if (sqrDistance >= sqrLowestDistance) continue;
            sqrLowestDistance = sqrDistance;
            nearestBuilding = building;
        }

        return nearestBuilding;
    }
    
    public Building GetEnemiesNearestBuilding(FactionType requester, Transform requesterTransform, BuildingType buildingType)
    {
        Building nearestBuilding = null;
        float sqrLowestDistance = Mathf.Infinity;
        foreach (FactionType factionType in buildingsPerFaction.Keys)
        {
            if (factionType == requester || factionType == FactionType.Neutral) continue;
            foreach (Building building in buildingsPerFaction[factionType])
            {
                if (!building.buildingTypeSet.Contains(buildingType)) continue;
                Vector3 offset = building.transform.position - requesterTransform.position;
                float sqrDistance = Vector3.SqrMagnitude(offset);
                if (sqrDistance >= sqrLowestDistance) continue;
                sqrLowestDistance = sqrDistance;
                nearestBuilding = building;
            }
        }

        return nearestBuilding;
    }

    public void AddBuilding(FactionType owner, Building building)
    {
        if (buildingsPerFaction.ContainsKey(owner))
        {
            buildingsPerFaction[owner].Add(building);
        }
        else
        {
            buildingsPerFaction.Add(owner, new List<Building> {building});
        }
    }
}
