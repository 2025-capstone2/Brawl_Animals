using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameStart : MonoBehaviour
{
    public NetworkRunner runner;
    public InputHandler inputHandler;

    void Start()
    {
        runner.ProvideInput = true;
        runner.AddCallbacks(inputHandler);
    }

}
