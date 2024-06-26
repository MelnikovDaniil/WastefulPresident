using LionStudios.Suite.Analytics;
using System;
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

    public static Action OnSkipLevel;
    [Space]
    public GameObject skipLevelPanel;
    public GameObject skipDescriptionPanel;
    public Animator lockedLevelAnimator;
    public int skipCost;
    public Text skipLevelNumberText;
    public Text currentMoneyText;
    public Text skipCostText;
    public Button skipButton;

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
        pausePanel?.SetActive(false);
        finishPanel?.SetActive(false);
        skipLevelPanel?.SetActive(false);

        if (gamePanel != null)
        {
            gamePanel.SetActive(true);
        }

        lvlText.text = SceneManager.GetActiveScene().name.ToUpper();
        pauseLvlText.text = $"LEVEL - {lvlText.text}";
        effectAnimator.SetTrigger("show");
    }

    private void Update()
    {
        if (GameManager.Instance?.president != null && Input.GetKeyDown(KeyCode.Escape))
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
        var currentVolume = SoundManager.GetMusicVolume();
        SoundManager.SetMusicVolume(currentVolume / 6);
        GameManager.Instance.president.isLocked = true;
        Time.timeScale = 0;
        isPaused = true;
        pausePanel?.SetActive(true);
    }

    public void Finish(string nextLevel)
    {
        var currentVolume = SoundManager.GetMusicVolume();
        SoundManager.SetMusicVolume(currentVolume / 6);
        SoundManager.PlaySoundUI("Victory");
        gamePanel?.SetActive(false);
        finishPanel?.SetActive(true);
        rewardManager.StartRewardCalculation();
        nextLevelButton.onClick.RemoveAllListeners();
        var comics = LevelMapper.GetComicsBeforeLevel(nextLevel);
        if (!string.IsNullOrEmpty(comics))
        {
            nextLevelButton.onClick.AddListener(() => GameManager.LoadComics(comics, nextLevel));
        }
        else
        {
            nextLevelButton.onClick.AddListener(() => GameManager.LoadLevel(nextLevel));
        }
    }

    public void SkipLevel(string lvlName)
    {
        var money = MoneyMapper.Get();
        skipLevelPanel?.SetActive(true);
        currentMoneyText.text = $"{money}$";
        skipCostText.text = $"{skipCost}$";
        skipButton.interactable = false;
        skipButton.onClick.RemoveAllListeners();

        if (money >= skipCost)
        {
            skipButton.interactable = true;
            skipButton.onClick.AddListener(() => PurchaseSkipLevel(lvlName));
        }
    }

    private void PurchaseSkipLevel(string lvlName)
    {
        MoneyMapper.Add(-skipCost);
        LevelMapper.Open(lvlName);
        var productSpend = new Product 
        {
            virtualCurrencies = new List<VirtualCurrency> { new VirtualCurrency("Dollar", "Main", skipCost) },
        };
        var productRecieved = new Product
        {
            items = new List<Item> { new Item($"lvl-{lvlName}", 1) },
        };
        LionAnalytics.EconomyEvent($"level-skip", productSpend, productRecieved);
        skipLevelNumberText.text = lvlName;
        lockedLevelAnimator.SetTrigger("unlock");
        skipDescriptionPanel?.SetActive(false);
        StartCoroutine(HidePanelRoutine());
    }

    private IEnumerator HidePanelRoutine()
    {
        yield return new WaitForSecondsRealtime(1.86f);
        OnSkipLevel?.Invoke();
        skipDescriptionPanel?.SetActive(true);
        skipLevelPanel?.SetActive(false);
    }

    public void Continue()
    {
        var currentVolume = SoundManager.GetMusicVolume();
        SoundManager.SetMusicVolume(currentVolume * 6);
        GameManager.Instance.president.isLocked = false;
        Time.timeScale = 1;
        isPaused = false;
        pausePanel?.SetActive(false);
    }

    public void LoadMainMenu()
    {
        GameManager.LoadMainMenu();
    }

    public void ReloadLevel()
    {
        GameManager.LevelRestart();
    }
}
