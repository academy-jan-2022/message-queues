using System;

namespace Message.Listener;

public class Message
{
    public int Number;
    public string guid;
    public Guid correlationId;

    public Message(int number, string guid, Guid correlationId) {
        this.Number = number;
        this.guid = guid;
        this.correlationId = correlationId;
    }
} 