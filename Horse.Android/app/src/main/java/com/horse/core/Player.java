package com.horse.core;

/**
 * Created by trhous on 8/29/2017.
 */

public class Player {
    public String Name;
    public String Id;

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
}
