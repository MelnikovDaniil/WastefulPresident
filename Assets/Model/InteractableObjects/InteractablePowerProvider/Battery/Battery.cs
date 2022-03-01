using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class Battery : InteractableObject
{
    public Rigidbody2D rigidbody2D;
    public SpriteRenderer spriteRenderer;
    public float throwForce = 3;

    private List<Collider2D> colliders;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        colliders = GetComponents<Collider2D>().ToList();
    }

    private void Start()
    {
        if (transform.parent == null)
        {
            Throw();
        }
    }

    public override void StartInteraction(IVisitor visitor)
    {
        base.StartInteraction(visitor);
    }

    public override void SuccessInteraction(IVisitor visitor)
    {
        visitor.TryTakeBattery(this);
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
        spriteRenderer.sortingLayerName = "Default";
        colliders.ForEach(x => x.enabled = false);
        rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
    }
}
