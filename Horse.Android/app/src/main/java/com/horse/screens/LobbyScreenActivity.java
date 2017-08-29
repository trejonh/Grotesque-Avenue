package com.horse.screens;

import android.Manifest;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.app.Activity;
import android.provider.Settings;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.Toast;

import com.horse.R;
import com.horse.core.Player;
import com.horse.core.ServerConnection;

import java.net.InetAddress;
import java.util.HashMap;

public class LobbyScreenActivity extends Activity {
    private Player[] Players;
    private final int MAX_ALLOWED_PLAYERS = 8;
    private static String myHash;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_lobby_screen);
        checkPermissions();
        Players = new Player[MAX_ALLOWED_PLAYERS];
        String displayName = getIntent().getStringExtra("DisplayName");
        InetAddress inet = (InetAddress) getIntent().getExtras().get("InetAddr");
        new ServerConnection(inet,displayName);
        ServerConnection.executeServerConnection();
        new Thread(new Runnable() {
            @Override
            public void run() {
                while(ServerConnection.Ready == false){/*wait*/}
                sendInitialMessage();
                receiveOk();
            }
        }).start();
    }

    private void receiveOk() {
        String messageRecieved = ServerConnection.readMessage();
        if(!messageRecieved.contains("OK")){
            Toast.makeText(this,"Failed to connect, try again",Toast.LENGTH_LONG).show();
            finish();
        }else{
            myHash = messageRecieved.substring(2);
            findViewById(R.id.connecting_view).setVisibility(View.GONE);
            findViewById(R.id.lobby_scrollView).setVisibility(View.VISIBLE);
            final Button readBtn = (Button)findViewById(R.id.readInstructions);
            readBtn.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    findViewById(R.id.lobby_scrollView).setVisibility(View.GONE);
                    readBtn.setVisibility(View.GONE);
                    ArrayAdapter<Player> playerAdapter = new ArrayAdapter<>(LobbyScreenActivity.this, R.layout.player_list, R.id.playerName, Players);
                    ListView lv = (ListView)findViewById(R.id.players_list_view);
                    lv.setVisibility(View.VISIBLE);
                    lv.setAdapter(playerAdapter);
                }
            });
        }

    }

    private void sendInitialMessage() {
        String message = ServerConnection.DisplayName + "," + Settings.Secure.ANDROID_ID.toString();
        ServerConnection.sendMessage(message);
    }

    private void checkPermissions(){
        while (ContextCompat.checkSelfPermission(this, Manifest.permission.INTERNET)  != PackageManager.PERMISSION_GRANTED
                &&
                ContextCompat.checkSelfPermission(this,Manifest.permission.ACCESS_NETWORK_STATE) != PackageManager.PERMISSION_GRANTED){
            // Should we show an explanation?
            if (ActivityCompat.shouldShowRequestPermissionRationale(this, Manifest.permission.INTERNET)) {
                String str = "Explanation needed: Please I need to use internet";
                Toast.makeText(this, str, Toast.LENGTH_LONG).show();
            }
            if (ActivityCompat.shouldShowRequestPermissionRationale(this, Manifest.permission.ACCESS_NETWORK_STATE)) {
                String str = "Explanation needed: Please I need to use internet";
                Toast.makeText(this, str, Toast.LENGTH_LONG).show();
            }else {
                // No explanation needed, we can request the permission.
                ActivityCompat.requestPermissions(this,
                        new String[]{Manifest.permission.ACCESS_NETWORK_STATE,Manifest.permission.INTERNET },
                        1);
                String str = "No explanation needed: thanks.";
                Toast.makeText(this, str, Toast.LENGTH_LONG).show();
            }
        }
    }

    @Override
    protected void onStop(){
        super.onStop();
        ServerConnection.closeConnection();
    }

    @Override
    protected  void onDestroy(){
        super.onDestroy();
        ServerConnection.closeConnection();
    }

    @Override
    public void onBackPressed() {
        super.onBackPressed();
        ServerConnection.closeConnection();
    }
}
