using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using com.adjust.sdk;
using GameAnalyticsSDK;
using LionStudios.Suite.Analytics;
using LionStudios.Suite.Analytics.Events;
using LionStudios.Suite.Debugging;

public class GameManager : BaseManager
{
    public static GameManager Instance;
    public UIManager uiManager;
    public ControllerManager controllerManager;
    public President president;
    public ScriptableComicsChapter firstComics;
    public float reloadDelay = 5;
    public bool destroyOnLoad = false;

    [Space]
    public string soundtrackName;
    public float soundtrackVolume = 0.3f;

    private bool isBusy;
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
                GameManager.Instance.LoadManager();
                Destroy(gameObject);
                return;
            }
        }
        // end of new code
        Instance = this;
        Instantiate(uiManager);
        Instantiate(controllerManager);

        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
            // AppLovin SDK is initialized, start loading ads
            //MaxSdk.ShowMediationDebugger();
        };
        MaxSdk.SetSdkKey("CgG1BtqwUb8gNyhBVM-6AoTTU-yyGD9UyFS4QZzB7qdKR94hTICWTvRbNbGfmkw9VEQ8cUSDZFXLFELip15EZB");
        MaxSdk.SetUserId(SystemInfo.deviceUniqueIdentifier);
        MaxSdk.SetVerboseLogging(true);
        MaxSdk.InitializeSdk();
        GameAnalytics.Initialize();
        LionAnalytics.GameStart();
        LionDebugger.Hide();
        // import this package into the project:
        // https://github.com/adjust/unity_sdk/releases
#if UNITY_IOS
        /* Mandatory - set your iOS app token here */
        //InitAdjust("YOUR_IOS_APP_TOKEN_HERE");
#elif UNITY_ANDROID
        /* Mandatory - set your Android app token here */
        InitAdjust("dyqevqya0zy8");
