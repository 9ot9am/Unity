using System;
using System.Collections;
using DG.Tweening;
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

    [Header("Polaroid")] [SerializeField] private GameObject polaroidPrefab;
    
    [Header("Visuals to Hide")]
    [SerializeField] private GameObject leftControllerVisual;
    [SerializeField] private GameObject rightControllerVisual;
    
    [SerializeField] private GameObject nearFarLeft;
    [SerializeField] private GameObject pokeLeft;
    [SerializeField] private GameObject nearFarRight;
    [SerializeField] private GameObject pokeRight;
    
    [SerializeField] private Canvas subtitleCanvas;
    [SerializeField] private GameObject pastCaptureGameObject;
    [SerializeField] private GameObject npcHelperModelGameObject;
    [SerializeField] private GameObject fishDexCanvas;

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
        npcHelperModelGameObject?.SetActive(false);
        fishDexCanvas?.SetActive(false);
        nearFarLeft?.SetActive(false);
        pokeLeft?.SetActive(false);
        nearFarRight?.SetActive(false);
        pokeRight?.SetActive(false);
        
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
        npcHelperModelGameObject?.SetActive(true);
        fishDexCanvas?.SetActive(true);
        nearFarLeft?.SetActive(true);
        pokeLeft?.SetActive(true);
        nearFarRight?.SetActive(true);
        pokeRight?.SetActive(true);

        previousScreenCaptures[screenIndex] = Instantiate(screenCapture);
        screenIndex = (screenIndex + 1) % previousScreenCaptures.Length;
        OnPhotoTaken?.Invoke(previousScreenCaptures[screenIndex]);
    }

    private void ShowPhoto()
    {
        var newScreenCapture = Instantiate(screenCapture);
        Sprite photoSprite = Sprite.Create(newScreenCapture,
        new Rect(0.0f, 0.0f, newScreenCapture.width, newScreenCapture.height), new Vector2(0.5f, 0.5f), 100.0f);

        photoDisplayArea.sprite = photoSprite;

        photoFrame.SetActive(true);
        StartCoroutine(CameraFlashEffect());
        fadingAnimation.Play("PhotoFade");
        StartCoroutine(CreatePolaroid(photoSprite));
    }

    private IEnumerator CreatePolaroid(Sprite sprite)
    {
        yield return new WaitForSeconds(1.5f);
        
        if (viewingPhoto)
        {
            RemovePhoto();
        }
        
        var pos = cameraUI.transform.position;
        var player = GameObject.FindGameObjectWithTag("Player");
        var towardsPlayer = pos - player.transform.position;
        var rotation = Quaternion.LookRotation(towardsPlayer);
        var newPolaroid = Instantiate(polaroidPrefab, pos, rotation);

        
        var polaroid = newPolaroid.GetComponentInChildren<PolaroidDisplay>();
        //yield return new WaitUntil(() => polaroid.instancedMaterial != null);
        polaroid.SetPhoto(sprite.texture);
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
