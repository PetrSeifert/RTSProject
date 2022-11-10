using System.Collections.Generic;

public class Trader : Unit, ITrading
{
    public TradingState TradingState { get; set; }
    public int GoldAmount { get; set; }
    public float SqrTraveledDistance { get; set; }

    const float goldsPerSqrDistanceUnit = 0.000003f;

    protected override void Start()
    {
        base.Start();
        action = new TradingAction(this, GameManager.Instance.market);
    }

    protected override void Die()
    {
        base.Die();
        faction.tradersAmount--;
    }

    public void CollectGolds()
    {
        GoldAmount += (int)(SqrTraveledDistance * goldsPerSqrDistanceUnit);
    }
}
