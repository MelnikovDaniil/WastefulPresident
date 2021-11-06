using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    public Security security;

    private Vector2 position;

    public void Start()
    {
        security.GetComponent<Animator>().Play("Agent_Rope");
        security.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    }

    public void SeuritySpawned()
    {
        position = security.transform.position;
        security.transform.parent = null;
        security.GetComponent<Animator>().Play("Agent_PeelingOff");
        security.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        security.transform.position = position;
    }

    public void FinishSpawn()
    {
        Destroy(gameObject);
    }
}
