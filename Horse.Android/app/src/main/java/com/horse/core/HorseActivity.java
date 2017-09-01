package com.horse.core;

import android.app.Activity;
import android.content.pm.ActivityInfo;
import android.os.Bundle;

/**
 * Created by trhous on 8/30/2017.
 * @author TreJon House
 * @version 1.0
 * An activity wrapper to force the app to have portrait orientation
 */

public abstract class HorseActivity extends Activity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setRequestedOrientation (ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);
    }
}
