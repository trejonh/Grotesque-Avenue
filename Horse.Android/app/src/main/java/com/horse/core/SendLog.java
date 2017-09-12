package com.horse.core;

import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.app.Activity;
import android.view.View;
import android.view.Window;
import android.widget.Button;

import com.horse.R;
import com.horse.utils.HorseCache;
import com.orhanobut.logger.Logger;

public class SendLog extends Activity implements View.OnClickListener {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        requestWindowFeature (Window.FEATURE_NO_TITLE); // make a dialog without a titlebar
        setFinishOnTouchOutside (false); // prevent users from dismissing the dialog by tapping outside
        setContentView(R.layout.activity_send_log);
        ((Button)findViewById(R.id.sendLogBtn)).setOnClickListener(this);
        ((Button)findViewById(R.id.cancelLogBtn)).setOnClickListener(this);
    }

    @Override
    public void onClick (View v)
    {
        switch (v.getId()){
            case R.id.sendLogBtn:
                sendLogFile();
                break;
            case R.id.cancelLogBtn:
                finish();
                return;
        }
    }

    private void sendLogFile ()
    {
        Intent intent = new Intent (Intent.ACTION_SEND);
        intent.setType ("plain/text");
        intent.putExtra (Intent.EXTRA_EMAIL, new String[] {"trejon_house@yahoo.com"});
        intent.putExtra (Intent.EXTRA_SUBJECT, "HORSE log file");
        intent.putExtra (Intent.EXTRA_STREAM, Uri.parse ((String)HorseCache.getItem("LogFile")));
        intent.putExtra (Intent.EXTRA_TEXT, "Log file attached.");
        this.startActivity(intent);
        finish();
    }

}
