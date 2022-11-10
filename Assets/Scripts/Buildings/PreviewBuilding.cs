using UnityEngine;

public class PreviewBuilding : MonoBehaviour
{
    public bool isOverlapping;
    public bool isOnSteepTerrain;

    Renderer renderer;

    void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        renderer.material.color = GetCanBePlaced() ? Color.white : Color.red;
    }

    void OnTriggerEnter(Collider other)
    {
        isOverlapping = true;
    }

    void OnTriggerExit(Collider other)
    {
        isOverlapping = false;
    }

    public bool GetCanBePlaced() => !isOverlapping && !isOnSteepTerrain;
}
