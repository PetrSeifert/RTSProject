using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public struct FactionInitData
{
    public Vector3 startingPosition;
    public FactionType owner;
    public Color color;
    public bool localPlayerFaction;
}

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] float frequency;
    [SerializeField] TerrainData terrainData;
    [SerializeField] Transform resourceSourcesHolder;
    [SerializeField] Transform factionsHolder;
    [SerializeField] int seed;
    [SerializeField] float sourcesDensity;
    [SerializeField] float treesDensity;
    [SerializeField] ResourceSource[] resourceSources;
    [SerializeField] int sourcesSpawnRatePercentage;
    [SerializeField] float sourcesMapScale;
    [SerializeField] float treesMapScale;
    [SerializeField] Texture2D forestMap;

    [SerializeField] GameObject playerFactionPrefab;
    [SerializeField] GameObject aiFactionPrefab;
    [SerializeField] GameObject neutralFactionPrefab;

    FactionInitData[] factionsInitData;

    int terrainLayerMask;
    int noTerrainLayerMask;

    void Awake()
    {
        factionsInitData = new FactionInitData[]
        {
            new FactionInitData
            {
                startingPosition = new Vector3(GameManager.Instance.mapSize / 3f,
                        100, GameManager.Instance.mapSize / 3f),
                owner = FactionType.Green,
                color = Color.green,
                localPlayerFaction = true
            },
            new FactionInitData
            {
                startingPosition = new Vector3(-GameManager.Instance.mapSize / 3f,
                        100, GameManager.Instance.mapSize / 3f),
                owner = FactionType.Blue,
                color = Color.blue,
                localPlayerFaction = false
            },
            new FactionInitData
            {
                startingPosition = new Vector3(GameManager.Instance.mapSize / 3f,
                        100, -GameManager.Instance.mapSize / 3f),
                owner = FactionType.Red,
                color = Color.red,
                localPlayerFaction = false
            },
            new FactionInitData
            {
                startingPosition = new Vector3(-GameManager.Instance.mapSize / 3f,
                        100, -GameManager.Instance.mapSize / 3f),
                owner = FactionType.Yellow,
                color = Color.yellow,
                localPlayerFaction = false
            }
        };
        terrainLayerMask = LayerMask.GetMask("Terrain");
        noTerrainLayerMask = ~LayerMask.GetMask("Terrain");
        GenerateWorld();
        GenerateFactions();
        EventManager.Instance.onTerrainGenerated.Invoke();
    }

    void GenerateSources()
    {
        for (int y = 0; y < GameManager.Instance.mapSize; y++)
        {
            for (int x = 0; x < GameManager.Instance.mapSize; x++)
            {
                float noise = OpenSimplex2.Noise2(seed, x * sourcesMapScale, y * sourcesMapScale);
                if (sourcesDensity < noise || noise < 0) continue;
                int resourceIndex = Random.Range(1, resourceSources.Length);
                int groupSize = Random.Range(1, resourceSources[resourceIndex].maxGroupSize + 1);
                for (int i = 0; i < groupSize; i++)
                {
                    Vector3 spawnPosition = new(x - GameManager.Instance.mapSize * 0.5f + Random.Range(-5f, 5f), 0,
                        y - GameManager.Instance.mapSize * 0.5f + Random.Range(-5f, 5f));
                    SpawnSource(spawnPosition, resourceSources[resourceIndex]);
                }
            }
        }
    }

    void GenerateFactions()
    {
        for (int i = 0; i < factionsInitData.Length; i++)
        {
            bool validPlacement = false;
            RaycastHit hitInfo = new();
            while (!validPlacement)
            {
                Physics.Raycast(factionsInitData[i].startingPosition, Vector3.down, out hitInfo, 110, terrainLayerMask);
                if (Vector3.Angle(hitInfo.normal, Vector3.up) > 26)
                    factionsInitData[i].startingPosition += new Vector3(Random.Range(-2f, 2f), 100, Random.Range(-2f, 2f));
                else
                    validPlacement = true;
            }
            Faction spawnedFaction = factionsInitData[i].localPlayerFaction ? Instantiate(playerFactionPrefab, hitInfo.point, Quaternion.identity, factionsHolder).GetComponent<Faction>() : Instantiate(aiFactionPrefab, hitInfo.point, Quaternion.identity, factionsHolder).GetComponent<Faction>();
            spawnedFaction.owner = factionsInitData[i].owner;
            spawnedFaction.color = factionsInitData[i].color;
            spawnedFaction.localPlayerControlled = factionsInitData[i].localPlayerFaction;
        }
    }

    void GenerateTrees()
    {
        for (int y = 0; y < GameManager.Instance.mapSize; y++)
        {
            for (int x = 0; x < GameManager.Instance.mapSize; x++)
            {
                float noise = OpenSimplex2.Noise2(seed, x * treesMapScale, y * treesMapScale);
                if (treesDensity < noise || noise < 0) continue;
                int groupSize = Random.Range(1, resourceSources[0].maxGroupSize + 1);
                for (int i = 0; i < groupSize; i++)
                {
                    Vector3 spawnPosition = new(x - GameManager.Instance.mapSize * 0.5f + Random.Range(-5f, 5f), 0,
                        y - GameManager.Instance.mapSize * 0.5f + Random.Range(-5f, 5f));
                    SpawnSource(spawnPosition, resourceSources[0]);
                }
            }
        }
    }

    void GenerateForest()
    {
        Color[] forestValues = forestMap.GetPixels(0, 0, forestMap.width, forestMap.height);
        for (int y = 0; y < forestMap.height; y++)
        {
            for (int x = 0; x < forestMap.width; x++)
            {
                float value = forestValues[x + y * forestMap.width].grayscale;
                if (value == 0)
                {
                    Vector3 spawnPosition = new(Mathf.RoundToInt(x * ((float)GameManager.Instance.mapSize / forestMap.width)) - GameManager.Instance.mapSize * 0.5f + Random.Range(-1f, 1f), 0, Mathf.RoundToInt(y * ((float)GameManager.Instance.mapSize / forestMap.height)) - GameManager.Instance.mapSize * 0.5f + Random.Range(-1f, 1f));
                    SpawnTree(spawnPosition);
                }
            }
        }
    }

    void SpawnTree(Vector3 spawnPosition)
    {
        Physics.Raycast(new Vector3(spawnPosition.x, 100, spawnPosition.z),
            Vector3.down, out RaycastHit hitInfo, 100f, terrainLayerMask);
        if (Vector3.Angle(hitInfo.normal, Vector3.up) > 25) return;
        spawnPosition = new Vector3(hitInfo.point.x,
            hitInfo.point.y + resourceSources[0].transform.localScale.y * 0.5f, hitInfo.point.z);
        if (Physics.CheckSphere(spawnPosition, resourceSources[0].transform.localScale.y, noTerrainLayerMask)) return;
        Instantiate(resourceSources[0], spawnPosition, Quaternion.identity, resourceSourcesHolder);
    }

    void SpawnSource(Vector3 spawnPosition, ResourceSource resourceSource)
    {
        Physics.Raycast(new Vector3(spawnPosition.x, 100, spawnPosition.z),
            Vector3.down, out RaycastHit hitInfo, 100f, terrainLayerMask);
        if (Vector3.Angle(hitInfo.normal, Vector3.up) > 25) return;
        spawnPosition = new Vector3(hitInfo.point.x,
            hitInfo.point.y + resourceSource.transform.localScale.y * 0.5f, hitInfo.point.z);
        if (Physics.CheckSphere(spawnPosition, resourceSource.transform.localScale.y, noTerrainLayerMask)) return;
        Instantiate(resourceSource, spawnPosition, Quaternion.identity, resourceSourcesHolder);
    }

    void GenerateWorld()
    {
        float[,] terrainMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        terrainMap = SimplexTerrainMap(terrainMap.GetLength(0), terrainMap.GetLength(1));
        terrainData.SetHeights(0, 0, terrainMap);
        GenerateForest();
        GenerateSources();
        GenerateTrees();
    }

    float[,] SimplexTerrainMap(int mapWidth, int mapHeight)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];
        Parallel.For(0, mapHeight, i =>
        {
            Parallel.For(0, mapWidth, i1 =>
            {
                float noise = OpenSimplex2.Noise2(seed, i * frequency, i1 * frequency);
                noiseMap[i, i1] = noise > 0 ? Mathf.Pow(noise, 2) : 0;
            });
        });
        
        return noiseMap;
    }
}