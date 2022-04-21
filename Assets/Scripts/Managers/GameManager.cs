using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : BaseManager
{
    public static GameManager Instance;
    public UIManager uiManager;
    public ControllerManager controllerManager;
    public Character character;
    public float reloadDelay = 5;
    public bool destroyOnLoad = false;
    public string soundtrackName = "Soundtrack";

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
        character = FindObjectOfType<Character>();
        if (character != null)
        {
            character.OnDeath += () => ReloadLevel(1);
        }
        SoundManager.PlayMusic(soundtrackName);
        isBusy = false;
    }

    public static void Load(string lvlName)
    {
        GameManager.Instance.LoadLevel(lvlName);
    }

    public void LoadLevel(string lvlName)
    {
        if (!Instance.isBusy)
        {
            Instance.isBusy = true;
            StartCoroutine(LoadLevelRoutine(lvlName));
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

    public static void ReloadLevel(float delay = 0)
    {
        if (!Instance.isBusy)
        {
            Instance.isBusy = true;
            SelectionMenu.Instance.Hide();
            var currentLevel = SceneManager.GetActiveScene();
            Instance.StartCoroutine(Instance.LoadLevelRoutine(currentLevel.name, delay));
        }
    }
}
