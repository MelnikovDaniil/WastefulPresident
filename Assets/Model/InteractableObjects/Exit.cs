using UnityEngine;

public class Exit : MonoBehaviour
{
    public string nextLevelName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var character = collision.gameObject.GetComponent<Character>();
        if (character != null)
        {
            UIManager.Instance.Finish(nextLevelName);
            if (!LevelMapper.IsOpen(nextLevelName))
            {
                LevelMapper.SetCurrentLevel(nextLevelName);
            }
        }
    }
}
