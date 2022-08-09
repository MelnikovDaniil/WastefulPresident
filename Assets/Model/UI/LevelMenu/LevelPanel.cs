using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelPanel : MonoBehaviour
{
    [Space]
    public Text moneyText;

    [Space]
    public LevelChapter chapterPrefab;

    [Space]
    public LevelButton passedButtonPrefab;
    public LevelButton currentButtonPrefab;
    public LevelButton nextButtonPrefab;
    public LevelButton futureButtonPrefab;

    [Space]
    public Transform chaptersPlace;

    [Space]
    public List<ScriptableChapter> chapters;

    [Space]
    public float movementTime = 0.2f;

    [Space]
    public ScrollRect scrollRect;
    public StartScreenScroll startScreenScrollRect;
    public CanvasGroup buttonsCanvasGroup;

    private ScriptableChapter currentChapter;

    private float currentMovementTime;
    private float firstPosition;
    private float secondPosition;

    public void Awake()
    {
        var firstLevel = chapters.First(x => x.levelNames.Any()).levelNames[0];
        if (LevelMapper.GetStatus(firstLevel) == LevelStatus.Locked)
        {
            LevelMapper.Open(firstLevel);
        }
        startScreenScrollRect.OnScrollFinished += ScrollToChapter;
    }

    public void Start()
    {
        UIManager.OnSkipLevel = UpdateMenu;
        buttonsCanvasGroup.alpha = 0;
        UpdateMenu();
    }

    public void UpdateMenu()
    {
        GenerateChapters();
        UpdateMoney();
    }

    private void Update()
    {
        if (currentMovementTime > 0)
        {
            var coof = (1.0f - currentMovementTime / movementTime);
            currentMovementTime -= Time.deltaTime;
            scrollRect.verticalNormalizedPosition = firstPosition + coof * (secondPosition - firstPosition);
            buttonsCanvasGroup.alpha = coof;
            if (currentMovementTime <= 0)
            {
                buttonsCanvasGroup.alpha = 1;
                scrollRect.verticalNormalizedPosition = secondPosition;
            }
            scrollRect.velocity = Vector2.zero;
        }
    }


    public void GenerateChapters()
    {
        var nextLevelSkip = false;
        var levelNumber = 1;
        LevelButton levelButton = null;

        foreach (Transform child in chaptersPlace)
        {
            Destroy(child.gameObject);
        }

        foreach (var chapter in chapters)
        {
            var createdChapter = Instantiate(chapterPrefab, chaptersPlace);
            createdChapter.background.sprite = chapter.backgroundSprite;

            if (chapter.comicsChapter != null)
            {
                LevelMapper.SetComicsBeforeLevel(chapter.comicsChapter.name, chapter.levelNames.FirstOrDefault());
                createdChapter.comicsNameText.text = chapter.comicsChapter.name;
                createdChapter.comicsButton.image.color = Color.white;
                createdChapter.comicsButton.image.sprite = chapter.comicsChapter.comicsButtonSprite;
                createdChapter.comicsButton.onClick.AddListener(
                    () => GameManager.LoadComics(chapter.comicsChapter.name, "LevelMenu"));
            }

            foreach (var level in chapter.levelNames)
            {
                var status = LevelMapper.GetStatus(level);
                if (status == LevelStatus.Complete)
                {
                    levelButton = Instantiate(passedButtonPrefab, createdChapter.levelContainer);
                    levelButton.button.onClick.AddListener(() => GameManager.LoadLevel(level));
                    levelButton.button.onClick.AddListener(() => SoundManager.PlaySound("LevelButton"));
                    
                }
                else if (status == LevelStatus.Avaliable)
                {
                    nextLevelSkip = true;
                    currentChapter = chapter;
                    levelButton = Instantiate(currentButtonPrefab, createdChapter.levelContainer);
                    levelButton.button.onClick.AddListener(() => GameManager.LoadLevel(level));
                    levelButton.button.onClick.AddListener(() => SoundManager.PlaySound("LevelButton"));
                }
                else if (nextLevelSkip)
                {
                    nextLevelSkip = false;
                    levelButton = Instantiate(nextButtonPrefab, createdChapter.levelContainer);
                    levelButton.button.onClick.AddListener(() => UIManager.Instance.SkipLevel(level));
                    levelButton.button.onClick.AddListener(() => SoundManager.PlaySound("LevelButton"));
                }
                else
                {
                    levelButton = Instantiate(futureButtonPrefab, createdChapter.levelContainer);
                }

                levelButton.levelNumber.text = levelNumber.ToString();
                levelNumber++;
            }
        }
    }

    public void ScrollToChapter()
    {
        SoundManager.PlaySound("SlideUp");
        currentMovementTime = movementTime;
        firstPosition = 1;
        secondPosition = 1.0f - (float)chapters.IndexOf(currentChapter) / (chapters.Count - 1f);
    }

    public void UpdateMoney()
    {
        var money = MoneyMapper.Get();
        moneyText.text = string.Empty;
        if (money > 0)
        {
            moneyText.text = money.ToString() + "$";
        }
    }
}
