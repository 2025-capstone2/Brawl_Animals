using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test_Start : MonoBehaviour
{
    [SerializeField] string roomSceneName;
    public void GoToGameScene()
    {
        SceneManager.LoadScene(roomSceneName);
    }
}