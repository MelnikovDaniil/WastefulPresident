using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    private float currentMovingTime;
    private RectTransform movingFrame;

    private Vector2 frameStartPosition;
    private Vector2 frameTargetPosition;

    private Vector2 showFrameSize = new Vector2(780, 1200);
    private Vector2 showScale;

    private float backgroundStartOpacity;

    private void Start()
    {
        backgroundStartOpacity = background.color.a;
        ComicsMapper.SetComicsToShow(chapters.FirstOrDefault().name);
        currentChapter = GetChapter();
        currentPage = currentChapter.comicsPages.FirstOrDefault();
        StartCoroutine(ShowPageRoutine(currentPage));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NextPage();
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
            SceneManager.LoadScene("LevelMenu");
        }
    }

    private IEnumerator ShowPageRoutine(ComicsPage page)
    {
        Clear();

        foreach (var frameInfo in page.frames)
        {
            var frameObj = Instantiate(frameInfo.frame, Vector2.zero, Quaternion.identity, animatedFramePlace);
            ShowFrame();
            if (frameInfo.appearanceSound != null)
            {
                SoundManager.PlaySound(frameInfo.appearanceSound);
            }

            showScale = CalculateShowScale(frameObj);
            frameObj.localScale = showScale;
            frameObj.anchoredPosition = Vector2.zero;

            //var frameShowingTime = frameAnimator.GetCurrentAnimatorStateInfo(0).length + frameAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            yield return new WaitForSeconds(frameShowTime);
            MoveFrame(frameObj, frameInfo.position);
            yield return new WaitForSeconds(movingTime);
        }
    }

    private Vector3 CalculateShowScale(RectTransform frameObj)
    {
        var xScale = showFrameSize.x / frameObj.sizeDelta.x;
        var yScale = showFrameSize.y / frameObj.sizeDelta.y;

        return Vector3.one * Mathf.Min(xScale, yScale);
    }

    private void MoveFrame(RectTransform frameObject, Vector2 position)
    {
        movingFrame = frameObject;
        movingFrame.parent = frameStorage;
        currentMovingTime = 0;
        frameStartPosition = movingFrame.anchoredPosition;
        frameTargetPosition = position;
    }

    private void ShowFrame()
    {
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
