using UnityEngine;

public class InitialGround : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;

    private GameConfig config;
    private bool destroyed = false;
    private Collider2D groundCollider;

    private void Awake()
    {
        config = ConfigManager.Config;
        groundCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>().transform;

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        if (destroyed || player == null || mainCamera == null)
            return;

        // Удаляем начальную земля, когда она выходит за нижнюю границу видимости
        float cameraBottomY = mainCamera.transform.position.y - mainCamera.orthographicSize;

        if (transform.position.y < cameraBottomY - 1f)
        {
            destroyed = true;
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
            collision.gameObject.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                config.initialPlatformJumpForce
            );
        }
    }
}
