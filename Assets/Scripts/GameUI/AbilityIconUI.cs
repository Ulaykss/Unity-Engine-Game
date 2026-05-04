using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class AbilityIconUI : MonoBehaviour
{
    private GameConfig config;

    [SerializeField] private Image iconImage;
    [SerializeField] private int abilitySlotIndex;
    [SerializeField] private PlayerController player;

    [Header("Press Effect")]
    [SerializeField] private Color pressedColor;
    [SerializeField] private float pressDuration;

    private Color originalColor;
    private Coroutine pressRoutine;
    private Button button;

    private void Awake()
    {
        if (iconImage == null)
            iconImage = GetComponent<Image>();

        button = GetComponent<Button>();

#if UNITY_ANDROID || UNITY_IOS
        button.interactable = true;
        button.onClick.AddListener(OnClick);
#else
        // ПК / Editor — кнопка отключена полностью
        button.interactable = false;
#endif
    }

    private void Start()
    {
        config = ConfigManager.Config;

        if (config == null)
        {
            Debug.LogError("AbilityIconUI: ConfigManager.Config is null!", this);
            return;
        }

        originalColor = iconImage.color;
        pressedColor = config.pressedColor;
        pressDuration = config.pressDuration;
    }

    public void PlayPressEffect()
    {
        if (pressRoutine != null)
            StopCoroutine(pressRoutine);

        pressRoutine = StartCoroutine(PressEffectCoroutine());
    }

    private IEnumerator PressEffectCoroutine()
    {
        iconImage.color = pressedColor;
        yield return new WaitForSeconds(pressDuration);
        iconImage.color = originalColor;
    }

#if UNITY_ANDROID || UNITY_IOS
    private void OnClick()
    {
        player.ActivateAbilityFromUI(abilitySlotIndex);
        PlayPressEffect();
    }
#endif
}
