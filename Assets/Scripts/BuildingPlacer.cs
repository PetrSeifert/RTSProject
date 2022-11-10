using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public bool placingBuilding;
    
    [SerializeField] Faction faction;
    [SerializeField] GameObject previewBuildingObject;

    PreviewBuilding previewBuilding;
    Building buildingToPlace;

    int terrainLayerMask;

    void Awake()
    {
        previewBuildingObject = Instantiate(previewBuildingObject);
        previewBuilding = previewBuildingObject.GetComponent<PreviewBuilding>();
        previewBuildingObject.SetActive(false);
        terrainLayerMask = LayerMask.GetMask("Terrain");
    }

    void Update()
    {
        if (placingBuilding) HandlePlacingBuilding();
    }

    void HandlePlacingBuilding()
    {
        Vector3 mouseInWorld = GameManager.Instance.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
        Vector3 clickDirection = (mouseInWorld - GameManager.Instance.cameraTransform.position).normalized;
        bool terrainHit = Physics.Raycast(mouseInWorld, clickDirection, out RaycastHit hitInfo, 300, terrainLayerMask);
        if (!terrainHit)
        {
            Debug.LogError("Did not hit terrain");
            return;
        }
        previewBuilding.isOnSteepTerrain = Vector3.Angle(hitInfo.normal, Vector3.up) > 26;
        Quaternion rotationForBuilding = Quaternion.FromToRotation(transform.up, hitInfo.normal);
        Vector3 positionForBuilding = new(hitInfo.point.x, hitInfo.point.y + buildingToPlace.meshFilter.sharedMesh.bounds.extents.y * buildingToPlace.transform.lossyScale.y, hitInfo.point.z);
        ActivatePreviewBuilding(positionForBuilding, rotationForBuilding);
        if (InputManager.Instance.primaryDown && GetCanPlaceBuilding())
        {
            if (UI_Manager.Instance.ClickedOnUI()) return;
            faction.UseResources(buildingToPlace.resourcesNeededToCreateMe);
            Building building = PlaceBuilding(previewBuilding.transform, buildingToPlace);
            previewBuildingObject.SetActive(false);
            foreach (Unit unit in faction.selectionController.selectedUnitsByUnitType[UnitType.Villager])
            {
                unit.SetAction(new BuildingAction(unit, building));
            }
            placingBuilding = false;
        }
    }

    void ActivatePreviewBuilding(Vector3 position, Quaternion rotation)
    {
        previewBuildingObject.SetActive(true);
        Transform previewBuildingTransform = previewBuilding.transform;
        previewBuildingTransform.localScale = buildingToPlace.transform.localScale;
        previewBuildingTransform.position = position;
        previewBuildingTransform.rotation = rotation;
    }

    public Building PlaceBuilding(Transform buildingTransform, Building building)
    {
        building.faction = faction;
        building = Instantiate(building, buildingTransform.position, Quaternion.identity, faction.buildingsHolder);
        return building;
    }

    bool GetCanPlaceBuilding() => previewBuilding.GetCanBePlaced();

    public void TogglePlacingBuilding(Building building)
    {
        if (placingBuilding)
        {
            if (buildingToPlace == building)
            {
                placingBuilding = false;
                previewBuildingObject.SetActive(false);
            }
            else buildingToPlace = building;
        }
        else
        {
            placingBuilding = true;
            buildingToPlace = building;
        }
    }
}
