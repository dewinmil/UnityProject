using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EndTurn : NetworkBehaviour
{

    [SyncVar]
    public int turn;

    public void endTurn()
    {
        if (turn == 1)
        {
            if (!isServer)
            {
                turn = 2;
            }
        }
        else
        {
            if (isServer)
            {
                turn = 1;
            }
        }
    }
}