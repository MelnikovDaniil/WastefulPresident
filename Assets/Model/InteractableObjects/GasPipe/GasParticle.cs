using UnityEngine;

public class GasParticle : MonoBehaviour
{
    private float lifetime;

    private void Update()
    {
        if (lifetime > 0)
        {
            lifetime -= Time.deltaTime;
            if (lifetime <= 0)
            {
                Disable();
            }
        }
    }

    public void Enable(float lifetime)
    {
        this.lifetime = lifetime;
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        lifetime = 0;
        gameObject.SetActive(false);
    }
}
