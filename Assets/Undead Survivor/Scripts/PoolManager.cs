using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // ��������� ������ ����
    public GameObject[] prefabs;

    // Ǯ ����� �ϴ� ����Ʈ��
    List<GameObject>[] pools;

    void Awake() // ���� �� 1ȸ ����
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

        // 1. ������ Ǯ�� ��� �ִ� ���ӿ�����Ʈ ����
        // 2. �߰��ϸ� select ������ �Ҵ�
        // 3. ��ã������ ���Ӱ� �����ϰ� select ������ �Ҵ�

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
        if(!select) // select == null�� ����
        {
            select = Instantiate(prefabs[index], transform);
            pools[index].Add(select);
        }
        return select;
    }
}
