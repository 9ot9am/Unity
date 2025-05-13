using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class RecordButton : MonoBehaviour
{
    public InputActionReference recordButtonRef;

    private RecordAudio _recordAudio;

    private void Awake()
    {
        _recordAudio = GetComponent<RecordAudio>();
    }

    private void OnEnable()
    {
        recordButtonRef.action.started += StartRecording;
        recordButtonRef.action.canceled += StopRecording;
    }

    private void StartRecording(InputAction.CallbackContext obj)
    {
        _recordAudio.StartRecording();
    }
    
    private void StopRecording(InputAction.CallbackContext obj)
    {
        _recordAudio.StopRecording();
        _recordAudio.PlayRecording();
    }
}
