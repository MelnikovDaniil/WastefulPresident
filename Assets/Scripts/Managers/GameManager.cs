using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Character character;
    public float reloadDelay = 5;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        if (character != null)
        {
            character.OnDeath += ReloadLevel;
        }
        SoundManager.PlayMusic("Soundtrack");
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

    public void ReloadLevel()
    {
        StartCoroutine(ReloadLevelRoutine());
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
