using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Game/Ability")]
public class Ability : ScriptableObject
{
    [Header("=== ОСНОВНАЯ ИНФОРМАЦИЯ ===")]
    public int abilityIndex;
    public string abilityName;
    public Sprite abilityIcon;

    [Header("=== ПАРАМЕТРЫ СПОСОБНОСТИ ===")]
    [Tooltip("Индекс типа способности для получения параметров из GameConfig")]
    public AbilityType abilityType;

    // Не сериализуем, устанавливается в рантайме
    [HideInInspector] public GameConfig config;
    [HideInInspector] public ProgressBar progressBar; // ← было Slider, стало ProgressBar

    private Rigidbody2D rb;
    private MonoBehaviour coroutineHost;
    private Coroutine dashCoroutine;
    private float originalSpeed;

    private float regenBoostTimer = 0f;
    private bool regenBoostActive = false;

    public int ResourceCost
    {
        get
        {
            if (config == null) return 0;
            return abilityType switch
            {
                AbilityType.DoubleJump => config.doubleJumpResourceCost,
                AbilityType.UpDash => config.upDashResourceCost,
                AbilityType.SideDash => config.sideDashResourceCost,
                AbilityType.Scream => config.screamResourceCost,
                _ => 0
            };
        }
    }

    public float AbilityPower
    {
        get
        {
            if (config == null) return 0;
            return abilityType switch
            {
                AbilityType.DoubleJump => config.doubleJumpForce,
                AbilityType.UpDash => config.upDashForce,
                AbilityType.SideDash => config.sideDashSpeed,
                AbilityType.Scream => config.screamForce,
                _ => 0
            };
        }
    }

    // Инициализация (вызывается из Slot)
    public void Initialize(GameObject user, GameConfig gameConfig, ProgressBar bar)
    {
        config = gameConfig;
        progressBar = bar; // теперь типы совпадают: ProgressBar = ProgressBar
        rb = user.GetComponent<Rigidbody2D>();
        coroutineHost = user.GetComponent<MonoBehaviour>();

        if (coroutineHost == null)
        {
            Debug.LogError("User GameObject must have a MonoBehaviour to host coroutines!");
        }
    }

    // Выполнение способности
    public void Execute(GameObject user)
    {
        if (rb == null) rb = user.GetComponent<Rigidbody2D>();
        if (coroutineHost == null) coroutineHost = user.GetComponent<MonoBehaviour>();

        switch (abilityType)
        {
            case AbilityType.DoubleJump:
                ExecuteDoubleJump(user);
                break;
            case AbilityType.UpDash:
                ExecuteUpDash(user);
                break;
            case AbilityType.SideDash:
                ExecuteSideDash(user);
                break;
            case AbilityType.Scream:
                ExecuteScream(user);
                break;
        }
    }

    private void ExecuteDoubleJump(GameObject user)
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * config.doubleJumpForce, ForceMode2D.Impulse);
        }
    }

    private void ExecuteUpDash(GameObject user)
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * config.upDashForce, ForceMode2D.Impulse);
        }
    }

    private void ExecuteSideDash(GameObject user)
    {
        if (rb != null && coroutineHost != null)
        {
            if (dashCoroutine == null)
            {
                originalSpeed = config.horizontalSpeed;
            }
            else
            {
                coroutineHost.StopCoroutine(dashCoroutine);
            }

            config.horizontalSpeed = config.sideDashSpeed;
            dashCoroutine = coroutineHost.StartCoroutine(ResetSpeedAfterDelay(config.sideDashTime));
        }
    }

    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (config != null)
            config.horizontalSpeed = originalSpeed;
        dashCoroutine = null;
    }

    private void ExecuteScream(GameObject user)
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * config.screamForce, ForceMode2D.Impulse);
        }

        ApplyRegenBoost();
    }

    private void ApplyRegenBoost()
    {
        if (progressBar == null) return;

        regenBoostActive = true;
        regenBoostTimer = config.screamRegenDuration;
        progressBar.SetRegenMultiplier(config.screamRegenMultiplier); // теперь работает

        if (coroutineHost != null)
        {
            coroutineHost.StartCoroutine(HandleRegenBoostTimerCoroutine());
        }
    }

    private IEnumerator HandleRegenBoostTimerCoroutine()
    {
        float timer = config.screamRegenDuration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        if (progressBar != null)
            progressBar.SetRegenMultiplier(1f); // и здесь тоже
    }
}

public enum AbilityType
{
    DoubleJump,
    UpDash,
    SideDash,
    Scream
}