using System;
using System.Collections.Concurrent;
using WebSocketSharp;
using UnityEngine;

public class Websocket : MonoBehaviour
{
    public WebSocket ws;
    public GameObject wheelObject;
    private WheelJoint2D wheel;
    ConcurrentQueue<string> incoming_messages = new ConcurrentQueue<string>();

    // Start is called before the first frame update
    void Start()
    {
        // Init socket and getting reference to the back wheel 
        ws = new WebSocket("ws://localhost:8080");
        wheel = wheelObject.GetComponent<WheelJoint2D>();
        
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
    void Update()
    {
        // Every frame update, try to dequeque to see if there is any outstanding messages
        if (incoming_messages.TryDequeue(out var message))
        {
            // Handle the messages here
            IncomingMessage serverMessage = new IncomingMessage();
            serverMessage = IncomingMessage.ParseJSON(message);
            Debug.Log(serverMessage.eventType);
                
            //USE SWITCH STATEMENT TO UPDATE POD STATE BASE ON EVENT TYPE
            // START
            // STOP
            // RESET
            wheel.useMotor = true;
        }
    }
}
