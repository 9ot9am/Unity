using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishdexUI : MonoBehaviour
{
    [SerializeField] private InputActionReference FishdexUIAction;
    [SerializeField] private GameObject uiPanelObject;
    
    private void Update()
    {
        if (FishdexUIAction.action.WasPressedThisFrame())
        {
            if (uiPanelObject.activeSelf)
            {
                uiPanelObject.SetActive(false);
            }
            else
            {
                uiPanelObject.SetActive(true);
            }
        }
    }   
}
