using UnityEngine;

/// <summary>
/// Синглтон. Хранит индексы способностей назначенных в 4 слота.
/// Живёт между сценами (DontDestroyOnLoad).
/// Данные сохраняются в PlayerPrefs — не теряются при перезапуске игры.
/// </summary>
public class AbilityLoadout : MonoBehaviour
{
    public static AbilityLoadout Instance { get; private set; }

    // 4 слота. Значение = abilityIndex способности, -1 = слот пуст
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

        LoadFromPlayerPrefs();
    }

    // ─────────────────────────────────────────────────────────────
    // Публичный API
    // ─────────────────────────────────────────────────────────────

    /// <summary>Получить abilityIndex для слота (0-3). -1 если пусто.</summary>
    public int GetAbilityIndex(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _slots.Length) return -1;
        return _slots[slotIndex];
    }

    /// <summary>Назначить способность в слот. -1 очищает слот.</summary>
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

    // ─────────────────────────────────────────────────────────────
    // Сохранение / загрузка
    // ─────────────────────────────────────────────────────────────

    private void SaveToPlayerPrefs()
    {
        for (int i = 0; i < _slots.Length; i++)
            PlayerPrefs.SetInt($"ActiveSlot_{i}", _slots[i]);
        PlayerPrefs.Save();
    }

    private void LoadFromPlayerPrefs()
    {
        for (int i = 0; i < _slots.Length; i++)
            _slots[i] = PlayerPrefs.GetInt($"ActiveSlot_{i}", -1);
    }
}