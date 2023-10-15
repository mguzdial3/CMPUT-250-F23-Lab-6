using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : AnimatedEntity
{
    [Header("Patrol Settings")]
    public bool patrolling = true;
    public List<CinematicStep> patrol;
    private int _patrolIndex;
    private float _patrolTimer = 0;
    public GameObject player;

    [Header("Animation Settings")]
    public float Speed = 2f;
    public Sprite idleUp, idleRight, idleDown, idleLeft;
    public List<Sprite> upWalkCycle, rightWalkCycle, downWalkCycle, leftWalkCycle;
    private Vector3 _priorPosition;
    private int _direction = -1;//0 is up, 1 is right, 2 is down, 3 is left
    private float minDiff = 0.00001f;
    

    // Start is called before the first frame update
    void Start()
    {
        AnimationSetup();
        _priorPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(patrolling){
            if(_patrolIndex<patrol.Count){
                //Move player to first cinematicSteps location if not there yet
                if((transform.position-patrol[_patrolIndex].location).magnitude>0.005f){
                    transform.position+= (patrol[_patrolIndex].location-transform.position).normalized*Time.deltaTime*Speed;
                }
                else{
                    transform.position = patrol[_patrolIndex].location;
                    if (_patrolTimer>=patrol[_patrolIndex].timeAtLocation){
                        _patrolIndex+=1;//Move on to next one
                        _patrolTimer=0;
                    }
                    else{
                        _patrolTimer+=Time.deltaTime;
                    }
                }
            }
            else{
                // Repeat patrol
                _patrolIndex = 0;
                _patrolTimer = 0;
            }
        }
        else{
            //TODO: SEEK PLAYER TO BE IMPLEMENTED

        }

        //TODO: DETERMINE IF WE NEED TO SEEK PLAYER OR PATROL
    
    
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
}
