using TMPro;
using UnityEngine;

public class SubtitleManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI subtitleText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var servertest = FindFirstObjectByType<ServerTest>();
        if (!servertest) return;
        
        servertest.audioReceived += (ac) => ServertestOnaudioReceived();
    }

    private void ServertestOnaudioReceived()
    {
        subtitleText.text = $"해마: {Random.Range(0, 100)}";
    }
}
