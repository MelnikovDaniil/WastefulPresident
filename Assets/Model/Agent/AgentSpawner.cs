using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    public Agent agent;

    private Vector2 position;

    public void Start()
    {
        agent.GetComponent<Animator>().Play("Agent_Rope");
        agent.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    }

    public void SeuritySpawned()
    {
        position = agent.transform.position;
        agent.transform.parent = null;
        agent.GetComponent<Animator>().Play("Agent_PeelingOff");
        agent.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        agent.transform.position = position;
    }

    public void FinishSpawn()
    {
        Destroy(gameObject);
    }
}
