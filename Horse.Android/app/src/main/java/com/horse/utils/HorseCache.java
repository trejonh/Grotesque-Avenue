package com.horse.utils;

import java.util.HashMap;

/**
 * Created by trhous on 8/31/2017.
 */

public class HorseCache {
    private static HashMap<String,Object> HORSE_CACHE;

    public static HashMap<String, Object> getInstance() {
        if(HORSE_CACHE == null){
            synchronized (HorseCache.class){
                HORSE_CACHE = new HashMap<>();
            }
        }
        return HORSE_CACHE;
    }

    public static void addItem(String name, Object item){
        getInstance().put(name,item);
    }

    public static Object getItem(String name){
        Object obj =getInstance().get(name);
        if(obj == null)
            LogManager.getInstance().warn(name + " does not exist in the cache");
        return obj;
    }
}
