package com.horse.utils;

import android.content.Context;
import android.os.Environment;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;

/**
 * Created by yorel56 on 9/10/2017.
 */

public class HorseLogHandler extends Handler {
    private int MAX_FILE_SIZE;
    private String folder;
    public HorseLogHandler(Looper looper){
        super(looper);
        MAX_FILE_SIZE = 500*1024;
        folder = Environment.getExternalStorageDirectory().getPath()+System.getProperty("file.separator")+((Context)HorseCache.getItem("ApplicationContext")).getPackageName();
    }

    @Override
    public void handleMessage(Message msg) {
        String content = (String) msg.obj;

        FileWriter fileWriter = null;
        File logFile = getLogFile(folder, "horse_log");

        try {
            fileWriter = new FileWriter(logFile, true);

            writeLog(fileWriter, content);

            fileWriter.flush();
            fileWriter.close();
        } catch (IOException e) {
            if (fileWriter != null) {
                try {
                    fileWriter.flush();
                    fileWriter.close();
                } catch (IOException e1) { /* fail silently */ }
            }
        }
    }

    /**
     * This is always called on a single background thread.
     * Implementing classes must ONLY write to the fileWriter and nothing more.
     * The abstract class takes care of everything else including close the stream and catching IOException
     *
     * @param fileWriter an instance of FileWriter already initialised to the correct file
     */
    private void writeLog(FileWriter fileWriter, String content) throws IOException {
        fileWriter.append(content);
    }

    private File getLogFile(String folderName, String fileName) {

        File folder = new File(folderName);
        if (!folder.exists()) {
            folder.mkdirs();
        }

        int newFileCount = 0;
        File newFile;
        File existingFile = null;

        newFile = new File(folder, String.format("%s_%s.csv", fileName, newFileCount));
        while (newFile.exists()) {
            existingFile = newFile;
            newFileCount++;
            newFile = new File(folder, String.format("%s_%s.csv", fileName, newFileCount));
        }
        HorseCache.addItem("LogFile", newFile.getAbsolutePath());
        if (existingFile != null) {
            if (existingFile.length() >= MAX_FILE_SIZE) {
                return newFile;
            }
            return existingFile;
        }

        return newFile;
    }
}
