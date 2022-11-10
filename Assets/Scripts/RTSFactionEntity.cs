using UnityEngine;

public abstract class RTSFactionEntity : MonoBehaviour
{
    public string entityName;
    
    [Header("Setup")]
    public Faction faction;
    public DictionaryAmountPerResource resourcesNeededToCreateMe;
    
    public MeshFilter meshFilter;

    [SerializeField] GameObject selectionCircle;

    protected int terrainLayerMask;

    protected virtual void Awake()
    {
        terrainLayerMask = LayerMask.GetMask("Terrain");
    }

    protected virtual void Start()
    {
        transform.position += Vector3.up * 100;
    }

    public void ToggleSelectionCircle()
    {
        selectionCircle.SetActive(!selectionCircle.activeSelf);
    }
}
