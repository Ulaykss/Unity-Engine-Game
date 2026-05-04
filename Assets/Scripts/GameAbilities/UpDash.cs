using UnityEngine;

public class UpDash : Ability
{
    private Rigidbody2D rb;
    private GameConfig config;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        config = ConfigManager.Config;
        resourceCost = config.upDashResourceCost;
    }

    public override void Activate(GameObject caster)
    {
        Execute();
    }

    private void Execute()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * config.upDashForce, ForceMode2D.Impulse);
    }
}