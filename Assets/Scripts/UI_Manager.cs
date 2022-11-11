using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UI_Manager : Singleton<UI_Manager>
{
    public List<UI_ButtonWithRTSEntity> buttonsWithRtsEntity;
    
    public Faction playerFaction;
    
    [SerializeField] GameObject entityPanel;
    [SerializeField] TMP_Text entityNameText;
    [SerializeField] GameObject villagerUIPrefab;
    [SerializeField] GameObject soldierUIPrefab;
    [SerializeField] GameObject archerUIPrefab;    
    [SerializeField] GameObject traderUIPrefab;
    [SerializeField] GameObject mainHallUIPrefab;
    [SerializeField] GameObject barracksUIPrefab;
    [SerializeField] TMP_Text idleVillagersCountText;
    [SerializeField] TMP_Text coinsText;
    [SerializeField] TMP_Text foodsText;
    [SerializeField] TMP_Text woodsText;
    [SerializeField] TMP_Text stonesText;
    [SerializeField] EventSystem eventSystem;

    GraphicRaycaster raycaster;
    PointerEventData pointerEventData;
    GameObject currentActiveUI;
    RectTransform entityPanelRectTransform;
    Dictionary<UnitType, GameObject> uiPerUnitType;

    protected override void Awake()
    {
        base.Awake();
        raycaster = GetComponent<GraphicRaycaster>();
        entityPanelRectTransform = entityPanel.GetComponent<RectTransform>();
        uiPerUnitType = new Dictionary<UnitType, GameObject>
        {
            {UnitType.Villager, villagerUIPrefab},
            {UnitType.Soldier, soldierUIPrefab},
            {UnitType.Archer, archerUIPrefab},
            {UnitType.Trader, traderUIPrefab}
        };
        EventManager.Instance.onRTSFactionEntitiesSelected.AddListener(AddEntitiesToSelectionUIAndUpdate);
        EventManager.Instance.onRTSFactionEntitiesDeselected.AddListener(RemoveEntitiesFromSelectionUIAndUpdate);
        EventManager.Instance.onSelectionCleared.AddListener(HidePanels);
        EventManager.Instance.onUnitsSpawningQueueChanged.AddListener(SetIfButtonsUsable);
        EventManager.Instance.onResourcesAmountChanged.AddListener(SetIfButtonsUsable);
        EventManager.Instance.onResourcesAmountChanged.AddListener(UpdateResourcesUI);
        EventManager.Instance.onIdleVillagersCountChanged.AddListener(UpdateIdleVillagersCountText);
    }

    void Start()
    {
        HidePanels();
    }

    public bool ClickedOnUI()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = InputManager.Instance.mousePosition;
        List<RaycastResult> results = new();
        raycaster.Raycast(pointerEventData, results);
        return results.Count > 0;
    }

    void ActivateUnitUI(UnitType unitType)
    {
        if (currentActiveUI != null) Destroy(currentActiveUI);
        
        currentActiveUI = Instantiate(uiPerUnitType[unitType], new Vector3(entityPanelRectTransform.sizeDelta.x * 0.5f, entityPanelRectTransform.sizeDelta.y * 0.5f), Quaternion.identity, entityPanel.transform);
        SetIfButtonsUsable();
    }
    
    void ActivateUnitSpawnerPanel()
    {   
        if (currentActiveUI != null) Destroy(currentActiveUI);
        if (playerFaction.selectionController.selectedBuilding is Baracks)
        {
            currentActiveUI = Instantiate(barracksUIPrefab, new Vector3(entityPanelRectTransform.sizeDelta.x * 0.5f, entityPanelRectTransform.sizeDelta.y * 0.5f), Quaternion.identity, entityPanel.transform);
        }
        else if (playerFaction.selectionController.selectedBuilding is MainHall)
        {
            currentActiveUI = Instantiate(mainHallUIPrefab, new Vector3(entityPanelRectTransform.sizeDelta.x * 0.5f, entityPanelRectTransform.sizeDelta.y * 0.5f), Quaternion.identity, entityPanel.transform);
        }
        SetIfButtonsUsable();
    }

    void AddEntitiesToSelectionUIAndUpdate(RTSFactionEntity[] entitiesToAdd)
    {
        entityPanel.SetActive(true);
        foreach (RTSFactionEntity rtsFactionEntity in entitiesToAdd)
        {
            entityNameText.text = rtsFactionEntity.entityName;
            Unit unitToAdd = rtsFactionEntity as Unit;
            if (unitToAdd)
            {
                ActivateUnitUI(unitToAdd.type);
            }
            else
            {
                ActivateUnitSpawnerPanel();
            }

            if (currentActiveUI == null) return;
            RectTransform currentUIRectTransform = currentActiveUI.GetComponent<RectTransform>();
            currentUIRectTransform.offsetMin = new Vector2(0, 0);
            currentUIRectTransform.offsetMax = new Vector2(0, 0);
        }
    }
    
    void RemoveEntitiesFromSelectionUIAndUpdate(RTSFactionEntity[] entitiesToRemove)
    {
        foreach (RTSFactionEntity rtsFactionEntity in entitiesToRemove)
        {
            Building buildingToRemove = rtsFactionEntity as Building;
            if (buildingToRemove) HidePanels();
        }
    }

    public void UnitSpawnerButtonClicked(Unit unitToSpawn)
    {
        if (playerFaction.selectionController.selectedBuilding.TryGetComponent(out UnitSpawner unitSpawner))
            unitSpawner.AddUnitToQueue(unitToSpawn);
    }

    void HidePanels()
    {
        entityPanel.SetActive(false);
        Destroy(currentActiveUI);
    }

    void UpdateResourcesUI()
    {
        coinsText.text = playerFaction.resourceStorage[ResourceType.Money].ToString();
        foodsText.text = playerFaction.resourceStorage[ResourceType.Food].ToString();
        woodsText.text = playerFaction.resourceStorage[ResourceType.Wood].ToString();
        stonesText.text = playerFaction.resourceStorage[ResourceType.Stone].ToString();
    }

    void UpdateIdleVillagersCountText()
    {
        idleVillagersCountText.text = $"IV: {playerFaction.idleVillagers.Count}";
    }

    void SetIfButtonsUsable()
    {
        foreach (UI_ButtonWithRTSEntity buttonWithRtsEntity in buttonsWithRtsEntity)
        {
            buttonWithRtsEntity.button.interactable = playerFaction.HasEnoughResources(buttonWithRtsEntity.rtsFactionEntity.resourcesNeededToCreateMe);

            if (!buttonWithRtsEntity.button.interactable) continue;
            Unit unit = buttonWithRtsEntity.rtsFactionEntity as Unit;
            if (unit)
            {
                buttonWithRtsEntity.button.interactable =
                    playerFaction.unitsSpaceOccupied + unit.housingSize <= playerFaction.unitsSpace;
            }
        }
    }

    public void TogglePlacingBuilding(Building building)
    {
        playerFaction.buildingPlacer.TogglePlacingBuilding(building);
    }
}