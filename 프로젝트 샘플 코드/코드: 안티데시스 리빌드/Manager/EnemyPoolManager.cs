using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance;
    public bool EnemyCanBeDamaged = false;

    public GameObject[] Prefabs;
    public List<GameObject>[] Pool;


    void OnEnable()
    {

        Instance = this;
        Pool = new List<GameObject>[Prefabs.Length];
        
        int count = BossPhaseManager.Instance.currentPhase switch
        {
            1 => 3, 
            2 => 5, 
            3 => 7 
        };

        for (int index = 0; index < Pool.Length; index++)
        {
            Pool[index] = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(Prefabs[index], transform);
                obj.SetActive(false);
                Pool[index].Add(obj);
            }
        }
    }

    public GameObject Get(int index)
    {
        GameObject select = null;

        foreach (GameObject item in Pool[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                //Debug.Log($"[Pool] Ȱ��ȭ �õ�: {item.name}, ����: {item.activeSelf}");
                return select; 
            }
        }
        
        select = Instantiate(Prefabs[index], transform);
        Pool[index].Add(select);
        select.SetActive(true);     // �� �ν��Ͻ� ������ ���̰�
        //Debug.Log($"[Pool] �� �ν��Ͻ� ����: {select.name}, ����: {select.activeSelf}");
        return select;
    }
}
