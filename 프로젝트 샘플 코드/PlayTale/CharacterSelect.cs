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
        if (sceneName == "Lobby")
        {
            StartCoroutine(Select());
        }
    }

    IEnumerator Select()
    {
        if (girl.gameObject.activeSelf)
        {
            CharacterNumber = 1; 
        }
        if (boy.gameObject.activeSelf)
        {
            CharacterNumber = 2;
        }
        
        yield return null;
    }
}
