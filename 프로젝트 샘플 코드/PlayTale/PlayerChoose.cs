using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChoose : MonoBehaviour
{
    int num;
    GameObject Select;
    public GameObject girl;
    public GameObject boy;
    public GameObject playerCamera;

    void Start()
    {
        // 로비씬에서 건너온 플레이어 성별 참조
        Select = GameObject.Find("PlayerSelect");
        num = Select.GetComponent<CharacterSelect>().CharacterNumber;
    }

    void Update()
    {
        StartCoroutine(Player());
    }

    IEnumerator Player()
    {
        if (num == 1)    // 여자 플레이어
        {
            girl.gameObject.SetActive(true);
            boy.gameObject.SetActive(false);
        }
        else    // 남자 플레이어
        { 
            boy.gameObject.SetActive(true);
            girl.gameObject.SetActive(false);
        }

        playerCamera.gameObject.SetActive(true);
        yield return null;
    }
}
