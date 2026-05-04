using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class GameConfigIni
{
    // Хранилище значений ini
    private readonly Dictionary<string, string> values = new();

    // =============================
    // LOAD
    // =============================

    public static GameConfigIni Load(string text)
    {
        var ini = new GameConfigIni();

        foreach (var raw in text.Split('\n'))
        {
            var line = raw.Trim();

            if (string.IsNullOrEmpty(line) || line.StartsWith(";"))
                continue;

            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                ini.values[parts[0].Trim()] = parts[1].Trim();
            }
        }

        return ini;
    }

    // =============================
    // GET
    // =============================

    public float GetFloat(string key, float def)
    {
        if (values.TryGetValue(key, out var v) &&
            float.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out var r))
            return r;

        return def;
    }

    public int GetInt(string key, int def)
    {
        if (values.TryGetValue(key, out var v) &&
            int.TryParse(v, out var r))
            return r;

        return def;
    }

    public string GetString(string key, string def)
    {
        return values.TryGetValue(key, out var v) ? v : def;
    }

    public Color GetColor(string key, Color def)
    {
        if (!values.TryGetValue(key, out var v))
            return def;

        var parts = v.Split(',');
        if (parts.Length != 4)
            return def;

        if (float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var r) &&
            float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var g) &&
            float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var b) &&
            float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out var a))
        {
            return new Color(r, g, b, a);
        }

        return def;
    }

    // =============================
    // GENERATE FROM GAMECONFIG
    // =============================

    public static GameConfigIni GenerateFromGameConfig(GameConfig config)
    {
        var ini = new GameConfigIni();

        ini.Set("horizontalSpeed", config.horizontalSpeed);
        ini.Set("initialNegative", config.initialNegative);
        ini.Set("maxNegative", config.maxNegative);
        ini.Set("fillDuration", config.fillDuration);

        ini.Set("platformJumpForce", config.platformJumpForce);
        ini.Set("singlePlatformHeightOffset", config.singlePlatformHeightOffset);
        ini.Set("platformHorizontalOverflow", config.platformHorizontalOverflow);

        ini.Set("destroyHeightThreshold", config.destroyHeightThreshold);
        ini.Set("cameraSmoothSpeed", config.cameraSmoothSpeed);

        ini.Set("distanceUnit", config.distanceUnit);
        ini.Set("centimetersPerUnit", config.centimetersPerUnit);

        ini.Set("progressbarAcceleration", config.progressbarAcceleration);
        ini.Set("progressbarDeceleration", config.progressbarDeceleration);
        ini.Set("progressbarMaxSpeed", config.progressbarMaxSpeed);
        ini.Set("progressbarSegmentSwitchBoost", config.progressbarSegmentSwitchBoost);

        ini.Set("doubleJumpForce", config.doubleJumpForce);
        ini.Set("upDashForce", config.upDashForce);
        ini.Set("sideDashSpeed", config.sideDashSpeed);
        ini.Set("sideDashTime", config.sideDashTime);
        ini.Set("screamForce", config.screamForce);

        ini.Set("doubleJumpResourceCost", config.doubleJumpResourceCost);
        ini.Set("upDashResourceCost", config.upDashResourceCost);
        ini.Set("sideDashResourceCost", config.sideDashResourceCost);
        ini.Set("screamResourceCost", config.screamResourceCost);

        ini.Set("pressedColor", config.pressedColor);
        ini.Set("pressDuration", config.pressDuration);

        return ini;
    }

    // =============================
    // SAVE
    // =============================

    public void Save(string path)
    {
        File.WriteAllText(path, ToString());
    }

    // =============================
    // SET
    // =============================

    public void Set(string key, float value)
    {
        values[key] = value.ToString(CultureInfo.InvariantCulture);
    }

    public void Set(string key, int value)
    {
        values[key] = value.ToString();
    }

    public void Set(string key, string value)
    {
        values[key] = value;
    }

    public void Set(string key, Color color)
    {
        values[key] =
            $"{color.r.ToString(CultureInfo.InvariantCulture)}," +
            $"{color.g.ToString(CultureInfo.InvariantCulture)}," +
            $"{color.b.ToString(CultureInfo.InvariantCulture)}," +
            $"{color.a.ToString(CultureInfo.InvariantCulture)}";
    }

    // =============================
    // TO STRING
    // =============================

    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var kv in values)
            sb.AppendLine($"{kv.Key} = {kv.Value}");

        return sb.ToString();
    }
}
