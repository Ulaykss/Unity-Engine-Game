using UnityEngine;
using UnityEngine.UI;

public class FollowUI : MonoBehaviour
{
    public Transform playerTransform;
    public Vector3 offset = new Vector3(0, 0.5f, 0); // Значение по умолчанию

    void Update()
    {
        transform.position = playerTransform.position + offset;
    }
}