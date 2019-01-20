using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class UIManagerScript : MonoBehaviour {

    public void StartGame()
    {
        SceneManager.LoadScene("ball");
    }

}
