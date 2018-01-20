using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatus : MonoBehaviour {
    public int teamNum;
    public float maxAction;
    public float currentAction;
    public float maxHealth;
    public float currentHealth;
    public Image healthBar;
    public Image actionBar;
    public Text healthBarText;
    public Text actionBarText;


    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateStatusBars();
    }

    public void updateStatusBars()
    {
        float ratio = currentHealth / maxHealth;
        healthBar.rectTransform.localScale  = new Vector3(ratio, 1, 1);
        healthBarText.text = currentHealth.ToString() + " / " + maxHealth.ToString();

        ratio = currentAction / maxAction;
        actionBar.rectTransform.localScale = new Vector3(ratio, 1, 1);
        actionBarText.text = currentAction.ToString() + " / " + maxAction.ToString();
    }
    
    public void loseHealth()
    {

    }
    public void gainHealth()
    {

    }
    public void loseAction()
    {

    }
    public void gainAction()
    {

    }
    
}
