using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Faction))]
public class SelectionController : MonoBehaviour
{
    [HideInInspector] public Faction faction;
    [HideInInspector] public List<Unit> selectableUnits = new();
    [HideInInspector] public Dictionary<UnitType, List<Unit>> selectedUnitsByUnitType = new();
    [HideInInspector] public Building selectedBuilding;


    void Awake()
    {
        faction = GetComponent<Faction>();
        foreach (UnitType unitType in GameManager.Instance.unitTypes)
        {
            selectedUnitsByUnitType.Add(unitType, new List<Unit>());
        }
    }

    public void SelectUnit(Unit unit)
    {
        selectedUnitsByUnitType[unit.type].Add(unit);
        EventManager.Instance.onRTSFactionEntitiesSelected.Invoke(new RTSFactionEntity[]{unit});
        unit.ToggleSelectionCircle();
    }

    public void DeselectUnit(Unit unit)
    {
        selectedUnitsByUnitType[unit.type].Remove(unit);
        unit.ToggleSelectionCircle();
        EventManager.Instance.onRTSFactionEntitiesDeselected.Invoke(new RTSFactionEntity[]{unit});
        
        if (selectedUnitsByUnitType[unit.type].Count == 0) 
            EventManager.Instance.onSelectionCleared.Invoke(); 
    }

    public void ClearSelection()
    {
        EventManager.Instance.onSelectionCleared.Invoke();
        if (selectedBuilding)
        {
            selectedBuilding.ToggleSelectionCircle();
            selectedBuilding = null;
        }
        foreach (UnitType unitType in selectedUnitsByUnitType.Keys.ToList())
        {
            foreach (Unit unit in selectedUnitsByUnitType[unitType])
            {
                unit.ToggleSelectionCircle();
            }
            selectedUnitsByUnitType[unitType].Clear();
        }
    }
}
