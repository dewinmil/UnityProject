using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EndTurn : NetworkBehaviour
{

    [SyncVar]
    public int turn;
    public AudioManager _manager;
    //public Camera camera;

    public void endTurn()
    {
        if (turn == 1)
        {
            if (!isServer)
            {
                _manager.endTurn();
                turn = 2;
            }
        }
        else
        {
            if (isServer)
            {
                _manager.endTurn();
                turn = 1;

            }
        }
    }
}