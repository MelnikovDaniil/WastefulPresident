using LionStudios.Suite.Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    public string nextLevelName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var character = collision.gameObject.GetComponent<Character>();
        if (character != null)
        {
            UIManager.Instance.Finish(nextLevelName);

            if (LevelMapper.GetStatus(nextLevelName) == LevelStatus.Locked)
            {
                LevelMapper.Open(nextLevelName);
            }
            GameManager.CompleteLevel();
        }
    }
}
