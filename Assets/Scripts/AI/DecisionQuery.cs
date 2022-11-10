using System;
using UnityEngine;

public abstract class DecisionQuery : Decision
{
    protected DecisionQuery(EconomyAI economyAI) : base(economyAI){}
    
    protected override void Evaluate()
    {
        bool result = Test();
        
        if (result)
            PositiveTransition();
        else
            NegativeTransition();
    }
    
    protected abstract void PositiveTransition();

    protected abstract void NegativeTransition();

    protected abstract bool Test();
}
