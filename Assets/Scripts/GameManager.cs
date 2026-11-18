using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Transform player;
    public Text scoreText, finalScoreText;
    public GameObject pausePanel, gameOverPanel;
    public Button resumeButton, retryButton;

    float startX;
    int score;

    void Start()
    {
        Time.timeScale = 1f;
        startX = player.position.x;
    }

    void Update()
    {
        if (gameOverPanel.activeSelf) return;
        score = Mathf.Max(score, Mathf.FloorToInt(player.position.x - startX));
        scoreText.text = $"Distância: {score} m";
        if (Keyboard.current.escapeKey.wasPressedThisFrame) SetPaused(!pausePanel.activeSelf);
    }

    public void SetPaused(bool paused)
    {
        pausePanel.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;
        EventSystem.current.SetSelectedGameObject(paused ? resumeButton.gameObject : null);
    }

    public void GameOver()
    {
        int best = Mathf.Max(score, PlayerPrefs.GetInt("Recorde"));
        PlayerPrefs.SetInt("Recorde", best);
        finalScoreText.text = $"Distância: {score} m\nRecorde: {best} m";
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        EventSystem.current.SetSelectedGameObject(retryButton.gameObject);
    }

    public void Restart() => SceneManager.LoadScene("Game");

    public void Menu() => SceneManager.LoadScene("Menu");
}
