using UnityEngine;
using System.Collections.Generic;

public class FishDexUI : MonoBehaviour
{
    [Header("Setup")]
    public FishSlotUI[] SlotUis;

    /// 사진 찍힐 때 호출
    public void RevealFish(FishType type, Texture2D photo)
    {
        foreach (var slots in SlotUis)
        {
            if (slots.fishData.fishType == type)
            {
                slots.Reveal(photo);
            }
        }
    }
}