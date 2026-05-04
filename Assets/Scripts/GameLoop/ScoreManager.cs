using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Tooltip("UI элемент для отображения счета (в метрах)")]
    public TMP_Text scoreText;

    [Tooltip("UI элемент для отображения лучшего счета")]
    public TMP_Text bestScoreText;

    [Tooltip("Ссылка на объект игрока")]
    public Transform player;

    [Tooltip("Название уровня (если не указано, берется из SceneManager)")]
    public string levelName = "";

    private float startHeight;
    private float maxHeight;
    private float metersPerUnit;
    private float scoreMeters;
    private string distanceUnit;

    private PlayersSettingsManager playersSettings;
    private string currentLevelName;

    private void Start()
    {
        if (!player || !scoreText)
        {
            Debug.LogError("ScoreManager: Player or ScoreText is missing!");
            enabled = false;
            return;
        }

        // Получаем ссылку на PlayersSettings
        playersSettings = PlayersSettingsManager.Instance;

        var config = ConfigManager.Config;
        distanceUnit = config.distanceUnit;

        // 1 unit = X cm → X / 100 метров
        metersPerUnit = config.centimetersPerUnit / 100f;

        startHeight = player.position.y;
        maxHeight = startHeight;
        scoreMeters = 0f;

        // Определяем название уровня
        if (string.IsNullOrEmpty(levelName))
        {
            currentLevelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }
        else
        {
            currentLevelName = levelName;
        }

        Debug.Log($"ScoreManager initialized for level: {currentLevelName}");

        UpdateUI();
        UpdateBestScoreUI();
    }

    private void Update()
    {
        // Запоминаем максимальную достигнутую высоту
        if (player.position.y > maxHeight)
        {
            maxHeight = player.position.y;
            float heightDeltaUnits = maxHeight - startHeight;
            scoreMeters = heightDeltaUnits * metersPerUnit;

            UpdateUI();

            // Проверяем и сохраняем лучший счет
            if (playersSettings.SaveBestScore(scoreMeters))
            {
                Debug.Log($"New best score for '{currentLevelName}'! {scoreMeters:F2} {distanceUnit}");
                UpdateBestScoreUI();
            }
        }
    }

    private void UpdateUI()
    {
        // Текущий счет - 2 знака после запятой
        scoreText.text = $"{scoreMeters:F2} {distanceUnit}";
    }

    private void UpdateBestScoreUI()
    {
        if (bestScoreText != null)
        {
            bestScoreText.text = $"Best: {playersSettings.BestScoreFormatted}";
        }
    }

    public float CurrentScoreMeters => scoreMeters;

    // Получение лучшего счета для текущего уровня
    public float GetBestScoreForCurrentLevel()
    {
        return playersSettings.BestScoreMeters;
    }
}