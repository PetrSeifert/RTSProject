using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] Faction faction;
    
    Dictionary<UnitType, List<Unit>> waitingUnitsByUnitType = new();

    int terrainLayerMask;
    int unitLayerMask;
    int buildingLayerMask;
    int resourceSourceLayerMask;

    void Awake()
    {
        terrainLayerMask = LayerMask.GetMask("Terrain");
        unitLayerMask = LayerMask.GetMask("Unit");
        buildingLayerMask = LayerMask.GetMask("Building");
        resourceSourceLayerMask = LayerMask.GetMask("ResourceSource");
    }

    void Update()
    {
        if (faction.buildingPlacer.placingBuilding) return;
        
        if (InputManager.Instance.secondaryHeld) DecideAndSetUnitActions();
    }
    
    void DecideAndSetUnitActions()
    {
        foreach (UnitType unitType in faction.selectionController.selectedUnitsByUnitType.Keys)
            waitingUnitsByUnitType[unitType] = new List<Unit>(faction.selectionController.selectedUnitsByUnitType[unitType]); //Used list to deep copy units from dictionary

        Ray mouseRay = GameManager.Instance.mainCamera.ScreenPointToRay(InputManager.Instance.mousePosition);
        
        bool rayHitUnit = Physics.Raycast(mouseRay, out RaycastHit hitInfoUnit, 210, unitLayerMask);
        bool rayHitBuilding = Physics.Raycast(mouseRay, out RaycastHit hitInfoBuilding, 210, buildingLayerMask);
        bool rayHitTerrain = Physics.Raycast(mouseRay, out RaycastHit hitInfoTerrain, 210, terrainLayerMask);
        bool rayHitResourceSource = Physics.Raycast(mouseRay, out RaycastHit hitInfoResourceSource, 210, resourceSourceLayerMask);
        
        foreach (UnitType unitType in waitingUnitsByUnitType.Keys)
        {
            Building hitBuilding = rayHitBuilding ? hitInfoBuilding.collider.GetComponent<Building>() : null;
            foreach (Unit unit in waitingUnitsByUnitType[unitType].ToList())
            {
                if (rayHitUnit && hitInfoUnit.collider.GetComponentInParent<Unit>().faction != faction)
                {
                    unit.SetAction(new AttackAction(unit, hitInfoUnit.collider.GetComponentInParent<IDamageable>()));
                    waitingUnitsByUnitType[unitType].Remove(unit);
                }
                else if (rayHitBuilding && hitBuilding.buildingTypeSet.Contains(BuildingType.Market) && unit is ITrading)
                {
                    unit.SetAction(new TradingAction(unit, hitBuilding));
                    waitingUnitsByUnitType[unitType].Remove(unit);
                }
                else if (rayHitBuilding)
                {
                    if (hitBuilding.faction == faction)
                        unit.SetAction(new BuildingAction(unit, hitBuilding));
                    else
                        unit.SetAction(new AttackAction(unit, hitBuilding as IDamageable));

                    waitingUnitsByUnitType[unitType].Remove(unit);
                }
                else if (rayHitResourceSource && unit is IGathering)
                {
                    unit.SetAction(new GatheringAction(unit, hitInfoResourceSource.collider.GetComponent<ResourceSource>()));
                    waitingUnitsByUnitType[unitType].Remove(unit);
                }
                else if (rayHitTerrain)
                {
                    unit.SetAction(new MoveToPointAction(unit, hitInfoTerrain.point));
                    waitingUnitsByUnitType[unitType].Remove(unit);
                }
            }
            if (waitingUnitsByUnitType[unitType].Count != 0) Debug.LogError("List isn't clear");
        }
    }
}
