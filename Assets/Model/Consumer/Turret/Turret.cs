using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : PowerConsumer
{
    public LineRenderer lazerPrefab;
    public Vector3 laserStartPosition;
    public float bulletsForce;
    public GameObject shootingParticles;

    private LineRenderer lazer;
    private bool isShooting;
    public new void Start()
    {
        lazer = Instantiate(lazerPrefab, transform);
        lazer.positionCount = 2;
        lazer.SetPosition(0, laserStartPosition + transform.position);
        base.Start();
    }

    private void Update()
    {
        if (isActive)
        {
            var rayCastHit = Physics2D.Raycast(laserStartPosition + transform.position, Vector2.right * Mathf.Sign(transform.localScale.x));
            lazer.SetPosition(1, rayCastHit.point);
            if (!isShooting && rayCastHit.collider.gameObject.layer == 3)
            {
                isShooting = true;
                shootingParticles.SetActive(true);
            }
            else if (isShooting && rayCastHit.collider.gameObject.layer != 3)
            {
                isShooting = false;
                shootingParticles.SetActive(false);
            }
        }
    }
    public override void UpdateState()
    {
        lazer.enabled = isActive;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var rayCastHit = Physics2D.Raycast(laserStartPosition + transform.position, Vector2.right * Mathf.Sign(transform.localScale.x));
        Gizmos.DrawLine(laserStartPosition + transform.position, rayCastHit.point);
    }
}
