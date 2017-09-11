package com.horse.screens.gameScreens;

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

public class ColorMePretty extends HorseActivity implements View.OnClickListener {
    private Button redBtn;
    private Button blackBtn;
    private Button blueBtn;
    private Button orangeBtn;
    private Button purpleBtn;
    private Button greenBtn;
    private Button yellowBtn;
    private Button brownBtn;
    private Button readyBtn;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_color_me_pretty);
        Player myPlayer = Player.getMobileNetworkPlayers().get(HorseCache.getItem("MyDeviceId"));
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
        if(myPlayer.IsCurrentlyPlaying){
            findViewById(R.id.cmp_gameControls).setVisibility(View.VISIBLE);
        }else{
            waitForReadyCmd();
        }
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
                        readyBtn.setVisibility(View.VISIBLE);
                    }
                });
            }
        });
        waitForReadyMessage.start();
    }

    @Override
    public void onClick(View v) {
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
}
