using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishSlotUI : MonoBehaviour
{
    [Header("Data")]
    public FishData fishData;       // ScriptableObject 레퍼런스

    [Header("Components")]
    public RawImage fishImage;
    public TMP_Text nameText;

    /// 초기화: 마스크 + "??"
    public void Start()
    {
        fishImage.texture = fishData.maskedSprite.texture;
        nameText.text = "??";
    }

    /// 해금: 찍은 사진 + 실제 이름
    public void Reveal(Texture2D photo)
    {
        fishImage.texture = photo;
        nameText.text = fishData.displyaName;
    }
}