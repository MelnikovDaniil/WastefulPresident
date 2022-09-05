using System;
using UnityEngine;

[Serializable]
public class ComicsFrame
{
    public Vector2 position;
    public RectTransform frame;
    public AudioClip appearanceSound;
    public string changeSoundTrackTo;
    public float shakeForce;
}
