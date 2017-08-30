package com.horse.core;

import java.util.Date;

/**
 * Created by trhous on 8/30/2017.
 */

public class Message{
    public String Message;
    public String Type;
    public Date ReceivedDate;

    public Message(String message, String type, Date receivedDate) {
        Message = message;
        Type = type;
        ReceivedDate = receivedDate;
    }
}