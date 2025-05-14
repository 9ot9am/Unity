using DG.Tweening;
using UnityEngine;

public class NPCHelper3DController : MonoBehaviour
{
    public enum NPCState
    {
        Idle,
        Talking,
        Excited,
        Encouraging
    }

    [Header("References")]
    public Transform playerTransform; // Assign your Player's Transform here
    public Transform npcModelTransform; // Assign the visual part of the NPC (e.g., the Sphere child)

    [Header("Positioning")]
    public Vector3 offsetFromPlayer = new Vector3(0.8f, 0.5f, 0.3f); // X: right, Y: up, Z: forward from player
    public float lookAtPlayerSmoothness = 5f; // How quickly the NPC turns to look near the player

    [Header("NPC Settings")]
    public NPCState previousState = NPCState.Idle;
    public NPCState currentState = NPCState.Idle;

    // Stored initial/base values for animations (relative to its own local space after parenting)
    private Vector3 _baseLocalPosition;
    private Vector3 _initialLocalScale;
    private Quaternion _initialLocalRotation; // Initial local rotation of the model itself

    [Header("Idle Animation")]
    public float idleBobAmount = 0.1f; // World units
    public float idleBobDuration = 1.5f;
    public Ease idleEase = Ease.InOutSine;

    [Header("Talking Animation")]
    public float talkingBobAmount = 0.15f;
    public float talkingBobDuration = 0.7f;
    public Vector3 talkingSquashScale = new Vector3(1.1f, 0.9f, 1.1f); // Relative to initial scale
    public Ease talkingEase = Ease.InOutQuad;

    [Header("Excited Animation")]
    public float excitedJumpAmount = 0.3f;
    public float excitedJumpDuration = 0.4f;
    public Vector3 excitedShakeRotationStrength = new Vector3(0, 0, 20f); // Degrees on Z-axis
    public int excitedShakeVibrato = 10;
    public float excitedShakeRandomness = 90f;
    public Ease excitedJumpEase = Ease.OutQuad;

    [Header("Encouraging Animation")]
    public float encouragingNodRotation = 15f; // Degrees on X-axis (pitch)
    public float encouragingNodDuration = 0.8f;
    public float encouragingPulseScaleFactor = 1.1f;
    public float encouragingPulseDuration = 0.6f;
    public Ease encouragingEase = Ease.OutBack;

    private Sequence _activeSequence;


    void Awake()
    {
        if (npcModelTransform == null)
        {
            // Try to find a child model if not assigned
            if (transform.childCount > 0) npcModelTransform = transform.GetChild(0);
            else Debug.LogError("NPC Model Transform not assigned and no child found!", this);
        }

        if (npcModelTransform != null)
        {
            _initialLocalScale = npcModelTransform.localScale;
            _initialLocalRotation = npcModelTransform.localRotation;
        }
        else
        {
            enabled = false; // Disable script if no model
            return;
        }

        if (playerTransform == null)
        {
            Debug.LogError("Player Transform not assigned to NPCHelper3DController!", this);
            enabled = false;
            return;
        }
    }

    void Start()
    {
        // Parent self to Player and set initial offset position
        // The NPCHelper3D (this script's GameObject) will handle the offset
        transform.SetParent(playerTransform);
        transform.localPosition = offsetFromPlayer;
        transform.localRotation = Quaternion.identity; // Face same direction as player initially

        // The animations will operate on the npcModelTransform's local space
        // So its base local position should be zero relative to its parent (NPCHelper3D)
        _baseLocalPosition = Vector3.zero; // npcModelTransform starts at (0,0,0) relative to NPCHelper3D
        
        SetState(currentState); // Start with the initial state

        var servertest = FindFirstObjectByType<ServerTest>();
        if (!servertest) return;
        servertest.audioReceived += ServertestOnaudioReceived;
    }

    private void ServertestOnaudioReceived(AudioClip clip)
    {
        previousState = currentState;
        SetState(NPCState.Talking);
        DOVirtual.DelayedCall(clip.length, () => SetState(previousState));
    }

    void LateUpdate()
    {
        // Optional: Smoothly orient the NPC root to generally face where the player is looking,
        // but don't make it rigidly lock on.
        if (playerTransform != null && lookAtPlayerSmoothness > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(playerTransform.forward, playerTransform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lookAtPlayerSmoothness);
        }
    }


