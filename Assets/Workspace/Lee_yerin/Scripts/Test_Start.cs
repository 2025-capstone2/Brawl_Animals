using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test_Start : MonoBehaviour
{
    public void GoToGameScene()
    {
        SceneManager.LoadScene("Room_Test");
    }
}