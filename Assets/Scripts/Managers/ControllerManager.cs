using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerManager : BaseManager
{
    public static ControllerManager Instance;
    public List<AgentSkin> skins;
    public List<Agent> agents;
    public Character character;
    public Sprite characterIcon;

    public float followPeriod = 0.5f;
    public float securityDistacnceGap = 1;

    public Color startColor;

    private float presidentDistance;
    private bool validInput;
    private Color currentHumanColor;

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
        currentHumanColor = startColor;
        base.LoadManager();
        presidentDistance = 2f;
        agents = new List<Agent>();

        character = FindObjectOfType<Character>();
        character.icon = characterIcon;
        character.characterColor.color = startColor;
        character.SetColor(startColor);
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
            Color.RGBToHSV(currentHumanColor, out var H, out var S, out var V);
            currentHumanColor = Color.HSVToRGB((H + SelectionMenu.Instance.itemGap / 360.0f) % 1, S, V);

            agent.humanState = HumanState.Follow;

            agent.SetColor(currentHumanColor);
            agent.characterColor.color = currentHumanColor;
            agent.presidentStopDistance = presidentDistance;
            presidentDistance += securityDistacnceGap;

            var skin = skins.GetRandom();
            agent.spriteRenderer.sprite = skin.skin;
            agent.icon = skin.icon;
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
