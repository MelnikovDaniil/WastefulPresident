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

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        clickHand = Instantiate(clickHandPrefab, transform);
        clickHand.gameObject.SetActive(false);
        if (steps.Any())
        {
            ShowStep(0);
        }
    }

    public void ShowStep(int stepNumber)
    {
        waitingStep = true;
        var step = steps[stepNumber];
        if (step != null)
        {
            step.gameObject.SetActive(true);
            clickHand.position = step.transform.position;
            clickHand.gameObject.SetActive(true);
        }
    }

    public void HideHand()
    {
        clickHand.gameObject.SetActive(false);
    }

    public void StopGuide()
    {
        waitingStep = false;
    }

    public void Select(Human human)
    {
        SelectionMenu.Instance.SetUpNextCharacter(human);
    }
}
