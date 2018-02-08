using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndTurn : MonoBehaviour
{

    public Abilities currUnit;
    public List<Node> turnUsed = new List<Node>();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void switchTeam()
    {
        // switch control to other player.
    }

    public void switchUnit()
    {
        // switch control to different unit on the same team.
    }
}