#endif

        if (!destroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);                                                
        }

        LoadManager();
    }

    public override void LoadManager()
    {
        base.LoadManager();
        UIManager.Instance.LoadManager();
        SelectionMenu.Instance.LoadManager();
        ControllerManager.Instance.LoadManager();

        Time.timeScale = 1;
        president = FindObjectOfType<President>();
        if (president != null)
        {
            president.OnDeath += () => LevelFail();
        }


        var sceenName = SceneManager.GetActiveScene().name;
        SoundManager.SetMusicVolume(soundtrackVolume);
        if (string.IsNullOrEmpty(soundtrackName))
        {
            var storedSoundtrack = SoundtrackMapper.GetSoundtrack(sceenName);
            SoundManager.PlayMusic(storedSoundtrack);
        }
        else
        {
            SoundtrackMapper.SetSoundtrack(soundtrackName, sceenName);
            SoundManager.PlayMusic(soundtrackName);
        }

        isBusy = false;

        if (GlobalMapper.IsFirstPlay())
        {
            LoadComics(firstComics.name, "1");
        }
    }

    public static void LoadLevel(string lvlName)
    {

        if (!Instance.isBusy)
        {
            Instance.isBusy = true;
            Instance.LoadLevelInternal(lvlName);
        }
    }

    public static void LoadComics(string comicsName, string levelAfterComics)
    {
        if (!Instance.isBusy)
        {
            Instance.isBusy = true;
            Instance.LoadComicsInternal(comicsName, levelAfterComics);
        }
    }

    public static void LoadMainMenu()
    {
        if (!Instance.isBusy)
        {
            Instance.isBusy = true;
            Instance.LoadMainMenuInternal();
        }
    }

    public static void LevelFail()
    {
        var restartDelay = 1f;
        if (!Instance.isBusy)
        {
            Instance.isBusy = true;
            SelectionMenu.Instance.Hide();
            var currentLevel = SceneManager.GetActiveScene().name;
            if (int.TryParse(currentLevel, out int intLevel))
            {
                var attemptNum = LevelMapper.GetAttempts(currentLevel);
                LionAnalytics.LevelFail(intLevel, attemptNum);
            }
            else
            {
                var message = $"Level can't be parsed. Level name: {currentLevel}";
                LionAnalytics.ErrorEvent(ErrorEventType.Critical, message);
                Debug.LogError(message);
            }

            Instance.LoadLevelInternal(currentLevel, restartDelay);
        }
    }

    public static void CompleteLevel()
    {
        var currentLevel = SceneManager.GetActiveScene().name;
        if (int.TryParse(currentLevel, out int intLevel))
        {
            if (LevelMapper.GetStatus(currentLevel) != LevelStatus.Complete)
            {
                var attemptNum = LevelMapper.GetAttempts(currentLevel);
                var reward = new Reward("Dollar", "Main", RewardManager.rewardSum);
                LionAnalytics.LevelComplete(intLevel, attemptNum, reward: reward);
                LevelMapper.Complete(currentLevel);
                LevelMapper.ResetAttempt(currentLevel);
            }
        }
        else
        {
            var message = $"Level can't be parsed. Level name: {currentLevel}";
            LionAnalytics.ErrorEvent(ErrorEventType.Critical, message);
            Debug.LogError(message);
        }
    }

    public static void LevelRestart()
    {
        if (!Instance.isBusy)
        {
            Instance.isBusy = true;
            SelectionMenu.Instance.Hide();
            var currentLevel = SceneManager.GetActiveScene().name;

            if (int.TryParse(currentLevel, out int intLevel))
            {
                var attemptNum = LevelMapper.GetAttempts(currentLevel);
                LionAnalytics.LevelRestart(intLevel, attemptNum);
            }
            else
            {
                var message = $"Level can't be parsed. Level name: {currentLevel}";
                LionAnalytics.ErrorEvent(ErrorEventType.Critical, message);
                Debug.LogError(message);
            }

            Instance.LoadLevelInternal(currentLevel);
        }
    }

    private IEnumerator LoadLevelRoutine(string lvlName, float delay = 0)
    {
        yield return new WaitForSecondsRealtime(delay);
        UIManager.Instance.Hide();
        yield return new WaitForSecondsRealtime(reloadDelay);
        SelectionMenu.Instance.Hide();
        var asyncOperation = SceneManager.LoadSceneAsync(lvlName);

        while (!asyncOperation.isDone)
        {
            Instance.isBusy = false;
            yield return null;
        }
    }

    private void LoadComicsInternal(string comicsName, string levelAfterComics)
    {
        ComicsMapper.SetAfterShow(levelAfterComics);
        ComicsMapper.SetComicsToShow(comicsName);
        StartCoroutine(LoadLevelRoutine("Comics"));
    }

    private void LoadMainMenuInternal()
    {
        StartCoroutine(LoadLevelRoutine("LevelMenu"));
    }

    private void LoadLevelInternal(string lvlName, float delay = 0)
    {
        StartCoroutine(LoadLevelRoutine(lvlName, delay));
        if (int.TryParse(lvlName, out int intLevel))
        {
            LevelMapper.AddAttempt(lvlName);
            var attemptNum = LevelMapper.GetAttempts(lvlName);
            LionAnalytics.LevelStart(intLevel, attemptNum);
        }
        else
        {
            var message = $"Level can't be parsed. Level name: {lvlName}";
            LionAnalytics.ErrorEvent(ErrorEventType.Critical, message);
            Debug.LogError(message);
        }
    }

    private void InitAdjust(string adjustAppToken)
    {
        var adjustConfig = new AdjustConfig(
            adjustAppToken,
            AdjustEnvironment.Production, // AdjustEnvironment.Sandbox to test in dashboard
            true
        );
        adjustConfig.setLogLevel(AdjustLogLevel.Info); // AdjustLogLevel.Suppress to disable logs
        adjustConfig.setSendInBackground(true);
        new GameObject("Adjust").AddComponent<Adjust>(); // do not remove or rename
        // Adjust.addSessionCallbackParameter("foo", "bar"); // if requested to set session-level parameters
        //adjustConfig.setAttributionChangedDelegate((adjustAttribution) => {
        //  Debug.LogFormat("Adjust Attribution Callback: ", adjustAttribution.trackerName);
        //});
        Adjust.start(adjustConfig);
    }
}
