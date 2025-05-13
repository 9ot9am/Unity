using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;

    private void LateUpdate()
    {
        transform.position = player.position;
        transform.rotation = player.rotation;
    }
}
