using System.IO;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    private static ConfigManager _instance;
    public static ConfigManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<ConfigManager>();
                if (_instance == null)
                {
                    var go = new GameObject("GameConfigManager");
                    _instance = go.AddComponent<ConfigManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    [SerializeField] private GameConfig gameConfig;

    private const string IniFileName = "GameConfig.ini";

    public static GameConfig Config => Instance.gameConfig;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            LoadConfig();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    // =============================
    // LOAD
    // =============================

    private void LoadConfig()
    {
        if (gameConfig == null)
        {
            gameConfig = Resources.Load<GameConfig>("GameConfig");

            if (gameConfig == null)
            {
                Debug.LogError("GameConfig.asset не найден в Resources");
                return;
            }
        }

        LoadIniIfExists();
    }

    private void LoadIniIfExists()
    {
        string path = GetIniPath();

        if (!File.Exists(path))
        {
            Debug.Log("INI не найден, используется ScriptableObject");
            return;
        }

        string text = File.ReadAllText(path);
        GameConfigIni ini = GameConfigIni.Load(text);
        gameConfig.ApplyIni(ini);

        Debug.Log("INI применён к GameConfig");
    }

    // =============================
    // SAVE / GENERATE
    // =============================

    [ContextMenu("Generate INI from GameConfig")]
    public void GenerateIniFromGameConfig()
    {
        GameConfigIni ini = GameConfigIni.GenerateFromGameConfig(gameConfig);
        ini.Save(GetIniPath());

        Debug.Log("INI сгенерирован из GameConfig");
    }

    [ContextMenu("Save current config to INI")]
    public void SaveIni()
    {
        GenerateIniFromGameConfig();
    }

    // =============================
    // PATH
    // =============================

    private string GetIniPath()
    {
        // Всегда в редакторе используем persistentDataPath
        if (Application.isEditor)
        {
            return Path.Combine(Application.persistentDataPath, IniFileName);
        }

        // В сборке - рядом с .exe
        string exeFolder = Path.GetDirectoryName(Application.dataPath);
        return Path.Combine(exeFolder, IniFileName);
    }
}
