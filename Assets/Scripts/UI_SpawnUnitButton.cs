using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SpawnUnitButton : MonoBehaviour
{
    [SerializeField] UI_ButtonWithRTSEntity uiButtonWithRtsEntity;

    public void SpawnUnit()
    {
        UI_Manager.Instance.UnitSpawnerButtonClicked(uiButtonWithRtsEntity.rtsFactionEntity as Unit);
    }
}
