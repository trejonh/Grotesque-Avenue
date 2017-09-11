package com.horse.core;

import com.orhanobut.logger.Logger;

import java.util.ArrayList;
import java.util.HashMap;

/**
 * Created by trhous on 8/29/2017.
 * @author TreJon House
 * @version 0.1
 *
 * A container for mobile players in the game
 */

public class Player {

    private static HashMap<String, Player> MobileNetworkPlayers;
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

    public boolean IsCurrentlyPlaying;

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

    public boolean isCurrentlyPlaying(){return IsCurrentlyPlaying;}

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

    public  void setCurrentlyPlaying(boolean currentlyPlaying){ IsCurrentlyPlaying = currentlyPlaying;}

    /**
     * Retrieve all players currently in the game
     * @return The players in the game
     */
    public static ArrayList<Player> getPlayersFromServer(){
        String players = "";
        Message playerListMessage = null;
        for (Message message: ServerConnection.getMessages()) {
            if(!message.Type.equals(MessageType.INFO))
                continue;
            if(!message.Message.contains("playerList["))
                continue;
            playerListMessage = message;
            players = message.Message.substring(message.Message.indexOf('[')+1,message.Message.length()-2);
        }
        if(players.length() == 0)
            return new ArrayList<>(getMobileNetworkPlayers().values());
        String[] indivPlayers = players.split("player: ");
        for (String inPlay: indivPlayers) {
            if(inPlay.length() == 0)
                continue;
            if(!inPlay.contains(","))
                continue;
            String[] playerProps = inPlay.split(",");
            Player player = new Player(playerProps[0],playerProps[1]);
            if(playerProps[2].equals("true"))
                player.setVip(true);
            if(playerProps[3].equals("true"))
                player.setNext(true);
            if(playerProps[4].equals("true"))
                player.setCurrentlyPlaying(true);
            synchronized (getMobileNetworkPlayers()) {
                getMobileNetworkPlayers().put(player.getId(), player);
            }
        }
        if(playerListMessage != null){
            synchronized (ServerConnection.getMessages()) {
                if (!ServerConnection.getMessages().remove(playerListMessage))
                    Logger.e("Could not delete message");
            }
        }
        return new ArrayList<>(getMobileNetworkPlayers().values());
    }

    public static HashMap<String, Player> getMobileNetworkPlayers(){
        if(MobileNetworkPlayers == null){
            synchronized (Player.class){
                MobileNetworkPlayers = new HashMap<>();
            }
        }
        return MobileNetworkPlayers;
    }
}
