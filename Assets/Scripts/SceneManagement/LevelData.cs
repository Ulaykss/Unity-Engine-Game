using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level")]
public class LevelData : ScriptableObject
{
    public string levelTitle;
    public string sceneName;
    public Sprite previewImage;
}