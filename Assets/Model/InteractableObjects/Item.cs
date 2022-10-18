using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Item : InteractableObject
{
    public float throwForce = 3;
    public Rigidbody2D rigidbody2D;
    public SpriteRenderer spriteRenderer;

    private List<Collider2D> colliders;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        colliders = GetComponents<Collider2D>().ToList();
    }

    public void Start()
    {
        if (transform.parent == null)
        {
            Throw();
        }
    }

    public void Throw()
    {
        spriteRenderer.sortingLayerName = "Item";
        colliders.ForEach(x => x.enabled = true);
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        rigidbody2D.freezeRotation = true;
        rigidbody2D.AddForce(new Vector2(Random.Range(-1f, 1f), 1) * throwForce, ForceMode2D.Impulse);
    }

    public void Hold()
    {
        transform.localScale = Vector3.one;
        spriteRenderer.enabled = true;
        spriteRenderer.sortingLayerName = "Default";
        colliders.ForEach(x => x.enabled = false);
        rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
    }

    public override void StartInteraction(ICharacterVisitor visitor)
    {
        visitor.StartTakingItem(this);
        base.StartInteraction(visitor);
    }

    public override void SuccessInteraction(ICharacterVisitor visitor)
    {
        visitor.TryTakeItem(this);
    }
}
