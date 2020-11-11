using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    bool gameIsPaused = false;


    // Start is called before the first frame update
    void Start()
    {
        Trigger();
    }

    public void Trigger()
    {
        Debug.Log("Trigger");
        if (gameIsPaused)
        {
            Resume();
        }
        else 
        {
            Pause();
        }
    }

    public void Pause() 
    {
        Time.timeScale = 0.0f;
        gameIsPaused = true;
        gameObject.SetActive(true);
    }

    void Resume() 
    {
    
        Time.timeScale = 1.0f;
        gameIsPaused = false;
        gameObject.SetActive(false);
    }

    public void QuitGame() 
    {
        Debug.Log("Application has quitted");
        Application.Quit();
    }
}
