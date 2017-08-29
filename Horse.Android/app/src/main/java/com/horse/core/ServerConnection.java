package com.horse.core;

import android.os.AsyncTask;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.net.InetAddress;
import java.net.Socket;

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

    public ServerConnection(InetAddress inet, String name){
        _inet = inet;
        DisplayName = name;
    }

    public static void executeServerConnection(){
        AsyncConnectionTask act = new AsyncConnectionTask();
        act.execute();
    }

    public static void sendMessage(String message){
        if(_out == null || _serverConnectionSocket.isClosed())
            return;
        try{
            _out.writeUTF(message+" ENDTRANS");
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
                int read =_in.read(messageByte);
                sb.append(new String(messageByte, 0, read));
                if(sb.toString().contains("ENDTRANS"))
                    endReached = true;
            }
            return sb.toString().replace(" ENDTRANS","");
        }catch(IOException ex){
            //log it
        }
        return "";
    }

    public static void closeConnection(){
        if(_in == null || _out == null || _serverConnectionSocket.isClosed())
            return;
        try {
            _in.close();
            _out.close();
            _serverConnectionSocket.close();
            Ready = false;
        }catch(IOException  ex){
            //ex
        }
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
            }catch (IOException ex){
                //log it
            }
        }
    }
}
