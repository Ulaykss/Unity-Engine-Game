using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Слот активной способности в игровой сцене (EndlessLevel и др.).
/// 
/// Логика при старте:
/// 1. Читает назначение из AbilityLoadout.
/// 2. Если слот пуст (-1) — подставляет способность по умолчанию:
///    для слота 0 → способность с наименьшим abilityIndex,
///    для слота 1 → следующая по индексу, и т.д.
/// 3. Если способностей меньше чем слотов — остаток остаётся пустым.
/// </summary>
public class Slot : MonoBehaviour
{
    [Header("=== НАСТРОЙКА СЛОТА ===")]
    [Tooltip("Индекс слота 0-3. Назначить вручную в инспекторе!")]
    public int slotIndex;

    [Header("=== UI ЭЛЕМЕНТЫ ===")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Text abilityNameText;
    [SerializeField] private Text costText;
    [SerializeField] private GameObject emptySlotVisual;

    [Header("=== РЕСУРСЫ ===")]
    [Tooltip("Все Ability ScriptableObject-ы. Тот же массив что в ActiveSlotUI и AbilitiesInventoryUI.")]
    [SerializeField] private Ability[] availableAbilities;

    // ─────────────────────────────────────────────────────────────
    // Приватные поля
    // ─────────────────────────────────────────────────────────────

    private Ability _currentAbility;
    private GameConfig _config;
    private PlayerController _playerController;
    private ProgressBar _progressBar;

    // ─────────────────────────────────────────────────────────────
    // Инициализация
    // ─────────────────────────────────────────────────────────────

    private void Start()
    {
        _config = ConfigManager.Config;
        _playerController = FindObjectOfType<PlayerController>();
        _progressBar = FindObjectOfType<ProgressBar>();

        // ДОБАВИТЬ: принудительная загрузка/проверка AbilityLoadout
        if (AbilityLoadout.Instance == null)
        {
            Debug.LogWarning("[Slot] AbilityLoadout.Instance не найден! Пытаемся найти существующий или создаём новый.");
            AbilityLoadout loadout = FindObjectOfType<AbilityLoadout>();
            if (loadout == null)
            {
                GameObject go = new GameObject("AbilityLoadout");
                loadout = go.AddComponent<AbilityLoadout>();
            }
        }

        int abilityIndex = GetAbilityIndexForThisSlot();
        Debug.Log($"[Slot {slotIndex}] Получен abilityIndex = {abilityIndex}");

        if (abilityIndex >= 0)
            ApplyAbilityByIndex(abilityIndex);
        else
            ShowEmpty();
    }

    /// <summary>
    /// Определяет abilityIndex для этого слота.
    /// Сначала смотрит в AbilityLoadout, если там -1 — подбирает дефолтную.
    /// </summary>
    private int GetAbilityIndexForThisSlot()
    {
        // Шаг 1: проверяем что назначено в главном меню
        if (AbilityLoadout.Instance != null)
        {
            int saved = AbilityLoadout.Instance.GetAbilityIndex(slotIndex);
            Debug.Log($"[Slot {slotIndex}] Загружено из AbilityLoadout: {saved}");

            if (saved >= 0)
            {
                // ДОПОЛНИТЕЛЬНАЯ ПРОВЕРКА: существует ли такая способность в availableAbilities?
                if (IsAbilityIndexValid(saved))
                    return saved;
                else
                    Debug.LogWarning($"[Slot {slotIndex}] Способность с индексом {saved} не найдена в availableAbilities!");
            }
        }
        else
        {
            Debug.LogError($"[Slot {slotIndex}] AbilityLoadout.Instance == null!");
        }

        // Шаг 2: дефолт — только если в сохранении нет данных или они невалидны
        Debug.Log($"[Slot {slotIndex}] Используем DEFAULT способность");
        return GetDefaultAbilityIndexForSlot();
    }

    private bool IsAbilityIndexValid(int abilityIndex)
    {
        if (availableAbilities == null) return false;
        foreach (var ab in availableAbilities)
        {
            if (ab != null && ab.abilityIndex == abilityIndex)
                return true;
        }
        return false;
    }

    private int GetDefaultAbilityIndexForSlot()
    {
        if (availableAbilities == null || availableAbilities.Length == 0)
            return -1;

        // Сортируем по abilityIndex
        Ability[] sorted = (Ability[])availableAbilities.Clone();
        System.Array.Sort(sorted, (a, b) =>
        {
            if (a == null) return 1;
            if (b == null) return -1;
            return a.abilityIndex.CompareTo(b.abilityIndex);
        });

        if (slotIndex < sorted.Length && sorted[slotIndex] != null)
            return sorted[slotIndex].abilityIndex;

        return -1;
    }

    // ─────────────────────────────────────────────────────────────
    // Применение способности
    // ─────────────────────────────────────────────────────────────

    private void ApplyAbilityByIndex(int abilityIndex)
    {
        if (availableAbilities == null) { ShowEmpty(); return; }

        // Ищем в массиве способность с нужным abilityIndex
        foreach (var ab in availableAbilities)
        {
            if (ab != null && ab.abilityIndex == abilityIndex)
            {
                ApplyAbility(ab);
                return;
            }
        }

        Debug.LogWarning($"[Slot {slotIndex}] Способность с abilityIndex={abilityIndex} не найдена в массиве!");
        ShowEmpty();
    }

    private void ApplyAbility(Ability ability)
    {
        _currentAbility = ability;

        // Инициализируем способность: передаём ей ссылки на игрока и шкалу ресурсов
        if (_playerController != null)
            _currentAbility.Initialize(_playerController.gameObject, _config, _progressBar);

        // Обновляем UI
        if (iconImage != null)
        {
            iconImage.sprite = _currentAbility.abilityIcon;
            iconImage.color = Color.white;
            iconImage.gameObject.SetActive(true);
        }
        if (abilityNameText != null)
            abilityNameText.text = _currentAbility.abilityName;
        if (costText != null)
            costText.text = _currentAbility.ResourceCost.ToString();
        if (emptySlotVisual != null)
            emptySlotVisual.SetActive(false);
    }

    private void ShowEmpty()
    {
        _currentAbility = null;

        if (emptySlotVisual != null) emptySlotVisual.SetActive(true);
        if (iconImage != null) iconImage.gameObject.SetActive(false);
        if (abilityNameText != null) abilityNameText.text = "Empty";
        if (costText != null) costText.text = "";
    }

    // ─────────────────────────────────────────────────────────────
    // Активация (вызывается из PlayerController)
    // ─────────────────────────────────────────────────────────────

    public void Activate(int index)
    {
        if (index != slotIndex) return;

        if (_currentAbility == null)
        {
            Debug.Log($"[Slot {slotIndex}] Пуст.");
            return;
        }

        int cost = _currentAbility.ResourceCost;

        if (_progressBar != null && !_progressBar.HasEnoughNegative(cost))
        {
            Debug.Log($"[Slot {slotIndex}] Недостаточно ресурсов. Нужно: {cost}, есть: {_progressBar.GetCurrentResource()}");
            return;
        }

        if (_progressBar != null)
            _progressBar.UseNegative(cost);

        _currentAbility.Execute(_playerController.gameObject);

        StartCoroutine(PressFeedback());
    }

    // ─────────────────────────────────────────────────────────────
    // Визуальный фидбек
    // ─────────────────────────────────────────────────────────────

    private IEnumerator PressFeedback()
    {
        if (iconImage == null || _config == null) yield break;

        Color original = iconImage.color;
        iconImage.color = _config.pressedColor;
        yield return new WaitForSeconds(_config.pressDuration);
        iconImage.color = original;
    }
}