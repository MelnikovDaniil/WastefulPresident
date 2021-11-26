using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComicsManager : BaseManager
{
    public List<Sprite> slides;
    public Image slide;

    private Queue<Sprite> sprites;

    private void Awake()
    {
        sprites = new Queue<Sprite>(slides);
    }
    // Start is called before the first frame update
    void Start()
    {
        NextSlide();
    }

    public void NextSlide()
    {
        if (sprites.Count > 0)
        {
            var sprite = sprites.Dequeue();
            slide.sprite = sprite;
        }
        else
        {
            GameManager.Instance.LoadLevel("Prologue");
        }
    }
}
