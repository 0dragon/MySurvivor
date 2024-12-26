using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // 프리펩들을 보관할 변수
    public GameObject[] prefabs;

    // 풀 담당을 하는 리스트들
    List<GameObject>[] pools;

    void Awake() // 최초 단 1회 실행
    {
        pools = new List<GameObject>[prefabs.Length];

        for(int index = 0; index < pools.Length;  index++)
        {
            pools[index] = new List<GameObject>();
        }

        // Debug.Log(pools.Length);
    }
    public GameObject Get(int index)
    {
        GameObject select = null;

        // 1. 선택한 풀의 놀고 있는 게임오브젝트 접근
        // 2. 발견하면 select 변수에 할당
        // 3. 못찾았으면 새롭게 생성하고 select 변수에 할당

        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf)
            {
                // 2.
                select = item; ;
                select.SetActive(true);
                break;
            }
        }
        // 3.
        if(!select) // select == null과 같다
        {
            select = Instantiate(prefabs[index], transform);
            pools[index].Add(select);
        }
        return select;
    }
}
