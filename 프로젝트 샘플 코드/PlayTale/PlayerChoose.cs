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
        
        Select = GameObject.Find("PlayerSelect");  // �κ������ �ǳʿ� �÷��̾� ���� ����
        num = Select.GetComponent<CharacterSelect>().CharacterNumber;

    }

    void Update()
    {
        StartCoroutine(Player());
    }

    IEnumerator Player()
    {
        if (num == 1) // �����϶�
        {
            girl.gameObject.SetActive(true);
            boy.gameObject.SetActive(false);

            playerCamera.gameObject.SetActive(true);
        }
        else if (num == 2) // �����϶�
        { 

            boy.gameObject.SetActive(true);
            girl.gameObject.SetActive(false);

            playerCamera.gameObject.SetActive(true);
        }

        yield return null;
    }
}
