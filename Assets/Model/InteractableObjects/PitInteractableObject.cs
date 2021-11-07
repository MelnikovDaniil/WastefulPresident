using System;
using System.Collections;
using UnityEngine;

public class PitInteractableObject : InteractableObject
{
    public event Action OnPitClosing;
    public event Action OnPitOpenning;

    public float pitTime = 10f;
    public Collider2D collider;

    public override void Interect()
    {
        OnPitClosing?.Invoke();
        collider.enabled = true;
        StartCoroutine(OpenPitRoutine());
        enabled = false;
    }

    public IEnumerator OpenPitRoutine()
    {
        yield return new WaitForSeconds(pitTime);
        OnPitOpenning?.Invoke();
        collider.enabled = false;
        enabled = true;
    }
}
