﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ToolTipScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setToolTip(int abilityNum)
    {
        //background width 225
        //tooltip width 210
        Text toolTip = gameObject.GetComponent<Text>();
        if(abilityNum == 1)
        {
            toolTip.text = "";
        }
        if (abilityNum == 2)
        {
            toolTip.text = "";
        }
        if (abilityNum == 3)
        {
            toolTip.text = "";
        }
        if (abilityNum == 4)
        {
            toolTip.text = "";
        }
        if (abilityNum == 5)
        {
            toolTip.text = "";
        }
        if (abilityNum == 6)
        {
            toolTip.text = "";
        }
        if (abilityNum == 7)
        {
            toolTip.text = "";
        }
        if (abilityNum == 8)
        {
            toolTip.text = "";
        }
        if (abilityNum == 9)
        {
            toolTip.text = string.Format("Description: A melee attack without much penetration" +
                "{0}Damage Type-Physical    Range-1   Cost-3     Damage-8   Penetration-0%", Environment.NewLine);
        }
        if (abilityNum == 10)
        {
            toolTip.text = "";
        }
        if (abilityNum == 11)
        {
            toolTip.text = "";
        }
        if (abilityNum == 12)
        {
            toolTip.text = string.Format("Description: Basic attack with spear" +
                "{0}Damage Type-Physical    Range-2   Cost-3     Damage-5   Penetration-0%", Environment.NewLine);
        }
        if (abilityNum == 13)
        {
            toolTip.text = string.Format("Description: Spear attack with some penetration" +
                "{0}Damage Type-Physical    Range-2   Cost-5     Damage-5   Penetration-20%", Environment.NewLine);
        }
        if (abilityNum == 14)
        {
            toolTip.text = string.Format("Description: High cost spear attack with medium penetration" +
                "{0}Damage Type-Physical    Range-2   Cost-7     Damage-5   Penetration-40%", Environment.NewLine);
        }
        if (abilityNum == 15)
        {
            toolTip.text = string.Format("Description: Basic melee attack" +
                "{0}Damage Type-Physical    Range-1   Cost-3     Damage-4   Penetration-0%", Environment.NewLine);
        }
        if (abilityNum == 16)
        {
            toolTip.text = string.Format("Description: Melee attack with minimal penetration" +
                "{0}Damage Type-Physical    Range-1   Cost-5     Damage-4   Penetration-10%", Environment.NewLine);
        }
        if (abilityNum == 17)
        {
            toolTip.text = string.Format("Description: Melee attack with higher cost and medium penetration" +
                "{0}Damage Type-Physical    Range-1   Cost-6     Damage-4   Penetration-30%", Environment.NewLine);
        }
        if (abilityNum == 18)
        {
            toolTip.text = "";
        }
        if (abilityNum == 19)
        {
            toolTip.text = "";
        }
        if (abilityNum == 20)
        {
            toolTip.text = string.Format("Description: Penetration buff" +
                "{0}Damage Type-Physical    Range-1   Cost-6     Damage-4   Penetration-30%", Environment.NewLine);
        }
        if (abilityNum == 21)
        {
            toolTip.text = string.Format("Description: Gain more movement" +
                "{0}Damage Type-Physical    Range-1   Cost-6     Damage-4   Penetration-30%", Environment.NewLine);
        }
        if (abilityNum == 22)
        {
            toolTip.text = string.Format("Description: Gain action points from remaining moves" +
                "{0}Damage Type-Physical    Range-1   Cost-6     Damage-4   Penetration-30%", Environment.NewLine);
        }
        if (abilityNum == 23)
        {
            toolTip.text = string.Format("Description: Spend health to gain movements" +
                "{0}Damage Type-Physical    Range-1   Cost-6     Damage-4   Penetration-30%", Environment.NewLine);
        }
    }
}
