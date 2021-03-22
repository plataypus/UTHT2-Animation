using System;
using UnityEngine;

[Serializable]
public class Message 
{
    public string eventType;
    public MessageData data;
}

[Serializable]
public class MessageData
{
    public string clientType;
}

[Serializable]
public class incomingData
{
    public string command;
    public incomingParam param;

}

[Serializable]
public class incomingParam
{
    public float brakeTorque;
    public float brakeSpeed;
    public float acceleration;

}


[System.Serializable]
public class IncomingMessage
{
    public string eventType;
    public incomingData data;
    // public int lives;

    public static IncomingMessage ParseJSON(string jsonString)
    {
        return JsonUtility.FromJson<IncomingMessage>(jsonString);
    }
}