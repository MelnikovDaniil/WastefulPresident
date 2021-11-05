//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class ShakingManager : MonoBehaviour
//{
//    public static ShakingManager Instance;
//    public float maxShakingTime = 5;
//    public int maxTabs = 30;
//    public float maxSplashForce;
//    public float maxAnimationSpeed = 2;
//    public float minVelocity = 0.3f;
//    public Transform handTransform;
//    public ParticleSystem splashParticles;
//    public Vector3 splashCameraShift;

//    [Space(20)]
//    public float bonusStartPosition = 7.5f;
//    public float bonusGap = 11.5f;
//    public TextMeshPro bonusText;
//    public List<int> rewardList;

//    [Space(20)]
//    public Image timerImage;
//    public Image timerArrowImage;
//    public TextMeshProUGUI tabCounter;
//    public Transform tabsParticles;

//    private Animator tabAnimator;
//    private float shakingTimeLeft;
//    private Animator _characterAnimator;
//    private bool shaking;
//    private float boostStep;
//    private int tabs;
//    private ParticleSystem particle;
//    private Rigidbody2D particleRigidbody;
//    private float nextBonusPosition;
//    private Queue<int> rewardQueue;

//    private void Awake()
//    {
//        Instance = this;
//        _characterAnimator = GetComponent<Animator>();
//        boostStep = maxAnimationSpeed / maxTabs;
//        rewardQueue = new Queue<int>(rewardList);
//    }

//    private void Update()
//    {
//        if (!GameManager.isPaused)
//        {
//            if (shaking)
//            {
//                if (Input.GetKeyDown(KeyCode.Mouse0))
//                {
//                    if (tabs == 0)
//                    {
//                        Time.timeScale = 1;
//                        SoundManager.PlaySound("shaking");
//                        tabCounter.gameObject.SetActive(true);
//                        StartCoroutine(FinishShaking());
//                    }
//                    tabs += 1;
//                    var tabsCoof = (float)tabs / maxTabs;
//                    tabCounter.transform.parent.localScale = Vector3.one * (tabsCoof * 0.5f + 1f);
//                    tabsParticles.localScale = Vector3.one * 120 * tabsCoof;
//                    tabCounter.text = tabs.ToString();
//                    tabAnimator.Play("Pulse");
//                    var speed = _characterAnimator.GetFloat("shakingSpeed") + boostStep;
//                    _characterAnimator.SetFloat("shakingSpeed", speed);

//                    if (tabs >= maxTabs)
//                    {
//                        Splash();
//                    }
//                }
//                if (tabs > 0)
//                {
//                    shakingTimeLeft -= Time.deltaTime;
//                    timerImage.fillAmount = shakingTimeLeft / maxShakingTime;
//                    timerArrowImage.transform.rotation = Quaternion.Euler(0, 0, 360 * shakingTimeLeft / maxShakingTime);
//                }
//            }

//            if (particle != null)
//            {
//                if (particle.transform.position.y > nextBonusPosition)
//                {
//                    SoundManager.PlaySound("lengthMark");
//                    nextBonusPosition += bonusGap;
//                    var text = Instantiate(bonusText, particle.transform.position + new Vector3(2, 0), Quaternion.identity);
//                    var beer = rewardQueue.Dequeue();
//                    GameManager.beer += beer;
//                    text.text = $"+{beer}";
//                }

//                var main = particle.main;
//                main.startLifetimeMultiplier = Mathf.Clamp((particle.transform.position.y - handTransform.position.y) / 5f, 0, 4f);

//                if (particleRigidbody.velocity.y < minVelocity)
//                {
//                    particleRigidbody.velocity = Vector2.zero;
//                    particle = null;
//                    Time.timeScale = 0;
//                    GameManager.minigameBeer = GameManager.beer - GameManager.colectedBeer;
//                    SoundManager.PlaySoundUI("cameraShutter");
//                    SoundManager.PlaySoundWithDelay("calculating", 0.5f, false);
//                    LevelManager.FinishLevel();
//                    GameManager.Instance.StopPoits();
//                    UIManager.FinishMenu();
//                }

//            }
//        }
//    }

//    public void StartShaking()
//    {
//        UIManager.StartShaking();
//        shaking = true;
//        tabs = 0;
//        shakingTimeLeft = maxShakingTime;
//        Time.timeScale = 0;
//        tabCounter.gameObject.SetActive(false);
//        tabAnimator = tabCounter.GetComponent<Animator>();
//    }

//    private IEnumerator FinishShaking()
//    {
//        yield return new WaitForSeconds(maxShakingTime);
//        Splash();
//    }

//    private void Splash()
//    {
//        if (shaking)
//        {
//            timerImage.fillAmount = 0;
//            timerArrowImage.transform.rotation = Quaternion.identity;

//            SoundManager.PlaySound("hissOpening");
//            SoundManager.PlaySound("longHiss");
//            UIManager.FinishShaking();
//            nextBonusPosition = bonusStartPosition;
//            _characterAnimator.Play("SodaSplash");
//            shaking = false;
//            StartCoroutine(ThrowParticles());
//        }
//    }

//    private IEnumerator ThrowParticles()
//    {
//        yield return new WaitForSeconds(0.25f);
//        particle = Instantiate(splashParticles, handTransform.transform.position, Quaternion.identity);
//        particleRigidbody = particle.GetComponent<Rigidbody2D>();
//        var splashVector = Vector2.up * tabs / maxTabs * maxSplashForce;
//        particleRigidbody.AddForce(splashVector, ForceMode2D.Impulse);
//        CameraManager.Instance.SetTarget(particle.gameObject, -1, splashCameraShift);
//    }
//}
