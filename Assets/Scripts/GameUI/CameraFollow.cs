using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("Цель, за которой следует камера (обычно игрок)")]
    public Transform target;

    [Tooltip("Смещение относительно цели")]
    public Vector3 offset = new Vector3(0, 0, -10);

    private float smoothSpeed; // Кэшируем для производительности

    private void Start()
    {
        // Кэшируем скорость сглаживания
        smoothSpeed = ConfigManager.Config.cameraSmoothSpeed;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Камера поднимается только если цель выше текущей позиции камеры
        if (target.position.y > transform.position.y)
        {
            Vector3 desiredPosition = new Vector3(
                transform.position.x,
                target.position.y,
                transform.position.z
            );

            // Сглаживаем движение камеры
            Vector3 smoothedPosition = Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed
            );

            transform.position = smoothedPosition;
        }
    }
}
