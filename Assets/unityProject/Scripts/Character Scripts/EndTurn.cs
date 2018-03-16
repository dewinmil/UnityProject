using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Networking;

public class EndTurn : NetworkBehaviour
{
    public Unit unit;
    public CharacterStatus unitStatus;
    public MoveInput unitMoveInput;
    public Abilities currUnit;
    //public int currTeam;
    public List<Abilities> turnUsed = new List<Abilities>();
    public List<Abilities> unitList = new List<Abilities>();

    // Use this for initialization
    void Start()
    {
        this.currUnit._unit.unitId = 0;
    //    currTeam = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void switchTeam(int currTeam)
    {
        foreach (Abilities unit in unitList)
        {
            if (unit._casterStatus.teamNum == currTeam)
            {
                unit._casterStatus.gainAction();
            }
        }
        // switch control to other player.
        currTeam = (currTeam + 1) % 2;
    }

    public void switchUnit(int currTeam)
    {
        // switch control to different unit on the same team.
        turnUsed.Add(currUnit);

        foreach (Abilities unit in unitList)
        {
            if (unit._casterStatus.teamNum == currTeam && !turnUsed.Contains(unit))
            {
                currUnit = unit;
            }
        }
    }
}
