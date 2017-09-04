package com.horse;

import com.horse.utils.HorseCache;

import org.junit.Test;

import java.util.Objects;

import static org.junit.Assert.*;

public class UtilityTests {
    @Test
    public void cacheTest() throws Exception {
        assertNotNull(HorseCache.getInstance());
        assertNull(HorseCache.getItem("item"));
        HorseCache.addItem("item", new Object());
        assertNotNull(HorseCache.getItem("item"));
    }
}