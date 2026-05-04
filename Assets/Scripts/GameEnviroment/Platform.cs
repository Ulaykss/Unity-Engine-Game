using UnityEngine;

public class Platform : MonoBehaviour
{
    private GameConfig config;
    private bool used;

    private void Awake()
    {
        config = ConfigManager.Config;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Уничтожение в Deadzone
        if (collision.gameObject.CompareTag("Deadzone"))
        {
            Destroy(gameObject);
            return;
        }

        // Нас интересует только игрок
        if (!collision.gameObject.CompareTag("Player"))
            return;

        foreach (var contact in collision.contacts)
        {
            // Игрок приземлился СВЕРХУ
            if (contact.normal.y < 0.5f)
            {
                HandleTopCollision(collision);
                return;
            }

            // Игрок ударился СНИЗУ (шипы)
            if (contact.normal.y > -0.5f)
            {
                HandleBottomCollision(collision.gameObject);
                return;
            }
        }
    }

    private void HandleTopCollision(Collision2D collision)
    {
        if (used) return;
        used = true;

        if (collision.gameObject.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                config.platformJumpForce
            );
        }

        // Одноразовая платформа
        Destroy(gameObject);
    }

    private void HandleBottomCollision(GameObject player)
    {
        // Смерть игрока при ударе головой
        if (player.TryGetComponent<PlayerDeath>(out var death))
        {
            death.GameLoose();
        }
    }
}
