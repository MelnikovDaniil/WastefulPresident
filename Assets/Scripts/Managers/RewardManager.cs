using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RewardManager : BaseManager
{
    public Text currentMoneyText;
    public Text rewardText;
    public ParticleSystem moneyParticles;
    public int tickMoneySum = 50;

    private int reward;

    public void StartRewardCalculation()
    {
        var currentLevel = SceneManager.GetActiveScene();
        GetComponent<Animator>().SetBool("reward", LevelMapper.GetCurrentLevel() == currentLevel.name);
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
