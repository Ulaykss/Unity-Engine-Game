using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class PlayersSettingsManager : MonoBehaviour
{
    private static PlayersSettingsManager instance;
    private string settingsFilePath;

    // Хранилище лучших счетов для разных сцен
    private Dictionary<string, float> bestScoresByScene = new Dictionary<string, float>();
    private string currentSceneName;
    private string distanceUnit = "m";

    public static PlayersSettingsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayersSettingsManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("PlayersSettings");
                    instance = obj.AddComponent<PlayersSettingsManager>();
                }
            }
            return instance;
        }
    }

    public float BestScoreMeters => GetBestScoreForCurrentScene();
    public string BestScoreFormatted => $"{GetBestScoreForCurrentScene():F2} {distanceUnit}";

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        settingsFilePath = Path.Combine(Application.persistentDataPath, "PlayersSettings.ini");

        // Загружаем настройки при старте
        LoadSettings();
    }

    void Start()
    {
        var config = ConfigManager.Config;
        distanceUnit = config.distanceUnit;

        // Определяем текущую сцену
        SetCurrentScene();
    }

    // Установка текущей сцены
    private void SetCurrentScene()
    {
        currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Debug.Log($"Current scene: {currentSceneName}");
    }

    // Получение лучшего счета для текущей сцены
    private float GetBestScoreForCurrentScene()
    {
        if (string.IsNullOrEmpty(currentSceneName))
            SetCurrentScene();

        if (bestScoresByScene.ContainsKey(currentSceneName))
            return bestScoresByScene[currentSceneName];

        return 0f;
    }

    // Получение лучшего счета для конкретной сцены
    public float GetBestScoreForScene(string sceneName)
    {
        if (bestScoresByScene.ContainsKey(sceneName))
            return bestScoresByScene[sceneName];

        return 0f;
    }

    // Сохранение лучшего счета для текущей сцены
    public bool SaveBestScore(float scoreMeters)
    {
        float currentBest = GetBestScoreForCurrentScene();

        if (scoreMeters <= currentBest)
            return false;

        bestScoresByScene[currentSceneName] = scoreMeters;
        SaveSettings();
        return true;
    }

    // Сохранение лучшего счета для указанной сцены
    public bool SaveBestScoreForScene(string sceneName, float scoreMeters)
    {
        float currentBest = GetBestScoreForScene(sceneName);

        if (scoreMeters <= currentBest)
            return false;

        bestScoresByScene[sceneName] = scoreMeters;
        SaveSettings();
        return true;
    }

    // Загрузка настроек из INI файла
    private void LoadSettings()
    {
        if (!File.Exists(settingsFilePath))
        {
            // Создаем файл с настройками по умолчанию
            bestScoresByScene.Clear();
            SaveSettings();
            return;
        }

        try
        {
            string[] lines = File.ReadAllLines(settingsFilePath);
            string currentSection = "";

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string trimmedLine = line.Trim();

                // Проверяем секцию
                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    currentSection = trimmedLine.TrimStart('[').TrimEnd(']');
                    continue;
                }

                // Пропускаем комментарии
                if (trimmedLine.StartsWith(";"))
                    continue;

                // Парсим ключ=значение
                string[] parts = line.Split('=');
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    if (key == "BestScoreMeters" && !string.IsNullOrEmpty(currentSection))
                    {
                        if (float.TryParse(value, out float score))
                        {
                            bestScoresByScene[currentSection] = score;
                        }
                    }
                }
            }

            Debug.Log($"Loaded best scores for {bestScoresByScene.Count} scenes");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load settings: {e.Message}");
        }
    }

    // Сохранение настроек в INI файл
    private void SaveSettings()
    {
        try
        {
            string content = $"; Players Settings File\n" +
                            $"; Saved: {System.DateTime.Now}\n\n";

            // Сохраняем лучшие счета для каждой сцены
            foreach (var kvp in bestScoresByScene)
            {
                content += $"[{kvp.Key}]\n";
                content += $"BestScoreMeters={kvp.Value:F3}\n";
                content += $"; Last updated: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n";
            }

            // Если нет ни одной записи, создаем пустую секцию для Endless level
            if (bestScoresByScene.Count == 0)
            {
                content += $"[Endless level]\n";
                content += $"BestScoreMeters=0.000\n";
                content += $"; Last updated: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n";
            }

            File.WriteAllText(settingsFilePath, content);
            Debug.Log($"Settings saved to: {settingsFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save settings: {e.Message}");
        }
    }

    // Сброс лучшего счета для текущей сцены
    public void ResetBestScoreForCurrentScene()
    {
        if (!string.IsNullOrEmpty(currentSceneName))
        {
            bestScoresByScene[currentSceneName] = 0f;
            SaveSettings();
            Debug.Log($"Best score reset to 0 for scene: {currentSceneName}");
        }
    }

    // Сброс лучшего счета для указанной сцены
    public void ResetBestScoreForScene(string sceneName)
    {
        bestScoresByScene[sceneName] = 0f;
        SaveSettings();
        Debug.Log($"Best score reset to 0 for scene: {sceneName}");
    }

    // Сброс всех лучших счетов
    public void ResetAllBestScores()
    {
        bestScoresByScene.Clear();
        SaveSettings();
        Debug.Log("All best scores reset");
    }

    // Получение пути к файлу для отладки
    public string GetSettingsFilePath()
    {
        return settingsFilePath;
    }

    // Получение списка всех сцен с рекордами
    public string[] GetScenesWithBestScores()
    {
        return new List<string>(bestScoresByScene.Keys).ToArray();
    }
}