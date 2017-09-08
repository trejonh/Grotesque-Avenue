package com.horse.screens;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ListView;
import android.widget.Toast;

import com.horse.R;
import com.horse.core.Game;
import com.horse.core.GameAdapter;
import com.horse.core.HorseActivity;
import com.horse.core.MessageType;
import com.horse.core.Player;
import com.horse.core.ServerConnection;
import com.horse.utils.HorseCache;
import com.horse.utils.LogManager;

import java.util.ArrayList;

public class SelectAGameScreenActivity extends HorseActivity implements AdapterView.OnItemClickListener {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_select_agame_screen);
        Player myPlayer = Player.getMobileNetworkPlayers().get(HorseCache.getItem("MyDeviceId"));
        if(myPlayer == null){
            LogManager.getInstance().error("My player does not exist in this game");
            Toast.makeText(this, "You are  no longer in the game, try again", Toast.LENGTH_LONG).show();
            startActivity(new Intent(this, WelcomeScreenActivity.class));
            finish();
        }else {
            if(!myPlayer.IsCurrentlyPlaying){
                findViewById(R.id.waitingForPlayer).setVisibility(View.VISIBLE);
                findViewById(R.id.waitingForPlayerProgressBar).setVisibility(View.VISIBLE);
                findViewById(R.id.gameListView).setVisibility(View.GONE);
            }else{
                findViewById(R.id.waitingForPlayer).setVisibility(View.GONE);
                findViewById(R.id.waitingForPlayerProgressBar).setVisibility(View.GONE);
                loadGames();
                findViewById(R.id.gameListView).setVisibility(View.VISIBLE);
            }
            ServerConnection.sendTimedMessage(MessageType.CMD+" getplayerlist","getplayerlist",1);
        }

    }

    private void loadGames() {
        GameAdapter gameAdapter = new GameAdapter(this, Game.getGameList());
        ListView lv = (ListView)findViewById(R.id.gameListView);
        lv.setAdapter(gameAdapter);
        lv.setOnItemClickListener(this);
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
        super.onBackPressed();;
        ServerConnection.closeConnection();
    }

    @Override
    public void onItemClick(AdapterView<?> parent, View view, int position, long id) {

    }
}
