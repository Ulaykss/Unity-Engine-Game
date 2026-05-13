using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private GameConfig config;
    private Slot[] slots; // ← массив вместо одного слота

    [SerializeField] private float _horizontalSpeed;
    private Vector2 _direction;

    void Start()
    {
        config = ConfigManager.Config;
        slots = FindObjectsOfType<Slot>(); // ← находим все слоты
    }

    void Update()
    {
        _horizontalSpeed = config.horizontalSpeed;
        Move(_direction);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<Vector2>();
    }

    public Vector2 GetDirection()
    {
        return _direction;
    }

    // Активирует только целевой слот с указанным индексом
    public void ActivateSlot(int slotIndex)
    {
        if (slots == null || slots.Length == 0)
            return;

        // Находим слот с нужным индексом и вызываем его Activate
        foreach (var slot in slots)
        {
            if (slot != null && slot.slotIndex == slotIndex)
            {
                slot.Activate(slotIndex);
                return; // Выходим после активации
            }
        }

        Debug.LogWarning($"Slot with index {slotIndex} not found!");
    }

    public void OnFirstSlot(InputAction.CallbackContext context)
    {
        if (context.performed) { ActivateSlot(0); }
    }

    public void OnSecondSlot(InputAction.CallbackContext context)
    {
        if (context.performed) { ActivateSlot(1); }
    }

    public void OnThirdSlot(InputAction.CallbackContext context)
    {
        if (context.performed) { ActivateSlot(2); }
    }

    public void OnFourthSlot(InputAction.CallbackContext context)
    {
        if (context.performed) { ActivateSlot(3); }
    }

    private void Move(Vector2 directionMove)
    {
        Vector3 direction = new Vector3(directionMove.x, 0, 0);
        transform.position += direction * _horizontalSpeed * Time.deltaTime;
    }

    public void OnMoveMobile(Vector2 direction)
    {
        _direction = direction;
    }
}
