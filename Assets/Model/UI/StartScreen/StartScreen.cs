using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
    public RectTransform startScreenRect;
    public RectTransform levelMenuRect;

    public Animator presidentAnimator;
    public float idlePeriod = 30f;

    private void Start()
    {
        startScreenRect.sizeDelta = new Vector2(800f, 800f / Camera.main.aspect);
        levelMenuRect.sizeDelta = new Vector2(800f, 800f / Camera.main.aspect);
        InvokeRepeating(nameof(PresidentIdle), idlePeriod, idlePeriod);
    }

    private void PresidentIdle()
    {
        presidentAnimator.SetTrigger("longIdle");
    }
}
