using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class ControllerManager : BaseManager
{
    public static ControllerManager Instance;
    public List<AgentSkin> skins;
    public List<Agent> agents;
    public President president;
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

    private Character currentCharacter;
    private Queue<AgentSkin> unUsedSkins;

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
        GuideManager.waitingStep = false;
        currentHumanColor = startColor;
        base.LoadManager();
        presidentDistance = 2f;
        agents = new List<Agent>();
        currentCharacter = null;

        actionIcons = new List<Actionicon>();
        for (var i = 0; i < iconsPullSize; i++)
        {
            actionIcons.Add(Instantiate(actionIconPrefab));
        }

        president = FindObjectOfType<President>();
        AddAgents(FindObjectsOfType<Agent>().ToList());

        if (president != null)
        {
            president.icon = characterIcon;
            president.characterColor.color = startColor;
            president.SetColor(startColor);
            SelectionMenu.Instance.AddItem(president);
            // StartCoroutine(FollowPreidentRoutine());
        }
    }

    private void Update()
    {
        ValidateInput();
        if (president != null
            && !DialogueManager.isWorking
            && !CameraManager.Instance.isMovingByTaps
            && president.characterState != CharacterState.Dead 
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

            agent.characterState = CharacterState.Follow;

            agent.SetColor(currentHumanColor);
            agent.characterColor.color = currentHumanColor;
            agent.presidentStopDistance = presidentDistance;
            presidentDistance += securityDistacnceGap;

            var skin = GetAgentSkin();
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
        var character = hitObjects.FirstOrDefault(x => x.GetComponent<Character>())?.GetComponent<Character>();



        if (!GuideManager.waitingStep || guideStep != null)
        {
            if (character != null 
                && character.characterState != CharacterState.Dead
                && character != currentCharacter)
            {
                if (guideStep?.characterToSelect == character)
                {
                    guideStep?.Interact();
                }
                currentCharacter?.HideColor();
                currentCharacter = character;
                character.ShowColor();
            }
            else if (currentCharacter != null)
            {
                if (guideStep?.characterToSelect == null)
                {
                    guideStep?.Interact();
                }

                actionIcons.FirstOrDefault(x => x.character == currentCharacter)?.Hide();
                var actionIcon = actionIcons.FirstOrDefault(x => x.character == null) ?? actionIcons.First();

                if (interactableObject != null
                    && ((currentCharacter is President && interactableObject.forCharacter)
                        || (currentCharacter is Agent && interactableObject.forAgent)))
                {

                    SendForInteraction(currentCharacter, interactableObject);
                    actionIcon.transform.position = new Vector2(interactableObject.transform.position.x, interactableObject.transform.position.y + 2);
                    actionIcon.SetInteraction();

                    if (currentCharacter is Agent)
                    {
                        president.SendOrder();
                    }
                }
                else
                {
                    currentCharacter.WalkTo(position);
                    actionIcon.transform.position = (Vector2)position;
                    actionIcon.SetWaking();
                }

                actionIcon.Show(currentCharacter);
                var deathInfo = new { actionIcon, currentCharacter };
                currentCharacter.OnDeath += () => DisableActionIconOnDeath(deathInfo.actionIcon, deathInfo.currentCharacter);
                currentCharacter.OnMovementFinish = () =>
                {
                    deathInfo.currentCharacter.OnDeath -= () => DisableActionIconOnDeath(deathInfo.actionIcon, deathInfo.currentCharacter);
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
            if (agent.characterState == CharacterState.Follow)
            {
                agent.FollowPresedent(president.transform.position);
            }
        }
    }

    public void SendForInteraction(Character character, InteractableObject interactableObject)
    {
        var complexPositioning = interactableObject as IComplexPositioning;
        if (complexPositioning != null)
        {
            var position = complexPositioning.GetPositionForInteraction(character);
            character.SetTarget(position);
        }
        else
        {
            character.SetTarget(interactableObject.transform.position);
        }
    }

    private AgentSkin GetAgentSkin()
    {
        if (unUsedSkins == null || !unUsedSkins.Any())
        {
            unUsedSkins = new Queue<AgentSkin>(skins.OrderBy(a => Random.value));
        }
        return unUsedSkins.Dequeue();
    }

    private void DisableActionIconOnDeath(Actionicon actionIcon, Character character)
    {
        actionIcon.Hide();
        if (currentCharacter == character)
        {
            currentCharacter.HideColor();
            currentCharacter = null;
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
