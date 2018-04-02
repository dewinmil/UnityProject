using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CharacterStatus : NetworkBehaviour {
    [SyncVar]
    public int teamNum;
    [SyncVar]
    public float maxAction;
    [SyncVar]
    public float currentAction;
    [SyncVar]
    public float maxHealth;
    [SyncVar]
    public float currentHealth;
    [SyncVar]
    public float physicalArmor;//a value of 1 is 100% resistance
    [SyncVar]
    public float magicArmor;//a value of 1 is 100% resistance
    public Image healthBar;
    public Image actionBar;
    public Text healthBarText;
    public Text actionBarText;
    public Image healthBarUI;
    public Image actionBarUI;
    public Text healthBarTextUI;
    public Text actionBarTextUI;
    public Unit _unit;
    public EndTurn endTurn;
    private int previousTurn;
    [SyncVar]
    public bool startOfTurn;
    public bool isLeader;
    public GameObject winScreen;
    public GameObject loseScreen;


    // Use this for initialization
    void Start()
    {
        previousTurn = 1;
        endTurn = FindObjectOfType<EndTurn>();
        startOfTurn = false;
        winScreen = GameObject.FindWithTag("winScreen");
        loseScreen = GameObject.FindWithTag("loseScreen");
    }

    // Update is called once per frame
    void Update()
    {
        if(endTurn.turn != previousTurn)
        {
            FindObjectOfType<AudioManager>().endTurn();
            previousTurn = endTurn.turn;
            if (hasAuthority)
            {
                CmdUpdateTurn(endTurn.turn);
            }
        }
        if (endTurn.turn == teamNum)
        {
            if(startOfTurn == true)
            {
                startOfTurn = false;
                if (currentHealth > 0)
                {
                    currentAction = currentAction + 5;
                    if(currentAction > maxAction)
                    {
                        currentAction = maxAction;
                    }
                }
            }
        }
        else
        {
            startOfTurn = true;
        }
        updateStatusBars();
        if(currentHealth <= 0)
        {
            _unit.DeathAnim();
        }
    }

    public void updateStatusBars()
    {
        //update the size of the healthbars on screen depending upon how "full" they are
        float ratio = currentHealth / maxHealth;
        healthBar.rectTransform.localScale  = new Vector3(ratio, 1, 1);
        healthBarText.text = currentHealth.ToString() + " / " + maxHealth.ToString();

        ratio = currentAction / maxAction;
        actionBar.rectTransform.localScale = new Vector3(ratio, 1, 1);
        actionBarText.text = currentAction.ToString() + " / " + maxAction.ToString();

        ratio = currentHealth / maxHealth;
        healthBarUI.rectTransform.localScale = new Vector3(ratio, 1, 1);
        healthBarTextUI.text = currentHealth.ToString() + " / " + maxHealth.ToString();

        ratio = currentAction / maxAction;
        actionBarUI.rectTransform.localScale = new Vector3(ratio, 1, 1);
        actionBarTextUI.text = currentAction.ToString() + " / " + maxAction.ToString();
    }
    
    public void loseHealth(float damage)
    {
        currentHealth -= damage;
        _unit.react = true;
    }
    public void gainHealth(float healing)
    {
        currentHealth += healing;
    }
    public void loseAction(float apCost)
    {
        currentAction -= apCost;
    }
    public void gainAction()
    {
        // At the beginning of the turn
        currentAction += 8;
    }
    
    [Command]
    public void CmdSyncValues(int teamNumVal, float maxActionVal, float currentActionVal,
        float maxHealthVal, float currentHealthVal, float physicalArmorVal, float magicArmorVal)
    {
        teamNum = teamNumVal;
        maxAction = maxActionVal;
        currentAction = currentActionVal;
        maxHealth = maxHealthVal;
        currentHealth = currentHealthVal;
        physicalArmor = physicalArmorVal;
        magicArmor = magicArmorVal;
    }

    [Command]
    public void CmdUpdateTurn(int turn)
    {
        endTurn.turn = turn;
        previousTurn = turn;
    }

    [Command]
    public void CmdEndGame(int teamNumber)
    {
        if (teamNumber == 2)
        {
            loseScreen.GetComponent<Canvas>().enabled = true;
            RpcWinGame();
        }
        else
        {
            winScreen.GetComponent<Canvas>().enabled = true;
            RpcLoseGame();

        }
    }

    [ClientRpc]
    public void RpcWinGame()
    {
        if (!isServer)
        {
            winScreen.GetComponent<Canvas>().enabled = true;
        }
    }

    [ClientRpc]
    public void RpcLoseGame()
    {
        if (!isServer)
        {
            loseScreen.GetComponent<Canvas>().enabled = true;
        }
    }

}
