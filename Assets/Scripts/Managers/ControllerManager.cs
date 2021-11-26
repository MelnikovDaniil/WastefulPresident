using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerManager : BaseManager
{
    public static ControllerManager Instance;
    public List<Agent> agents;
    public Character character;

    public float followPeriod = 0.5f;
    public float securityDistacnceGap = 1;

    private float presidentDistance;
    private bool validInput;

    private void Awake()
    {
        // start of new code
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // end of new code

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public override void LoadManager()
    {
        base.LoadManager();
        presidentDistance = 2f;
        agents = new List<Agent>();

        character = FindObjectOfType<Character>();
        SelectionMenu.Instance.AddItem(character);
        AddAgents(FindObjectsOfType<Agent>().ToList());

        StartCoroutine(FollowPreidentRoutine());
    }

    private void Update()
    {
        ValidateInput();
        if (!DialogueManager.isWorking
            && !CameraManager.Instance.isMovingByTaps
            && character.humanState != HumanState.Dead 
            && Input.GetMouseButtonUp(0) 
            && validInput)
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

        var interactableObject = hitObjects.FirstOrDefault(x => x.GetComponent<InteractableObject>())?.GetComponent<InteractableObject>();

        if (interactableObject != null)
        {
            SelectionMenu.Instance.Show(interactableObject);
        }
        else if (SelectionMenu.isSelecting)
        {
            SelectionMenu.Instance.Hide();
        }
        else if (character.humanState == HumanState.Waiting 
            || character.humanState == HumanState.MovingToInteract
            || character.humanState == HumanState.Walking)
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

    private void ValidateInput()
    {
    #if UNITY_STANDALONE || UNITY_EDITOR
        //DESKTOP COMPUTERS
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                validInput = false;
            else
                validInput = true;
        }
    #else
        //MOBILE DEVICES
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                validInput = false;
            else
                validInput = true;
        }
    #endif
    }
}
