using System.Collections.Generic;
using UnityEngine;

public class Actionicon : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public List<SpriteRenderer> waves;
    public SpriteRenderer icon;
    public Animator animator;

    public Sprite walkingSprite;
    public Sprite interactionSprite;

    public Human human;

    public void SetWaking()
    {
        icon.sprite = walkingSprite;
    }

    public void SetInteraction()
    {
        icon.sprite = interactionSprite;
    }

    public void Show(Human human)
    {
        this.human = human;
        animator.SetBool("IsActive", true);
        spriteRenderer.color = human.characterColor.color;
        foreach (var wave in waves)
        {
            wave.color = human.characterColor.color;
        }
    }

    public void Hide()
    {
        human = null;
        animator.SetBool("IsActive", false);
    }
}
