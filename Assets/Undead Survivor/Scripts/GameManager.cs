using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // static으로 선언하면 인스펙터 창에 뜨지 않음.
    [Header("# Game Control")] // 인스펙터 창 정리
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 2 * 10f; // 테스트 20초

    [Header("# Player Info")]
    public int playerId;
    public float health;
    public float maxHealth = 100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 5, 10, 30, 100, 150, 200, 260, 320, 400, 500};

    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;
    public LevelUp uiLevelUp;
    public Result uiResult;
    public Transform uiJoystick;
    public GameObject enemyCleaner;

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60; // 60프레임
    }

    public void GameStart(int id)
    {
        playerId = id;
        health = maxHealth;

        player.gameObject.SetActive(true);
        uiLevelUp.Select(playerId % 2); // 임시 스크립트 (첫번째 캐릭터 선택)
        Resume();

        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }

    public void GameWin()
    {
        StartCoroutine(GameWinRoutine());
    }

    IEnumerator GameWinRoutine()
    {
        isLive = false;
        enemyCleaner.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        Stop();

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
    }

    public void GameRestart()
    {
        SceneManager.LoadScene(0); // 씬 이름이나 인덱스로 불러올 수 있다.
    }

    public void GameQuit()
    {
        Application.Quit();
    }


    void Update()
    {
        if (!isLive)
            return;

        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameWin();
        }
    }

    public void GetExp()
    {
        if (!isLive)
            return;

        exp++;

        if(exp == nextExp[Mathf.Min(level, nextExp.Length-1)])
        {
            level++;
            exp = 0;
            uiLevelUp.Show();
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;  // 기존값 1
        uiJoystick.localScale = Vector3.zero;
    }

    public void Resume()
    {
        isLive= true;
        Time.timeScale = 1; // 모바일 게임 전투배속 올리는 기능으로 사용.
        uiJoystick.localScale = Vector3.one;
    }
}
