package com.horse.screens;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Build;
import android.os.Bundle;
import android.text.InputType;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import com.horse.R;
import com.horse.core.HorseActivity;
import com.orhanobut.logger.Logger;

import java.net.InetAddress;
import java.net.UnknownHostException;

import static android.content.DialogInterface.BUTTON_NEGATIVE;
import static android.content.DialogInterface.BUTTON_POSITIVE;

/**
 * @author TreJon House
 * @version 0.1
 * A welcome screen for users to get to lobby
 */
public class WelcomeScreenActivity extends HorseActivity implements View.OnClickListener, DialogInterface.OnClickListener {
    private String name;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_welcome_screen);
        Button joinGameBtn = (Button)findViewById(R.id.join_game_lobby_btn);
        joinGameBtn.setOnClickListener(this);
    }

    @Override
    public void onClick(View v) {
        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setTitle("Connect to game server");
        builder.setView(R.layout.connect_to_game_server);
        builder.setPositiveButton("OK", this);
        builder.setNegativeButton("Cancel",this);
        builder.show();
    }

    @Override
    public void onClick(final DialogInterface dialog, int which) {
        switch (which){
            case BUTTON_NEGATIVE:
                dialog.dismiss();
                break;
            case BUTTON_POSITIVE:
                String displayName = ((EditText)((AlertDialog)dialog).findViewById(R.id.displayName)).getText().toString();
                if(displayName.length() < 3){
                    Toast.makeText(this,"Chosen display name is too short",Toast.LENGTH_SHORT).show();
                    dialog.dismiss();
                    break;
                }
                String ip = ((EditText)((AlertDialog)dialog).findViewById(R.id.serverAddress)).getText().toString();
                if(ip == null || ip.length() == 0){
                    Toast.makeText(this,"Bad Server address",Toast.LENGTH_SHORT).show();
                    dialog.dismiss();
                }
                name = displayName;
                dialog.dismiss();
                Intent intent = new Intent(this, LobbyScreenActivity.class);
                intent.putExtra("DisplayName",name);
                intent.putExtra("InetAddr",ip);
                startActivity(intent);
                break;
        }
    }
}
