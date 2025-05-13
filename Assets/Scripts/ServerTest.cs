using UnityEngine;

public class ServerTest: MonoBehaviour
{
    public AudioSource audioSource;
    
    public void TestServerAudio()
    {
        StartCoroutine(GetAudioClipFromServer.SendWav("20250513_203122_0_3.107078.wav", OnComplted));
    }

    private void OnComplted(AudioClip obj)
    {
        audioSource.PlayOneShot(obj);
    }
}
