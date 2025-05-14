using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PastCaptureUI : MonoBehaviour
{
    [SerializeField] private InputActionReference captureOpenReference;
    [SerializeField] private GameObject uiPanelObject;
    public List<RawImage> images = new();

    private void Start()
    {
        var photoCapture = FindFirstObjectByType<PhotoCapture>();
        if (photoCapture)
        {
            photoCapture.OnPhotoTaken += (t) => SetUpImages();
        }
    }

    private void Update()
    {
        if (captureOpenReference.action.WasPressedThisFrame())
        {
            if (uiPanelObject.activeSelf)
            {
                uiPanelObject.SetActive(false);
            }
            else
            {
                SetUpImages();
                uiPanelObject.SetActive(true);
            }
        }
    }

    private void SetUpImages()
    {
        var photocapture = FindFirstObjectByType<PhotoCapture>();
        Debug.Log(photocapture);
        var previousCaptures = photocapture.previousScreenCaptures;
        Debug.Log(previousCaptures);
        for (int i = 0; i < previousCaptures.Length; i++)
        {
            if (images.Count <= i) return;
            Debug.Log(previousCaptures[i]);
            images[i].texture = previousCaptures[i];
        }
    }
}
