using UnityEngine;

public abstract class Decision
{
    protected EconomyAI economyAI;

    protected Decision(EconomyAI economyAI)
    {
        this.economyAI = economyAI;
        Evaluate();
    }
    
    protected abstract void Evaluate();
}
