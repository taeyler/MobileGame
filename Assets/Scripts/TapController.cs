using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]         //create rigid body 2D
public class TapController : MonoBehaviour {

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10;                 //force of tap
    public float tiltSmooth = 5;                //rotate downwards as fall
    public Vector3 startPos;                    //start position

    Rigidbody2D rigidbody;
    Quaternion downRotation;                    //implements rotations
    Quaternion forwardRotation;

    GameManager game;
    //TrailRenderer trail;

    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();        //gets the component
        downRotation = Quaternion.Euler(0, 0, -80);     //facing down
        forwardRotation = Quaternion.Euler(0, 0, 45);   //facing up
        game = GameManager.Instance;
        rigidbody.simulated = false;                    //bird doesn't fall
    }

    void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;                 //Reconfiguring game objects -> reset pipes
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted()
    {
        rigidbody.velocity = Vector3.zero;      //reset velocity to zero
        rigidbody.simulated = true;             //reset the simulated
    }

    void OnGameOverConfirmed()                    
    {
        transform.localPosition = startPos;             //Bird set to start position
        transform.rotation = Quaternion.identity;       //reset rotation
    }


    void Update()
    {   
        if (game.GameOver) return;                      //stops rotation after respawn



        if (Input.GetMouseButtonDown(0))                //left click on mouse, translates as tap on mobile
        {
//            Time.timeScale += 1;
//            rigidbody.velocity = Vector2.zero;
            transform.rotation = forwardRotation;
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);    //moves down automatically
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "ScoreZone")
        {
            OnPlayerScored();               //event sent to game manager
           
                                            //register score event
                                            //play sound
        }
        
        if  (col.gameObject.tag == "DeadZone")
        {
            rigidbody.simulated = false;    //freeze bird where he hits
            OnPlayerDied();                 //event sent to game manager
                                            //register a dead event



            //play a sound
        }
    }
}
