﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// An inline struct to contain cinematic steps
/// </summary>
[System.Serializable]
public struct CinematicStep
{
    public Vector3 location;
    public string statement;
    public float timeAtLocation; 
}

public class Blacksmith : AnimatedEntity
{

    [Header("Cinematic Settings")]
    public bool cinematicControlled = true;
    public List<CinematicStep> cinematicSteps;
    private int _cinematicIndex;

    public GameObject uiCanvas;
    public Text text;

    private float _cutsceneTimer = 0;

    [Header("Animation Settings")]
    public float Speed = 2f;
    public Sprite idleUp, idleRight, idleDown, idleLeft;
    public List<Sprite> upWalkCycle, rightWalkCycle, downWalkCycle, leftWalkCycle;
    private Vector3 _priorPosition;
    private int _direction = -1;//0 is up, 1 is right, 2 is down, 3 is left
    private float minDiff = 0.00001f;

    void Start()
    {
        AnimationSetup();
        _priorPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //What to do if the player is being controlled by a cinemtic
        if(cinematicControlled){
            if(_cinematicIndex<cinematicSteps.Count){
                //Move player to first cinematicSteps location if not there yet
                if((transform.position-cinematicSteps[_cinematicIndex].location).magnitude>0.005f){
                    transform.position+= (cinematicSteps[_cinematicIndex].location-transform.position).normalized*Time.deltaTime*Speed;
                }
                else{
                    //Set player location to avoid float issues
                    transform.position = cinematicSteps[_cinematicIndex].location;
                    if (_cutsceneTimer>=cinematicSteps[_cinematicIndex].timeAtLocation){
                        _cinematicIndex+=1;//Move on to next step if there is one
                        _cutsceneTimer=0;
                        uiCanvas.SetActive(false);
                    }
                    else{
                        //Display text during timer if there is any
                        if(cinematicSteps[_cinematicIndex].statement!=""){
                            uiCanvas.SetActive(true);
                            text.text = cinematicSteps[_cinematicIndex].statement;
                        }

                        _cutsceneTimer+=Time.deltaTime;
                    }
                }
            }
            else{
                // Return control to the player at the end of this
                cinematicControlled = false;
            }
        }
        else{
            //Movement controls if not cinematic controlled
            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
                transform.position+= (Vector3.up+Vector3.forward)*Time.deltaTime*Speed;
            }
            if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
                transform.position+= Vector3.left*Time.deltaTime*Speed;
            }
            if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
                transform.position+= (Vector3.down+Vector3.back)*Time.deltaTime*Speed;
            }
            if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
                transform.position+= Vector3.right*Time.deltaTime*Speed;
            }
        }

        //Animation Update based on movement
        if((transform.position.y-_priorPosition.y)>minDiff){
            //Moving Up
            if(_direction!=0){
                _direction = 0;
                DefaultAnimationCycle = upWalkCycle;
            }
        }
        if((_priorPosition.y-transform.position.y)>minDiff){
            //Moving Down
            if(_direction!=2){
                _direction = 2;
                DefaultAnimationCycle = downWalkCycle;
            }
        }

        if((transform.position.x-_priorPosition.x)>minDiff){
            //Moving right
            if(_direction!=1){
                _direction = 1;
                DefaultAnimationCycle = rightWalkCycle;
            }
        }
        if((_priorPosition.x-transform.position.x)>minDiff){
            //Moving left
            if(_direction!=3){
                _direction = 3;
                DefaultAnimationCycle = leftWalkCycle;
            }
        }

        //Animation Handling!
        if((_priorPosition-transform.position).magnitude>minDiff){
            AnimationUpdate();//Animate if moving
        }
        else{//Pick idle sprite if not moving
            if(_direction==0){
                SpriteRenderer.sprite = idleUp;
            }
            else if(_direction==1){
                SpriteRenderer.sprite = idleRight;
            }
            else if(_direction==2){
                SpriteRenderer.sprite = idleDown;
            }
            else if(_direction==3){
                SpriteRenderer.sprite = idleLeft;
            }
            
        }

        //Grab the priorPosition
        _priorPosition = transform.position;
    }

    void OnTriggerEnter(Collider other){
        Enemy enemy = other.gameObject.GetComponent<Enemy>();

        if(enemy!=null){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else{
            Time.timeScale = 0;
            uiCanvas.SetActive(true);
            text.text = "I win!";
        }
    }
}
