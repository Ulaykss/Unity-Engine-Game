using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using System.Collections.Generic;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MobileMoveInput : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    // ID касания которое отвечает за движение (-1 = нет активного касания)
    private int _moveTouchId = -1;

    private void Awake()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>();

        EnhancedTouchSupport.Enable();
    }

    private void OnDestroy()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
#if UNITY_EDITOR
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
    }

    // ─────────────────────────────────────────────────────────────
    // Мышь (только в редакторе)
    // ─────────────────────────────────────────────────────────────

    private void HandleMouseInput()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 mousePos = mouse.position.ReadValue();

        if (mouse.leftButton.wasPressedThisFrame)
        {
            // Проверяем: нажали ли на UI элемент?
            if (IsPointerOverUI(mousePos))
            {
                // Нажали на UI (кнопку способности и т.д.) — движение не начинаем
                _moveTouchId = -1;
                return;
            }

            // Нажали на игровую область — начинаем движение
            _moveTouchId = 0; // для мыши используем условный ID = 0
            SetDirection(mousePos.x);
        }
        else if (mouse.leftButton.isPressed && _moveTouchId == 0)
        {
            SetDirection(mousePos.x);
        }
        else if (mouse.leftButton.wasReleasedThisFrame)
        {
            _moveTouchId = -1;
            StopMove();
        }
    }

    // ─────────────────────────────────────────────────────────────
    // Тач (на устройстве)
    // ─────────────────────────────────────────────────────────────

    private void HandleTouchInput()
    {
        var touches = Touch.activeTouches;

        // Обрабатываем все касания
        foreach (var touch in touches)
        {
            Vector2 pos = touch.screenPosition;

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                // Если уже есть активное касание для движения — игнорируем новые
                if (_moveTouchId != -1) continue;

                // Нажали на UI? Пропускаем — EventSystem сам обработает кнопку
                if (IsPointerOverUI(pos)) continue;

                // Запоминаем именно этот палец как «палец движения»
                _moveTouchId = touch.touchId;
                SetDirection(pos.x);
            }
            else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved ||
                     touch.phase == UnityEngine.InputSystem.TouchPhase.Stationary)
            {
                // Обновляем только если это наш «палец движения»
                if (touch.touchId == _moveTouchId)
                    SetDirection(pos.x);
            }
            else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                     touch.phase == UnityEngine.InputSystem.TouchPhase.Canceled)
            {
                // Палец движения отпустили — останавливаемся
                if (touch.touchId == _moveTouchId)
                {
                    _moveTouchId = -1;
                    StopMove();
                }
            }
        }

        // Если нет ни одного касания — обязательно останавливаемся
        if (touches.Count == 0 && _moveTouchId != -1)
        {
            _moveTouchId = -1;
            StopMove();
        }
    }

    // ─────────────────────────────────────────────────────────────
    // Утилиты
    // ─────────────────────────────────────────────────────────────

    private void SetDirection(float screenX)
    {
        float half = Screen.width * 0.5f;
        float dir = screenX < half ? -1f : 1f;
        player.OnMoveMobile(new Vector2(dir, 0));
    }

    private void StopMove()
    {
        player.OnMoveMobile(Vector2.zero);
    }

    /// <summary>
    /// Проверяет, находится ли указатель над UI-элементом.
    /// Корректно работает и для мыши, и для тача.
    /// </summary>
    private bool IsPointerOverUI(Vector2 screenPosition)
    {
        var eventSystem = EventSystem.current;
        if (eventSystem == null) return false;

        // Создаём PointerEventData с позицией
        var pointerData = new PointerEventData(eventSystem)
        {
            position = screenPosition
        };

        var results = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerData, results);

        // Если хоть что-то есть под курсором — это UI
        return results.Count > 0;
    }
}