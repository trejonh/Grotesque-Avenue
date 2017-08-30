package com.horse.core;

import android.os.AsyncTask;
import android.os.Handler;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.net.InetAddress;
import java.net.Socket;
import java.util.Date;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.Queue;
import java.util.Timer;
import java.util.TimerTask;

/**
 * Created by trhous on 8/29/2017.
 */

public class ServerConnection {
    private static InetAddress _inet;
    public static String DisplayName;
    private static Socket _serverConnectionSocket;
    private static DataOutputStream _out;
    private static DataInputStream _in;
    private static final int _port = 54000;
    public static boolean Ready = false;
    public static HashMap<String, Timer> ServerConnectionTasks;
    public static Queue<Message> MessagesIn;
    private static Timer _readMessageTimer;
    public ServerConnection(InetAddress inet, String name){
        _inet = inet;
        DisplayName = name;
        ServerConnectionTasks = new HashMap<>();
        MessagesIn = new LinkedList<>();
    }

    public static void executeServerConnection(){
        AsyncConnectionTask act = new AsyncConnectionTask();
        act.execute();
    }

    public static void sendMessage(String message){
        if(_out == null || _serverConnectionSocket.isClosed())
            return;
        try{
            synchronized (_out) {
                _out.writeUTF(message + " ENDTRANS");
            }
        }catch(IOException ex){
            //log it
        }
    }

    public static String readMessage(){
        if(_in == null || _serverConnectionSocket.isClosed())
            return "";
        try{
            boolean endReached = false;
            byte[] messageByte = new byte[256];
            StringBuilder sb = new StringBuilder();
            while(!endReached){
                int read;
                synchronized (_in) {
                    if(_in.available() <=  0) {
                        endReached = true;
                        continue;
                    }
                    read = _in.read(messageByte);
                }
                sb.append(new String(messageByte, 0, read));
                if(sb.toString().contains("ENDTRANS"))
                    endReached = true;
            }
            String message = sb.toString().replace(" ENDTRANS","");
            String cmd = message.substring(0, message.indexOf(" "));
            if(cmd.equals(MessageType.CMD))
                MessagesIn.add(new Message(message.substring(cmd.length()),MessageType.CMD, new Date()));
            if(cmd.equals(MessageType.DATA))
                MessagesIn.add(new Message(message.substring(cmd.length()),MessageType.DATA, new Date()));
            if(cmd.equals(MessageType.INFO))
                MessagesIn.add(new Message(message.substring(cmd.length()),MessageType.INFO, new Date()));
            return message.substring(cmd.length());
        }catch(IOException ex){
            //log it
        }
        return "";
    }

    public static void closeConnection(){
        if(_in == null || _out == null || _serverConnectionSocket.isClosed())
            return;
        try {
            for (Timer timer:ServerConnectionTasks.values()) {
                if(timer != null)
                    timer.cancel();
            }
            if(_readMessageTimer != null)
                _readMessageTimer.cancel();
            _in.close();
            _out.close();
            _serverConnectionSocket.close();
            Ready = false;
        }catch(IOException  ex) {
            //ex
        }
    }

    public static void sendTimedMessage(final String message, String name, int seconds){
        Timer timer = new Timer();
        timer.scheduleAtFixedRate(new TimerTask() {
            @Override
            public void run() {
                sendMessage(message);
            }
        },0, seconds * 1000);
        Timer currentTask = ServerConnectionTasks.put(name,timer);
        if(currentTask != null)
            currentTask.cancel();
    }

    public static void cancelTimedMessage(String name){
        Timer taskToCancel = ServerConnectionTasks.get(name);
        if(taskToCancel != null)
            taskToCancel.cancel();
    }


    private static class AsyncConnectionTask extends AsyncTask<Void, Void, Void>{

        @Override
        protected Void doInBackground(Void... voids) {
            if(_serverConnectionSocket != null && _serverConnectionSocket.isClosed() == false)
                return null;
            try {
                _serverConnectionSocket = new Socket(_inet, _port);
                _serverConnectionSocket.setKeepAlive(true);
            }
            catch (IOException ex){
                //log it
            }
            return null;
        }

        @Override
        protected  void onPostExecute(Void result){
            super.onPostExecute(result);
            if(_serverConnectionSocket == null || _serverConnectionSocket.isClosed())
                return;
            try{
                _out = new DataOutputStream(_serverConnectionSocket.getOutputStream());
                _in = new DataInputStream(_serverConnectionSocket.getInputStream());
                Ready = true;
                _readMessageTimer = new Timer();
                _readMessageTimer.schedule(new TimerTask() {
                    @Override
                    public void run() {
                        readMessage();
                    }
                }, 1000,1000);
            }catch (IOException ex){
                //log it
            }
        }
    }

    private static class Message{
        public String Message;
        public String Type;
        public Date ReceivedDate;

        public Message(String message, String type, Date receivedDate) {
            Message = message;
            Type = type;
            ReceivedDate = receivedDate;
        }
    }
}
