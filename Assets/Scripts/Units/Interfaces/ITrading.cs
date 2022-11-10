using System.Collections.Generic;

public interface ITrading
{
    TradingState TradingState { get; set; }
    float SqrTraveledDistance { get; set; }
    int GoldAmount { get; set; }

    void CollectGolds();
}
