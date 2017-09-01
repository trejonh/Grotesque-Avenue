package com.horse.utils;

import java.util.HashMap;

/**
 * Created by trhous on 8/31/2017.
 * @version 1.0
 * @author TreJon House
 * A public cache to access non-static resources that may be needed globally
 */

public class HorseCache {
    private static HashMap<String,Object> HORSE_CACHE;

    /**
     * Gets an instance of the cache
     * @return The cache
     */
    public static HashMap<String, Object> getInstance() {
        if(HORSE_CACHE == null){
            synchronized (HorseCache.class){
                HORSE_CACHE = new HashMap<>();
            }
        }
        return HORSE_CACHE;
    }

    /**
     * Add an item to the cache
     * @param name Name of the item
     * @param item The item to add
     */
    public static void addItem(String name, Object item){
        getInstance().put(name,item);
    }

    /**
     * Get an item from the cache
     * @param name The name of the item to retrieve
     * @return null if no item was found
     */
    public static Object getItem(String name){
        Object obj =getInstance().get(name);
        if(obj == null)
            LogManager.getInstance().warn(name + " does not exist in the cache");
        return obj;
    }
}
