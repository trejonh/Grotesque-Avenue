package com.horse.core;

import android.os.AsyncTask;
import android.os.Handler;

import com.horse.utils.LogManager;

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

/**
 * @author TreJon House
 * @version 0.0.1
 * The source of  the connection with the server and mobile client
 * Is used to send and retrieve messages to the server.
 */
public class ServerConnection {
    private static InetAddress _inet;
    /**
     * The name displayed to the server
     */
    public static String DisplayName;
    private static Socket _serverConnectionSocket;
    private static DataOutputStream _out;
    private static DataInputStream _in;
    private static final int _port = 54000;
    public static boolean Ready = false;
    /**
     * Cached tasks to run against the server
     */
    public static HashMap<String, Timer> ServerConnectionTasks;
    /**
     * Message in queue
     */
    public static Queue<Message> MessagesIn;
    private static Timer _readMessageTimer;

    /**
     * Sets up our connection parameters
     * @param inet The ip address of the server
     * @param name The name to display to the server
     */
    public ServerConnection(InetAddress inet, String name){
        _inet = inet;
        DisplayName = name;
        ServerConnectionTasks = new HashMap<>();
        MessagesIn = new LinkedList<>();
    }

    /**
     * Attempt to connect to the server
     */
    public static void executeServerConnection(){
        AsyncConnectionTask act = new AsyncConnectionTask();
        act.execute();
    }

    /**
     * Send a message to the server
     * @param message The message to send
     */
    public static void sendMessage(String message){
        if(_out == null || _serverConnectionSocket.isClosed())
            return;
        try{
            synchronized (_out) {
                _out.writeUTF(message + " ENDTRANS");
            }
        }catch(IOException ex){
            //log it
            LogManager.getInstance().error(ex);
        }
    }

    /**
     * Read a message from the server if data is available to be read
     * @return The message received
     */
    public static String readMessage(){
        if(_in == null || _serverConnectionSocket.isClosed())
            return "";
        try{
            boolean endReached = false;
            byte[] messageByte = new byte[256];
            StringBuilder sb = new StringBuilder();
            long startTime = System.currentTimeMillis();
            while(!endReached){
                int read;
                synchronized (_in) {
                    if(_in.available() <=  0) {
                        continue;
                    }
                    read = _in.read(messageByte);
                }
                sb.append(new String(messageByte, 0, read));
                if(sb.toString().contains("ENDTRANS"))
                    endReached = true;
                else if(!sb.toString().contains("ENDTRANS") && (System.currentTimeMillis()-startTime)/1000 >= 5){
                    //taking too long to retrieve message exiting early
                    LogManager.getInstance().warn("Message was taking to long to receive: ", sb.toString());
                    return "FAILED";
                }
            }
            String message = sb.toString().replace(" ENDTRANS","");
            String cmd = message.substring(0,4);
            if(cmd.equals(MessageType.CMD))
                MessagesIn.add(new Message(message.substring(5),MessageType.CMD, new Date()));
            if(cmd.equals(MessageType.DATA))
                MessagesIn.add(new Message(message.substring(5),MessageType.DATA, new Date()));
            if(cmd.equals(MessageType.INFO))
                MessagesIn.add(new Message(message.substring(5),MessageType.INFO, new Date()));
            if(cmd.equals(MessageType.ERR))
                MessagesIn.add(new Message(message.substring(5),MessageType.ERR, new Date()));
            return message.substring(cmd.length());
        }catch(IOException ex){
            LogManager.getInstance().error(ex);
        }
        return "";
    }

    /**
     * Close any open connections to the server and stop running tasks
     */
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
            LogManager.getInstance().error(ex);
        }
    }

    /**
     * Send a repeated message to the server
     * @param message The message to send
     * @param name The name of the message
     * @param seconds How often to repeat the message
     */
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

    /**
     * Cancel a repeated message
     * @param name Name of the message to stop sending
     */
    public static void cancelTimedMessage(String name){
        Timer taskToCancel = ServerConnectionTasks.get(name);
        if(taskToCancel != null)
            taskToCancel.cancel();
    }

    /**
     * @author TreJon House
     * @version 1.0
     * Asynchronously connects to the server and if a connection is made then setup data streams
     */
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
                LogManager.getInstance().error(ex);
            }
        }
    }
}
