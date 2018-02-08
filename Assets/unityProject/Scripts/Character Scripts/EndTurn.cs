using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class EndTurn : MonoBehaviour
{
    public Unit unit;
    public CharacterStatus unitStatus;
    public MoveInput unitMoveInput;
    public Abilities currUnit;
    public List<Node> turnUsed = new List<Node>();

    // Use this for initialization
    void Start()
    {
        this.currUnit._unit.unitId = 0;
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