    public void SetState(NPCState newState)
    {
        currentState = newState;
        KillActiveSequence();
        ResetModelTransform(); // Reset model to base before starting new animation

        // Ensure the model is at its base local position for calculations
        if (npcModelTransform) npcModelTransform.localPosition = _baseLocalPosition;

        switch (currentState)
        {
            case NPCState.Idle:
                AnimateIdle();
                break;
            case NPCState.Talking:
                AnimateTalking();
                break;
            case NPCState.Excited:
                AnimateExcited();
                break;
            case NPCState.Encouraging:
                AnimateEncouraging();
                break;
        }
    }

    void KillActiveSequence()
    {
        if (_activeSequence != null && _activeSequence.IsActive())
        {
            _activeSequence.Kill();
        }
        // Kill any other tweens directly on the model transform if they were not part of a sequence
        if (npcModelTransform) npcModelTransform.DOKill();
    }

    void ResetModelTransform()
    {
        if (npcModelTransform)
        {
            npcModelTransform.localPosition = _baseLocalPosition; // Should be Vector3.zero
            npcModelTransform.localScale = _initialLocalScale;
            npcModelTransform.localRotation = _initialLocalRotation;
        }
    }

    // --- Animation Methods ---
    // Note: Animations are applied to npcModelTransform (the visual part)
    // _baseLocalPosition for the model is Vector3.zero relative to NPCHelper3D root

    void AnimateIdle()
    {
        if (!npcModelTransform) return;
        _activeSequence = DOTween.Sequence();
        _activeSequence.Append(npcModelTransform.DOLocalMoveY(_baseLocalPosition.y + idleBobAmount, idleBobDuration))
                .SetEase(idleEase)
                .SetLoops(-1, LoopType.Yoyo)
            .Join(npcModelTransform.DOLocalMoveX(_baseLocalPosition.x + idleBobAmount / 2f, idleBobDuration))
                .SetEase(idleEase)
                .SetLoops(-1, LoopType.Yoyo)
            ;
    }

    void AnimateTalking()
    {
        if (!npcModelTransform) return;
        _activeSequence = DOTween.Sequence();
        _activeSequence.Append(npcModelTransform.DOLocalMoveY(_baseLocalPosition.y + talkingBobAmount, talkingBobDuration / 2)
                .SetEase(talkingEase).SetLoops(2, LoopType.Yoyo))
            .Join(npcModelTransform.DOScale(Vector3.Scale(_initialLocalScale, talkingSquashScale), talkingBobDuration / 4)
                .SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo))
            .SetLoops(-1, LoopType.Restart);
    }

    void AnimateExcited()
    {
        if (!npcModelTransform) return;
        _activeSequence = DOTween.Sequence();
        _activeSequence.Append(npcModelTransform.DOLocalMoveY(_baseLocalPosition.y + excitedJumpAmount, excitedJumpDuration / 2)
                .SetEase(excitedJumpEase))
            .Append(npcModelTransform.DOLocalMoveY(_baseLocalPosition.y, excitedJumpDuration / 2)
                .SetEase(Ease.InQuad))
            .Join(npcModelTransform.DOShakeRotation(excitedJumpDuration, excitedShakeRotationStrength, excitedShakeVibrato, excitedShakeRandomness)
                .SetDelay(excitedJumpDuration / 4))
            .SetLoops(-1, LoopType.Restart);
    }

    void AnimateEncouraging()
    {
        if (!npcModelTransform) return;
        _activeSequence = DOTween.Sequence();
        _activeSequence.Append(npcModelTransform.DOScale(Vector3.Scale(_initialLocalScale, new Vector3(encouragingPulseScaleFactor, encouragingPulseScaleFactor, encouragingPulseScaleFactor)), encouragingPulseDuration / 2)
                .SetEase(encouragingEase))
            .Append(npcModelTransform.DOScale(_initialLocalScale, encouragingPulseDuration / 2)
                .SetEase(Ease.InSine))
            .Join(npcModelTransform.DOLocalRotateQuaternion(_initialLocalRotation * Quaternion.Euler(encouragingNodRotation, 0, 0), encouragingNodDuration / 2) // Nod on X-axis
                .SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo))
            .SetLoops(-1, LoopType.Restart);
    }

    // --- Example Test Input ---
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetState(NPCState.Idle);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetState(NPCState.Talking);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetState(NPCState.Excited);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetState(NPCState.Encouraging);
    }

    void OnDestroy()
    {
        KillActiveSequence(); // Clean up tweens when the object is destroyed
    }
}