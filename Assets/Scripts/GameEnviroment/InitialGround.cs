using UnityEngine;

public class InitialGround : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScoreManager scoreManager;

    private GameConfig config;
    private bool destroyed = false;

    private void Awake()
    {
        config = ConfigManager.Config;
    }

    private void Update()
    {
        if (destroyed || scoreManager == null)
            return;

        if (scoreManager.CurrentScoreMeters >= config.destroyHeightThreshold)
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
