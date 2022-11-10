using Pathfinding;
using UnityEngine;

public class GraphManager : Singleton<GraphManager>
{
    protected override void Awake()
    {
        base.Awake();
        EventManager.Instance.onTerrainGenerated.AddListener(SetupGraph);
    }

    void SetupGraph()
    {
        RecastGraph recastGraph = AstarPath.active.data.recastGraph;
        recastGraph.SnapForceBoundsToScene();
        recastGraph.forcedBoundsSize = new Vector3(GameManager.Instance.mapSize, recastGraph.forcedBoundsSize.y, GameManager.Instance.mapSize);
        UpdateGraph();
    }

    void UpdateGraph()
    {
        AstarPath.active.Scan();
    }
}
