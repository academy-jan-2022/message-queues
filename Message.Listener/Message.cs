using System;

namespace Message.Listener;

public class Message
{
    public int Number;
    public string guid;

    public Message(int number, string guid) {
        this.Number = number;
        this.guid = guid;
    }
} 