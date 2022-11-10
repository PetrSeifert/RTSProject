using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ToggleBuildingPlacingButton : MonoBehaviour
{
    [SerializeField] UI_ButtonWithRTSEntity uiButtonWithRtsEntity;
    
    public void TogglePlacingBuilding()
    {
        UI_Manager.Instance.TogglePlacingBuilding(uiButtonWithRtsEntity.rtsFactionEntity as Building);
    }
}
