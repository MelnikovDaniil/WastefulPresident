using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour, IPortableObject
{
    [NonSerialized]
    public float speed;

    private LayerMask characterMask;
    private LayerMask wallMask;
    private int bulletSide;
    private TrailRenderer trailRenderer;

    public bool IsSmallTeleport => true;

    private void Start()
    {

        trailRenderer = GetComponent<TrailRenderer>();
        characterMask = LayerMask.GetMask("Characters");
        wallMask = LayerMask.GetMask("Door", "Ground");
    }

    private void Update()
    {
        bulletSide = (int)Mathf.Sign(transform.localScale.x);
        transform.position += transform.rotation * new Vector2(speed * bulletSide * Time.deltaTime, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((characterMask & (1 << collision.gameObject.layer)) > 0)
        {
            SoundManager.PlaySound("BulletBodyHit");
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((wallMask & (1 << collision.gameObject.layer)) > 0)
        {
            SoundManager.PlaySound("BulletWallHit");
            gameObject.SetActive(false);
        }
    }

    public void Teleport(Vector3 position, Quaternion rotationDifference)
    {
        trailRenderer.emitting = false;

        gameObject.transform.position = position;
        gameObject.transform.localRotation = gameObject.transform.localRotation * rotationDifference;
    }

    public void AfterTeleport(Vector2 direciton)
    {
        trailRenderer.emitting = true;
    }
}
