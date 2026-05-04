using UnityEngine;
using System.Collections;

public class SideDash : Ability
{
    private Rigidbody2D rb;
    private GameConfig config;
    private Coroutine dashCoroutine;
    private float originalSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        config = ConfigManager.Config;
        resourceCost = config.sideDashResourceCost;
    }

    public override void Activate(GameObject caster)
    {
        Execute();
    }

    private void Execute()
    {
        // Сохраняем оригинальную скорость
        originalSpeed = config.horizontalSpeed;

        // Устанавливаем новую скорость
        config.horizontalSpeed = config.sideDashSpeed;

        // Запускаем корутину для возврата скорости через время
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
        }
        dashCoroutine = StartCoroutine(ResetSpeedAfterDelay(config.sideDashTime));
    }

    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Возвращаем оригинальную скорость
        config.horizontalSpeed = originalSpeed;
        dashCoroutine = null;
    }
}