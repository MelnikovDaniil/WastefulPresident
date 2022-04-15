using System;
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
    public float tapRadius = 0.5f;

    public Color startColor;

    public bool destroyOnLoad = false;

    [Space]
    public int iconsPullSize = 3;
    public Actionicon actionIconPrefab;

    private List<Actionicon> actionIcons;

    private float presidentDistance;
    private bool validInput;
    private Color currentHumanColor;

    private Human currentHuman;

    private void Awake()
    {
        // start of new code
        if (Instance != null)
        {
            if (destroyOnLoad)
            {
                Destroy(Instance.gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
        // end of new code

        Instance = this;
        if (!destroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    public override void LoadManager()
    {
        currentHumanColor = startColor;
        base.LoadManager();
        presidentDistance = 2f;
        agents = new List<Agent>();
        currentHuman = null;

        actionIcons = new List<Actionicon>();
        for (var i = 0; i < iconsPullSize; i++)
        {
            actionIcons.Add(Instantiate(actionIconPrefab));
        }

        character = FindObjectOfType<Character>();
        AddAgents(FindObjectsOfType<Agent>().ToList());

        if (character != null)
        {
            character.icon = characterIcon;
            character.characterColor.color = startColor;
            character.SetColor(startColor);
            SelectionMenu.Instance.AddItem(character);
            // StartCoroutine(FollowPreidentRoutine());
        }
    }

    private void Update()
    {
        ValidateInput();
        if (character != null
            && !DialogueManager.isWorking
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
            agent.skinRenderer.sprite = skin.skin;
            agent.icon = skin.icon;
            SelectionMenu.Instance.AddItem(agent);
        }
    }

    public void SetUpTarget()
    {
        var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hitObjects = Physics2D.OverlapCircleAll(position, tapRadius)
            .Where(x => x.transform?.gameObject != null )
            .Select(x => x.transform.gameObject)
            .OrderBy(x => Vector2.Distance(x.transform.position, position))
            .ToList();

        var interactableObject = hitObjects.FirstOrDefault(x => x.GetComponent<InteractableObject>())?.GetComponent<InteractableObject>();
        var guideStep = hitObjects.FirstOrDefault(x => x.GetComponent<GuideStep>())?.GetComponent<GuideStep>();
        var human = hitObjects.FirstOrDefault(x => x.GetComponent<Human>())?.GetComponent<Human>();



        if (!GuideManager.waitingStep || guideStep != null)
        {
            if (human != null 
                && human.humanState != HumanState.Dead
                && human != currentHuman)
            {
                if (guideStep?.humanToSelect == human)
                {
                    guideStep?.Interact();
                }
                currentHuman?.HideColor();
                currentHuman = human;
                human.ShowColor();
            }
            else if (currentHuman != null)
            {
                if (guideStep?.humanToSelect == null)
                {
                    guideStep?.Interact();
                }

                actionIcons.FirstOrDefault(x => x.human == currentHuman)?.Hide();
                var actionIcon = actionIcons.FirstOrDefault(x => x.human == null) ?? actionIcons.First();

                if (interactableObject != null
                    && ((currentHuman is Character && interactableObject.forCharacter)
                        || (currentHuman is Agent && interactableObject.forAgent)))
                {

                    SendForInteraction(currentHuman, interactableObject);
                    actionIcon.transform.position = new Vector2(interactableObject.transform.position.x, interactableObject.transform.position.y + 2);
                    actionIcon.SetInteraction();

                    if (currentHuman is Agent)
                    {
                        character.SendOrder();
                    }
                }
                else
                {
                    currentHuman.WalkTo(position);
                    actionIcon.transform.position = (Vector2)position;
                    actionIcon.SetWaking();
                }

                actionIcon.Show(currentHuman);
                var deathInfo = new { actionIcon, currentHuman };
                currentHuman.OnDeath += () => DisableActionIconOnDeath(deathInfo.actionIcon, deathInfo.currentHuman);
                currentHuman.OnMovementFinish = () =>
                {
                    deathInfo.currentHuman.OnDeath -= () => DisableActionIconOnDeath(deathInfo.actionIcon, deathInfo.currentHuman);
                    actionIcon.Hide();
                };
            }
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

    public void SendForInteraction(Human human, InteractableObject interactableObject)
    {
        var complexPositioning = interactableObject as IComplexPositioning;
        if (complexPositioning != null)
        {
            var position = complexPositioning.GetPositionForInteraction(human);
            human.SetTarget(position);
        }
        else
        {
            human.SetTarget(interactableObject.transform.position);
        }
    }

    private void DisableActionIconOnDeath(Actionicon actionIcon, Human human)
    {
        actionIcon.Hide();
        if (currentHuman == human)
        {
            currentHuman.HideColor();
            currentHuman = null;
        };
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
