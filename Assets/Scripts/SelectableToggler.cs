using UnityEngine;

public class SelectableToggler : MonoBehaviour
{
    [SerializeField] RTSFactionEntity rtsFactionEntity;

    protected virtual void OnBecameVisible()
    {
        if (rtsFactionEntity.faction.localPlayerControlled)
            rtsFactionEntity.faction.selectionController.selectableUnits.Add(rtsFactionEntity as Unit);
    }

    protected virtual void OnBecameInvisible()
    {
        if (rtsFactionEntity.faction.localPlayerControlled)
            rtsFactionEntity.faction.selectionController.selectableUnits.Remove(rtsFactionEntity as Unit);
    }
}
