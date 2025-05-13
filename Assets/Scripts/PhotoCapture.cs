using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Screen = UnityEngine.Device.Screen;

public class PhotoCapture : MonoBehaviour
{
    [Header("Photo Taker")]
    [SerializeField] private Image photoDisplayArea;
    [SerializeField] private GameObject photoFrame;

    [Header("Flash Effect")]
    [SerializeField] private GameObject cameraFlash;
    [SerializeField] private float flashTime;

    [Header("Photo Fader Effect")]
    [SerializeField] private Animator fadingAnimation;
    
    private Texture2D screenCapture;
    private bool viewingPhoto;
    private void Start()
    {
        screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!viewingPhoto)
            {
                StartCoroutine(CapturePhoto());
            }
            else
            {
                RemovePhoto();
            }
        }
    }

    IEnumerator CapturePhoto()
    {
        //camera UI set false;
        viewingPhoto = true;
        
        yield return new WaitForEndOfFrame();

        Rect regionToRead = new Rect(0, 0, Screen.width, Screen.height);
        
        screenCapture.ReadPixels(regionToRead, 0, 0, false);
        screenCapture.Apply();
        ShowPhoto();
    }

    private void ShowPhoto()
    {
        Sprite photoSprite = Sprite.Create(screenCapture,
        new Rect(0.0f, 0.0f, screenCapture.width, screenCapture.height), new Vector2(0.5f, 0.5f), 100.0f);

        photoDisplayArea.sprite = photoSprite;

        photoFrame.SetActive(true);
        StartCoroutine(CameraFlashEffect());
        fadingAnimation.Play("PhotoFade");
    }

    IEnumerator CameraFlashEffect()
    {
        //play audio
        cameraFlash.SetActive(true);
        yield return new WaitForSeconds(flashTime);
        cameraFlash.SetActive(false);
    }
    

    private void RemovePhoto()
    {
        viewingPhoto = false;
        photoFrame.SetActive(false);
        //Camera UI = true;
    }
}
