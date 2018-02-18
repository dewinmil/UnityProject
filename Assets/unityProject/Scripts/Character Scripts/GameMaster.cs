using UnityEngine;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour
{
    public List<Abilities> unitList = new List<Abilities>();
    public int activeTeam;
    public Abilities acitveUnit;

    // Use this for initialization
    void Start()
    {
        activeTeam = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeTeam()
    {
        activeTeam = (activeTeam + 1) % 2;
    }

    public void 
}
