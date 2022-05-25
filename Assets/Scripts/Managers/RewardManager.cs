using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RewardManager : BaseManager
{
    public static int rewardSum = 250;
    public Text currentMoneyText;
    public Text rewardText;
    public ParticleSystem moneyParticles;

    private const int TickCount = 5;
    private int tickMoneySum;

    private int reward;

    private void Awake()
    {
        tickMoneySum = rewardSum / TickCount;
    }

    public void StartRewardCalculation()
    {
        var currentLevel = SceneManager.GetActiveScene();
        var rewardLevel = LevelMapper.GetStatus(currentLevel.name) == LevelStatus.Avaliable;
        GetComponent<Animator>().SetBool("reward", rewardLevel);
        GetComponent<Canvas>().worldCamera = Camera.main;
        GetComponent<Canvas>().sortingLayerName = "SelectionMenu";
        rewardText.text = string.Empty;
        currentMoneyText.text = MoneyMapper.Get().ToString() + "$";
        reward = 0;
    }

    public void FinishRewardCalculation()
    {
        MoneyMapper.Add(reward);
        currentMoneyText.text = MoneyMapper.Get().ToString() + "$";
    }

    public void CollectBank()
    {
        reward += tickMoneySum;
        rewardText.text = reward.ToString() + "$";
        moneyParticles.Play();
    }
}
