using UnityEngine;

public class SpriteFlip : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        // Получаем направление из PlayerController
        if (playerController != null)
        {
            UpdateFacingDirection(playerController.GetDirection());
        }
    }

    private void UpdateFacingDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > 0.1f)
        {
            bool shouldFaceRight = direction.x < 0;

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = !shouldFaceRight;
            }
        }
    }
}