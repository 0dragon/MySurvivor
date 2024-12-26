using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Player.cs의 OnEnable에서 사용
    public static float Speed
    {
        get { return GameManager.Instance.playerId == 0 ? 1.1f : 1f; } // 벼농부일 때 이동속도 1.1f  나머지 캐릭터는 1f
    }

    public static float WeaponSpeed // 근거리무기 공속
    {
        get { return GameManager.Instance.playerId == 1 ? 1.1f : 1f; }
    }

    public static float WeaponRate // 원거리무기 공속
    {
        get { return GameManager.Instance.playerId == 1 ? 0.9f : 1f; }
    }

    public static float Damage
    {
        get { return GameManager.Instance.playerId == 2 ? 1.2f : 1f; }
    }

    public static int Count
    {
        get { return GameManager.Instance.playerId == 3 ? 1 : 0; }
    }
}