using UnityEngine;

public class House : Building
{
    [Header("House")]
    [SerializeField] int housingSpace;

    protected override void Awake()
    {
        base.Awake();
        onBuilt.AddListener(AddHousingSpace);
    }

    void AddHousingSpace()
    {
        faction.housingSpace += housingSpace;
        EventManager.Instance.onHousingSpaceChanged.Invoke();
    }

    void OnDestroy()
    {
        onBuilt.RemoveListener(AddHousingSpace);
        faction.housingSpace -= housingSpace;
    }
}