using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Screen = UnityEngine.Device.Screen;

public class PhotoCapture : MonoBehaviour
{
    public event Action<Texture2D> OnPhotoTaken;
    
    [SerializeField] private InputActionReference cameraInputAction;

    [Header("Photo Taker")]
    [SerializeField] private Image photoDisplayArea;
    [SerializeField] private GameObject photoFrame;
    [SerializeField] private GameObject cameraUI;

    [Header("Flash Effect")]
    [SerializeField] private GameObject cameraFlash;
    [SerializeField] private float flashTime;

    [Header("Photo Fader Effect")]
    [SerializeField] private Animator fadingAnimation;

    [Header("Audio")]
    [SerializeField] private AudioSource cameraAudio;

    [Header("Visuals to Hide")]
    [SerializeField] private GameObject leftControllerVisual;
    [SerializeField] private GameObject rightControllerVisual;
    [SerializeField] private Canvas subtitleCanvas;
    [SerializeField] private GameObject pastCaptureGameObject;

    public Texture2D[] previousScreenCaptures = new Texture2D[10];
    
    private int screenIndex = 0;
    private Texture2D screenCapture;
    private bool viewingPhoto;
    private void Start()
    {
        screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    }

    private void Update()
    {
        if (cameraInputAction.action.WasPressedThisFrame())
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
        cameraUI.SetActive(false);
        subtitleCanvas?.gameObject.SetActive(false);
        leftControllerVisual?.SetActive(false);
        rightControllerVisual?.SetActive(false);
        pastCaptureGameObject?.SetActive(false);
        viewingPhoto = true;
        
        yield return new WaitForEndOfFrame();

        Rect regionToRead = new Rect(0, 0, Screen.width, Screen.height);
        
        screenCapture.ReadPixels(regionToRead, 0, 0, false);
        screenCapture.Apply();
        ShowPhoto();
        subtitleCanvas?.gameObject.SetActive(true);
        leftControllerVisual?.SetActive(true);
        rightControllerVisual?.SetActive(true);
        pastCaptureGameObject?.SetActive(true);

        previousScreenCaptures[screenIndex] = Instantiate(screenCapture);
        screenIndex = (screenIndex + 1) % previousScreenCaptures.Length;
        OnPhotoTaken?.Invoke(previousScreenCaptures[screenIndex]);
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
        cameraAudio.Play();
        if (cameraFlash)
        {
            cameraFlash.SetActive(true);
            yield return new WaitForSeconds(flashTime);
            cameraFlash.SetActive(false);
        }
    }
    

    private void RemovePhoto()
    {
        viewingPhoto = false;
        photoFrame.SetActive(false);
        cameraUI.SetActive(true);
    }
}
