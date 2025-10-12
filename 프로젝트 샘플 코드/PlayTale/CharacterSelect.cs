using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    private int CharacterNumber;
    public GameObject girl;
    public GameObject boy;


    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        string sceneName = currentScene.name;
        if (sceneName == "Lobby")  // 로비창일때만 플레이어 선택
            {
                StartCoroutine(Select());
            }

    }

    
IEnumerator Select()
    {
        if (girl.gameObject.activeSelf)
        {
            CharacterNumber = 1; // 여자
        }
        if (boy.gameObject.activeSelf)
        {
            CharacterNumber = 2; // 남자
        }
        yield return null;
    }
}
