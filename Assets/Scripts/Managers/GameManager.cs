using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Character character;
    public float reloadDelay = 5;
    // Start is called before the first frame update
    void Start()
    {
        character.OnDeath += ReloadLevel;
    }

    public void LoadLevel(string lvlName)
    {
        StartCoroutine(LoadLevelRoutine(lvlName));
    }

    private IEnumerator LoadLevelRoutine(string lvlName)
    {
        yield return new WaitForSeconds(1);
        UIManager.Instance.Hide();
        yield return new WaitForSeconds(reloadDelay - 1);
        SceneManager.LoadScene(lvlName);
    }

    private void ReloadLevel()
    {
        StartCoroutine(ReloadLevelRoutine());
    }

    private IEnumerator ReloadLevelRoutine()
    {
        yield return new WaitForSeconds(1);
        UIManager.Instance.Hide();
        yield return new WaitForSeconds(reloadDelay - 1);
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
