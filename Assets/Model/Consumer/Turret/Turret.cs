using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : PowerConsumer
{
    public LineRenderer lazerPrefab;
    public Transform lazerPlace;
    public GameObject shootingParticles;
    public Animator animator;

    private LineRenderer lazer;
    private bool isShooting;
    public new void Start()
    {
        lazer = Instantiate(lazerPrefab, lazerPlace);
        lazer.positionCount = 2;
        lazer.SetPosition(0, lazerPlace.position);
        base.Start();
    }

    private void Update()
    {
        if (isActive)
        {
            var mask = LayerMask.GetMask("Ground", "Door", "Characters");
            var rayCastHit = Physics2D.Raycast(lazerPlace.position, Vector2.right * Mathf.Sign(transform.localScale.x), 200, mask);
            lazer.SetPosition(0, lazerPlace.position);
            lazer.SetPosition(1, rayCastHit.point);
            if (!isShooting && rayCastHit.collider.gameObject.layer == 3)
            {
                isShooting = true;
                animator.SetBool("isShooting", true);
                shootingParticles.SetActive(true);
            }
            else if (isShooting && rayCastHit.collider.gameObject.layer != 3)
            {
                animator.SetBool("isShooting", false);
                isShooting = false;
                shootingParticles.SetActive(false);
            }
        }
    }
    public override void UpdateState()
    {
        lazer.enabled = isActive;
        animator.SetBool("isActive", isActive);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var rayCastHit = Physics2D.Raycast(lazerPlace.position, Vector2.right * Mathf.Sign(transform.localScale.x));
        Gizmos.DrawLine(lazerPlace.position, rayCastHit.point);
    }
}
