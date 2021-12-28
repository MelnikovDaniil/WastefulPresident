using UnityEngine;

public class Exit : MonoBehaviour
{
    public string nextLevelName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Character>())
        {
            GameManager.Load(nextLevelName);
            if (!LevelMapper.IsOpen(nextLevelName))
            {
                LevelMapper.SetCurrentLevel(nextLevelName);
            }
        }
    }
}
