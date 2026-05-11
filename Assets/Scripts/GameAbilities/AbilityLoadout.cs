using UnityEngine;

/// <summary>
/// Синглтон. Хранит индексы способностей, назначенных в 4 активных слота главного меню.
/// Живёт между сценами (DontDestroyOnLoad).
/// Индекс = abilityIndex из ScriptableObject Ability. -1 означает «слот пуст».
/// </summary>
public class AbilityLoadout : MonoBehaviour
{
    public static AbilityLoadout Instance { get; private set; }

    // 4 слота. Значение = abilityIndex выбранной способности, -1 = пусто
    private int[] _slots = new int[4] { -1, -1, -1, -1 };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadFromPlayerPrefs(); // Восстанавливаем сохранённые данные
    }

    // ── Публичный API ──────────────────────────────────────────────

    /// <summary>Получить abilityIndex для слота (0-3). Возвращает -1 если пусто.</summary>
    public int GetAbilityIndex(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _slots.Length) return -1;
        return _slots[slotIndex];
    }

    /// <summary>Назначить способность в слот. abilityIndex = -1 очищает слот.</summary>
    public void SetAbilityIndex(int slotIndex, int abilityIndex)
    {
        if (slotIndex < 0 || slotIndex >= _slots.Length) return;
        _slots[slotIndex] = abilityIndex;
        SaveToPlayerPrefs();
    }

    /// <summary>Очистить слот.</summary>
    public void ClearSlot(int slotIndex)
    {
        SetAbilityIndex(slotIndex, -1);
    }

    // ── Сохранение / загрузка через PlayerPrefs ────────────────────
    // PlayerPrefs — встроенный в Unity способ хранить небольшие данные
    // (числа, строки). Сохраняется между запусками игры.

    private void SaveToPlayerPrefs()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            PlayerPrefs.SetInt($"ActiveSlot_{i}", _slots[i]);
        }
        PlayerPrefs.Save();
    }

    private void LoadFromPlayerPrefs()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            // Если ключа нет — используем -1 (пусто)
            _slots[i] = PlayerPrefs.GetInt($"ActiveSlot_{i}", -1);
        }
    }
}
