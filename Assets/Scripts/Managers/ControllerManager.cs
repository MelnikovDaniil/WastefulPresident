using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerManager : MonoBehaviour
{
    public List<Agent> agents;
    public Character character;

    public float followPeriod = 0.5f;
    public float securityDistacnceGap = 1;

    private float presidentDistance;

    private void Start()
    {
        presidentDistance = 2f;
        agents = new List<Agent>();

        character = FindObjectOfType<Character>();
        SelectionMenu.Instance.AddItem(character);
        AddAgents(FindObjectsOfType<Agent>().ToList());

        CameraManager.Instance.CharacterCamera();
        StartCoroutine(FollowPreidentRoutine());
    }

    private void Update()
    {
        if (!DialogueManager.isWorking && character.humanState != HumanState.Dead && Input.GetMouseButtonDown(0))
        {
            SetUpTarget();
        }
    }
    public void AddAgents(List<Agent> newSecurities)
    {
        this.agents.AddRange(newSecurities);
        foreach (var agent in agents)
        {
            agent.humanState = HumanState.Follow;

            var color = Random.ColorHSV();
            agent.characterColor.color = color;
            agent.presidentStopDistance = presidentDistance;
            presidentDistance += securityDistacnceGap;
            SelectionMenu.Instance.AddItem(agent);
        }
    }

    public void SetUpTarget()
    {
        var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hitObjects = Physics2D.RaycastAll(position, Vector3.forward)
            .Where(x => x.transform?.gameObject != null )
            .Select(x => x.transform.gameObject)
            .ToList();

        var pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        var interactableObject = hitObjects.FirstOrDefault(x => x.GetComponent<InteractableObject>());

        if (interactableObject != null)
        {
            SelectionMenu.Instance.Show(interactableObject.transform.position);
        }
        else if (SelectionMenu.isSelecting && !raycastResults.Any())
        {
            SelectionMenu.Instance.Hide();
        }
        else if ((character.humanState == HumanState.Waiting 
            || character.humanState == HumanState.MovingToInteract) && !raycastResults.Any())
        {
            character.WalkTo(position);
        }
    }

    public IEnumerator FollowPreidentRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(followPeriod);
            FollowPresident();
        }
    }

    public void FollowPresident()
    {
        foreach (var agent in agents)
        {
            if (agent.humanState == HumanState.Follow)
            {
                agent.FollowPresedent(character.transform.position);
            }
        }
    }
}
