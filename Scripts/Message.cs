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


[System.Serializable]
public class IncomingMessage
{
    public string eventType;
    // public int lives;

    public static IncomingMessage ParseJSON(string jsonString)
    {
        return JsonUtility.FromJson<IncomingMessage>(jsonString);
    }
}