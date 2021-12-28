using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelPanel : MonoBehaviour
{
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
        if (string.IsNullOrEmpty(LevelMapper.GetCurrentLevel()))
        {
            var firstLevel = chapters[0].levels[0];
            LevelMapper.SetCurrentLevel(firstLevel.name);
        }
        startScreenScrollRect.OnScrollFinished += ScrollToChapter;
    }

    public void Start()
    {
        buttonsCanvasGroup.alpha = 0;
        GenerateChapters();
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
        var currentLevelName = LevelMapper.GetCurrentLevel();
        var nextLevelSkip = false;
        var lockedLevels = false;
        var levelNumber = 1;
        LevelButton levelButton = null;

        foreach (var chapter in chapters)
        {
            var createdChapter = Instantiate(chapterPrefab, chaptersPlace);
            createdChapter.background.sprite = chapter.backgroundSprite;

            foreach (var level in chapter.levels)
            {
                if (level.name == currentLevelName)
                {
                    nextLevelSkip = true;
                    currentChapter = chapter;
                    levelButton = Instantiate(currentButtonPrefab, createdChapter.transform);
                    levelButton.button.onClick.AddListener(() => LoadLevel(level.name));
                }
                else if (nextLevelSkip)
                {
                    nextLevelSkip = false;
                    lockedLevels = true;
                    levelButton = Instantiate(nextButtonPrefab, createdChapter.transform);
                }
                else if (lockedLevels)
                {
                    levelButton = Instantiate(futureButtonPrefab, createdChapter.transform);
                }
                else
                {
                    levelButton = Instantiate(passedButtonPrefab, createdChapter.transform);
                    levelButton.button.onClick.AddListener(() => LoadLevel(level.name));
                }

                levelButton.levelNumber.text = levelNumber.ToString();
                levelNumber++;
            }
        }
    }

    public void ScrollToChapter()
    {
        currentMovementTime = movementTime;
        firstPosition = 1;
        secondPosition = 1.0f - (float)chapters.IndexOf(currentChapter) / (chapters.Count - 1f);
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
