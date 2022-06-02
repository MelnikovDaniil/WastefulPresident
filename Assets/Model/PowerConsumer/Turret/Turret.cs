using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Turret : PowerConsumer
{
    public LineRenderer lazerPrefab;
    public Transform lazerPlace;
    public ParticleSystem sleevesParticles;
    public Animator animator;
    public float activationDelay = 0.5f;

    [Space]
    public Transform shootingPlace;
    public Bullet bulletPrefab;
    public int fireRateOverTime = 10;
    public float bulletSpeed;
    public int bulletPoolSize = 1000;

    private List<GameObject> bulletsPool;
    private List<LineRenderer> lazers;
    private bool isShooting;
    private bool isReadyForShoot;

    private LayerMask lazerMask;
    private LayerMask portalMask;

    public new void Start()
    {
        lazerMask = LayerMask.GetMask("Ground", "Door", "Characters", "Portal");
        portalMask = LayerMask.GetMask("Portal");
        bulletsPool = new List<GameObject>();
        for (int i = 0; i < bulletPoolSize; i++)
        {
            var bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.speed = bulletSpeed;
            bullet.gameObject.SetActive(false);
            bulletsPool.Add(bullet.gameObject);
        }

        lazers = new List<LineRenderer>();
        StartCoroutine(ShootingRouting());
        base.Start();
    }

    private void Update()
    {
        if (isReadyForShoot)
        {
            var hitCollider = RaycastLazer(
                lazerPlace.position,
                transform.rotation * Vector2.right * Mathf.Sign(transform.localScale.x));
            if (!isShooting && hitCollider.gameObject.layer == 3)
            {
                isShooting = true;
                animator.SetBool("isShooting", true);
                sleevesParticles.Play();
            }
            else if (isShooting && hitCollider.gameObject.layer != 3)
            {
                animator.SetBool("isShooting", false);
                isShooting = false;
                sleevesParticles.Stop();
            }
        }
        else if (isShooting)
        {
            animator.SetBool("isShooting", false);
            isShooting = false;
            sleevesParticles.Stop();
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
        lazers.ForEach(lazer => lazer.enabled = isActive);
        isReadyForShoot = isActive;
    }

    public IEnumerator ShootingRouting()
    {
        while (true)
        {
            if (isShooting)
            {
                var bullet = GetPooledBullet();
                if (bullet != null)
                {
                    bullet.transform.position = shootingPlace.position;
                    bullet.transform.rotation = transform.rotation;
                    bullet.transform.localScale = new Vector3(
                        Mathf.Abs(bullet.transform.localScale.x) * Mathf.Sign(transform.localScale.x),
                        bullet.transform.localScale.y,
                        bullet.transform.localScale.z);
                    bullet.SetActive(true);
                }
            }

            yield return new WaitForSeconds(1f / fireRateOverTime);
        }
    }

    public Collider2D RaycastLazer(Vector2 startPosition, Vector2 direction, int lazerNumber = 0)
    {
        if (lazers.Count < lazerNumber + 1)
        {
            var newLaser = Instantiate(lazerPrefab);
            newLaser.positionCount = 2;
            lazers.Add(newLaser);
        }
        var rayCastHit = Physics2D.Raycast(startPosition, direction, 200, lazerMask);

        var lazer = lazers[lazerNumber];
        lazer.enabled = true;
        lazer.SetPosition(0, startPosition);
        lazer.SetPosition(1, rayCastHit.point);

        var portalHit = Physics2D.OverlapCircle(rayCastHit.point, 0.1f, portalMask);
        if (lazerNumber != 20 && portalHit != null && portalHit.TryGetComponent(out Portal portal))
        {
            var nextPortalDirection =
                portal.secondPortal.transform.localRotation * new Vector2(portal.secondPortal.transform.localScale.x, 0);
            return RaycastLazer(
                portal.secondPortal.transform.position + nextPortalDirection,
                nextPortalDirection,
                lazerNumber + 1);
        }
        else
        {
            for (int i = lazerNumber + 1; i < lazers.Count; i++)
            {
                lazers[i].enabled = false;
            }
        }

        return rayCastHit.collider;
    }

    public void OnDrawGizmos()
    {
        var mask = LayerMask.GetMask("Ground", "Door", "Characters");
        var rayCastHit = Physics2D.Raycast(lazerPlace.position, Vector2.right * Mathf.Sign(transform.localScale.x), 200, mask);
        Gizmos.DrawLine(lazerPlace.position, rayCastHit.point);
    }

    private GameObject GetPooledBullet()
    {
        return bulletsPool.FirstOrDefault(x => !x.gameObject.activeInHierarchy);
    }
}
