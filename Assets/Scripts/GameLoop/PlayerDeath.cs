using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    private GameConfig config;
    private float originalSpeed;

    void Start()
    {
        config = ConfigManager.Config;
        originalSpeed = config.horizontalSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Смерть от Deadzone
        if (collision.gameObject.CompareTag("Deadzone"))
        {
            GameLoose();
        }
    }

    public void GameLoose()
    {
        ResetToDefaults();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ResetToDefaults()
    {
        config.horizontalSpeed = originalSpeed;
    }
}
