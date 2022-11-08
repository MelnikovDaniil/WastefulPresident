using UnityEngine;

public class GasParticle : MonoBehaviour, IPortableObject
{
    public float appearanceTime = 0.5f;

    private float lifetime;
    private float currentAppearanceTime;
    
    private Rigidbody2D rigidbody2D;

    public bool IsSmallTeleport => true;

    public bool TriggerTeleport => false;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (lifetime > 0)
        {
            lifetime -= Time.deltaTime;
            currentAppearanceTime -= Time.deltaTime;
            if (currentAppearanceTime > 0)
            {
                var coof = 1f - (currentAppearanceTime / appearanceTime);
                transform.localScale = coof * Vector2.one;
            }

            if (lifetime <= 0)
            {
                Disable();
            }
        }
    }

    public void Enable(float lifetime)
    {
        currentAppearanceTime = appearanceTime;
        this.lifetime = lifetime;
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        lifetime = 0;
        gameObject.SetActive(false);
    }

    public void Teleport(Vector3 position, Quaternion rotationDifference)
    {
        currentAppearanceTime = appearanceTime;
        gameObject.transform.position = position;
        rigidbody2D.velocity = rotationDifference * rigidbody2D.velocity;
    }

    public void AfterTeleport(Vector2 direction, Quaternion rotationDifference)
    {
        Debug.Log("puh");
        direction += new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
        rigidbody2D.AddForce(direction * 3, ForceMode2D.Force);
    }
}
