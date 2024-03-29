﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Universe : MonoBehaviour
{
    private GameObject selected;
    [SerializeField] Text indic;
    [SerializeField] Text tour;
    [SerializeField] Text joueur;
    private bool gameStart = false;
    public bool fini = false;
    public bool GameStart { get { return gameStart; } set { gameStart = value; } }
    GameObject clone;
    private bool destroyed = false;
    private int turn = 1;
    private int NbTour = 1;
    GameObject selectedPlayer;

    List<int> lNumWormsJ1 = new List<int>();
    List<int> lNumWormsJ2 = new List<int>();
    int cursorJ1 = 1;
    int cursorJ2 = 1;
    private int playingTeam;
    public int PlayingTeam { get { return playingTeam; } set { playingTeam = value; } }

    private int wormsToPlay;
    public int WormsToPlay { get { return wormsToPlay; } set { wormsToPlay = value; } }
    int wrms;
    private int numWormsMax;
    public int NumWormsMax { get { return numWormsMax; } private set { numWormsMax = value; } }

    private float timeBeforeChange;
    public float TimeBeforeChange { get { return timeBeforeChange; } set { timeBeforeChange = value; } }

    bool AllSoldiersCreated = false;
    public float startTime;
    private float interval;
    private int[] characNum = new int[] { 0, 0 };
    [SerializeField] GameObject charact_ketchup;
    [SerializeField] GameObject charact_wasabi;
    Vector2 mouse;
    GameObject HUD;

    private void Start()
    {
        //Récupération du nombre de soldat et du volume de la musique de la scène précèdente
        NumWormsMax = PlayerPrefs.GetInt("nbSoldiers");
        GameObject.Find("MusicIG").GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("volume");
        TimeBeforeChange = PlayerPrefs.GetFloat("timer");
        HUD = GameObject.Find("HUD");
        indic.text = "Joueur " + turn + "\nCliquez pour poser votre soldat " + (characNum[turn - 1] + 1);
        tour.text = "Déploiement\n des unités";
        interval = Time.time;
    }
    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Main Menu");
        }
        if(!AllSoldiersCreated)
        {
            joueur.text = "Joueur " + turn;
            indic.text = "Veuillez patienter\n" + Math.Round( (interval - Time.time + 1), 2) + " s"; 
            CreateSoldiers();
        }
        else if (!gameStart && AllSoldiersCreated)
        {
            indic.text = "Veuillez patienter\n" + Math.Round((interval - Time.time + 2), 2) + " s";
            if (Time.time - interval >= 2)
            {
                //réactivation des textes
                HUD.GetComponentInChildren<HealthDisplay>().healthText.enabled = true;
                HUD.GetComponentInChildren<HealthDisplay>().infoEnnemy.enabled = true;
                indic.enabled = false;

                PlayingTeam = 1;
                WormsToPlay = new System.Random().Next(lNumWormsJ1.Count);
                wrms = lNumWormsJ1[WormsToPlay];
                gameStart = true;
                //Démarrage du timer du joueur
                startTime = Time.time;
                tour.text = "Début du tour " + NbTour;
                foreach(GameObject character1 in  GameObject.FindGameObjectsWithTag("characters"))
                {
                    foreach (GameObject character2 in GameObject.FindGameObjectsWithTag("characters"))
                    {
                        Physics2D.IgnoreCollision(character1.GetComponent<CapsuleCollider2D>(), character2.GetComponent<CapsuleCollider2D>());
                    }
                }

                foreach (GameObject charact in GameObject.FindGameObjectsWithTag("characters"))
                {
                    if (charact.GetComponent<Char_script>().NumEquipe == PlayingTeam && charact.GetComponent<Char_script>().NumWorms == wrms)
                    {
                        selectedPlayer = charact;
                        charact.GetComponent<Char_script>().Selected = true;
                        charact.GetComponent<SoldatoControl>().Selected = true;
                    }
                }
                Timer();
                //Met la caméra en vue centrée sur le soldat actuel
                Camera.main.GetComponent<CameraFollow>().Target = selectedPlayer.transform;
                Camera.main.orthographicSize = 15;
                Char_script first = selectedPlayer.GetComponent<Char_script>();
                GameObject.Find("HUD").GetComponentInChildren<HealthDisplay>().SetText(first.NumEquipe, first.NumWorms, first.Hp, first.HpMax);
                
            }            
        }
        else if(gameStart)
        {
            joueur.text = "Joueur " + PlayingTeam;
            if (Time.time - startTime >= TimeBeforeChange)
            {
                selectedPlayer.GetComponent<Char_script>().Selected = false;
                selectedPlayer.GetComponent<SoldatoControl>().Selected = false;

                ChangePlayer();
                //Redémarrage du timer
                startTime = Time.time;
            }
            Timer();
        }
        //Debug.Log(1.0f/Time.smoothDeltaTime);
    }

    void ChangePlayer()
    {
        int NewPlayingTeam = PlayingTeam == 1 ? 2 : 1;
        
        if (NewPlayingTeam == 1)
        {
            WormsToPlay = new System.Random().Next(lNumWormsJ1.Count);
            NbTour++;
            if (!fini)
            {
                tour.text = "Début du tour " + NbTour;
            }            
            wrms = lNumWormsJ1[WormsToPlay];
        }
        else
        {
            WormsToPlay = new System.Random().Next(lNumWormsJ2.Count);
            wrms = lNumWormsJ2[WormsToPlay];
        }
        
        PlayingTeam = NewPlayingTeam;
        foreach (GameObject charact in GameObject.FindGameObjectsWithTag("characters"))
        {
            if (charact.GetComponent<Char_script>().NumEquipe == PlayingTeam && charact.GetComponent<Char_script>().NumWorms == wrms)
            {
                selectedPlayer = charact;
                charact.GetComponent<Char_script>().Selected = true;
                charact.GetComponent<SoldatoControl>().Selected = true;
            }
        }
        if (selectedPlayer.GetComponent<Char_script>().Shoted)
        {
            selectedPlayer.GetComponent<Char_script>().Shoted = false;
            selectedPlayer.GetComponent<Char_script>().Create = false;
        }
    }

    public void Timer()
    {
         HUD.GetComponentInChildren<HealthDisplay>().SetTime(Time.time - startTime);
    }

    void CreateSoldiers()
    {
        if (Time.time - interval >= 1)
        {
            //Met la caméra en vue globale
            Camera.main.GetComponent<CameraFollow>().Start();
            indic.text = "Joueur " + turn + "\nCliquez pour poser votre soldat " + (characNum[turn - 1] + 1);
            if (Input.GetMouseButton(0))
            {
                //Récupère position de la souris selon la scène
                mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (turn == 1)
                {
                    clone = Instantiate(charact_ketchup, new Vector3(mouse.x, 1), transform.rotation);
                    lNumWormsJ1.Add(characNum[turn - 1] + 1);
                }
                else if(turn == 2)
                {
                    clone = Instantiate(charact_wasabi, new Vector3(mouse.x, 1), transform.rotation);
                    lNumWormsJ2.Add(characNum[turn - 1] + 1);
                }
                interval = Time.time;
            
                clone.GetComponent<Char_script>().NumEquipe = turn;
                clone.GetComponent<Char_script>().NumWorms = characNum[turn - 1] + 1;
                clone.GetComponent<Char_script>().name = "Equipe " + turn + " Worms " + (characNum[turn - 1] + 1);
                characNum[turn - 1]++;            
                if (turn == 2 && clone.GetComponent<Char_script>().NumWorms == numWormsMax)
                {
                    AllSoldiersCreated = true;                    
                }
                turn = turn == 1 ? 2 : 1;

                //Met la caméra en vue proche sur le personnage créé
                Camera.main.GetComponent<CameraFollow>().Target = clone.transform;
                Camera.main.orthographicSize = 25;
            }
        }
    }

    void RemoveSoldier (GameObject sender)
    {
        Camera.main.GetComponent<CameraFollow>().Start();
        if (sender.GetComponent<Char_script>().NumEquipe == 1)
        {
            lNumWormsJ1.Remove(sender.GetComponent<Char_script>().NumWorms);
        }
        else
        {
            lNumWormsJ2.Remove(sender.GetComponent<Char_script>().NumWorms);
        }
        //destroyed = true;
        sender.SetActive(false);
    }

    public GameObject Selected
    {
        get { return selected; }
        set { selected = value; }
    }    
}
