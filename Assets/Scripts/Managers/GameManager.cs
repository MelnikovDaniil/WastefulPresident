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

    private void Awake()
    {
        // start of new code
        if (Instance != null)
        {
            GameManager.Instance.LoadManager();

            Destroy(gameObject);
        }
        // end of new code
        else
        {
            Instance = this;
            Instantiate(uiManager);
            Instantiate(controllerManager);
            DontDestroyOnLoad(gameObject);

            LoadManager();
        }
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
        Instance.StartCoroutine(Instance.ReloadLevelRoutine());
    }

    private IEnumerator ReloadLevelRoutine()
    {
        yield return new WaitForSecondsRealtime(1);
        UIManager.Instance.Hide();
        yield return new WaitForSecondsRealtime(reloadDelay - 1);
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
