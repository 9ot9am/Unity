using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class FishdexUI : MonoBehaviour
{
    [SerializeField] private InputActionReference FishdexUIAction;
    [SerializeField] private GameObject uiPanelObject;
    [SerializeField] public FishDexUI fishDexUI;

    private Tween _tween;
    
    private void Start()
    {
        var photoCapture = FindFirstObjectByType<PhotoCapture>();
        photoCapture.OnPhotoTaken += PhotoCaptureOnOnPhotoTaken;
    }

    private void PhotoCaptureOnOnPhotoTaken(Texture2D obj)
    {
        fishDexUI.RevealFish((FishType)Random.Range(0,4), obj);
    }


    private void Update()
    {
        if (FishdexUIAction.action.WasPressedThisFrame())
        {
            if (_tween != null) return;
            if (!uiPanelObject.activeSelf)
            {
                OpenUI();
            }
            else
            {
                CloseUI();
            }
        }
    }


    public void OpenUI()
    {
        uiPanelObject.SetActive(true);
        uiPanelObject.GetComponent<CanvasGroup>().alpha = 0f;
        _tween = uiPanelObject.GetComponent<CanvasGroup>()
            .DOFade(1f, 0.5f)
            .OnComplete(() => _tween = null);
    }
    
    public void CloseUI()
    {
        uiPanelObject.SetActive(true);
        uiPanelObject.GetComponent<CanvasGroup>().alpha = 1f;
        _tween = uiPanelObject.GetComponent<CanvasGroup>()
            .DOFade(0f, 0.5f)
            .OnComplete(() =>
            {
                uiPanelObject.SetActive(false);
                _tween = null;
            }); 
    }
}
