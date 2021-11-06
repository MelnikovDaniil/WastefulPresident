using UnityEngine;
using UnityEngine.UI;

public class StickerView : MonoBehaviour
{
    public string stickerName;
    public Text description;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
}
