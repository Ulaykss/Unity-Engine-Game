using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Слот ИНВЕНТАРЯ способностей (сетка в главном меню).
/// Отображает одну Ability ScriptableObject.
/// Поддерживает перетаскивание на ActiveSlotUI.
/// </summary>
public class InventorySlot : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("=== ДАННЫЕ ===")]
    [Tooltip("Способность, которую отображает этот слот. Назначается вручную или через AbilitiesInventoryUI.")]
    public Ability ability; // Назначается в инспекторе или скриптом

    [Header("=== UI ЭЛЕМЕНТЫ ===")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;

    // ── Drag-n-drop внутренние переменные ──────────────────────────

    // Временная «призрачная» иконка, которая следует за курсором
    private static GameObject _dragGhost;

    // Canvas нужен для правильного позиционирования ghost-иконки
    private Canvas _rootCanvas;

    private void Awake()
    {
        // Ищем корневой Canvas поднимаясь по иерархии
        _rootCanvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        RefreshUI();
    }

    /// <summary>Обновить отображение иконки и названия.</summary>
    public void RefreshUI()
    {
        if (ability == null)
        {
            if (iconImage != null) iconImage.color = new Color(1, 1, 1, 0.2f);
            if (nameText != null) nameText.text = "";
            return;
        }

        if (iconImage != null)
        {
            iconImage.sprite = ability.abilityIcon;
            iconImage.color = Color.white;
        }

        if (nameText != null)
            nameText.text = ability.abilityName;
    }

    // ── IBeginDragHandler ──────────────────────────────────────────

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ability == null) return;

        // Создаём ghost — полупрозрачную копию иконки, которая «летит» за курсором
        _dragGhost = new GameObject("DragGhost");

        // Помещаем ghost в корневой Canvas, чтобы он был поверх всего
        _dragGhost.transform.SetParent(_rootCanvas.transform, false);
        _dragGhost.transform.SetAsLastSibling(); // Поверх всех элементов

        // Добавляем компонент изображения
        Image ghostImage = _dragGhost.AddComponent<Image>();
        ghostImage.sprite = ability.abilityIcon;
        ghostImage.raycastTarget = false; // Ghost не должен мешать raycast-ам

        // Размер ghost = размер слота
        RectTransform ghostRect = _dragGhost.GetComponent<RectTransform>();
        RectTransform myRect = GetComponent<RectTransform>();
        ghostRect.sizeDelta = myRect.sizeDelta;

        // Ставим ghost под курсор
        UpdateGhostPosition(eventData);
    }

    // ── IDragHandler ───────────────────────────────────────────────

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragGhost == null) return;
        UpdateGhostPosition(eventData);
    }

    // ── IEndDragHandler ────────────────────────────────────────────

    public void OnEndDrag(PointerEventData eventData)
    {
        // Удаляем ghost в любом случае
        if (_dragGhost != null)
        {
            Destroy(_dragGhost);
            _dragGhost = null;
        }

        // Проверяем, над каким объектом отпустили
        // eventData.pointerEnter — объект под курсором в момент отпускания
        if (eventData.pointerEnter == null) return;

        ActiveSlotUI activeSlot = eventData.pointerEnter.GetComponentInParent<ActiveSlotUI>();
        if (activeSlot != null)
        {
            activeSlot.ReceiveDrop(ability);
        }
    }

    // ── Вспомогательные ───────────────────────────────────────────

    private void UpdateGhostPosition(PointerEventData eventData)
    {
        // Конвертируем позицию курсора (пикселы экрана) в локальные координаты Canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rootCanvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        _dragGhost.GetComponent<RectTransform>().localPosition = localPoint;
    }
}
