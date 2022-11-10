using UnityEngine;

[RequireComponent(typeof(SelectionController))]
public class ClickSelectionController : MonoBehaviour
{
    [SerializeField] float dragDelay;

    SelectionController selectionController;
    int clickSelectionLayerMask;
    float remainingDragDelay;
    
    void Awake()
    {
        selectionController = GetComponent<SelectionController>();
        clickSelectionLayerMask =~ LayerMask.GetMask("Terrain");
    }

    void Update()
    {
        if (InputManager.Instance.primaryHeld)
        {
            remainingDragDelay = dragDelay - InputManager.Instance.primaryHoldTime;
        }
        else if (InputManager.Instance.primaryUp && remainingDragDelay > 0)
        { 
            ClickSelection();
        }
    }

    void ClickSelection()
    {
        if (UI_Manager.Instance.ClickedOnUI() || selectionController.faction.buildingPlacer.placingBuilding) return;

        if (!InputManager.Instance.shiftHeld)
            selectionController.ClearSelection();

        bool hitSelectable = Physics.Raycast(GameManager.Instance.mouseCameraRay, out RaycastHit hitInfo, 210, clickSelectionLayerMask);
        if (!hitSelectable) return;

        RTSFactionEntity hitFactionEntity = hitInfo.collider.GetComponent<RTSFactionEntity>();
        if (!hitFactionEntity)
        {
            hitFactionEntity = hitInfo.collider.GetComponentInParent<RTSFactionEntity>();
            if (!hitFactionEntity) return;
        }

        if (hitFactionEntity.faction != selectionController.faction) return;

        Unit unit = hitFactionEntity as Unit;
        Building building = hitFactionEntity as Building;
        if (unit)
        {
            if (selectionController.selectedBuilding)
            {
                EventManager.Instance.onRTSFactionEntitiesDeselected.Invoke(new RTSFactionEntity[]{selectionController.selectedBuilding});
                selectionController.selectedBuilding.ToggleSelectionCircle();
                selectionController.selectedBuilding = null; 
            }
            
            if (selectionController.selectedUnitsByUnitType[unit.type].Contains(unit))
            {
                if (!InputManager.Instance.shiftHeld) return;
                selectionController.DeselectUnit(unit); // If unit is already selected and player is holding shift, than deselect that unit
                return;
            }

            selectionController.SelectUnit(unit);
        }
        else if (building)
        {
            selectionController.ClearSelection(); //We want to have only one building selected and nothing else
            selectionController.selectedBuilding = building;
            selectionController.selectedBuilding.ToggleSelectionCircle();
            EventManager.Instance.onRTSFactionEntitiesSelected.Invoke(new RTSFactionEntity[]{building});
        }
    }
    
}
