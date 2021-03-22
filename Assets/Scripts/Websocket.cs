using System;
using System.Collections.Concurrent;
using WebSocketSharp;
using UnityEngine;
using UnityEngine.UI;

public class Websocket : MonoBehaviour
{
    public WebSocket ws;
    public GameObject wheelObject;
    public Rigidbody2D player;

    public Text uiText;
    public double initialPose;
    public double accel;
    public double timePassed;
    private float vel;
    private double adjustedPose;
    public double initialV = 0;
    public double maxBrakeSpeed;
    public double conversionConst = 0.0423639;
    public bool passedMid = false;
    public string command;
    ConcurrentQueue<string> incoming_messages = new ConcurrentQueue<string>();

    // Start is called before the first frame update
    void Start()
    {
        initialPose = player.position[0];
        timePassed = 0;
        accel = 7; //m/s^2
        maxBrakeSpeed = 16.7;

        // Init socket and getting reference to the back wheel 
        ws = new WebSocket("ws://localhost:8080");

        // On every server-sent message, push the message onto the concurrent queque to be handled in the update function
        ws.OnMessage += (sender, e) => {
            if (!e.IsText) { return; }
            incoming_messages.Enqueue(e.Data);
        };

        ws.Connect();
        
        // Prep the message to be sent to the server during initial connection (see the Message script)
        Message message = new Message();
        message.eventType = "connection";
        MessageData messagedata = new MessageData();
        messagedata.clientType = "pod";
        message.data = messagedata;
        // Convert the object into JSON
        string messageJSON = JsonUtility.ToJson(message);

        ws.Send(messageJSON);
    }

    // Update is called once per frame
    
    //motorSpeed of 1000 = 2.66 m/s
    
    void Update()
    {
        if (command == "start"){
            //wheel.useMotor = false;
            timePassed+= Time.deltaTime; //calculates how much time has passed since pod started moving
            vel = (float)((initialV + accel * timePassed)/conversionConst); //calculates the speed of the pod in every frame

            adjustedPose = (player.position[0] - initialPose)*conversionConst;
            
            //makes sure the speed is constant when brake speed is reached
            if(vel>=maxBrakeSpeed/conversionConst){
            vel = (float)(maxBrakeSpeed/conversionConst);
            }

            //slows down pod after 50m mark
            if ((adjustedPose>50) && !passedMid){
                Debug.Log("Deccelerate");
                passedMid = true;
                initialV = vel*conversionConst;
                accel =-2.78;
                timePassed = 0;
            }

            //Stops pod before it starts going backwards
            if (adjustedPose>50 && vel<=0){
                vel = 0;
            }

            player.transform.Translate ((float)vel*Time.deltaTime,0,0); //moves the pod in every frame 
            uiText.text = (vel*conversionConst).ToString("F2") + " m/s"; //updates the speedometer every frame
        }

        // Every frame update, try to dequeque to see if there is any outstanding messages
        if (incoming_messages.TryDequeue(out var message))
        {
            // Handle the messages here
            IncomingMessage serverMessage = new IncomingMessage();
            serverMessage = IncomingMessage.ParseJSON(message);
            Debug.Log(serverMessage.data.param.brakeSpeed);
            Debug.Log(serverMessage.data.command);

            command = serverMessage.data.command;
            maxBrakeSpeed = serverMessage.data.param.brakeSpeed;
            accel = serverMessage.data.param.acceleration;
                
            //USE SWITCH STATEMENT TO UPDATE POD STATE BASE ON EVENT TYPE
            // START
            // STOP
            // RESET
            //wheel.useMotor = true;
        }
    }
}
