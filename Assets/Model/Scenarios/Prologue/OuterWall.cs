using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuterWall : MonoBehaviour
{
    public float wallTime = 2f;
    private float currentAlpha = 0;

    private bool isShowing;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isShowing && spriteRenderer.color.a > 0)
        {
            currentAlpha = spriteRenderer.color.a - Time.deltaTime / wallTime;

            spriteRenderer.color = new Color(
                spriteRenderer.color.r,
                spriteRenderer.color.g,
                spriteRenderer.color.b,
                currentAlpha);
        }
        else if (spriteRenderer.color.a < 1)
        {
            currentAlpha = spriteRenderer.color.a + Time.deltaTime / wallTime;
            spriteRenderer.color = new Color(
                spriteRenderer.color.r,
                spriteRenderer.color.g,
                spriteRenderer.color.b,
                currentAlpha);
        }
    }

    public void Hide()
    {
        isShowing = false;
    }

    public void Show()
    {
        isShowing = true;
    }
}
