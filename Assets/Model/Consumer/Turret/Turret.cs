using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : PowerConsumer
{
    public LineRenderer lazerPrefab;
    public Transform lazerPlace;
    public ParticleSystem shootingParticles;
    public Animator animator;
    public float activationDelay = 0.5f;

    private LineRenderer lazer;
    private bool isShooting;
    private bool isReadyForShoot;

    public new void Start()
    {
        lazer = Instantiate(lazerPrefab, lazerPlace);
        lazer.positionCount = 2;
        lazer.SetPosition(0, lazerPlace.position);
        base.Start();
    }

    private void Update()
    {
        if (isReadyForShoot)
        {
            var mask = LayerMask.GetMask("Ground", "Door", "Characters");
            var rayCastHit = Physics2D.Raycast(lazerPlace.position, Vector2.right * Mathf.Sign(transform.localScale.x), 200, mask);
            lazer.SetPosition(0, lazerPlace.position);
            lazer.SetPosition(1, rayCastHit.point);
            if (!isShooting && rayCastHit.collider.gameObject.layer == 3)
            {
                isShooting = true;
                animator.SetBool("isShooting", true);
                shootingParticles.Play();
            }
            else if (isShooting && rayCastHit.collider.gameObject.layer != 3)
            {
                animator.SetBool("isShooting", false);
                isShooting = false;
                shootingParticles.Stop();
            }
        }
        else if (isShooting)
        {
            animator.SetBool("isShooting", false);
            isShooting = false;
            shootingParticles.Stop();
        }
    }
    public override void UpdateState()
    {
        animator.SetBool("isActive", isActive);
        StartCoroutine(UpdateStateRoutine());
    }

    public IEnumerator UpdateStateRoutine()
    {
        if (isActive == true)
        {
            yield return new WaitForSeconds(activationDelay);
        }
        lazer.enabled = isActive;
        isReadyForShoot = isActive;
    }

    public void OnDrawGizmos()
    {
        var mask = LayerMask.GetMask("Ground", "Door", "Characters");
        var rayCastHit = Physics2D.Raycast(lazerPlace.position, Vector2.right * Mathf.Sign(transform.localScale.x), 200, mask);
        Gizmos.DrawLine(lazerPlace.position, rayCastHit.point);
    }
}
