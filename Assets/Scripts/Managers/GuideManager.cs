using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuideManager : BaseManager
{
    public static bool waitingStep;
    public static GuideManager Instance;
    public List<GuideStep> steps;
    public Transform clickHandPrefab;

    private Transform clickHand;
    private Queue<GuideStep> stepQueue;
    private float nextStepDelay;
    public void Awake()
    {
        Instance = this;
        stepQueue = new Queue<GuideStep>();
    }

    private void Start()
    {
        steps.ForEach(x => x.gameObject.SetActive(false));
        stepQueue = new Queue<GuideStep>(steps);
        clickHand = Instantiate(clickHandPrefab, transform);
        clickHand.gameObject.SetActive(false);
        if (steps.Any())
        {
            NextStep(0);
        }
    }

    public void NextStep(float delay)
    {
        StartCoroutine(NextStepRoutine(delay));
    }

    private IEnumerator NextStepRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (stepQueue.Any())
        {
            waitingStep = true;
            var step = stepQueue.Dequeue();
            step.gameObject.SetActive(true);
            clickHand.parent = step.transform;
            clickHand.localPosition = Vector2.zero;
            clickHand.gameObject.SetActive(true);
        }
        else
        {
            waitingStep = false;
            HideHand();
        }
    }

    public void HideHand()
    {
        clickHand.gameObject.SetActive(false);
    }

    public void SetNextStepDelay(float delay)
    {
        nextStepDelay = delay;
    }

    public void Select(Character character)
    {
        SelectionMenu.Instance.SetUpNextCharacter(character);
        SelectionMenu.Instance.OnSelection.AddListener(
            () => {
                NextStep(nextStepDelay);
                SelectionMenu.Instance.OnSelection.RemoveAllListeners();
            });
    }
}
