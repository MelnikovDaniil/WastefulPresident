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
            character.OnDeath += ReloadLevel;
        }
        SoundManager.PlayMusic("Soundtrack");
        isBusy = false;
    }

    public static void Load(string lvlName)
    {
        GameManager.Instance.LoadLevel(lvlName);
    }

    public void LoadLevel(string lvlName)
    {
        StartCoroutine(LoadLevelRoutine(lvlName));
    }

    private IEnumerator LoadLevelRoutine(string lvlName)
    {
        yield return new WaitForSecondsRealtime(1);
        UIManager.Instance.Hide();
        yield return new WaitForSecondsRealtime(reloadDelay - 1);
        SceneManager.LoadScene(lvlName);
    }

    public static void ReloadLevel()
    {
        if (!Instance.isBusy)
        {
            Instance.isBusy = true;
            SelectionMenu.Instance.Hide();
            Instance.StartCoroutine(Instance.ReloadLevelRoutine(0));
        }
    }

    private IEnumerator ReloadLevelRoutine(float delay = 1f)
    {
        yield return new WaitForSecondsRealtime(delay);
        UIManager.Instance.Hide();
        yield return new WaitForSecondsRealtime(reloadDelay - 1);
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
