package com.horse.core;

import java.util.Date;

/**
 * Created by trhous on 8/30/2017.
 * @version 1.0
 * @author TreJon House
 * A message received from the server
 */

public class Message{
    /**
     * Message received
     */
    public String Message;
    /**
     * The type of message
     */
    public String Type;
    /**
     * The date it was received
     */
    public Date ReceivedDate;

    /**
     * Create a message
     * @param message The received message
     * @param type The message type
     * @param receivedDate The time it was received
     */
    public Message(String message, String type, Date receivedDate) {
        Message = message;
        Type = type;
        ReceivedDate = receivedDate;
    }
}