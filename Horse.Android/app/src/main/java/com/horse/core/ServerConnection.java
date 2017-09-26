package com.horse.core;

import android.app.Activity;
import android.os.AsyncTask;
import android.widget.Toast;

import com.horse.screens.LobbyScreenActivity;
import com.horse.utils.HorseCache;
import com.orhanobut.logger.Logger;

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
    private static String _inet;
    private static InetAddress _inetAddr;
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
    private static Queue<Message> MessagesIn;
    private static Queue<String> MessagesOut;
    private static Timer _readMessageTimer;
    private static Thread _sendMessageTimer;
    private static boolean _keepSending;

    /**
     * Sets up our connection parameters
     * @param inet The ip address of the server
     * @param name The name to display to the server
     */
    public ServerConnection(String inet, String name){
        _inet = inet;
        DisplayName = name;
        ServerConnectionTasks = new HashMap<>();
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
        message += " ENDTRANS";
        if(MessagesOut == null){
            synchronized (ServerConnection.class) {
                MessagesOut = new LinkedList<>();
            }
        }
        synchronized (MessagesOut){
            Logger.i(String.format("Adding message to out queue: %s",message));
            MessagesOut.add(message);
        }
    }

    /**
     * Read a message from the server if data is available to be read
     * @return The message received
     */
    public static String readMessage(){
        if(_in == null || _serverConnectionSocket.isClosed()) {
            Logger.w("Connection to server socket is terminated");
            return "";
        }
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
                    Logger.w(String.format("Message was taking to long to receive: %s",sb));
                    return "FAILED";
                }
            }
            String message = sb.toString().replace(" ENDTRANS","");
            String cmd = message.substring(0,4);
            synchronized (getMessages()) {
                if (cmd.equals(MessageType.CMD))
                    getMessages().add(new Message(message.substring(5), MessageType.CMD, new Date()));
                if (cmd.equals(MessageType.DATA))
                    getMessages().add(new Message(message.substring(5), MessageType.DATA, new Date()));
                if (cmd.equals(MessageType.INFO))
                    getMessages().add(new Message(message.substring(5), MessageType.INFO, new Date()));
                if (cmd.equals(MessageType.ERR))
                    getMessages().add(new Message(message.substring(5), MessageType.ERR, new Date()));
            }
            Logger.i(String.format("Received the following message: %s", message));
            return message.substring(cmd.length());
        }catch(IOException ex){
            Logger.e(ex, ex.toString());
        }
        return "";
    }

    public static Message readMessage(String messageType){
        if(MessagesIn == null || MessagesIn.size() == 0)
            return null;
        Message message = null;
        synchronized (getMessages()){
            for (Message mess: getMessages()){
                if(mess.Type.equals(messageType)){
                    message = mess;
                    break;
                }
            }
            if(message != null)
                getMessages().remove(message);
        }
        return message;
    }

    public static Message readMessage(String messageType, String data){
        if(MessagesIn == null || MessagesIn.size() == 0)
            return null;
        Message message = null;
        synchronized (getMessages()){
            for (Message mess: getMessages()){
                if(mess.Type.equals(messageType) && mess.Message.contains(data)){
                    message = mess;
                    break;
                }
            }
            if(message != null)
                getMessages().remove(message);
        }
        return message;
    }

    public static void removeMessage(Message message){
        if(message == null) return;
        synchronized (getMessages()){
            if(!getMessages().contains(message))
                return;
            if(!getMessages().remove(message))
                Logger.e("Failed to remove message");
        }
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
            if(_sendMessageTimer != null)
                _keepSending = false;
            _in.close();
            _out.close();
            _serverConnectionSocket.close();
            Ready = false;
        }catch(IOException  ex) {
            //ex
            Logger.e(ex, ex.toString());
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
        Logger.i("Sending time message: ", name, message);
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
        Logger.i("Canceling message: "+name);
        Timer taskToCancel = ServerConnectionTasks.get(name);
        if(taskToCancel != null)
            taskToCancel.cancel();
    }

    public static Queue<Message> getMessages(){
        if(MessagesIn == null){
            synchronized (ServerConnection.class){
                MessagesIn = new LinkedList<>();
            }
        }
        return MessagesIn;
    }

    private static void sendMessagesOut(){
        while(_keepSending) {
            if (MessagesOut == null || MessagesOut.size() == 0) {
                Logger.i("No messages to send");
                continue;
            }
            if (_out == null || _serverConnectionSocket.isClosed() || !_serverConnectionSocket.isConnected()) {
                Logger.w("The connection to the server is closed");
                continue;
            }
            synchronized (MessagesOut) {
                String message;
                for (; ; ) {
                    message = MessagesOut.poll();
                    if (message == null || message.trim().length() == 0) {
                        Logger.i("Sent all messages");
                        break;
                    }
                    try {
                        Logger.i(String.format("Attempting to send messages to server: %s", message));
                        _out.writeUTF(message);
                    } catch (IOException e) {
                        Logger.e(e.toString());
                    }
                }
            }
        }
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
                _inetAddr = InetAddress.getByName(_inet);
                if(_inetAddr == null)
                    throw new Exception("Bad server address");
                _serverConnectionSocket = new Socket(_inetAddr, _port);
                _serverConnectionSocket.setKeepAlive(true);
            }
            catch (Exception ex){
                //log it
                Logger.e(ex,ex.toString());
                ((Activity)HorseCache.getItem("CurrentScreenActivity")).runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        Toast.makeText((Activity)HorseCache.getItem("CurrentScreenActivity"), "Failed to connect to server try again.", Toast.LENGTH_LONG).show();
                        LobbyScreenActivity.Failure = true;
                        ((Activity) HorseCache.getItem("CurrentScreenActivity")).finish();
                    }
                });
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
                _keepSending = true;
                _sendMessageTimer = new Thread(new Runnable() {
                    @Override
                    public void run() {
                        sendMessagesOut();
                    }
                });
                _sendMessageTimer.start();
                /* new Timer();
                _sendMessageTimer.schedule(new TimerTask() {
                    @Override
                    public void run() {
                        sendMessagesOut();
                    }
                }, 500,500);*/
            }catch (IOException ex){
                //log it
                Logger.e(ex,ex.toString());
            }
        }
    }
}
