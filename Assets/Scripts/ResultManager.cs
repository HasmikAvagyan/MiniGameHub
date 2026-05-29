using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultManager : MonoBehaviour
{
    public TMP_Text resultText;

    void Start()
    {
        if (resultText != null)
            resultText.text = GameResult.winnerText;
    }

    public void OnMainMenuButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}