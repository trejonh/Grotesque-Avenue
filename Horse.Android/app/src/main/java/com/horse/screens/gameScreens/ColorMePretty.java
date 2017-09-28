package com.horse.screens.gameScreens;

import android.app.AlertDialog;
import android.content.DialogInterface;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;

import com.horse.R;
import com.horse.core.HorseActivity;
import com.horse.core.Message;
import com.horse.core.MessageType;
import com.horse.core.Player;
import com.horse.core.ServerConnection;
import com.horse.utils.HorseCache;
import com.orhanobut.logger.Logger;

public class ColorMePretty extends GameActivity implements View.OnClickListener {
    private Button redBtn;
    private Button blackBtn;
    private Button blueBtn;
    private Button orangeBtn;
    private Button purpleBtn;
    private Button greenBtn;
    private Button yellowBtn;
    private Button brownBtn;
    private Button readyBtn;
    private Player myPlayer;
    private boolean askedIfReady;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_color_me_pretty);
        myPlayer = Player.getMobileNetworkPlayers().get(HorseCache.getItem("MyDeviceId"));
        redBtn = (Button)findViewById(R.id.redBtn);
        redBtn.setOnClickListener(this);
        blackBtn = (Button)findViewById(R.id.blackBtn);
        blackBtn.setOnClickListener(this);
        blueBtn = (Button)findViewById(R.id.blueBtn);
        blueBtn.setOnClickListener(this);
        orangeBtn = (Button)findViewById(R.id.orangeBtn);
        orangeBtn.setOnClickListener(this);
        purpleBtn = (Button)findViewById(R.id.purpleBtn);
        purpleBtn.setOnClickListener(this);
        greenBtn = (Button)findViewById(R.id.greenBtn);
        greenBtn.setOnClickListener(this);
        yellowBtn = (Button)findViewById(R.id.yellowBtn);
        yellowBtn.setOnClickListener(this);
        brownBtn = (Button)findViewById(R.id.brownBtn);
        brownBtn.setOnClickListener(this);
        readyBtn = (Button)findViewById(R.id.redBtn);
        readyBtn.setOnClickListener(this);
        presentInstructions();
        askedIfReady = false;
    }

    private void presentInstructions() {
        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setTitle("Color Me Pretty");
        builder.setMessage("Game Instructions");
        builder.setPositiveButton("Ready to Play", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                if(myPlayer.IsCurrentlyPlaying){
                    findViewById(R.id.cmp_gameControls).setVisibility(View.VISIBLE);
                    waitForReadyCmd();
                }else{
                    waitForReadyCmd();
                }
                ServerConnection.sendMessage(MessageType.INFO+" readinstructions");
            }
        });
        builder.show();
    }

    private void waitForReadyCmd() {
        Thread waitForReadyMessage = new Thread(new Runnable() {
            @Override
            public void run() {
                Message message;
                while(true){
                    message = ServerConnection.readMessage(MessageType.CMD, "sendreadysignal");
                    if(message == null) continue;
                    break;
                }
                ServerConnection.removeMessage(message);
                runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        askedIfReady = true;
                        readyBtn.setVisibility(View.VISIBLE);
                    }
                });
            }
        });
        waitForReadyMessage.start();
    }

    @Override
    public void onClick(View v) {
        if(!askedIfReady)
            return;
        switch (v.getId()){
            case R.id.redBtn:
                ServerConnection.sendMessage(MessageType.DATA+" red");
                break;
            case R.id.blackBtn:
                ServerConnection.sendMessage(MessageType.DATA+" black");
                break;
            case R.id.blueBtn:
                ServerConnection.sendMessage(MessageType.DATA+" blue");
                break;
            case R.id.orangeBtn:
                ServerConnection.sendMessage(MessageType.DATA+" orange");
                break;
            case R.id.purpleBtn:
                ServerConnection.sendMessage(MessageType.DATA+" purple");
                break;
            case R.id.greenBtn:
                ServerConnection.sendMessage(MessageType.DATA+" green");
                break;
            case R.id.yellowBtn:
                ServerConnection.sendMessage(MessageType.DATA+" yellow");
                break;
            case R.id.brownBtn:
                ServerConnection.sendMessage(MessageType.DATA+" brown");
                break;
            case R.id.readySignalBtn:
                ServerConnection.sendMessage(MessageType.CMD+" ready");
                readyBtn.setVisibility(View.INVISIBLE);
                findViewById(R.id.cmp_gameControls).setVisibility(View.VISIBLE);
                break;
        }
    }

    @Override
    public void handleIncomingMessages() {
        Message toRmv = null;
        synchronized (ServerConnection.getMessages()){
            for (Message message: ServerConnection.getMessages()){
                if(!message.Type.equals(MessageType.CMD))continue;
                toRmv = message;
            }
            if (toRmv != null)
                ServerConnection.removeMessage(toRmv);
        }
        if(toRmv == null)
            return;
        switch (toRmv.Message){
            case "gameover":
                gameOver();
                break;
            case "getplayerlist":
                break;
            default:
                Logger.i(String.format("%s is not a valid command CMP handles.",toRmv.Message));
                break;
        }
    }

    private void gameOver(){
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                AlertDialog.Builder builder = new AlertDialog.Builder(ColorMePretty.this);
                builder.setTitle("Round Over");
                builder.setMessage("Your turn is now over, let's hope you performed well.");
                builder.setPositiveButton("Go back to lobby", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        findViewById(R.id.cmp_gameControls).setVisibility(View.GONE);
                        dialog.dismiss();
                    }
                });
                builder.show();
            }
        });
    }
}
