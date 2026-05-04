using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private GameConfig config;

    [SerializeField] private float _horizontalSpeed;
    private Vector2 _direction;

    [Header("Ability Slots")]
    [SerializeField] private Ability[] abilitySlots = new Ability[4];

    // Ссылка на ProgressBar для проверки ресурсов
    [SerializeField] private ProgressBar negativeResourceBar;

    [Header("Ability UI")]
    [SerializeField] private AbilityIconUI[] abilityIcons = new AbilityIconUI[4];

    void Start()
    {
        config = ConfigManager.Config;
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

    public void OnFirstSlot(InputAction.CallbackContext context)
    {
        if (context.performed) // Активируем только при нажатии, а не при отпускании
        {
            ActivateAbility(0);
        }
    }

    public void OnSecondSlot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ActivateAbility(1);
        }
    }

    public void OnThirdSlot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ActivateAbility(2);
        }
    }

    public void OnFourthSlot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ActivateAbility(3);
        }
    }

    private void Move(Vector2 directionMove)
    {
        Vector3 direction = new Vector3(directionMove.x, 0, 0);
        transform.position += direction * _horizontalSpeed * Time.deltaTime;
    }

    private void ActivateAbility(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= abilitySlots.Length) return;
        if (abilitySlots[slotIndex] == null) return;

        Ability ability = abilitySlots[slotIndex];

        // Проверяем, можно ли активировать способность (хватает ли ресурсов)
        if (ability.CanActivate(negativeResourceBar))
        {
            // Пытаемся использовать ресурсы
            if (negativeResourceBar != null && !negativeResourceBar.UseNegative(ability.ResourceCost))
                return;

            // Активируем способность
            ability.Activate(gameObject);

            // ▶ UI эффект нажатия
            if (slotIndex < abilityIcons.Length && abilityIcons[slotIndex] != null)
            {
                abilityIcons[slotIndex].PlayPressEffect();
            }
        }
    }

    public void OnMoveMobile(Vector2 direction)
    {
        _direction = direction;
    }

    public void ActivateAbilityFromUI(int slotIndex)
    {
        ActivateAbility(slotIndex);
    }
}