package com.horse.screens.gameScreens;

import com.horse.core.HorseActivity;
import com.horse.core.Message;
import com.horse.core.MessageType;
import com.horse.core.ServerConnection;

/**
 * Created by yorel56 on 9/27/2017.
 */

public abstract class GameActivity extends HorseActivity implements Runnable {
    private Thread _messageConsumerThread;
    private boolean _run;

    @Override
    public void onStart(){
        super.onStart();
        if(_messageConsumerThread == null){
            _messageConsumerThread = new Thread(this);
            _run = true;
            _messageConsumerThread.start();
            return;
        }
        if(!_messageConsumerThread.getState().equals(Thread.State.TERMINATED)){
            _run = false;
            _messageConsumerThread = new Thread(this);
            _messageConsumerThread.start();
        }
    }

    @Override
    public void onStop(){
        super.onStop();
        _run = false;
    }

    @Override
    public void onPause(){
        super.onPause();
        _run = false;
    }

    @Override
    public void onResume(){
        super.onResume();
        if(_messageConsumerThread == null){
            _messageConsumerThread = new Thread(this);
            _run = true;
            _messageConsumerThread.start();
            return;
        }
        if(!_messageConsumerThread.getState().equals(Thread.State.TERMINATED)){
            _run = false;
            _messageConsumerThread = new Thread(this);
            _messageConsumerThread.start();
        }
    }

    @Override
    public void run() {
        while (_run){
            handleIncomingMessages();
        }
    }

    public abstract void handleIncomingMessages();
}
