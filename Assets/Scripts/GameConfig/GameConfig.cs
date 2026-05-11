using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Configuration")]
public class GameConfig : ScriptableObject
{
    void Awake()
    {
        Application.targetFrameRate = 120;
    }

    [Header("=== ИГРОК ===")]
    [Tooltip("Горизонтальная скорость перемещения игрока")]
    public float horizontalSpeed = 3f;
    [Tooltip("Начальный уровень негатива")]
    public float initialNegative = 0;
    [Tooltip("Максимальный уровень негатива")]
    public float maxNegative = 10;
    [Tooltip("Время полного заполнения шкалы")]
    public float fillDuration = 1;

    [Header("=== ПЛАТФОРМЫ ===")]
    [Tooltip("Сила прыжка от начальной платформы")]
    public float initialPlatformJumpForce = 6f;
    [Tooltip("Сила прыжка от появляющиеся платформы")]
    public float platformJumpForce = 10f;
    [Tooltip("Фиксированная высота следующей платформы над игроком")]
    public float singlePlatformHeightOffset = 12f;
    [Tooltip("Насколько платформа может выходить за экран (0.5 = наполовину)")]
    [Range(0f, 0.5f)]
    public float platformHorizontalOverflow = 0.3f;

    [Header("=== НАЧАЛЬНАЯ ЗЕМЛЯ ===")]
    [Tooltip("Высота, при которой начальная земля пропадает")]
    public float destroyHeightThreshold = 2f;

    [Header("=== КАМЕРА ===")]
    [Range(0.01f, 1f), Tooltip("Скорость следования камеры (0-1)")]
    public float cameraSmoothSpeed = 0.25f;

    [Header("=== СЧЕТ ===")]
    [Tooltip("Символ единицы измерения")]
    public string distanceUnit = "m";
    [Tooltip("Сколько сантиметров в одной игровой единице (Unity unit)")]
    public float centimetersPerUnit = 40f;

    [Header("=== PROGRESS BAR ===")]
    [Tooltip("Ускорение в начале сегмента")]
    public float progressbarAcceleration = 2f;
    [Tooltip("Замедление в конце сегмента")]
    public float progressbarDeceleration = 3f;
    [Tooltip("Максимальная скорость")]
    public float progressbarMaxSpeed = 5f;
    [Tooltip("Буст при переходе на новый сегмент")]
    public float progressbarSegmentSwitchBoost = 1.2f;

    [Header("=== НАСТРОЙКА ПАРАМЕТРОВ СПОСОБНОСТЕЙ ===")]
    [Tooltip("Сила двойного прыжка")]
    public float doubleJumpForce = 5f;
    [Tooltip("Сила рывка вверх")]
    public float upDashForce = 17f;
    [Tooltip("Скорость горизонтального перемещения")]
    public float sideDashSpeed = 7f;
    [Tooltip("Время действия способности SideDash")]
    public float sideDashTime = 0.5f;
    [Tooltip("Сила крика")]
    public float screamForce = 1f;
    [Tooltip("Множитель скорости восстановления негатива после крика")]
    public float screamRegenMultiplier = 1.5f;
    [Tooltip("Длительность ускоренного восстановления (сек)")]
    public float screamRegenDuration = 3f;

    [Header("=== СТОИМОСТИ СПОСОБНОСТЕЙ ===")]
    [Tooltip("Стоимость двойного прыжка")]
    public int doubleJumpResourceCost = 5;
    [Tooltip("Стоимость рывка вверх")]
    public int upDashResourceCost = 8;
    [Tooltip("Стоимость рывка в сторону")]
    public int sideDashResourceCost = 5;
    [Tooltip("Стоимость крика под себя")]
    public int screamResourceCost = 1;

    [Header("=== UI СПОСОБНОСТЕЙ ===")]
    [Tooltip("Степень затенения иконки при нажатии")]
    public Color pressedColor = new Color(0.6f, 0.6f, 0.6f, 1f);
    [Tooltip("Время действия затенения")]
    public float pressDuration = 0.1f;

    //public int GetCostFor(AbilityType type)
    //{
    //    return type switch
    //    {
    //        AbilityType.DoubleJump => doubleJumpResourceCost,
    //        AbilityType.UpDash => upDashResourceCost,
    //        AbilityType.SideDash => sideDashResourceCost,
    //        AbilityType.Scream => screamResourceCost,
    //        _ => 0
    //    };
    //}

    public void ApplyIni(GameConfigIni ini)
    {
        // === ИГРОК ===
        horizontalSpeed = ini.GetFloat("horizontalSpeed", horizontalSpeed);
        initialNegative = ini.GetFloat("initialNegative", initialNegative);
        maxNegative = ini.GetFloat("maxNegative", maxNegative);
        fillDuration = ini.GetFloat("fillDuration", fillDuration);

        // === ПЛАТФОРМЫ ===
        platformJumpForce = ini.GetFloat("platformJumpForce", platformJumpForce);
        singlePlatformHeightOffset = ini.GetFloat("singlePlatformHeightOffset", singlePlatformHeightOffset);
        platformHorizontalOverflow = ini.GetFloat("platformHorizontalOverflow", platformHorizontalOverflow);

        // === НАЧАЛЬНАЯ ЗЕМЛЯ ===
        destroyHeightThreshold = ini.GetFloat("destroyHeightThreshold", destroyHeightThreshold);

        // === КАМЕРА ===
        cameraSmoothSpeed = ini.GetFloat("cameraSmoothSpeed", cameraSmoothSpeed);

        // === СЧЕТ ===
        distanceUnit = ini.GetString("distanceUnit", distanceUnit);
        centimetersPerUnit = ini.GetFloat("centimetersPerUnit", centimetersPerUnit);

        // === PROGRESS BAR ===
        progressbarAcceleration = ini.GetFloat("progressbarAcceleration", progressbarAcceleration);
        progressbarDeceleration = ini.GetFloat("progressbarDeceleration", progressbarDeceleration);
        progressbarMaxSpeed = ini.GetFloat("progressbarMaxSpeed", progressbarMaxSpeed);
        progressbarSegmentSwitchBoost = ini.GetFloat("progressbarSegmentSwitchBoost", progressbarSegmentSwitchBoost);

        // === СПОСОБНОСТИ ===
        doubleJumpForce = ini.GetFloat("doubleJumpForce", doubleJumpForce);
        upDashForce = ini.GetFloat("upDashForce", upDashForce);
        sideDashSpeed = ini.GetFloat("sideDashSpeed", sideDashSpeed);
        sideDashTime = ini.GetFloat("sideDashTime", sideDashTime);
        screamForce = ini.GetFloat("screamForce", screamForce);
        screamRegenMultiplier = ini.GetFloat("screamRegenMultiplier", screamRegenMultiplier);
        screamRegenDuration = ini.GetFloat("screamRegenDuration", screamRegenDuration);

        // === СТОИМОСТИ ===
        doubleJumpResourceCost = ini.GetInt("doubleJumpResourceCost", doubleJumpResourceCost);
        upDashResourceCost = ini.GetInt("upDashResourceCost", upDashResourceCost);
        sideDashResourceCost = ini.GetInt("sideDashResourceCost", sideDashResourceCost);
        screamResourceCost = ini.GetInt("screamResourceCost", screamResourceCost);

        // === UI ===
        pressedColor = ini.GetColor("pressedColor", pressedColor);
        pressDuration = ini.GetFloat("pressDuration", pressDuration);
    }
}
