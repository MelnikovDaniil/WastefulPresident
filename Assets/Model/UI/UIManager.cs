using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Animator effectAnimator;

    private void Awake()
    {
        Instance = this;
    }

    public void Light()
    {
        effectAnimator.SetTrigger("light");
    }

    public void Hide()
    {
        effectAnimator.SetTrigger("hide");
    }
}
