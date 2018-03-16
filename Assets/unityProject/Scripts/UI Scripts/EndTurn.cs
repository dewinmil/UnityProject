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
        CmdEndTurn();
    }

    [Command]
    public void CmdEndTurn()
    {
        if (turn == 1)
        {
            turn = 2;
        }
        else
        {
            turn = 1;
        }
        print(turn);
    }
}