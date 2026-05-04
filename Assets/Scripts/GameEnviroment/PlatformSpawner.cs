using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;

    private GameConfig config;
    private GameObject currentPlatform;

    private void Start()
    {
        config = ConfigManager.Config;

        if (!mainCamera)
            mainCamera = Camera.main;

        if (!player)
        {
            Debug.LogError("PlatformSpawner: Player reference is missing!");
            enabled = false;
            return;
        }

        SpawnPlatform();
    }

    private void Update()
    {
        // Если текущей платформы больше нет — создаём следующую
        if (currentPlatform == null)
        {
            SpawnPlatform();
        }
    }

    private void SpawnPlatform()
    {
        if (!platformPrefab)
        {
            Debug.LogError("PlatformSpawner: platformPrefab is NULL or destroyed!");
            return;
        }

        // Высота — строго выше игрока
        float y = player.position.y + config.singlePlatformHeightOffset;

        // Горизонталь — от ширины экрана с выходом за границы
        float screenHalfWidth = mainCamera.orthographicSize * Screen.width / Screen.height;
        float overflow = screenHalfWidth * config.platformHorizontalOverflow;

        float x = Random.Range(
            -screenHalfWidth - overflow,
             screenHalfWidth + overflow
        );

        Vector3 spawnPosition = new Vector3(x, y, 0f);

        currentPlatform = Instantiate(
            platformPrefab,
            spawnPosition,
            Quaternion.identity
        );
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!player || !mainCamera) return;

        if (config == null)
            config = ConfigManager.Config;

        if (config == null) return;

        float screenHalfWidth = mainCamera.orthographicSize * Screen.width / Screen.height;
        float overflow = screenHalfWidth * config.platformHorizontalOverflow;

        float y = player.position.y + config.singlePlatformHeightOffset;

        Gizmos.color = Color.green;
        Vector3 center = new Vector3(0, y, 0);
        Vector3 size = new Vector3(
            (screenHalfWidth + overflow) * 2f,
            0.2f,
            0.1f
        );

        Gizmos.DrawWireCube(center, size);
    }
#endif
}
