using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class GetAudioClipFromServer
{
    public static IEnumerator SendWav(string filename, Action<AudioClip> onComplted)
    {
        var webRequest = UnityWebRequestMultimedia.GetAudioClip($"{Constants.Url}/get-file/{filename}", AudioType.WAV);
        Debug.Log(webRequest.uri.ToString());
        
        webRequest.SetRequestHeader("Content-Type", "text/plain");
        
        yield return webRequest.SendWebRequest();

        AudioClip clip = GetAudioClip(webRequest);
        onComplted.Invoke(clip);
    }
    
    private static AudioClip GetAudioClip(UnityWebRequest webRequest)
    {
        var audioClip = DownloadHandlerAudioClip.GetContent(webRequest);
        audioClip.name = Path.GetFileName(webRequest.url);
        return audioClip;
    }
}
