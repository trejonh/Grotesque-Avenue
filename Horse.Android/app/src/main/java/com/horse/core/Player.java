package com.horse.core;

import java.util.ArrayList;

/**
 * Created by trhous on 8/29/2017.
 * @author TreJon House
 * @version 0.1
 *
 * A container for mobile players in the game
 */

public class Player {
    /**
     * Name of the player
     */
    public String Name;

    /**
     * The player's unique id
     */
    public String Id;

    /**
     * True if the player is the leader
     */
    public boolean IsVip;

    /**
     * True if the player is next up to play
     */
    public boolean IsNext;

    /**
     * Create a new player
     * @param name Name of the player
     * @param id Unique id of the player
     */
    public Player(String name, String id){
        Name = name;
        Id = id;
    }

    /**
     * The player's name
     * @return The player's name
     */
    public String getName() {
        return Name;
    }

    /**
     * Get the unique player id
     * @return The player's id
     */
    public String getId() {
        return Id;
    }

    /**
     * Is the player the leader
     * @return true if the player is the leader
     */
    public boolean isVip() {
        return IsVip;
    }

    /**
     * Is the player next up to play
     * @return true if the player is next to play
     */
    public boolean isNext() {
        return IsNext;
    }

    /**
     * Set the vip status
     * @param vip True if the player is the leader
     */
    public void setVip(boolean vip) {
        IsVip = vip;
    }

    /**
     * Set if the player is next to play
     * @param next True is the player is next
     */
    public void setNext(boolean next) {
        IsNext = next;
    }

    /**
     * Retrieve all players currently in the game
     * @return The players in the game
     */
    public static ArrayList<Player> getPlayersFromServer(){
        ArrayList<Player> pList = new ArrayList<>();
        String players = "";
        for (Message message: ServerConnection.MessagesIn) {
            if(!message.Type.equals(MessageType.INFO))
                continue;
            if(message.Message.contains("playerList[") == false)
                continue;
            players = message.Message.substring(message.Message.indexOf('[')+1,message.Message.length()-2);
        }
        if(players.length() == 0)
            return pList;
        String[] indivPlayers = players.split("player: ");
        for (String inPlay: indivPlayers) {
            if(inPlay.length() == 0)
                continue;
            if(inPlay.contains(",") == false)
                continue;
            String[] playerProps = inPlay.split(",");
            Player player = new Player(playerProps[0],playerProps[1]);
            if(playerProps[2].equals("true"))
                player.setVip(true);
            if(playerProps[3].equals("true"))
                player.setNext(true);
            pList.add(player);
        }
        return pList;
    }
}
