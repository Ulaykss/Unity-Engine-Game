using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Слот активной способности в игровой сцене (EndlessLevel и др.).
/// Активация: клавиши 1-4 с клавиатуры ИЛИ нажатие мышью/тапом на слот.
/// </summary>
public class Slot : MonoBehaviour, IPointerDownHandler
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
    [Tooltip("Все Ability ScriptableObject-ы.")]
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

        if (AbilityLoadout.Instance == null)
        {
            AbilityLoadout loadout = FindObjectOfType<AbilityLoadout>();
            if (loadout == null)
            {
                GameObject go = new GameObject("AbilityLoadout");
                loadout = go.AddComponent<AbilityLoadout>();
            }
        }

        int abilityIndex = GetAbilityIndexForThisSlot();

        if (abilityIndex >= 0)
            ApplyAbilityByIndex(abilityIndex);
        else
            ShowEmpty();
    }

    private int GetAbilityIndexForThisSlot()
    {
        if (AbilityLoadout.Instance != null)
        {
            int saved = AbilityLoadout.Instance.GetAbilityIndex(slotIndex);
            if (saved >= 0 && IsAbilityIndexValid(saved))
                return saved;
        }

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
    // Активация по нажатию мышью / тапом
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Вызывается автоматически при нажатии мышью или тапе на этот объект.
    /// IPointerDownHandler работает и на ПК и на мобильных устройствах.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        TryActivate();
    }

    // ─────────────────────────────────────────────────────────────
    // Активация по индексу (от PlayerController с клавиатуры)
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Вызывается из PlayerController при нажатии клавиши 1/2/3/4.
    /// </summary>
    public void Activate(int index)
    {
        if (index != slotIndex) return;
        TryActivate();
    }

    // ─────────────────────────────────────────────────────────────
    // Общая логика активации
    // ─────────────────────────────────────────────────────────────

    private void TryActivate()
    {
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
    // Применение способности
    // ─────────────────────────────────────────────────────────────

    private void ApplyAbilityByIndex(int abilityIndex)
    {
        if (availableAbilities == null) { ShowEmpty(); return; }

        foreach (var ab in availableAbilities)
        {
            if (ab != null && ab.abilityIndex == abilityIndex)
            {
                ApplyAbility(ab);
                return;
            }
        }

        ShowEmpty();
    }

    private void ApplyAbility(Ability ability)
    {
        _currentAbility = ability;

        if (_playerController != null)
            _currentAbility.Initialize(_playerController.gameObject, _config, _progressBar);

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