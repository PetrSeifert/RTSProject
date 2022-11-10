using UnityEngine;

public class Market : Building
{
    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.market = this;
    }

    public override void TakeDamage(int amount) 
    {
        //Market can't be destroyed
    }
}
