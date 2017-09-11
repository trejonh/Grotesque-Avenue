package com.horse.core;

import android.content.Context;

import com.horse.R;
import com.horse.utils.HorseCache;

import java.util.ArrayList;
import java.util.Arrays;

/**
 * Created by trhous on 9/8/2017.
 */

public class Game {
    public String Title;
    public String DetailedInfo;
    public int ImageSrc;
    public String ShortName;

    public Game(String title, String details, String shortName, int img){
        Title = title;
        DetailedInfo = details;
        ImageSrc = img;
        ShortName = shortName;
    }

    public static ArrayList<Game> getGameList(){
        Context appContext = (Context) HorseCache.getItem("ApplicationContext");
        Game[] games= { new Game("Color Me Pretty", appContext.getString(R.string.cmp_details), "cmp",R.drawable.cmp)};
        return new ArrayList<>(Arrays.asList(games));
    }
}
