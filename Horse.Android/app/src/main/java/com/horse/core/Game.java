package com.horse.core;

import java.util.ArrayList;

/**
 * Created by trhous on 9/8/2017.
 */

public class Game {
    public String Title;
    public String DetailedInfo;
    public String ImageSrc;

    public Game(String title, String details, String img){
        Title = title;
        DetailedInfo = details;
        ImageSrc = img;
    }

    public static ArrayList<Game> getGameList(){
        return new ArrayList<Game>();
    }
}
