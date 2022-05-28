using System.Collections;
using UnityEngine;

public class Portal : InteractableObject
{
    public Portal secondPortal;
    public Vector3 wallCheckSize;
    public Vector3 wallCheckOffset;
    public LayerMask wallCheckLayers;

    private Animator _animator;
    private Collider2D _portalZone;
    private bool afterTeleport;
    private bool isClosed;

    //private Vector3 internalWallCheckSize;
    private Vector3 internalWallCheckOffset;

    private void Awake()
    {
        //internalWallCheckSize = transform.localRotation * wallCheckSize;
        internalWallCheckOffset = transform.localRotation * new Vector3(Mathf.Sign(transform.localScale.x) * wallCheckOffset.x, 0);
        _animator = GetComponent<Animator>();
        _portalZone = GetComponent<Collider2D>();
        if (secondPortal != null)
        {
            secondPortal.secondPortal = this;
        }
    }

    private void Start()
    {
        _animator.SetBool("open", true);
        if (secondPortal == null)
        {
            Debug.LogError("Second portal not found");
        }
    }

    private void Update()
    {
        if (!isClosed)
        {
            var colliders = Physics2D.OverlapBox(
                internalWallCheckOffset + transform.position,
                wallCheckSize,
                transform.rotation.z,
                wallCheckLayers);

            if (colliders == null)
            {
                ClosePortal();
                secondPortal.ClosePortal();
            }
        }
    }

    public void ClosePortal()
    {
        isClosed = true;
        _animator.SetBool("open", false);
        Destroy(gameObject, 1);
    }

    public override void SuccessInteraction(IVisitor visitor)
    {
    }

    public void Teleport(IVisitor visitor)
    {
        var offset = transform.localRotation * new Vector3(Mathf.Sign(transform.localScale.x), 0);
        var newPosition = transform.position + offset;
        visitor.Teleport(newPosition, offset);

        _portalZone.enabled = false;
        afterTeleport = true;
        StartCoroutine(EnableProtalZoneRoutine());
    }

    private IEnumerator EnableProtalZoneRoutine()
    {
        yield return new WaitForSeconds(0.25f);
        _portalZone.enabled = true;
        afterTeleport = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isClosed && !afterTeleport && collision.gameObject.TryGetComponent(out IVisitor visitor))
        {
            afterTeleport = false;
            secondPortal.Teleport(visitor);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<IVisitor>() != null)
        {
            afterTeleport = false;
            _portalZone.enabled = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        var offset = transform.localRotation * new Vector3(Mathf.Sign(transform.localScale.x) * wallCheckOffset.x, 0);
        var size = transform.localRotation * wallCheckSize;
        Gizmos.DrawWireCube(offset + transform.position, size);
    }
}
