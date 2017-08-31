package com.horse.screens;

import android.Manifest;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.app.Activity;
import android.provider.Settings;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.view.View;
import android.widget.Button;
import android.widget.ListView;
import android.widget.Toast;

import com.horse.R;
import com.horse.core.HorseActivity;
import com.horse.core.MessageType;
import com.horse.core.Player;
import com.horse.core.PlayerAdapter;
import com.horse.core.ServerConnection;

import java.net.InetAddress;
import java.util.ArrayList;
import java.util.Timer;
import java.util.TimerTask;

public class LobbyScreenActivity extends HorseActivity {
    private final int MAX_ALLOWED_PLAYERS = 8;
    public static String MyHash;
    private static Timer _getPlayerList;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_lobby_screen);
        checkPermissions();
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

    private void initInstructionset(){
        findViewById(R.id.connecting_view).setVisibility(View.GONE);
        findViewById(R.id.instructionSet_title).setVisibility(View.VISIBLE);
        findViewById(R.id.lobby_scrollView).setVisibility(View.VISIBLE);
        final Button readBtn = (Button)findViewById(R.id.readInstructions);
        readBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                ServerConnection.sendTimedMessage(MessageType.CMD+" getplayerlist","getplayerlist",15);
                readBtn.setVisibility(View.GONE);
                findViewById(R.id.instructionSet).setVisibility(View.GONE);
                findViewById(R.id.instructionSet_title).setVisibility(View.GONE);
                findViewById(R.id.legend).setVisibility(View.VISIBLE);
                findViewById(R.id.lobby_title).setVisibility(View.VISIBLE);
                final PlayerAdapter playerAdapter = new PlayerAdapter(LobbyScreenActivity.this, new ArrayList<Player>());
                ListView lv = (ListView)findViewById(R.id.players_list_view);
                lv.setVisibility(View.VISIBLE);
                lv.setAdapter(playerAdapter);
                _getPlayerList = new Timer();
                _getPlayerList.scheduleAtFixedRate(new TimerTask() {
                    @Override
                    public void run() {
                        runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                playerAdapter.clear();
                                playerAdapter.addAll(Player.getPlayersFromServer());
                            }
                        });
                    }
                }, 0, 20);
            }
        });
    }

    private void receiveOk() {
        String messageRecieved = ServerConnection.readMessage();
        if(!messageRecieved.contains("OK")){
            runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    Toast.makeText(LobbyScreenActivity.this,"Failed to connect, try again",Toast.LENGTH_LONG).show();
                }
            });
            finish();
        }else{
            MyHash = messageRecieved.substring(2);
            runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    initInstructionset();
                }
            });
        }

    }

    private void sendInitialMessage() {
        String message = ServerConnection.DisplayName + "," + Settings.Secure.getString(this.getContentResolver(), Settings.Secure.ANDROID_ID);;
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
        if(_getPlayerList != null)
            _getPlayerList.cancel();
        ServerConnection.closeConnection();
    }
}
