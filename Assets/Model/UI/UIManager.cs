using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : BaseManager
{
    public static UIManager Instance;
    public Animator effectAnimator;
    public GameObject pausePanel;
    public bool isPaused; 

    private void Awake()
    {
        // start of new code
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // end of new code

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override void LoadManager()
    {
        base.LoadManager();
        effectAnimator.SetTrigger("show");
    }

    private void Update()
    {
        if (GameManager.Instance.character != null && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Continue();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Light()
    {
        effectAnimator.SetTrigger("light");
    }

    public void Hide()
    {
        effectAnimator.SetTrigger("hide");
    }

    public void Pause()
    {
        GameManager.Instance.character.isLocked = true;
        Time.timeScale = 0;
        isPaused = true;
        pausePanel?.SetActive(true);
    }

    public void Continue()
    {
        GameManager.Instance.character.isLocked = false;
        Time.timeScale = 1;
        isPaused = false;
        pausePanel?.SetActive(false);
    }
}
