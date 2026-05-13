using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelCardUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text bestScoreText;
    public Image previewImage;

    private string sceneName;

    public void Setup(LevelData data)
    {
        sceneName = data.sceneName;

        titleText.text = data.levelTitle;
        previewImage.sprite = data.previewImage;

        float best =
            PlayersSettingsManager.Instance.GetBestScoreForScene(sceneName);

        bestScoreText.text = "Best: " + best.ToString("F2") + " m";
    }

    public void OpenLevel()
    {
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1f;
    }
}