using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class FactionColor : MonoBehaviour
{
    [SerializeField] RTSFactionEntity factionEntity;
    [SerializeField] Renderer renderer;

    void Start()
    {
        renderer.material.color = factionEntity.faction.color;
    }
}
