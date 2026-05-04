using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    [Header("Ability Settings")]
    [SerializeField] protected string abilityName;

    [Header("Resource Cost")]
    [SerializeField] protected int resourceCost = 1;

    // Активация способности
    public abstract void Activate(GameObject caster);

    // Свойство для получения стоимости способности
    public int ResourceCost => resourceCost;

    // Метод для проверки, может ли способность быть активирована
    public virtual bool CanActivate(ProgressBar resourceBar)
    {
        if (resourceBar == null) return true; // Если нет ProgressBar, всегда можно активировать
        return resourceBar.HasEnoughNegative(resourceCost);
    }
}