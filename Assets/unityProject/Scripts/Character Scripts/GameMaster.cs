using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class GameMaster : NetworkManager
{
    //public List<Abilities> unitList = new List<Abilities>();
    //public int activeTeam;
    //public Abilities abil;
    public GameObject Unit1;
    public GameObject Unit2;
    public GameObject Unit3;
    public GameObject Unit4;
    public short _playerID;
    public TileMap _map;
    private const int NUM_UNITS_PER_TEAM = 5;
    public List<Unit> _units;
    private int _prevX;

    // Use this for initialization
    void Start()
    {
        _playerID = 0;
    }
    /*
    // Update is called once per frame
    void Update()
    {

    }
    
    public void ChangeTeam()
    {
        activeTeam = (activeTeam + 1) % 2;
    }
    */
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        _playerID = 0;
        _units = new List<Unit>();
        _prevX = 0;

    }
    /*
    // Update is called once per frame
    void Update()
    {
        //NetworkMessage test = new NetworkMessage();
        //test.chosenClass = chosenCharacter;
        ClientScene.AddPlayer(conn, playerID);
        playerID++;
        ClientScene.AddPlayer(conn, playerID);
        playerID++;
        ClientScene.AddPlayer(conn, playerID);
        playerID++;
        ClientScene.AddPlayer(conn, playerID);
        playerID++;
        ClientScene.AddPlayer(conn, playerID);
        playerID++;
    }
*/
    public void SwitchTeam()
    {
        
    }

}
/*
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
 
public class NetworkCustom : NetworkManager
{

    public int chosenCharacter = 0;
    public GameObject[] characters;

    //subclass for sending network messages
    public class NetworkMessage : MessageBase
    {
        public int chosenClass;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        NetworkMessage message = extraMessageReader.ReadMessage<NetworkMessage>();
        int selectedClass = message.chosenClass;
        Debug.Log("server add with message " + selectedClass);

        GameObject player;
        Transform startPos = GetStartPosition();

        if (startPos != null)
        {
            player = Instantiate(characters[chosenCharacter], startPos.position, startPos.rotation) as GameObject;
        }
        else
        {
            player = Instantiate(characters[chosenCharacter], Vector3.zero, Quaternion.identity) as GameObject;

        }

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        NetworkMessage test = new NetworkMessage();
        test.chosenClass = chosenCharacter;

        ClientScene.AddPlayer(conn, 0, test);
    }


    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        //base.OnClientSceneChanged(conn);
    }

    public void btn1()
    {
        chosenCharacter = 0;
    }

    public void btn2()
    {
        chosenCharacter = 1;
    }
}
*/