using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class DecisionAction : Decision
{
    protected DecisionAction(EconomyAI economyAI) : base(economyAI){}

    protected abstract void Transition();

    protected abstract void Action();
    
    protected override void Evaluate()
    {
        Action();
        Transition();
    }
}
