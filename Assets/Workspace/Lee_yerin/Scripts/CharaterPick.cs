using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaterPick : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] Animal animal;

    public void AddPlayerPick()
    {
        gameManager.RPCAddPlayerPick(gameManager.Runner.LocalPlayer, (int)animal);
    }
}