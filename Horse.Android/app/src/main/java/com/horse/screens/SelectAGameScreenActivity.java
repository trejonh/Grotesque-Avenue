package com.horse.screens;

import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.horse.R;
import com.horse.core.Game;
import com.horse.core.GameAdapter;
import com.horse.core.HorseActivity;
import com.horse.core.Message;
import com.horse.core.MessageType;
import com.horse.core.Player;
import com.horse.core.ServerConnection;
import com.horse.screens.gameScreens.ColorMePretty;
import com.horse.utils.HorseCache;
import com.orhanobut.logger.Logger;

public class SelectAGameScreenActivity extends HorseActivity implements AdapterView.OnItemClickListener {
    private ListView gameList;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if(LobbyScreenActivity._getPlayerList!=null){
            LobbyScreenActivity._getPlayerList.cancel();
            LobbyScreenActivity._getPlayerList = null;
        }
        setContentView(R.layout.activity_select_agame_screen);
        Player myPlayer = Player.getMobileNetworkPlayers().get(HorseCache.getItem("MyDeviceId"));
        if(myPlayer == null){
           Logger.e("My player does not exist in this game");
            Toast.makeText(this, "You are  no longer in the game, try again", Toast.LENGTH_LONG).show();
            startActivity(new Intent(this, WelcomeScreenActivity.class));
            finish();
        }else {
            if(!myPlayer.IsCurrentlyPlaying){
                Player picker = null;
                for(Player p : Player.getMobileNetworkPlayers().values()){
                    if(p.isCurrentlyPlaying()){
                        picker = p;
                        break;
                    }
                }
                findViewById(R.id.waitingForPlayer).setVisibility(View.VISIBLE);
                if(picker != null)
                    ((TextView)findViewById(R.id.waitingForPlayer)).setText("Waiting for "+picker.Name+" to pick a game.");
                findViewById(R.id.waitingForPlayerProgressBar).setVisibility(View.VISIBLE);
                findViewById(R.id.gameListView).setVisibility(View.GONE);
                findViewById(R.id.loading).setVisibility(View.GONE);
                findViewById(R.id.loadingProgressbar).setVisibility(View.GONE);
                waitForSelection();
            }else{
                findViewById(R.id.waitingForPlayer).setVisibility(View.GONE);
                loadGames();
                findViewById(R.id.selectAGameViews).setVisibility(View.VISIBLE);
                findViewById(R.id.loading).setVisibility(View.INVISIBLE);
                findViewById(R.id.loadingProgressbar).setVisibility(View.INVISIBLE);
            }
            ServerConnection.sendTimedMessage(MessageType.CMD+" getplayerlist","getplayerlist",1);
        }

    }

    private void loadGames() {
        GameAdapter gameAdapter = new GameAdapter(this, Game.getGameList());
        gameList = (ListView)findViewById(R.id.gameListView);
        gameList.setAdapter(gameAdapter);
        gameList.setOnItemClickListener(this);
    }

    private void waitForSelection(){
        Thread waitForSelection = new Thread(new Runnable() {
            @Override
            public void run() {
                boolean continueToWait = true;
                Message toRmv = null;
                String screen= "";
                while(continueToWait){
                    synchronized (ServerConnection.getMessages()) {
                        for (Message message : ServerConnection.getMessages()) {
                            if (!message.Type.equals(MessageType.CMD)) continue;
                            if (!message.Message.contains("gotoscreen")) continue;
                            screen = message.Message.substring(message.Message.indexOf(":") + 1);
                            toRmv = message;
                            continueToWait = false;
                            break;
                        }
                    }
                }
                synchronized (ServerConnection.getMessages()){
                    if (!ServerConnection.getMessages().remove(toRmv))
                       Logger.e("Could not delete message");
                }
                switch (screen){
                    case "cmp":
                        ServerConnection.cancelTimedMessage("getplayerlist");
                        startActivity(new Intent(SelectAGameScreenActivity.this, ColorMePretty.class));
                        finish();
                        break;
                    default:
                        Logger.i(screen+" is not a valid screen to navigate to");
                        break;
                }
            }
        });
        waitForSelection.start();
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

    @Override
    public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
        final Game selectedGame = (Game)gameList.getAdapter().getItem(position);
        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setTitle(selectedGame.Title);
        builder.setMessage(selectedGame.DetailedInfo);
        builder.setPositiveButton("Play", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                ServerConnection.sendMessage(MessageType.CMD+" playgame: "+selectedGame.ShortName);
                findViewById(R.id.gameListView).setVisibility(View.GONE);
                findViewById(R.id.selectAGameTextView).setVisibility(View.GONE);
                dialog.dismiss();
                findViewById(R.id.loading).setVisibility(View.VISIBLE);
                findViewById(R.id.loadingProgressbar).setVisibility(View.VISIBLE);
                waitForSelection();
            }
        });
        builder.setNegativeButton("Cancel", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
                return;
            }
        });
        builder.show();
    }
}
