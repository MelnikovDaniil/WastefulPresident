using Assets.Model.InteractableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Scenario : MonoBehaviour
{
    public President character;
    public Animator gutAnimator;
    public GameObject secretWall;
    public GameObject fire;
    public GameObject donut;
    public BombInteractableObject granage;


    public DialogueTrigger dialogue1;
    public DialogueTrigger dialogue2;
    public void Start()
    {
        CameraManager.Instance.CharacterCamera();
        PlayWithDelay(Frame1, 0.5f);
    }

    public void Frame1()
    {
        dialogue1.TriggerDialogue();
    }

    public void Frame2()
    {
        character.isLocked = true;
        CameraManager.Instance.SetTarget(gutAnimator.gameObject, 1, Vector2.zero);
        PlayWithDelay(Frame3, 4);
    }

    public void Frame3()
    {
        gutAnimator.enabled = false;
        foreach(Transform tr in gutAnimator.transform)
        {
            tr.gameObject.SetActive(false);
        }
        PlayWithDelay(Frame4, 2);
    }

    public void Frame4()
    {
        CameraManager.Instance.CharacterCamera();
        dialogue2.TriggerDialogue();
        character.isLocked = false;
    }

    public void Frame5()
    {
        PlayWithDelay(Frame6, 0.75f);
    }

    public void Frame6()
    {
        gutAnimator.GetComponent<SpriteRenderer>().enabled = false;
        gutAnimator.GetComponent<BombInteractableObject>().Interect();
        secretWall.SetActive(false);
        donut.SetActive(false);
        fire.SetActive(true);
    }

    public void DonatCamera()
    {
        CameraManager.Instance.SetTarget(donut, 1, Vector2.zero);
    }

    public void Granade1()
    {
        granage.gameObject.SetActive(true);
        granage.GetComponent<Rigidbody2D>().AddForce(new Vector2(2, 2), ForceMode2D.Impulse);
        PlayWithDelay(Granade2, 1);
    }

    public void Granade2()
    {
        granage.Interect();
        granage.gameObject.SetActive(false);
    }

    public void PlayWithDelay(Action action, float delay)
    {
        StartCoroutine(PlayWithDelayRoutine(action, delay));
    }

    private IEnumerator PlayWithDelayRoutine(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
