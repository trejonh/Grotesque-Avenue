package com.horse.screens;

import android.Manifest;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.app.Activity;
import android.provider.Settings;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.ListView;
import android.widget.Toast;

import com.horse.R;
import com.horse.core.HorseActivity;
import com.horse.core.MessageType;
import com.horse.core.Player;
import com.horse.core.PlayerAdapter;
import com.horse.core.ServerConnection;
import com.horse.utils.LogManager;

import java.net.InetAddress;
import java.util.ArrayList;
import java.util.Timer;
import java.util.TimerTask;

/**
 * @author TreJon House
 * @version 0.1
 * A lobby screen to display the users connected to the server
 */
public class LobbyScreenActivity extends HorseActivity {
    private final int MAX_ALLOWED_PLAYERS = 8;
    public static String MyHash;
    private boolean _alreadyFoundVip =false;
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

    /**
     * Display the game instructions
     */
    private void initInstructionset(){
        findViewById(R.id.connecting_view).setVisibility(View.GONE);
        findViewById(R.id.instructionSet_title).setVisibility(View.VISIBLE);
        findViewById(R.id.lobby_scrollView).setVisibility(View.VISIBLE);
        final Button readBtn = (Button)findViewById(R.id.readInstructions);
        readBtn.setOnClickListener(new OnClickListener() {
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
                                ArrayList<Player> players = Player.getPlayersFromServer();
                                if(players.size() > 0) {
                                    playerAdapter.clear();
                                    playerAdapter.addAll(players);
                                }
                                for(Player player: players){
                                    if(_alreadyFoundVip)
                                        break;
                                    if(player.IsVip && player.Id.equals(MyHash)){
                                        _alreadyFoundVip = true;
                                        Button startGameButton = (Button)findViewById(R.id.launchGameButton);
                                        startGameButton.setVisibility(View.VISIBLE);
                                        startGameButton.setOnClickListener(new OnClickListener() {
                                            @Override
                                            public void onClick(View v) {
                                                if(Player.MobileNetworkPlayers != null && Player.MobileNetworkPlayers.size() >= 2) {
                                                    ServerConnection.cancelTimedMessage("getplayerlist");
                                                    _getPlayerList.cancel();
                                                    ServerConnection.sendMessage(MessageType.CMD + " StartGame");
                                                }
                                            }
                                        });
                                    }
                                }
                            }
                        });
                    }
                }, 0, 5*1000);
            }
        });
    }

    /**
     *
     * Wait for the 'OK' from the server that we are connected
     * and validated to play
     */
    private void receiveOk() {
        String messageRecieved = ServerConnection.readMessage();
        if(!messageRecieved.contains("OK")){
            runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    Toast.makeText(LobbyScreenActivity.this,"Failed to connect, try again",Toast.LENGTH_LONG).show();
                    LogManager.getInstance().warn("Failed to connect to address:" , getIntent().getExtras().get("InetAddr").toString());
                }
            });
            finish();
        }else{
            MyHash = messageRecieved.substring(messageRecieved.indexOf("OK")+3);
            runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    initInstructionset();
                }
            });
        }

    }

    /**
     * Send the server the initial connection message
     */
    private void sendInitialMessage() {
        String message = ServerConnection.DisplayName + "," + Settings.Secure.getString(this.getContentResolver(), Settings.Secure.ANDROID_ID);;
        ServerConnection.sendMessage(message);
    }

    /**
     * Verify we have the all the correct permissions before proceeding
     */
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
