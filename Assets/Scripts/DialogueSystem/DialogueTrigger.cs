using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public bool singleRun;

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue);

        if (singleRun)
        {
            Destroy(gameObject);
        }
    }
}
