using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Слот АКТИВНЫХ способностей в главном меню (4 штуки).
/// Поддерживает:
/// - Перетаскивание из InventorySlot
/// - Перетаскивание между активными слотами
/// - Правый клик для очистки
/// - Бросание в пустую область для очистки
/// </summary>
public class ActiveSlotUI : MonoBehaviour, IDropHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("=== НАСТРОЙКА ===")]
    [Tooltip("Индекс этого слота (0, 1, 2 или 3)")]
    public int slotIndex;

    [Header("=== UI ЭЛЕМЕНТЫ ===")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text costText;
    [SerializeField] private GameObject emptyVisual;

    [Header("=== РЕСУРСЫ ===")]
    [Tooltip("Все Ability ScriptableObject-ы")]
    [SerializeField] private Ability[] availableAbilities;

    private Ability _currentAbility;
    private Canvas _rootCanvas;
    private CanvasGroup _canvasGroup;
    private GameObject _dragGhost;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rootCanvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        if (AbilityLoadout.Instance != null)
        {
            int savedIndex = AbilityLoadout.Instance.GetAbilityIndex(slotIndex);
            ApplyAbilityByIndex(savedIndex);
        }
    }

    // ─────────────────────────────────────────────────────────────
    // IDropHandler — приём способности
    // ─────────────────────────────────────────────────────────────

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        // Попытка получить способность из InventorySlot
        InventorySlot inventorySlot = eventData.pointerDrag.GetComponent<InventorySlot>();
        if (inventorySlot != null && inventorySlot.ability != null)
        {
            TryAssignAbility(inventorySlot.ability);
            return;
        }

        // Попытка получить способность из другого ActiveSlotUI
        ActiveSlotUI otherSlot = eventData.pointerDrag.GetComponent<ActiveSlotUI>();
        if (otherSlot != null && otherSlot._currentAbility != null)
        {
            TrySwapOrMove(otherSlot);
        }
    }

    // ─────────────────────────────────────────────────────────────
    // Drag & Drop для перемещения между активными слотами
    // ─────────────────────────────────────────────────────────────

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_currentAbility == null) return;

        _canvasGroup.blocksRaycasts = false;

        _dragGhost = new GameObject("DragGhost");
        _dragGhost.transform.SetParent(_rootCanvas.transform, false);
        _dragGhost.transform.SetAsLastSibling();

        Image ghostImage = _dragGhost.AddComponent<Image>();
        ghostImage.sprite = _currentAbility.abilityIcon;
        ghostImage.raycastTarget = false;

        RectTransform ghostRect = _dragGhost.GetComponent<RectTransform>();
        ghostRect.sizeDelta = _rectTransform.sizeDelta;

        UpdateGhostPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragGhost != null)
            UpdateGhostPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;

        if (_dragGhost != null)
        {
            Destroy(_dragGhost);
            _dragGhost = null;
        }

        // Если курсок НЕ над каким-либо дропабельным объектом → очищаем слот
        if (eventData.pointerEnter == null ||
            (eventData.pointerEnter.GetComponent<IDropHandler>() == null &&
             eventData.pointerEnter.GetComponent<ActiveSlotUI>() == null &&
             eventData.pointerEnter.GetComponent<InventorySlot>() == null))
        {
            ClearSlot();
        }
    }

    // ─────────────────────────────────────────────────────────────
    // Логика назначения/обмена
    // ─────────────────────────────────────────────────────────────

    private void TryAssignAbility(Ability newAbility)
    {
        if (newAbility == null) return;

        // Проверка: нет ли уже такой способности в другом активном слоте?
        if (IsAbilityAlreadyEquipped(newAbility, out ActiveSlotUI slotWithSameAbility))
        {
            Debug.Log($"[ActiveSlotUI] Способность {newAbility.abilityName} уже назначена в слот {slotWithSameAbility.slotIndex}");
            return;
        }

        // Назначение
        if (AbilityLoadout.Instance != null)
            AbilityLoadout.Instance.SetAbilityIndex(slotIndex, newAbility.abilityIndex);

        ApplyAbility(newAbility);
    }

    private void TrySwapOrMove(ActiveSlotUI sourceSlot)
    {
        if (sourceSlot == this)
        {
            // Перетащили на самого себя — ничего не делаем
            return;
        }

        Ability sourceAbility = sourceSlot._currentAbility;
        Ability targetAbility = this._currentAbility;

        // Меняем местами
        if (AbilityLoadout.Instance != null)
        {
            AbilityLoadout.Instance.SetAbilityIndex(sourceSlot.slotIndex, targetAbility?.abilityIndex ?? -1);
            AbilityLoadout.Instance.SetAbilityIndex(this.slotIndex, sourceAbility.abilityIndex);
        }

        // Обновляем UI
        sourceSlot.ApplyAbilityByIndex(targetAbility?.abilityIndex ?? -1);
        this.ApplyAbility(sourceAbility);
    }

    private bool IsAbilityAlreadyEquipped(Ability ability, out ActiveSlotUI foundSlot)
    {
        ActiveSlotUI[] allSlots = FindObjectsOfType<ActiveSlotUI>();
        foreach (var slot in allSlots)
        {
            if (slot != this && slot._currentAbility != null && slot._currentAbility.abilityIndex == ability.abilityIndex)
            {
                foundSlot = slot;
                return true;
            }
        }
        foundSlot = null;
        return false;
    }

    // ─────────────────────────────────────────────────────────────
    // Очистка слота
    // ─────────────────────────────────────────────────────────────

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        _currentAbility = null;

        if (AbilityLoadout.Instance != null)
            AbilityLoadout.Instance.ClearSlot(slotIndex);

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.color = Color.clear;
        }
        if (nameText != null) nameText.text = "";
        if (costText != null) costText.text = "";
        if (emptyVisual != null) emptyVisual.SetActive(true);
    }

    // ─────────────────────────────────────────────────────────────
    // Применение способности
    // ─────────────────────────────────────────────────────────────

    private void ApplyAbilityByIndex(int abilityIndex)
    {
        if (abilityIndex < 0 || availableAbilities == null)
        {
            ClearSlot();
            return;
        }

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
        }
        if (nameText != null)
            nameText.text = ability.abilityName;
        if (costText != null)
            costText.text = ability.ResourceCost.ToString();
        if (emptyVisual != null)
            emptyVisual.SetActive(false);
    }

    private void UpdateGhostPosition(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rootCanvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );
        _dragGhost.GetComponent<RectTransform>().localPosition = localPoint;
    }
}