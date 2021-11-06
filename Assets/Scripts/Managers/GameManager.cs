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
        SceneManager.LoadScene(lvlName);
    }

    private void ReloadLevel()
    {
        StartCoroutine(ReloadLevelRoutine());
    }

    private IEnumerator ReloadLevelRoutine()
    {
        yield return new WaitForSeconds(reloadDelay);
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
