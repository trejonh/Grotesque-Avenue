package com.horse;

import android.content.Intent;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Handler;
import android.os.HandlerThread;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;

import com.horse.screens.WelcomeScreenActivity;
import com.horse.utils.HorseCache;
import com.horse.utils.HorseLogHandler;
import com.orhanobut.logger.CsvFormatStrategy;
import com.orhanobut.logger.DiskLogAdapter;
import com.orhanobut.logger.DiskLogStrategy;
import com.orhanobut.logger.FormatStrategy;
import com.orhanobut.logger.Logger;
import com.orhanobut.logger.PrettyFormatStrategy;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;

/**
 * An example full-screen activity that shows and hides the system UI (i.e.
 * status bar and navigation/system bar) with user interaction.
 */
public class SplashScreenActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_splash_screen);
        HorseCache.addItem("ApplicationContext",getApplicationContext());
        HandlerThread ht = new HandlerThread("AndroidFileLogger.Horse");
        ht.start();
        FormatStrategy formatStrategy = CsvFormatStrategy.newBuilder()
                .logStrategy(new DiskLogStrategy( new HorseLogHandler(ht.getLooper())))
                .dateFormat(new SimpleDateFormat("yyyy.MM.dd HH:mm:ss.SSS", Locale.US))
                .date(new Date())
                .build();

        Logger.addLogAdapter(new DiskLogAdapter(formatStrategy));
        String model = Build.MODEL;
        if (!model.startsWith(Build.MANUFACTURER))
            model = Build.MANUFACTURER + " " + model;
        Logger.i("Android version: " +  Build.VERSION.SDK_INT);
        Logger.i("Device: " + model);
        PackageManager manager = this.getPackageManager();
        PackageInfo info = null;
        try {
            info = manager.getPackageInfo (this.getPackageName(), 0);
        } catch (PackageManager.NameNotFoundException e2) {
        }
        Logger.i("App version: " + (info == null ? "(null)" : info.versionCode));
        Handler handler = new Handler();
        handler.postDelayed(new Runnable() {
            public void run() {
                startActivity(new Intent(SplashScreenActivity.this,WelcomeScreenActivity.class));
                finish();
            }
        }, 2500);
    }

    @Override
    protected void onPostCreate(Bundle savedInstanceState) {
        super.onPostCreate(savedInstanceState);
    }
}
