using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.STP;

public class ScreenBorderMovement : MonoBehaviour
{
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private float screenLeftEdge;
    private float screenRightEdge;
    private float lastAspect;

    void Awake()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        CalculateScreenEdges();
    }

    
    void Update()
    {
        // Пересчитываем границы, если изменился размер окна
        float currentAspect = (float)Screen.width / Screen.height;
        if (!Mathf.Approximately(currentAspect, lastAspect))
        {
            CalculateScreenEdges();
            lastAspect = currentAspect;
        }

        CheckScreenBounds();
    }

    /// <summary>
    /// Вычисляем границы экрана в мировых координатах.
    /// </summary>
    private void CalculateScreenEdges()
    {
        if (mainCamera == null) return;

        float screenAspect = (float)Screen.width / Screen.height;
        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * screenAspect;

        screenLeftEdge = mainCamera.transform.position.x - cameraWidth / 2f;
        screenRightEdge = mainCamera.transform.position.x + cameraWidth / 2f;
    }

    /// <summary>
    /// Проверяет, вышел ли игрок за границы экрана, и переносит его на противоположную сторону.
    /// </summary>
    private void CheckScreenBounds()
    {
        if (mainCamera == null) return;

        Vector3 pos = transform.position;
        float halfWidth = spriteRenderer != null ? spriteRenderer.bounds.extents.x : 0.2f;

        // Если игрок полностью вышел за правую границу
        if (pos.x - halfWidth > screenRightEdge)
        {
            pos.x = screenLeftEdge - halfWidth; // Перемещаем за левую границу
            transform.position = pos;
        }
        // Если игрок полностью вышел за левую границу
        else if (pos.x + halfWidth < screenLeftEdge)
        {
            pos.x = screenRightEdge + halfWidth; // Перемещаем за правую границу
            transform.position = pos;
        }
    }
}
