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
        
        Select = GameObject.Find("PlayerSelect");  // 로비씬에서 건너온 플레이어 성별 참조
        num = Select.GetComponent<CharacterSelect>().CharacterNumber;

    }

    void Update()
    {
        StartCoroutine(Player());
    }

    IEnumerator Player()
    {
        if (num == 1) // 여자일때
        {
            girl.gameObject.SetActive(true);
            boy.gameObject.SetActive(false);

            playerCamera.gameObject.SetActive(true);
        }
        else if (num == 2) // 남자일때
        { 

            boy.gameObject.SetActive(true);
            girl.gameObject.SetActive(false);

            playerCamera.gameObject.SetActive(true);
        }

        yield return null;
    }
}
