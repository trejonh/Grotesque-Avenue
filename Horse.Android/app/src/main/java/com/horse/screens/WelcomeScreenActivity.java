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

import java.net.InetAddress;
import java.net.UnknownHostException;

import static android.content.DialogInterface.BUTTON_NEGATIVE;
import static android.content.DialogInterface.BUTTON_POSITIVE;

/**
 * An example full-screen activity that shows and hides the system UI (i.e.
 * status bar and navigation/system bar) with user interaction.
 */
public class WelcomeScreenActivity extends Activity implements View.OnClickListener, DialogInterface.OnClickListener {
    private InetAddress inetAddress;
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
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            builder.setView(R.layout.connect_to_game_server);
            builder.setPositiveButton("OK", this);
            builder.setNegativeButton("Cancel",this);
            builder.show();
        }else {
            final EditText nameField = new EditText(this);
            final EditText serverAddress = new EditText(this);
            nameField.setHint("Display Name");
            serverAddress.setHint("Server Address: 0.0.0.0");
            AlertDialog.Builder obsBuilder = new AlertDialog.Builder(this);
            obsBuilder.setView(nameField);
            obsBuilder.setPositiveButton("OK", new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int which) {
                        String displayName = ((EditText)((AlertDialog)dialog).findViewById(R.id.displayName)).getText().toString();
                        if(displayName.length() < 3){
                            Toast.makeText(WelcomeScreenActivity.this,"Chosen display name is too short",Toast.LENGTH_SHORT).show();
                            dialog.dismiss();
                            return;
                        }
                        name = displayName;
                    dialog.dismiss();
                }
            });
            obsBuilder.setNegativeButton("Cancel", this);
            obsBuilder.show();
            builder.setView(serverAddress);
            builder.setPositiveButton("OK", new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int which) {
                    String ip = ((EditText)((AlertDialog)dialog).findViewById(R.id.serverAddress)).getText().toString();
                    try {
                        if(ip == null || ip.length() == 0 || (inetAddress = InetAddress.getByName(ip)) ==  null){
                            Toast.makeText(WelcomeScreenActivity.this,"Bad Server address",Toast.LENGTH_SHORT).show();
                            dialog.dismiss();
                            return;
                        }
                    } catch (UnknownHostException e) {
                        Toast.makeText(WelcomeScreenActivity.this,"Bad Server address",Toast.LENGTH_SHORT).show();
                        dialog.dismiss();
                        return;
                    }

                }
            });
            builder.setNegativeButton("Cancel",this);
            builder.show();
        }
    }

    @Override
    public void onClick(DialogInterface dialog, int which) {
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
                try {
                    if(ip == null || ip.length() == 0 || (inetAddress = InetAddress.getByName(ip)) ==  null){
                        Toast.makeText(this,"Bad Server address",Toast.LENGTH_SHORT).show();
                        dialog.dismiss();
                        break;
                    }
                } catch (UnknownHostException e) {
                    Toast.makeText(this,"Bad Server address",Toast.LENGTH_SHORT).show();
                    dialog.dismiss();
                    break;
                }
                name = displayName;
                dialog.dismiss();
                Intent intent = new Intent(this, LobbyScreenActivity.class);
                intent.putExtra("DisplayName",name);
                intent.putExtra("InetAddr",inetAddress);
                startActivity(intent);
                break;
        }
    }
}
