using Assets.Model.InteractableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Scenario : MonoBehaviour
{
    public President character;
    public GameObject turrel;
    public BombInteractableObject turrelBomb;
    public GameObject donut;
    public float donutTime = 3f;
    // Start is called before the first frame update

    private void Start()
    {
        CameraManager.Instance.CharacterCamera();
    }

    public void TurrelFrame()
    {
        CameraManager.Instance.SetTarget(turrel, 0.5f, Vector2.zero);
    }
    
    public void DonutFrame()
    {
        CameraManager.Instance.SetTarget(donut, 0.5f, Vector2.zero);
        StartCoroutine(BackToCharacter());
    }

    public void ExploadTurrelWithDelay()
    {
        StartCoroutine(ExploadRoutine());
    }

    private IEnumerator ExploadRoutine()
    {
        yield return new WaitForSeconds(0.6f);
        turrelBomb.Interect();
        turrelBomb.GetComponent<SpriteRenderer>().enabled = false;
    }

    private IEnumerator BackToCharacter()
    {
        yield return new WaitForSeconds(donutTime);
        CameraManager.Instance.CharacterCamera();
    }
}
