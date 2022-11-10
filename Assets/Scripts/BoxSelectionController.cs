using UnityEngine;

[RequireComponent(typeof(SelectionController))]
public class BoxSelectionController : MonoBehaviour
{
    [SerializeField] int selectionBoxBorderThickness;
    [SerializeField] Color selectionBoxColor;
    [SerializeField] float dragDelay;
    
    SelectionController selectionController;
    Texture2D whiteTexture;
    Vector2 boxSelectionStartPosition;
    Vector2 boxSelectionEndPosition;   
    Rect selectionBox;
    bool selectionBoxUnusable;
    [Range(0f, 1f)]
    [SerializeField] float minSelectionBoxSizeX;
    [Range(0f, 1f)]
    [SerializeField] float minSelectionBoxSizeY;

    void Awake()
    {
        selectionController = GetComponent<SelectionController>();
        whiteTexture = new Texture2D( 1, 1 );
        whiteTexture.SetPixel( 0, 0, Color.white );
        whiteTexture.Apply();
    }

    void Update()
    {
        if (selectionController.faction.buildingPlacer.placingBuilding) return;
        Vector2 mousePosition = InputManager.Instance.mousePosition; 
        if(InputManager.Instance.primaryDown)
        {
            boxSelectionStartPosition = mousePosition;
            selectionBoxUnusable = UI_Manager.Instance.ClickedOnUI();
        }
        else if (InputManager.Instance.primaryHeld)
        {
            boxSelectionEndPosition = mousePosition;
            if (selectionBoxUnusable) return;
            selectionBox = GetScreenRect(boxSelectionStartPosition, boxSelectionEndPosition);
        }
        else if (InputManager.Instance.primaryUp)
        {
            if (!selectionBoxUnusable && selectionBox.width / Screen.currentResolution.width >= minSelectionBoxSizeX && selectionBox.height / Screen.currentResolution.width >= minSelectionBoxSizeY)
                BoxSelection();
            
            boxSelectionStartPosition = Vector2.zero;
            boxSelectionEndPosition = Vector2.zero;
            selectionBox = Rect.zero;
        }
    }
    
    void BoxSelection()
    {
        if (selectionController.selectedBuilding)
        {
            EventManager.Instance.onSelectionCleared.Invoke();
            selectionController.selectedBuilding.ToggleSelectionCircle();
            selectionController.selectedBuilding = null;
        }
        if (!InputManager.Instance.shiftHeld)
            selectionController.ClearSelection();
        
        foreach (Unit selectableUnit in selectionController.selectableUnits)
        {
            if (!selectionBox.Contains(GameManager.Instance.mainCamera.WorldToScreenPoint(selectableUnit.transform.position)))
                continue;
                    
            if (selectionController.selectedUnitsByUnitType[selectableUnit.type].Contains(selectableUnit) && InputManager.Instance.shiftHeld)
                    selectionController.DeselectUnit(selectableUnit); // If unit is already selected and player is holding shift, than deselect that unit
            
            selectionController.SelectUnit(selectableUnit);
        }
    }

    void OnGUI()
    {
        if (selectionBox.width / Screen.currentResolution.width < minSelectionBoxSizeX || selectionBox.height / Screen.currentResolution.width < minSelectionBoxSizeY || selectionBoxUnusable) 
            return;
        DrawSelectionBox();
    }

    void DrawScreenRectangle(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, whiteTexture);
        GUI.color = Color.white;
    }

    void DrawSelectionBoxBorder(Rect rect, float thickness, Color color)
    {
        //Top
        DrawScreenRectangle(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        //Left
        DrawScreenRectangle(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        //Right
        DrawScreenRectangle(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        //Bottom
        DrawScreenRectangle(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }
    
    Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        Vector3 topLeft = Vector3.Min(screenPosition1, screenPosition2);
        Vector3 bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    void DrawSelectionBox()
    {
        Rect rect = Rect.MinMaxRect(selectionBox.xMin, selectionBox.yMax + (Screen.height / 2f - selectionBox.yMax) * 2, selectionBox.xMax, selectionBox.yMin + (Screen.height / 2f - selectionBox.yMin) * 2);
        DrawScreenRectangle(rect, new Color(selectionBoxColor.r, selectionBoxColor.g, selectionBoxColor.b, 0.5f));
        DrawSelectionBoxBorder(rect, selectionBoxBorderThickness, selectionBoxColor);
    }
}
