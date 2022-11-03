using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GasPipe : InteractableObject
{
    public Vector2 outputPoint;
    public GasParticle gasParticlePrefab;
    public float gasLifetime;
    public float gasSpawnRate;

    private Vector2 spawnPoint;
    private Transform particleStorage;
    private bool isWorking;
    private Collider2D triggerCollider;
    private List<GasParticle> gasParticlePool = new List<GasParticle>();

    private void Awake()
    {
        isWorking = true;
        triggerCollider = GetComponent<BoxCollider2D>();
        particleStorage = new GameObject("particleStorage").transform;
    }

    private void Start()
    {
        spawnPoint = GetOutputPosition();
        GenerateParticles();
        StartCoroutine(SpawnGasParticleRoutine());
    }
    private void GenerateParticles()
    {
        var particlesCount = gasLifetime / gasSpawnRate;
        for (int i = 0; i < particlesCount; i++)
        {
            var particle = Instantiate(gasParticlePrefab, particleStorage);
            particle.Disable();
            gasParticlePool.Add(particle);
        }
    }

    private IEnumerator SpawnGasParticleRoutine()
    {
        GasParticle particle;

        while (isWorking)
        {
            yield return new WaitForSeconds(gasSpawnRate);
            particle = gasParticlePool.FirstOrDefault(x => !x.gameObject.activeSelf);
            if (particle == null)
            {
                particle = Instantiate(gasParticlePrefab, particleStorage);
                particle.Disable();
                gasParticlePool.Add(particle);
            }

            var x = Random.Range(-0.5f, 0.5f);
            var y = Random.Range(-0.5f, 0.5f);
            particle.transform.position = spawnPoint + new Vector2(x, y);
            particle.Enable(gasLifetime);
        }
    }

    public Vector2 GetOutputPosition()
    {
        return transform.position + (transform.rotation * outputPoint);
    }

    public override void SuccessInteraction(ICharacterVisitor visitor)
    {
        if (isWorking && visitor.ShutOffGusPipe())
        {
            isWorking = false;
            triggerCollider.enabled = false;
        }
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GetOutputPosition(), 1);
    }
}
