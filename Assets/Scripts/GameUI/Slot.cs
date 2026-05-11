using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Слот активной способности в игровой сцене (EndlessLevel и др.).
/// При старте читает назначение из AbilityLoadout и инициализирует способность.
/// </summary>
public class Slot : MonoBehaviour
{
    [Header("=== НАСТРОЙКА СЛОТА ===")]
    public int slotIndex;           // Индекс слота (0-3) — назначить в инспекторе!

    [Header("=== UI ЭЛЕМЕНТЫ ===")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Text abilityNameText;
    [SerializeField] private Text costText;
    [SerializeField] private GameObject emptySlotVisual;

    [Header("=== РЕСУРСЫ ===")]
    [Tooltip("Все Ability ScriptableObject-ы (тот же массив что и везде)")]
    [SerializeField] private Ability[] availableAbilities;

    // ── Приватные поля ─────────────────────────────────────────────
    private Ability currentAbility;
    private GameConfig config;
    private PlayerController playerController;
    private ProgressBar negativeResourceBar;

    // ── Инициализация ──────────────────────────────────────────────

    void Start()
    {
        config = ConfigManager.Config;
        playerController = FindObjectOfType<PlayerController>();
        negativeResourceBar = FindObjectOfType<ProgressBar>();

        // Читаем, какая способность назначена в этот слот
        int abilityIndex = -1;

        if (AbilityLoadout.Instance != null)
        {
            abilityIndex = AbilityLoadout.Instance.GetAbilityIndex(slotIndex);
        }
        else
        {
            Debug.LogWarning($"[Slot {slotIndex}] AbilityLoadout не найден! " +
                             "Убедись, что объект с компонентом AbilityLoadout есть в сцене.");
        }

        // Применяем
        if (abilityIndex >= 0)
            SetAbilityByAbilityIndex(abilityIndex);
        else
            ClearSlot();
    }

    // ── Установка способности ──────────────────────────────────────

    /// <summary>
    /// Ищет в массиве способность с нужным abilityIndex и устанавливает её.
    /// </summary>
    private void SetAbilityByAbilityIndex(int abilityIndex)
    {
        if (availableAbilities == null) { ClearSlot(); return; }

        foreach (var ab in availableAbilities)
        {
            if (ab != null && ab.abilityIndex == abilityIndex)
            {
                ApplyAbility(ab);
                return;
            }
        }

        Debug.LogWarning($"[Slot {slotIndex}] Способность с abilityIndex={abilityIndex} не найдена!");
        ClearSlot();
    }

    private void ApplyAbility(Ability ability)
    {
        currentAbility = ability;

        // Инициализируем способность (передаём ей ссылки на игрока и ресурсы)
        if (playerController != null)
        {
            currentAbility.Initialize(playerController.gameObject, config, negativeResourceBar);
        }

        // Обновляем UI
        if (iconImage != null)
        {
            iconImage.sprite = currentAbility.abilityIcon;
            iconImage.color = Color.white;
            iconImage.gameObject.SetActive(true);
        }

        if (abilityNameText != null)
            abilityNameText.text = currentAbility.abilityName;

        if (costText != null)
            costText.text = currentAbility.ResourceCost.ToString();

        if (emptySlotVisual != null)
            emptySlotVisual.SetActive(false);
    }

    // ── Очистка слота ──────────────────────────────────────────────

    public void ClearSlot()
    {
        currentAbility = null;

        if (emptySlotVisual != null) emptySlotVisual.SetActive(true);
        if (iconImage != null) iconImage.gameObject.SetActive(false);
        if (abilityNameText != null) abilityNameText.text = "Empty";
        if (costText != null) costText.text = "";
    }

    // ── Активация (вызывается из PlayerController) ─────────────────

    public void Activate(int index)
    {
        if (index != slotIndex) return;

        if (currentAbility == null)
        {
            Debug.Log($"Слот {slotIndex} пуст!");
            return;
        }

        int cost = currentAbility.ResourceCost;

        if (negativeResourceBar != null && !negativeResourceBar.HasEnoughNegative(cost))
        {
            Debug.Log($"[Слот {slotIndex}] Недостаточно ресурсов! " +
                      $"Нужно: {cost}, есть: {negativeResourceBar.GetCurrentResource()}");
            return;
        }

        if (negativeResourceBar != null)
            negativeResourceBar.UseNegative(cost);

        currentAbility.Execute(playerController.gameObject);

        StartCoroutine(PressFeedback());
    }

    // ── Визуальный фидбек ──────────────────────────────────────────

    private IEnumerator PressFeedback()
    {
        if (iconImage != null && config != null)
        {
            Color originalColor = iconImage.color;
            iconImage.color = config.pressedColor;
            yield return new WaitForSeconds(config.pressDuration);
            iconImage.color = originalColor;
        }
    }
}