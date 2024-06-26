using LionStudios.Suite.Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    public string nextLevelName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var president = collision.gameObject.GetComponent<President>();
        if (president != null)
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
