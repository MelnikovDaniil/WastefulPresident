using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PrologueScenario : MonoBehaviour
{
    public Character character;
    public GameObject cityPlace;
    public GameObject destroedCity;
    public GameObject meteorit;
    public GameObject agetSpawnerPlace;

    public DialogueTrigger dialogue1;
    public DialogueTrigger dialogue2;

    public List<AgentSpawner> agentSpawners;
    public List<Security> agents;

    //public float frame2Delay = 2;
    public float frame3Delay = 5;
    public float frame4Delay = 5;
    public float frame5Delay;
    public float frame6Delay;
    public float frame7Delay;

    private void Start()
    {
        Frame1();
    }

    public void Frame1()
    {
        dialogue1.TriggerDialogue();
        CameraManager.Instance.CraracterCamera();
        character.PlayAnimation("Writting");
        character.isLocked = true;
        //PlayWithDelay(Frame2, frame2Delay);
    }

    public void Frame2()
    {
        CameraManager.Instance.SetTarget(cityPlace, 2, Vector2.zero);
        meteorit.SetActive(true);
        PlayWithDelay(Frame3, frame3Delay);
    }

    public void Frame3()
    {
        destroedCity.SetActive(true);
        meteorit.SetActive(false);
        UIManager.Instance.Light();
        ShakingManager.Instance.Shake();
        PlayWithDelay(Frame4, frame4Delay);
    }
    
    public void Frame4()
    {
        CameraManager.Instance.CraracterCamera();
        dialogue2.TriggerDialogue();
    }

    public void Frame5()
    {
        character.transform.localScale *= new Vector2(-1, 1);
        CameraManager.Instance.SetTarget(agetSpawnerPlace, 1, Vector2.zero);
        StartCoroutine(SpawnAgentsRoutine());
    }

    public void Frame6()
    {
        CameraManager.Instance.CraracterCamera();
        character.GetComponent<SecurityManager>().AddSecurities(agents);
        character.isLocked = false;
    }

    public IEnumerator SpawnAgentsRoutine()
    {
        foreach (var agent in agentSpawners)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));
            agent.gameObject.SetActive(true);
        }
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
