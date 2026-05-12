using UnityEngine;

public class LevelCarousel : MonoBehaviour
{
    public LevelData[] levels;
    public LevelCardUI levelCard;

    private int currentIndex = 0;

    void Start()
    {
        ShowCurrent();
    }

    public void Next()
    {
        currentIndex++;

        if (currentIndex >= levels.Length)
            currentIndex = 0;

        ShowCurrent();
    }

    public void Prev()
    {
        currentIndex--;

        if (currentIndex < 0)
            currentIndex = levels.Length - 1;

        ShowCurrent();
    }

    private void ShowCurrent()
    {
        levelCard.Setup(levels[currentIndex]);
    }
}