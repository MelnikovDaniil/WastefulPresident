using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Swapper : InteractableObject
{
    public Swapper secondSwapper;
    [Space]
    public SpriteRenderer detectionLight;
    [Space]
    public GameObject detectionIconFirst;
    public GameObject detectionIconSecond;

    public Vector3 detectionSize;
    public LayerMask swapObjectsLayers;

    public Color color;
    public List<SpriteRenderer> objectToColor;

    [NonSerialized]
    public bool creatureDetected;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        ColorSwapper(color);
        if (secondSwapper != null)
        {
            secondSwapper.ColorSwapper(color);
            secondSwapper.secondSwapper = this;
        }
    }

    private void Start()
    {
        //detectionLight.gameObject.SetActive(false);
        if (secondSwapper == null)
        {
            Debug.LogError("Second swapper not found");
        }
    }

    private void Update()
    {
        var colliders = Physics2D.OverlapBoxAll(transform.position, detectionSize, 0, swapObjectsLayers);

        if (IsDetectoinChanged(colliders))
        {
            creatureDetected = !creatureDetected;

            if (creatureDetected)
            {
                detectionIconFirst.SetActive(true);
                secondSwapper.detectionIconFirst.SetActive(true);
                if (secondSwapper.creatureDetected)
                {
                    detectionIconSecond.SetActive(true);
                    secondSwapper.detectionIconSecond.SetActive(true);
                }
                SoundManager.PlaySound("SwapperEnter");
            }
            else
            {
                detectionIconSecond.SetActive(false);
                secondSwapper.detectionIconSecond.SetActive(false);
                if (!secondSwapper.creatureDetected)
                {
                    detectionIconFirst.SetActive(false);
                    secondSwapper.detectionIconFirst.SetActive(false);
                }
                SoundManager.PlaySound("SwapperExit");
            }
            _animator.SetBool("detected", creatureDetected);
            //lightObject.SetActive(creatureDetected);
        }
    }
    public override void SuccessInteraction(ICharacterVisitor visitor)
    {
        if (creatureDetected)
        {
            if (secondSwapper.creatureDetected)
            {
                SoundManager.PlaySound("Swapper");
                Swap();
                secondSwapper.Swap();
            }
            else
            {
                NotEnoughthCreature();
                secondSwapper.NotEnoughthCreature();
            }
        }
    }

    public void Swap()
    {
        _animator.SetTrigger("swap");
        var colliders = Physics2D.OverlapBoxAll(transform.position, detectionSize, 0, swapObjectsLayers);

        var creatures = colliders.Where(x => x != null).Select(x => x.GetComponent<Creature>());
        var creature = creatures
            .OrderBy(x => Vector2.Distance(x.transform.position, transform.position))
            .FirstOrDefault(x => x.characterState != CharacterState.Dead);
        if (creature != null)
        {
            creature.WalkTo(secondSwapper.transform.position);
            creature.transform.position = secondSwapper.transform.position;
        }
        else
        {
            Debug.LogError("Trying to swap not a creature.");
        }
    }

    public void ColorSwapper(Color color)
    {
        this.color = color;
        foreach (var item in objectToColor)
        {
            item.color = color;
        }
        color.a = detectionLight.color.a;
        detectionLight.color = color;
    }

    public void NotEnoughthCreature()
    {
        _animator.SetTrigger("notEnoughth");
    }

    private bool IsDetectoinChanged(Collider2D[] colliders)
    {
        var creatures = colliders.Where(x => x != null).Select(x => x.GetComponent<Creature>());
        var creature = creatures
            .OrderBy(x => Vector2.Distance(x.transform.position, transform.position))
            .FirstOrDefault(x => x.characterState != CharacterState.Dead);
        return (creature != null && !creatureDetected)
            || ((colliders.Length == 0 || creature == null) && creatureDetected);
    }

    private void OnDrawGizmos()
    {
        ColorSwapper(color);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, detectionSize);
    }
}
