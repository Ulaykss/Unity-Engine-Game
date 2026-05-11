using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Слот АКТИВНЫХ способностей в главном меню (4 штуки внизу).
/// Принимает перетаскивание из InventorySlot и сохраняет выбор в AbilityLoadout.
/// </summary>
public class ActiveSlotUI : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [Header("=== НАСТРОЙКА ===")]
    [Tooltip("Индекс этого слота (0, 1, 2 или 3)")]
    public int slotIndex;

    [Header("=== UI ЭЛЕМЕНТЫ ===")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text costText;
    [SerializeField] private GameObject emptyVisual; // Объект, видимый когда слот пуст

    [Header("=== РЕСУРСЫ ===")]
    [Tooltip("Все доступные способности (тот же массив что и везде)")]
    [SerializeField] private Ability[] availableAbilities;

    // Текущая назначенная способность
    private Ability _currentAbility;

    private void Start()
    {
        // Восстанавливаем назначение из AbilityLoadout при открытии меню
        if (AbilityLoadout.Instance != null)
        {
            int savedIndex = AbilityLoadout.Instance.GetAbilityIndex(slotIndex);
            ApplyAbilityByIndex(savedIndex);
        }
    }

    // ── IDropHandler — вызывается когда на нас бросают объект ─────

    public void OnDrop(PointerEventData eventData)
    {
        // Получаем InventorySlot, с которого началось перетаскивание
        // pointerDrag — объект, с которого началось перетаскивание
        if (eventData.pointerDrag == null) return;

        InventorySlot source = eventData.pointerDrag.GetComponent<InventorySlot>();
        if (source == null) return;

        ReceiveDrop(source.ability);
    }

    /// <summary>Вызывается из InventorySlot.OnEndDrag тоже (запасной путь).</summary>
    public void ReceiveDrop(Ability ability)
    {
        if (ability == null) return;

        // Сохраняем в глобальный loadout
        if (AbilityLoadout.Instance != null)
        {
            AbilityLoadout.Instance.SetAbilityIndex(slotIndex, ability.abilityIndex);
        }

        // Обновляем отображение
        ApplyAbility(ability);
    }

    // ── IPointerClickHandler — правый клик очищает слот ──────────

    public void OnPointerClick(PointerEventData eventData)
    {
        // Правая кнопка мыши — очистить слот
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ClearSlot();
        }
    }

    // ── Приватные методы ──────────────────────────────────────────

    private void ApplyAbilityByIndex(int abilityIndex)
    {
        if (abilityIndex < 0 || availableAbilities == null)
        {
            ClearSlot();
            return;
        }

        // Ищем способность с нужным abilityIndex в массиве
        foreach (var ab in availableAbilities)
        {
            if (ab != null && ab.abilityIndex == abilityIndex)
            {
                ApplyAbility(ab);
                return;
            }
        }

        ClearSlot();
    }

    private void ApplyAbility(Ability ability)
    {
        _currentAbility = ability;

        if (iconImage != null)
        {
            iconImage.sprite = ability.abilityIcon;
            iconImage.color = Color.white;
            iconImage.gameObject.SetActive(true);
        }

        if (nameText != null)
            nameText.text = ability.abilityName;

        if (costText != null)
            costText.text = ability.ResourceCost.ToString();

        if (emptyVisual != null)
            emptyVisual.SetActive(false);
    }

    public void ClearSlot()
    {
        _currentAbility = null;

        if (AbilityLoadout.Instance != null)
            AbilityLoadout.Instance.ClearSlot(slotIndex);

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.gameObject.SetActive(false);
        }

        if (nameText != null) nameText.text = "";
        if (costText != null) costText.text = "";
        if (emptyVisual != null) emptyVisual.SetActive(true);
    }
}
