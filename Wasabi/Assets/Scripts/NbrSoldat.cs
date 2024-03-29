﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NbrSoldat : MonoBehaviour {

    Text Text;
    int nbr = 1;
    public int Nbr
    {
        private set { nbr = value; }
        get { return nbr; }
    }
    string soldat = " soldat";
    private void Start()
    {
        Text = GameObject.FindGameObjectWithTag("text").GetComponent<Text>();
        PlayerPrefs.SetInt("nbSoldiers", nbr);
        Text.text = nbr.ToString() + soldat;
    }
     public void OnclickPlus ()
    {
        nbr++;
        if (nbr > 1)
        {
            soldat = " soldats";
        }
        if (nbr > 5)
        {
            nbr = 5;
        }
        PlayerPrefs.SetInt("nbSoldiers", nbr); 
        Text.text = nbr.ToString() + soldat;
	}

    public void OnclickMoins()
    {
        nbr--;
        if (nbr > 1)
        {
            soldat = " soldats";
        }
        else if (nbr < 1)
        {
            nbr = 1;
        }
        else
        {
            soldat = " soldat";
        }
        PlayerPrefs.SetInt("nbSoldiers", nbr);
        Text.text = nbr.ToString() + soldat;
    }
}
