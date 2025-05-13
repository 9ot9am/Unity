using UnityEngine;

public class RecordAudio : MonoBehaviour
{
    private AudioClip recordedClip;
    [SerializeField] private AudioSource audioSource;

    private int recordedSamples;

    public void StartRecording()
    {
        string device = Microphone.devices[0];
        Debug.Log(device);
        
        int sampleRate = 44100;
        // can't be higher than 3599
        int lengthSec = 3599;

        recordedClip = Microphone.Start(device, false, lengthSec, sampleRate);
    }

    public void PlayRecording()
    {
        audioSource.clip = recordedClip;
        audioSource.Play();
    }

    public void StopRecording()
    {
        if (Microphone.IsRecording(Microphone.devices[0]))
        {
            Microphone.End(Microphone.devices[0]);
        }
        
        
        SaveAudioToWav.Save($"{recordedSamples} {Time.time}", recordedClip);
        recordedSamples++;
    }
}
