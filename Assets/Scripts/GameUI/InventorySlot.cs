using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Слот инвентаря способностей в главном меню.
/// Поддерживает перетаскивание на ActiveSlotUI.
/// 
/// КЛЮЧЕВАЯ ДЕТАЛЬ для работы drag-n-drop:
/// _canvasGroup.blocksRaycasts = false во время перетаскивания —
/// иначе сам слот перекрывает IDropHandler на ActiveSlotUI.
/// </summary>
public class InventorySlot : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("=== ДАННЫЕ ===")]
    public Ability ability;

    [Header("=== UI ЭЛЕМЕНТЫ ===")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;

    // Ghost-иконка которая следует за курсором во время перетаскивания
    private GameObject _dragGhost;

    // Ссылка на корневой Canvas (нужна для позиционирования ghost)
    private Canvas _rootCanvas;

    // CanvasGroup — позволяет отключать raycast у этого объекта
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _rootCanvas = GetComponentInParent<Canvas>();

        // Добавляем CanvasGroup если его нет — он нужен для блокировки/разблокировки raycast
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
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

    // ─────────────────────────────────────────────────────────────
    // Drag-n-Drop
    // ─────────────────────────────────────────────────────────────

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ability == null) return;

        // ВАЖНО: отключаем blocksRaycasts чтобы raycast "проходил сквозь" этот слот
        // и достигал ActiveSlotUI под курсором → IDropHandler сработает
        _canvasGroup.blocksRaycasts = false;

        // Создаём ghost-иконку поверх всего UI
        _dragGhost = new GameObject("DragGhost");
        _dragGhost.transform.SetParent(_rootCanvas.transform, false);
        _dragGhost.transform.SetAsLastSibling(); // Поверх всех элементов

        Image ghostImage = _dragGhost.AddComponent<Image>();
        ghostImage.sprite = ability.abilityIcon;
        ghostImage.raycastTarget = false; // Ghost сам не должен перехватывать raycast

        // Размер ghost совпадает с размером слота
        RectTransform ghostRect = _dragGhost.GetComponent<RectTransform>();
        ghostRect.sizeDelta = GetComponent<RectTransform>().sizeDelta;

        UpdateGhostPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragGhost == null) return;
        UpdateGhostPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Возвращаем блокировку raycast
        _canvasGroup.blocksRaycasts = true;

        // Удаляем ghost
        if (_dragGhost != null)
        {
            Destroy(_dragGhost);
            _dragGhost = null;
        }
    }

    // ─────────────────────────────────────────────────────────────
    // Вспомогательное
    // ─────────────────────────────────────────────────────────────

    private void UpdateGhostPosition(PointerEventData eventData)
    {
        // Переводим экранные координаты курсора в локальные координаты Canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rootCanvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        _dragGhost.GetComponent<RectTransform>().localPosition = localPoint;
    }

    public bool IsAbilityEquippedElsewhere(Ability ability, int currentSlotIndex)
    {
        if (AbilityLoadout.Instance == null) return false;

        for (int i = 0; i < 4; i++)
        {
            if (i != currentSlotIndex && AbilityLoadout.Instance.GetAbilityIndex(i) == ability.abilityIndex)
                return true;
        }
        return false;
    }
}