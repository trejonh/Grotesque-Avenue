package com.horse.core;

import android.app.Application;
import android.content.Intent;

import com.orhanobut.logger.Logger;

/**
 * Created by yorel56 on 9/10/2017.
 */

public class HorseApplication extends Application {
    public void onCreate ()
    {
        super.onCreate();
        // Setup handler for uncaught exceptions.
        Thread.setDefaultUncaughtExceptionHandler (new Thread.UncaughtExceptionHandler()
        {
            @Override
            public void uncaughtException (Thread thread, Throwable e)
            {
                handleUncaughtException (thread, e);
            }
        });
    }

    public void handleUncaughtException (Thread thread, Throwable e)
    {
        ServerConnection.closeConnection();
        e.printStackTrace();
        Logger.e(e, e.toString());
        Intent intent = new Intent();
        intent.setAction ("com.horse.SEND_LOG"); // see step 5.
        intent.setFlags (Intent.FLAG_ACTIVITY_NEW_TASK); // required when starting from Application
        startActivity (intent);

        System.exit(1); // kill off the crashed app
    }
}
