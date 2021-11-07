using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public bool allowForSecurity = true;
    public bool allowForPresedent = true;

    public abstract void Interect();
}
