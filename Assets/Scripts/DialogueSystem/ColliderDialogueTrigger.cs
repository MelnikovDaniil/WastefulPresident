using UnityEngine;

public class ColliderDialogueTrigger : DialogueTrigger
{
    private CircleCollider2D circle;

    private void OnDrawGizmos()
    {
        if (circle != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, circle.radius);
        }
        else
        {
            circle = GetComponent<CircleCollider2D>();
        }
    }
}
