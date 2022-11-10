using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5;
    [SerializeField] int maxDistanceFromGround;
    [SerializeField] int minDistanceFromGround;
    
    float lastDistanceFromGround;
    Transform cameraTransform;
    int mapSize;

    int terrainLayerMask;

    void Awake()
    {
        cameraTransform = transform;
        mapSize = GameManager.Instance.mapSize;
        terrainLayerMask = LayerMask.GetMask("Terrain");
    }

    void Start()
    {
        Physics.Raycast(cameraTransform.position, Vector3.down, 100f, terrainLayerMask);
        lastDistanceFromGround = minDistanceFromGround;
    }

    void Update()
    {
        //Moving
        Vector3 cameraPosition = cameraTransform.position;
        Physics.Raycast(cameraPosition, Vector3.down, out RaycastHit hit, 100f, terrainLayerMask);
        cameraPosition = new Vector3(cameraPosition.x, cameraPosition.y + lastDistanceFromGround - hit.distance, cameraPosition.z);
        cameraTransform.position = cameraPosition;
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        Vector2 mousePosition = new(Mathf.RoundToInt(InputManager.Instance.mousePosition.x), Mathf.RoundToInt(InputManager.Instance.mousePosition.y));
        float horizontal = InputManager.Instance.horizontal;
        float vertical = InputManager.Instance.vertical;
        if (mousePosition.x < 5 || mousePosition.x > Screen.currentResolution.width - 5 || mousePosition.y < 5 ||
            mousePosition.y > Screen.currentResolution.height - 5)
        {
            Vector2 moveDirection =
                new Vector2((mousePosition.x - Screen.currentResolution.width / 2) / Screen.currentResolution.width,
                        (mousePosition.y - Screen.currentResolution.height / 2) / Screen.currentResolution.height)
                    .normalized;
            horizontal = moveDirection.x * movementSpeed;
            vertical = moveDirection.y * movementSpeed;
        }
        cameraPosition += new Vector3(forward.x * vertical + right.x * horizontal, 0, forward.z * vertical + right.z * horizontal).normalized * (movementSpeed * Time.deltaTime);
        
        //Zooming
        float groundPositionY = cameraPosition.y - lastDistanceFromGround;
        float zoomDelta = InputManager.Instance.scrollDelta;
        if (maxDistanceFromGround < lastDistanceFromGround + forward.y * zoomDelta)
            zoomDelta = cameraPosition.y - maxDistanceFromGround - groundPositionY;
        else if (minDistanceFromGround > lastDistanceFromGround + forward.y * zoomDelta)
            zoomDelta = cameraPosition.y - minDistanceFromGround - groundPositionY;
        if (zoomDelta != 0)
        {
            cameraPosition += forward * zoomDelta;
            lastDistanceFromGround = cameraPosition.y - groundPositionY;
        }
        
        //Clamp to map border
        if (cameraPosition.x > mapSize / 2 - 10) cameraPosition.x = mapSize / 2 - 10;
        else if (cameraPosition.x < -mapSize / 2 - 10) cameraPosition.x = -mapSize / 2 - 10;
        if (cameraPosition.z > mapSize / 2 - 10) cameraPosition.z = mapSize / 2 - 10;
        else if (cameraPosition.z < -mapSize / 2 - 10) cameraPosition.z = -mapSize / 2 - 10;
        
        cameraTransform.position = cameraPosition;
    }
}
