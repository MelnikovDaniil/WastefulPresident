using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using LionStudios.Suite.Analytics;
using LionStudios.Suite.Analytics.Events;
using LionStudios.Suite.Debugging;
using System;
using Facebook.Unity;
using com.adjust.sdk;
using ByteBrewSDK;

public class GameManager : BaseManager
{
    public static bool sdksIntegrated;
    public static GameManager Instance;
    public UIManager uiManager;
    public ControllerManager controllerManager;
    public ScriptableComicsChapter firstComics;
    public float reloadDelay = 5;
    public bool destroyOnLoad = false;

    [Space]
    public string soundtrackName;
    public float soundtrackVolume = 0.3f;

    private bool isBusy;
    private void Awake()
    {
        if (!sdksIntegrated)
        {
            StartCoroutine(SdkIntegrationRoutine());
            sdksIntegrated = true;
        }

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


        var sceenName = SceneManager.GetActiveScene().name;
        SoundManager.SetMusicVolume(soundtrackVolume);
        SoundManager.Sounds.ForEach(x =>
        {
            if (!x.Source.ignoreListenerPause)
            {
                x.Stop();
            }
        });
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

    private IEnumerator SdkIntegrationRoutine()
    {
        Debug.Log("Integration started");
        try
        {
            if (!FB.IsInitialized)
            {
                // Initialize the Facebook SDK
                FB.Init(InitCallback, OnHideUnity);
            }
            else
            {
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
            }
            Debug.Log("FB integrated");
        }
        catch (Exception)
        {

            Debug.LogError("FB inegration failed");
        }



        try
        {
            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            {
                // AppLovin SDK is initialized, start loading ads
                //MaxSdk.ShowMediationDebugger();
            };
            MaxSdk.SetSdkKey("lt7nsQAGQuderPVtuZ142vUL2g7KegOa8GqDSsEWObUThTamw5UpC5k7u9kT3XQ3bFxr0kSuomgBHN-nENHley");
            MaxSdk.SetUserId(SystemInfo.deviceUniqueIdentifier);
            MaxSdk.SetVerboseLogging(true);
            MaxSdk.InitializeSdk();
            Debug.Log("Max integrated");
        }
        catch (Exception)
        {

            Debug.LogError("Max inegration failed");
        }

        try
        {
            ByteBrew.InitializeByteBrew();
            Debug.Log("ByteBrew integrated");
        }
        catch
        {
            Debug.LogError("ByteBrew integration failed");
        }

        yield return new WaitForEndOfFrame();
        try
        {

            // import this package into the project:
            // https://github.com/adjust/unity_sdk/releases
#if UNITY_IOS
        /* Mandatory - set your iOS app token here */
        //InitAdjust("YOUR_IOS_APP_TOKEN_HERE");
#elif UNITY_ANDROID
            /* Mandatory - set your Android app token here */
            InitAdjust("dyqevqya0zy8");
#endif
            Debug.Log("Adjust integrated");
        }
        catch
        {
            Debug.LogError("Adjust integrationl failed");
        }

        try
        {
            LionAnalytics.GameStart();
            Debug.Log("LionAnalytics integrated");

            LionDebugger.Hide();
        }
        catch
        {
            Debug.LogError("LionAnalytics integration failed");
        }

        Debug.Log("Integration finished");
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

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }
}
