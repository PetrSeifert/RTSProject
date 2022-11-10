using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BorderDecal : MonoBehaviour
{
    [SerializeField] DecalProjector decalProjector;

    void Awake()
    {
        decalProjector.size =
            new Vector3(GameManager.Instance.mapSize * 1.25f, GameManager.Instance.mapSize * 1.25f, 100);
        decalProjector.pivot = new Vector3(500, 500);
    }
}
