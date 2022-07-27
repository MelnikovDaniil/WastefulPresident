using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Swapper : InteractableObject
{
    public Swapper secondSwapper;
    [Space]
    public SpriteRenderer swapperColorRenderer;
    public GameObject detectionLight;
    public Vector3 detectionSize;
    public LayerMask swapObjectsLayers;

    [NonSerialized]
    public bool creatureDetected;

    private void Awake()
    {
        if (secondSwapper != null)
        {
            secondSwapper.secondSwapper = this;
            secondSwapper.swapperColorRenderer.color = swapperColorRenderer.color;
        }
    }

    private void Start()
    {
        detectionLight.SetActive(false);
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
            creatureDetected = !detectionLight.activeSelf;

            if (creatureDetected)
            {
                SoundManager.PlaySound("SwapperEnter");
            }
            else
            {
                SoundManager.PlaySound("SwapperExit");
            }
            detectionLight.SetActive(!detectionLight.activeSelf);
        }
    }
    public override void SuccessInteraction(ICharacterVisitor visitor)
    {
        if (creatureDetected && secondSwapper.creatureDetected)
        {
            SoundManager.PlaySound("Swapper");
            Swap();
            secondSwapper.Swap();
        }
    }

    public void Swap()
    {
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

    private bool IsDetectoinChanged(Collider2D[] colliders)
    {
        var creatures = colliders.Where(x => x != null).Select(x => x.GetComponent<Creature>());
        var creature = creatures
            .OrderBy(x => Vector2.Distance(x.transform.position, transform.position))
            .FirstOrDefault(x => x.characterState != CharacterState.Dead);
        return (creature != null && !detectionLight.activeSelf)
            || ((colliders.Length == 0 || creature == null) && detectionLight.activeSelf);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, detectionSize);
    }
}
