using UnityEngine;

/// <summary>
/// Автоматически заполняет сетку инвентаря всеми способностями по abilityIndex.
/// Добавь этот скрипт на объект "Abilities Inventory" в MainMenu.
/// Дочерние объекты пересоздаются при каждом открытии (это нормально).
/// </summary>
public class AbilitiesInventoryUI : MonoBehaviour
{
    [Header("=== РЕСУРСЫ ===")]
    [Tooltip("Все Ability ScriptableObject-ы. Порядок в массиве не важен — сортируем по abilityIndex.")]
    [SerializeField] private Ability[] allAbilities;

    [Tooltip("Prefab одного слота инвентаря (объект с компонентом InventorySlot)")]
    [SerializeField] private GameObject inventorySlotPrefab;

    private void Start()
    {
        PopulateInventory();
    }

    private void PopulateInventory()
    {
        if (allAbilities == null || inventorySlotPrefab == null)
        {
            Debug.LogError("[AbilitiesInventoryUI] Не назначены allAbilities или inventorySlotPrefab!");
            return;
        }

        // Сортируем по abilityIndex по возрастанию
        System.Array.Sort(allAbilities, (a, b) =>
        {
            if (a == null) return 1;
            if (b == null) return -1;
            return a.abilityIndex.CompareTo(b.abilityIndex);
        });

        // Удаляем старые слоты
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        // Создаём новые
        foreach (var ability in allAbilities)
        {
            if (ability == null) continue;

            GameObject go = Instantiate(inventorySlotPrefab, transform);
            InventorySlot slot = go.GetComponent<InventorySlot>();

            if (slot != null)
            {
                slot.ability = ability;
                slot.RefreshUI();
            }
            else
            {
                Debug.LogError($"[AbilitiesInventoryUI] Prefab {inventorySlotPrefab.name} не имеет компонента InventorySlot!");
            }
        }
    }
}