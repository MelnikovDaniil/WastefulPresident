using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelChapter : MonoBehaviour
{
    public Button comicsButton;
    public GameObject commingSoonPanel;
    public TextMeshProUGUI comicsNameText;

    [Space]
    public List<LevelButton> buttons;
    public Transform levelContainer;
    public Image background;
}
