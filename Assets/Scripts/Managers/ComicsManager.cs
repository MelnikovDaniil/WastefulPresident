using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ComicsManager : MonoBehaviour
{
    public List<ScriptableComicsChapter> chapters;
    public Transform frameStorage;

    public Animator frameAnimator;
    public Transform animatedFramePlace;
    public Image background;

    public float frameShowTime = 3;
    public float movingTime;

    private ScriptableComicsChapter currentChapter;
    private ComicsPage currentPage;

    private float currentShowTime;

    private float currentMovingTime;
    private RectTransform movingFrame;

    private Vector2 frameStartPosition;
    private Vector2 frameTargetPosition;

    private Vector2 showFrameSize = new Vector2(780, 1200);
    private Vector2 showScale;

    private float backgroundStartOpacity;
    private float shakeForce;
    private IEnumerator frameShowRoutine;

    private void Start()
    {
        currentShowTime = frameShowTime;
        currentMovingTime = movingTime;
        backgroundStartOpacity = background.color.a;
        currentChapter = GetChapter();
        currentPage = currentChapter.comicsPages.FirstOrDefault();
        StartCoroutine(ShowPageRoutine(currentPage));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (movingFrame != null)
            {
                NextFrame();
            }
            else
            {
                NextPage();
            }
        }


        if (shakeForce > 0 && currentShowTime < frameShowTime)
        {
            currentShowTime += Time.deltaTime;

            var coof = 1 - (currentShowTime / frameShowTime);
            var transform = animatedFramePlace.GetChild(0);

            transform.localPosition = new Vector2(
                Random.Range(-shakeForce, shakeForce),
                Random.Range(-shakeForce, shakeForce)) * coof;
        }

        if (movingFrame != null && currentMovingTime < movingTime)
        {
            currentMovingTime += Time.deltaTime;
            var coof = currentMovingTime / movingTime;

            movingFrame.anchoredPosition = Vector3.Lerp(frameStartPosition, frameTargetPosition, coof);
            movingFrame.localScale = Vector2.Lerp(showScale, Vector3.one, coof);
            background.color = new Color(background.color.r, background.color.g, background.color.b, (1 - coof) * backgroundStartOpacity);
        }
    }

    private void NextFrame()
    {
        StopCoroutine(frameShowRoutine);
        currentMovingTime = movingTime;
        currentShowTime = frameShowTime;

        if (movingFrame.TryGetComponent(out Animator frameAnimator))
        {
            frameAnimator.Play(0, 0, 1);
        }

        movingFrame.transform.localPosition = Vector2.zero;
        movingFrame.parent = frameStorage;
        movingFrame.anchoredPosition = frameTargetPosition;
        movingFrame.localScale = Vector3.one;
        movingFrame = null;
        background.color = new Color(background.color.r, background.color.g, background.color.b, 0);
    }

    private void NextPage()
    {
        var pageIndex = currentChapter.comicsPages.IndexOf(currentPage);
        var nextPage = currentChapter.comicsPages.ElementAtOrDefault(pageIndex + 1);
        if (nextPage != null)
        {
            currentPage = nextPage;
            StopAllCoroutines();
            StartCoroutine(ShowPageRoutine(nextPage));
        }
        else
        {
            var levelAfterShow = ComicsMapper.GetAfterShow();
            if (levelAfterShow != "LevelMenu")
            {
                ComicsMapper.SetAfterShow("LevelMenu");
                GameManager.LoadLevel(levelAfterShow);
            }
            else
            {
                GameManager.LoadMainMenu();
            }
        }
    }

    private IEnumerator ShowPageRoutine(ComicsPage page)
    {
        Clear();

        foreach (var frameInfo in page.frames)
        {
            var frameObj = Instantiate(frameInfo.frame, Vector2.zero, Quaternion.identity, animatedFramePlace);
            ShowFrame(frameInfo);

            showScale = CalculateShowScale(frameObj);
            frameObj.localScale = showScale;
            frameObj.anchoredPosition = Vector2.zero;

            movingFrame = frameObj;
            frameShowRoutine = MoveFrameRoutine(frameObj, frameInfo.position);
            StartCoroutine(frameShowRoutine);
            yield return new WaitUntil(() => movingFrame == null);
        }

        yield return null;
    }

    private Vector3 CalculateShowScale(RectTransform frameObj)
    {
        var xScale = showFrameSize.x / frameObj.sizeDelta.x;
        var yScale = showFrameSize.y / frameObj.sizeDelta.y;

        return Vector3.one * Mathf.Min(xScale, yScale);
    }

    private IEnumerator MoveFrameRoutine(RectTransform frameObject, Vector2 position)
    {
        frameTargetPosition = position;
        yield return new WaitForSeconds(frameShowTime);
        MoveFrame();
        yield return new WaitForSeconds(movingTime);
        movingFrame = null;
    }

    private void MoveFrame()
    {
        movingFrame.transform.localPosition = Vector2.zero;
        movingFrame.parent = frameStorage;
        currentMovingTime = 0;
        frameStartPosition = movingFrame.anchoredPosition;
    }

    private void ShowFrame(ComicsFrame showFrameInfo)
    {
        shakeForce = showFrameInfo.shakeForce;
        currentShowTime = 0;
        if (showFrameInfo.appearanceSound != null)
        {
            SoundManager.PlaySound(showFrameInfo.appearanceSound);
        }
        background.color = new Color(background.color.r, background.color.g, background.color.b, backgroundStartOpacity);
        var animationName = Enum.GetNames(typeof(FrameAnimation)).GetRandom(); 
        frameAnimator.Play(animationName, 0, 0);
    }

    private void Clear()
    {
        foreach (Transform frame in frameStorage)
        {
            Destroy(frame.gameObject);
        }

        foreach (Transform frame in animatedFramePlace)
        {
            Destroy(frame.gameObject);
        }
    }

    public ScriptableComicsChapter GetChapter()
    {
        var comicsName = ComicsMapper.GetComicsToShow();
        return chapters.FirstOrDefault(x => x.name == comicsName);
    }
}
