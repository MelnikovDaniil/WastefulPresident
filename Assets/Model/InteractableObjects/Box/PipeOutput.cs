using System;
using UnityEngine;

public class PipeOutput : MonoBehaviour
{
    public Vector2 outputPoint;

    public Vector2 GetOutputPosition()
    {
        return transform.position + (transform.rotation * outputPoint);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GetOutputPosition(), 1);
    }
}
