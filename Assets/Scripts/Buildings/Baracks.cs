public class Baracks : Building
{
    UnitSpawner unitSpawner;
    
    protected override void Awake()
    {
        base.Awake();
        unitSpawner = GetComponent<UnitSpawner>();
    }

    protected override void Update()
    {
        base.Update();
        if (!built) return;
        unitSpawner.HandleUnitSpawning();
    }
}
