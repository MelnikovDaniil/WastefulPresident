using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [NonSerialized]
    public float speed;

    private LayerMask layermask;
    private int bulletSide;

    private void Start()
    {
        layermask = LayerMask.GetMask("Ground", "Doors", "Characters");
    }

    private void Update()
    {
        bulletSide = (int)Mathf.Sign(transform.localScale.x);
        transform.position += transform.rotation * new Vector2(speed * bulletSide * Time.deltaTime, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((layermask & (1 << collision.gameObject.layer)) != 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((layermask & (1 << collision.gameObject.layer)) != 0)
        {
            gameObject.SetActive(false);
        }
    }
}
