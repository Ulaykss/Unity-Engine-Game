using UnityEngine;

/// <summary>
/// Автоматически заполняет сетку инвентаря всеми способностями по порядку (по abilityIndex).
/// Добавь этот скрипт на объект "Abilities Inventory" в MainMenu.
/// </summary>
public class AbilitiesInventoryUI : MonoBehaviour
{
    [Header("=== РЕСУРСЫ ===")]
    [Tooltip("Все Ability ScriptableObject-ы. Порядок не важен — сортируем по abilityIndex.")]
    [SerializeField] private Ability[] allAbilities;

    [Tooltip("Prefab одного слота инвентаря (с компонентом InventorySlot)")]
    [SerializeField] private GameObject inventorySlotPrefab;

    private void Start()
    {
        PopulateInventory();
    }

    private void PopulateInventory()
    {
        if (allAbilities == null || inventorySlotPrefab == null)
        {
            Debug.LogError("AbilitiesInventoryUI: не назначены allAbilities или inventorySlotPrefab!");
            return;
        }

        // Сортируем способности по abilityIndex (от меньшего к большему)
        System.Array.Sort(allAbilities, (a, b) =>
        {
            if (a == null) return 1;
            if (b == null) return -1;
            return a.abilityIndex.CompareTo(b.abilityIndex);
        });

        // Удаляем старые дочерние объекты (если были)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Создаём слот для каждой способности
        foreach (var ability in allAbilities)
        {
            if (ability == null) continue;

            GameObject slotGO = Instantiate(inventorySlotPrefab, transform);
            InventorySlot slot = slotGO.GetComponent<InventorySlot>();

            if (slot != null)
            {
                slot.ability = ability;
                slot.RefreshUI();
            }
            else
            {
                Debug.LogError($"AbilitiesInventoryUI: prefab {inventorySlotPrefab.name} не имеет компонента InventorySlot!");
            }
        }
    }
}
