package com.horse.core;

import java.util.ArrayList;

/**
 * Created by trhous on 8/29/2017.
 */

public class Player {
    public String Name;
    public String Id;
    public boolean IsVip;
    public boolean IsNext;

    public Player(String name, String id){
        Name = name;
        Id = id;
    }

    public String getName() {
        return Name;
    }

    public String getId() {
        return Id;
    }

    public boolean isVip() {
        return IsVip;
    }

    public boolean isNext() {
        return IsNext;
    }

    public void setVip(boolean vip) {
        IsVip = vip;
    }

    public void setNext(boolean next) {
        IsNext = next;
    }

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
