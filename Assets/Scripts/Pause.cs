using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class Pause : MonoBehaviour
{

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
    }
    public void Restart()
    {
        SceneManager.LoadScene("ball");
    }
}
