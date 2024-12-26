using System;
using System.Collections;
using UnityEngine;

public class AchieveManager : MonoBehaviour
{
    public GameObject[] lockCharacter;
    public GameObject[] unlockCharacter;
    public GameObject uiNotification;

    enum Achieve { UnlockPotato, UnlockBean}
    Achieve[] achieves;
    WaitForSecondsRealtime wait;
    
    void Awake()
    {
        achieves = (Achieve[])Enum.GetValues(typeof(Achieve));
        wait = new WaitForSecondsRealtime(5);

        if (!PlayerPrefs.HasKey("MyData")){
            Init();
        }
    }

    void Init()
    {
        PlayerPrefs.SetInt("MyData", 1);

        foreach(Achieve achieve in achieves)
        {
            PlayerPrefs.SetInt(achieve.ToString(), 0);
        }
        // Edit -> Clear All PlayerPrefs로 데이터를
        // 지우고 테스트

        // foreach문으로 대체
        // PlayerPrefs.SetInt("UnlockPotato", 0);
        // PlayerPrefs.SetInt("UnlockBean", 0);
    }

    void Start()
    {
        UnlockCharacter();
    }

    void UnlockCharacter()
    {
        for(int i=0;i<lockCharacter.Length; i++)
        {
            string achieveName = achieves[i].ToString();
            bool isUnlock = PlayerPrefs.GetInt(achieveName) == 1;
            lockCharacter[i].SetActive(!isUnlock);
            unlockCharacter[i].SetActive(isUnlock);
        }
    }

    void LateUpdate()
    {
        foreach(Achieve achieve in achieves)
        {
            CheckAchieve(achieve);
        }
    }

    void CheckAchieve(Achieve achieve)
    {
        bool isAchieve = false;

        switch (achieve)
        {
            case Achieve.UnlockPotato:
                isAchieve = GameManager.Instance.kill >= 10;
                break;
            case Achieve.UnlockBean:
                isAchieve = GameManager.Instance.gameTime == GameManager.Instance.maxGameTime;
                break;
        }

        if (isAchieve && PlayerPrefs.GetInt(achieve.ToString()) == 0)
        {
            PlayerPrefs.SetInt(achieve.ToString(), 1);

            for (int i = 0; i < uiNotification.transform.childCount; i++)
            {
                bool isActive = i == (int)achieve;
                uiNotification.transform.GetChild(i).gameObject.SetActive(isActive);
            }

            StartCoroutine(NotificationRoutine());
        }
    }

    IEnumerator NotificationRoutine()
    {
        uiNotification.SetActive(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);

        yield return wait;

        uiNotification.SetActive(false);
    }
}
