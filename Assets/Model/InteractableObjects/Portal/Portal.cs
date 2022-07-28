using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : InteractableObject
{
    public Portal secondPortal;
    public Vector3 wallCheckSize;
    public Vector3 wallCheckOffset;
    public LayerMask wallCheckLayers;
    public LayerMask portableObjectsLayers;

    public Color color;
    public List<SpriteRenderer> objectToColor;

    private Animator _animator;
    private Collider2D _portalZone;
    private bool afterTeleport;
    private bool isClosed;

    private Vector3 internalWallCheckOffset;

    private void Awake()
    {
        ColorPortal(color);
        internalWallCheckOffset = transform.localRotation * new Vector3(Mathf.Sign(transform.localScale.x) * wallCheckOffset.x, 0);
        _animator = GetComponent<Animator>();
        _portalZone = GetComponent<Collider2D>();
        if (secondPortal != null)
        {
            secondPortal.ColorPortal(color);
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

    public void ColorPortal(Color color)
    {
        this.color = color;
        foreach (var item in objectToColor)
        {
            item.color = color;
        }
    }

    public void ClosePortal()
    {
        isClosed = true;
        _animator.SetBool("open", false);
        Destroy(gameObject, 1);
    }

    public override void SuccessInteraction(ICharacterVisitor visitor)
    {
    }

    public IEnumerator TeleportHuman(IPortalVisitor visitor)
    {
        yield return new WaitForSeconds(0.15f);
        _animator.SetTrigger("enter");
        SoundManager.PlaySound("Portal");
        var offset = transform.localRotation * new Vector3(Mathf.Sign(transform.localScale.x), 0);
        var newPosition = transform.position + offset;
        visitor.Teleport(newPosition, offset);

        _portalZone.enabled = false;
        afterTeleport = true;
        StartCoroutine(EnableProtalZoneRoutine());
    }

    public void TeleportObject(GameObject obj)
    {
        var offset = transform.localRotation * new Vector3(Mathf.Sign(transform.localScale.x), 0);
        var previousPortalOffset = secondPortal.transform.localRotation 
            * new Vector3(Mathf.Sign(secondPortal.transform.localScale.x), 0);
        var rotationDifference = Quaternion.Euler(0, 0, Vector2.SignedAngle(-previousPortalOffset, offset));

        var newPosition = transform.position + offset;
        obj.transform.position = newPosition;
        obj.transform.localRotation = obj.transform.localRotation * rotationDifference;
        //obj.transform.localScale = new Vector3(
        //    Mathf.Abs(obj.transform.localScale.x) * Mathf.Sign(transform.localScale.x),
        //    obj.transform.localScale.y,
        //    obj.transform.localScale.z);
        obj.SetActive(true);

        if (obj.TryGetComponent(out Rigidbody2D rigidbody))
        {
            rigidbody.velocity = rotationDifference * rigidbody.velocity;
        }

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
        if (!isClosed)
        {
            if (!afterTeleport && collision.gameObject.TryGetComponent(out IPortalVisitor visitor))
            {
                afterTeleport = false;
                _animator.SetTrigger("enter");
                StartCoroutine(secondPortal.TeleportHuman(visitor));
            }
            else if ((portableObjectsLayers & (1 << collision.gameObject.layer)) > 0
                && !(collision.isTrigger && collision.GetComponent<InteractableObject>()))
            {
                afterTeleport = false;
                _animator.SetTrigger("enterObject");
                secondPortal.TeleportObject(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<ICharacterVisitor>() != null)
        {
            afterTeleport = false;
            _portalZone.enabled = true;
        }
    }

    private void OnDrawGizmos()
    {
        ColorPortal(color);
        Gizmos.color = Color.yellow;
        var offset = transform.localRotation * new Vector3(Mathf.Sign(transform.localScale.x) * wallCheckOffset.x, 0);
        var size = transform.localRotation * wallCheckSize;
        Gizmos.DrawWireCube(offset + transform.position, size);
    }
}
