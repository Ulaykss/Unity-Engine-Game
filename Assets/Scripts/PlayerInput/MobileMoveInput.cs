using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using System.Collections.Generic;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MobileMoveInput : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    private bool _moveTouchActive = false;

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

    void Update()
    {
#if UNITY_EDITOR
        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            _moveTouchActive = !IsTouchOverUI();
            if (_moveTouchActive)
                SetDirection(mouse.position.ReadValue().x);
        }
        else if (mouse.leftButton.isPressed && _moveTouchActive)
        {
            SetDirection(mouse.position.ReadValue().x);
        }
        else if (mouse.leftButton.wasReleasedThisFrame)
        {
            _moveTouchActive = false;
            StopMove();
        }

#elif UNITY_ANDROID || UNITY_IOS
        var touches = Touch.activeTouches;

        if (touches.Count > 0)
        {
            var touch = touches[0];
            Vector2 pos = touch.screenPosition;

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                _moveTouchActive = !IsTouchOverUI();
                if (_moveTouchActive)
                    SetDirection(pos.x);
            }
            else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved ||
                     touch.phase == UnityEngine.InputSystem.TouchPhase.Stationary)
            {
                if (_moveTouchActive)
                    SetDirection(pos.x);
            }
            else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                     touch.phase == UnityEngine.InputSystem.TouchPhase.Canceled)
            {
                _moveTouchActive = false;
                StopMove();
            }
        }
        else
        {
            _moveTouchActive = false;
            StopMove();
        }
#endif
    }

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

    private bool IsTouchOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}