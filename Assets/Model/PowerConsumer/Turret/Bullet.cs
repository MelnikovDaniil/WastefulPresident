using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour
{
    [NonSerialized]
    public float speed;

    private LayerMask characterMask;
    private LayerMask wallMask;
    private int bulletSide;

    private void Start()
    {
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
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((wallMask & (1 << collision.gameObject.layer)) > 0)
        {
            gameObject.SetActive(false);
        }
    }
}
