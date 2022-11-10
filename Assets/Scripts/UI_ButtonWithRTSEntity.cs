using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_ButtonWithRTSEntity : MonoBehaviour
{
    public Button button;
    public RTSFactionEntity rtsFactionEntity;

    void Awake()
    {
        UI_Manager.Instance.buttonsWithRtsEntity.Add(this);
    }

    void OnDestroy()
    {
        if (UI_Manager.Instance == null) return;
        UI_Manager.Instance.buttonsWithRtsEntity.Remove(this);
    }
}
