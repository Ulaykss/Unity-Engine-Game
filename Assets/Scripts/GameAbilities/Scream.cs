using UnityEngine;

public class Scream : Ability
{
    private Rigidbody2D rb;
    private GameConfig config;

    [Header("Dependencies")]
    [SerializeField] private ProgressBar progressBar;

    private float regenBoostTimer = 0f;
    private bool regenBoostActive = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        config = ConfigManager.Config;

        resourceCost = config.screamResourceCost;
    }

    void Update()
    {
        HandleRegenBoostTimer();
    }

    public override void Activate(GameObject caster)
    {
        Execute();
    }

    private void Execute()
    {
        // Физика
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * config.screamForce, ForceMode2D.Impulse);

        // Бафф регена
        ApplyRegenBoost();
    }

    private void ApplyRegenBoost()
    {
        regenBoostActive = true;
        regenBoostTimer = config.screamRegenDuration;

        progressBar.SetRegenMultiplier(config.screamRegenMultiplier);
    }

    private void HandleRegenBoostTimer()
    {
        if (!regenBoostActive)
            return;

        regenBoostTimer -= Time.deltaTime;

        if (regenBoostTimer <= 0f)
        {
            regenBoostActive = false;
            regenBoostTimer = 0f;

            if (progressBar != null)
                progressBar.SetRegenMultiplier(1f);
        }
    }
}
