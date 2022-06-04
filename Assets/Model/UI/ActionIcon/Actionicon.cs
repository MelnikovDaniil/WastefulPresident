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

    public Character character;

    public void SetWaking()
    {
        icon.sprite = walkingSprite;
    }

    public void SetInteraction()
    {
        icon.sprite = interactionSprite;
    }

    public void Show(Character character)
    {
        this.character = character;
        animator.SetBool("IsActive", true);
        spriteRenderer.color = character.characterColor.color;
        foreach (var wave in waves)
        {
            wave.color = character.characterColor.color;
        }
    }

    public void Hide()
    {
        character = null;
        animator.SetBool("IsActive", false);
    }
}
