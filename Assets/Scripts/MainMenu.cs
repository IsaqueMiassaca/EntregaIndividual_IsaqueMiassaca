using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Text recordText;

    void Start() => recordText.text = $"Recorde: {PlayerPrefs.GetInt("Recorde")} m";

    public void Play() => SceneManager.LoadScene("Game");

    public void Quit() => Application.Quit();
}
