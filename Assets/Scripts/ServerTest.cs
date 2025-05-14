using System;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class ServerTest: MonoBehaviour
{
    public event Action<AudioClip> audioReceived;
    public AudioSource audioSource;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TestServerAudio();
        }
    }
    
    public IEnumerator UploadWavFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found at path: " + filePath);
            yield break;
        }

        using (UnityWebRequest fileReader = UnityWebRequest.Get(filePath))
        {
            // Important: Set Load Type equivalent for runtime loading if possible,
            // or ensure the AudioClip is fully loaded before GetData.
            // For GetAudioClip, data should be accessible after download.

            yield return fileReader.SendWebRequest();
            byte[] binaryAudioData = fileReader.downloadHandler.data;
            Debug.Log(binaryAudioData.Length);
            WWWForm formData = new WWWForm();
            formData.AddBinaryData(
                "file",
                binaryAudioData,
                "aaa.wav",
                "audio/wav");
            


            // Create a new UnityWebRequest for POSTing or PUTting
            // For PUT:
            // UnityWebRequest uwr = UnityWebRequest.Put(uploadURL, File.ReadAllBytes(filePath)); // Reads all bytes into memory
            // Or, more efficiently for large files with UploadHandlerFile:
            UnityWebRequest uwr = UnityWebRequest.Post(Constants.AIUrl + "/api/audio/enhanced", formData); // Or kHttpVerbPOST

            // Setup UploadHandlerFile - this streams the file from disk
            //uwr.uploadHandler = new UploadHandlerFile(filePath);
            // Optionally set content type if the server requires it for raw uploads
            //uwr.uploadHandler.contentType = "application/octet-stream"; // Or "application/octet-stream"

            // Setup DownloadHandler to get the server's response
            uwr.downloadHandler = new DownloadHandlerAudioClip(Constants.AIUrl + "/api/audio/enhanced", AudioType.WAV);

            // Set any other headers if needed (e.g., authorization)
            // uwr.SetRequestHeader("Authorization", "Bearer YOUR_TOKEN");
            //uwr.SetRequestHeader("Content-Type", "application/octet-stream"); // Often needed for PUT or raw POST

            Debug.Log("Uploading " + filePath + " to " + Constants.AIUrl + "/api/audio/enhanced");

            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError ||
                uwr.result == UnityWebRequest.Result.ProtocolError ||
                uwr.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError("Upload Error: " + uwr.error);
                Debug.LogError("Server Response: " + (uwr.downloadHandler?.text ?? "No response text"));
            }
            else
            {
                Debug.Log("Upload successful!");
                
                AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
                audioReceived?.Invoke(clip);
                // Optionally, delete the local file after successful upload
                // File.Delete(filePath);
            }

            uwr.Dispose();
        }
    }


    public void TestServerAudio()
    {
        Debug.Log("TestServerAudio");
        StartCoroutine(GetAudioClipFromServer.SendWav("20250513_203122_0_3.107078.wav", OnComplted));
    }

    private void OnComplted(AudioClip obj)
    {
        audioSource.PlayOneShot(obj);
        audioReceived?.Invoke(obj);
    }
    
    public void SendTestForm()
    {
        StartCoroutine(Send("Test from unity", "password", "Test from unity"));
    }

    public class UserSubmitForm
    {
        public string username;
        public string password;
        public string nickname;
    }

    public static IEnumerator Send(string name, string passwd, string nickname)
    {
        var serialized = JsonConvert.SerializeObject(
            new UserSubmitForm
            {
                username = name,
                password = passwd,
                nickname = nickname
            });
        /*
        WWWForm formData = new WWWForm();
        formData.AddField("username", name);
        formData.AddField("password", passwd);
        formData.AddField("nickname", nickname);
        */
        
        
        var webRequest = UnityWebRequest.Post($"{Constants.AIUrl}/api/auth/signup", serialized, "application/json");
        
        yield return webRequest.SendWebRequest();
        
        Debug.Log(webRequest.downloadHandler.text);

        string returnText = webRequest.downloadHandler.text;
        Debug.Log($"{returnText}");
    }
}
