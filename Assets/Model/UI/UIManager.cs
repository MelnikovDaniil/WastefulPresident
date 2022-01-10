using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : BaseManager
{
    public static UIManager Instance;
    public Animator effectAnimator;

    [Space]
    public GameObject gamePanel;

    [Space]
    public GameObject pausePanel;
    public Text pauseLvlText;

    [Space]
    public RewardManager rewardManager;
    public GameObject finishPanel;
    public Button nextLevelButton;

    public bool isPaused;
    public Text lvlText;

    public bool destroyOnLoad = false;

    private void Awake()
    {
        // start of new code
        if (Instance != null)
        {
            if (destroyOnLoad)
            {
                Destroy(Instance.gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
        // end of new code

        Instance = this;
        if (!destroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public override void LoadManager()
    {
        base.LoadManager();
        isPaused = false;
        gamePanel?.SetActive(true);
        pausePanel?.SetActive(false);
        finishPanel?.SetActive(false);
        lvlText.text = SceneManager.GetActiveScene().name.ToUpper();
        pauseLvlText.text = $"LEVEL - {lvlText.text}";
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

    public void Finish(string nextLevel)
    {
        gamePanel?.SetActive(false);
        finishPanel?.SetActive(true);
        rewardManager.StartRewardCalculation();
        nextLevelButton.onClick.RemoveAllListeners();
        nextLevelButton.onClick.AddListener(() => GameManager.Load(nextLevel));
    }

    public void Continue()
    {
        GameManager.Instance.character.isLocked = false;
        Time.timeScale = 1;
        isPaused = false;
        pausePanel?.SetActive(false);
    }

    public void LoadMainMenu()
    {
        GameManager.Load("LevelMenu");
    }

    public void ReloadLevel()
    {
        GameManager.ReloadLevel();
    }
}
