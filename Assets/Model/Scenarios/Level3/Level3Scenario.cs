using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Scenario : MonoBehaviour
{
    public President character;

    public DialogueTrigger dialogue1;

    public void Start()
    {
        CameraManager.Instance.CharacterCamera();
        dialogue1.TriggerDialogue();
    }

    public void AddStickerHighRoom()
    {
        StickerManager.Instance.AddSticker("HighRoom", "Откройте проход в верхней комнате");
    }
}
