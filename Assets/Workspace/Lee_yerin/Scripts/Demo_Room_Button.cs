using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Demo_Room_Button : MonoBehaviour
{
    [SerializeField] bool isHost;

    private void Awake()
    {
        if (!isHost)
            gameObject.SetActive(false);
    }

    public void AfterPushButton()
    {
        gameObject.SetActive(false);
    }
}
