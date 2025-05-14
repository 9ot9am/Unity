using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FishBehaviour : MonoBehaviour
{
    [Tooltip("이 오브젝트가 어떤 물고기인지 선택하세요")]
    public FishType fishType;          // enum 사용
    [Tooltip("도감에 표시할 실제 이름")]
    public string displayName;         // “Clownfish” 등
}

