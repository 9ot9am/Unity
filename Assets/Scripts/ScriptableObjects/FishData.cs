using UnityEngine;

[CreateAssetMenu(menuName = "OceanLens/Fish Data")]
public class FishData : ScriptableObject
{
    public FishType fishType; //enum type
    public string displyaName; //도감에 표시할 이름
    public Sprite maskedSprite;
}
