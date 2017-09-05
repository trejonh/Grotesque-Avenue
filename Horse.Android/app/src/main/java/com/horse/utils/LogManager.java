package com.horse.utils;

import java.io.File;
import java.io.FileFilter;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.PrintWriter;
import java.text.MessageFormat;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;
import java.util.zip.ZipEntry;
import java.util.zip.ZipOutputStream;
import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.net.Uri;
import android.os.Environment;
import android.os.StatFs;
import android.util.Log;

/**
 * The LogManager class allows developers to write notes to a log
 * file named MobileTrace.log. This is commonly used to get diagnostic
 * information from a device in the field.
 * 
 * @since 5.4
 */
public class LogManager {
	private static LogManager mInstance = null;
	private static final int MAX_FILE_SIZE = 4194304;
	private static final String LOG_FILE_NAME = "MobileTrace.log";	
	private static final String zippedLogFile = "Mobile_log.zip";

	/**
	 * The logging level the message should be recorded as.
	 */
	public enum Level { DEBUG, INFO, WARN, ERROR }

	private Level mLevel = Level.ERROR;
	private int mMaxLogs = 10;
	private String mTag;
	private SimpleDateFormat mFormat = new SimpleDateFormat("MM/dd/yy hh:mm:ss a", Locale.US);
	private File mLog;
	private PrintWriter mWriter;
	public static String mSupportEmail = "trejon_house@yahoo.com;houset@msoe.edu"; // in 5.6, get from a metrix app param
	
	private static boolean mLoggingOn = true;
	private static String appDir;

	private void LoggerSetup(String tag, String logFilename, Level level ) {
		try {
	    //close previous
	    if( mWriter != null ) {
	        mWriter.flush();
	        mWriter.close();
	        mWriter = null;
	    }
		}catch(Exception ex){
			Log.e("Logging", ex.getMessage());
		}
	    //open new
		PackageManager m = ((Context)HorseCache.getItem("ApplicationContext")).getPackageManager();
		String s = ((Context)HorseCache.getItem("ApplicationContext")).getPackageName();
		try {
			PackageInfo p = m.getPackageInfo(s, 0);
			appDir = p.applicationInfo.dataDir;
		}catch (PackageManager.NameNotFoundException e) {
			Log.w("Logger", "Error Package name not found ", e);
		}
	    this.mTag = tag;
	    this.mLog = createWriter( logFilename );
	    this.mLevel = level;
		SharedPreferences settings = ((Context)HorseCache.getItem("ApplicationContext")).getSharedPreferences("AppSettings",0);
	    this.mMaxLogs = settings.getInt("MAX_LOGS",10);

	}

	public LogManager(Context context) {
	    LoggerSetup(context.getPackageName(), "MobileTrace.log", Level.ERROR);
	}

	public static LogManager getInstance(Context context) {
	    if (mInstance == null) {
	        mInstance = new LogManager(context);        
	    }
	    return mInstance;
	}
	
	public static LogManager getInstance() {
	    if (mInstance == null) {
	        mInstance = new LogManager();
	    }
	    return mInstance;
	}	

	public LogManager() {
	    LoggerSetup(((Context)HorseCache.getItem("ApplicationContext")).getPackageName(), "MobileTrace.log", Level.ERROR);
	}

	/**
	 * @param logFilename
	 * @return
	 */
	private File createWriter(String logFilename) {
	    try {
	        String state = Environment.getExternalStorageState();
	        if( state.equals(Environment.MEDIA_MOUNTED) ) {
				if(appDir == null)
					return null;
	            File dir = new File( Environment.getExternalStorageDirectory(),appDir);
	            if( !dir.exists() ) {
	                Log.w(mTag, "Could not get log directory: " + dir.getAbsolutePath() );
	                dir.mkdir();	                
	            }
	            File log = new File(dir, logFilename);	            
	            if( log.exists() ) {
	            	if(log.length()>= MAX_FILE_SIZE)
	                rotate( log );
	            }
	            Log.i(mTag, " Opening " + log.getAbsolutePath() );
	            mWriter = new PrintWriter( new FileWriter( log, true ), true );	            
	            return log;
	        } else {
	            Log.w(mTag, "Could not create log file because external storage state was " + state);
	        }
	    } catch( IOException ioe ) {
	        Log.e(mTag, "Failed while opening the log file.", ioe );
	    }

	    return null;
	}

	/**
	 * rotate log files
	 * @param log
	 */
	private void rotate(File log) {
	    int index = log.getName().lastIndexOf('.');
	    if( index < 0 ) index = log.getName().length();
	    String prefix = log.getName().substring(0, index );
	    String extension = log.getName().substring(index);

	    int lastLog = mMaxLogs - 1;
	    File lastLogFile = new File( log.getParentFile(), prefix + "-" + lastLog + extension );
	    if( lastLogFile.exists() ) lastLogFile.delete();

	    for( int i = lastLog; i >= 1; --i ) {
	        String filename = prefix + "-" + i + extension;
	        File l = new File( log.getParentFile(), filename );
	        if( l.exists() ) {
	            File newLog = new File( log.getParentFile(), prefix + "-" + (i+1) + extension );
	            l.renameTo( newLog );
	        }
	    }

	    log.renameTo( new File( log.getParentFile(), prefix + "-1" + extension ) );
	}
	
	/**
	 * Delete the log files
	 */
	public void delete() {
        File dir = new File( Environment.getExternalStorageDirectory(),appDir);
        if( !dir.mkdirs() ) {
            Log.w(mTag, "Could not create log directory: " + dir.getAbsolutePath() );
        }
        File log = new File(dir, LOG_FILE_NAME);		
		
	    int index = log.getName().lastIndexOf('.');
	    if( index < 0 ) index = log.getName().length();
	    String prefix = log.getName().substring(0, index );
	    String extension = log.getName().substring(index);

	    int lastLog = mMaxLogs - 1;
	    File lastLogFile = new File( log.getParentFile(), prefix + "-" + lastLog + extension );
	    if( lastLogFile.exists() ) lastLogFile.delete();
	    if(log.exists()) log.delete();
	    
	    for( int i = lastLog; i >= 1; --i ) {
	        String filename = prefix + "-" + i + extension;
	        File l = new File( log.getParentFile(), filename );
	        if( l.exists() ) {
	        	l.delete();
	        }
	    }
	    
	    LoggerSetup(((Context)HorseCache.getItem("ApplicationContext")).getPackageName(), "MobileTrace.log", Level.ERROR);
	}

	public Level getLevel() {
	    return mLevel;
	}

	public void setMaxLogs() {
		File path = Environment.getDataDirectory();
        StatFs dataStats = new StatFs(path.getPath());
        long blockSize = dataStats.getBlockSize();
        long availableBlocks = dataStats.getAvailableBlocks();
        long availableMB = (blockSize * availableBlocks) / 1024 / 1024;
        
        // logs are 4 MB in size ... use 1/3 of available space (max out at 400 MB)
        int maxLogs = (int) availableMB / 4 / 3;
        if (maxLogs > 100) {
        	maxLogs = 100;
        }
        
        this.mMaxLogs = maxLogs;
	}
	
	public void setLevel(Level level) {
	    this.mLevel = level;
	}

	public boolean isLoggable( Level level ) {
	    return level.ordinal() >= this.mLevel.ordinal();
	}
	
	public static void setLoggingOn(boolean mLoggingOn) {
		LogManager.mLoggingOn = mLoggingOn;
	}

	public static boolean isLoggingOn() {
		return mLoggingOn;
	}	
	
	/**
	 * @param message
	 * @param parameters
	 */
	public void debug( String message, Object... parameters ) {
		if(message == null || message.length() == 0)
			return;
		
	    if( parameters != null && parameters.length > 0 ) {
	        Log.d( mTag, MessageFormat.format( message, parameters ) );
	        log( Level.DEBUG, message, parameters );
	    } else {
	        Log.d( mTag, message );
	        log( Level.DEBUG, message);
	    }	    
	}

	/**
	 * @param message
	 * @param parameters
	 */
	public void info( String message, Object... parameters ) {
		if(message == null || message.length() == 0)
			return;
		
	    if( parameters != null && parameters.length > 0 ) {
	    	Log.i( mTag, MessageFormat.format( message, parameters ) );
	        log( Level.INFO, message, parameters );
	    } else {
	    	Log.i( mTag, message );
	        log( Level.INFO, message );
	    }	    
	}
	
	/**
	 * @param message
	 * @param parameters
	 */
	public void warn( String message, Object... parameters ) {
		if(message == null || message.length() == 0)
			return;
		
	    if( parameters != null && parameters.length > 0 ) {
	        Log.w( mTag, MessageFormat.format( message, parameters ) );
	        log( Level.WARN, message, parameters );
	    } else {
	        Log.w( mTag, message );
	        log( Level.WARN, message);
	    }	    
	}

	/**
	 * @param message
	 * @param parameters
	 */
	public void error( String message, Object... parameters ) {
		if(message == null || message.length() == 0)
			return;
		
	    if( parameters != null && parameters.length > 0 ) {
	        Log.e( mTag, MessageFormat.format( message, parameters ) );
	        log( Level.ERROR, message, parameters );
	    } else {
	        Log.e( mTag, message );
	        log( Level.ERROR, message);
	    }	    
	}

	/**
	 * @param throwable
	 */
	public void error(Throwable throwable) {
	    String message = Log.getStackTraceString( throwable );
	    
		if(message == null || message.length() == 0)
			return;
	    
	    Log.e( mTag, message, throwable );
	    log( Level.ERROR, message );
	}

	/**
	 * Close the file writer 
	 */
	public void close() {
		try {
		    if( mWriter != null ) {
		        mWriter.flush();
		        mWriter.close();
		        mWriter = null;
		    }
		}catch(Exception ex){
			Log.e( mTag, ex.getMessage());
		}
	}

	/**
	 * @param level
	 * @param message
	 * @param parameters
	 */
	private void log( Level level, String message, Object... parameters ) {
        if(mLog ==null)
            return;
		if(this.mLog.length()>= MAX_FILE_SIZE) {
            rotate( this.mLog );
            this.mLog = createWriter(LogManager.LOG_FILE_NAME );
		}
		
	    if( mWriter != null && isLoggingOn() && isLoggable(level) ) {
	        Date date = new Date();	        
	        try {
		        mWriter.print(mFormat.format(date) );	        
		        mWriter.print( "|" );
		        mWriter.print( level.toString() );
		        mWriter.print( "|" );
		        mWriter.print( getMethodInfo(4));
		        mWriter.print( "|" );
		        mWriter.print( Thread.currentThread().getName() );
		        mWriter.print( "|" );
		        if( parameters != null && parameters.length > 0 ) {
		            mWriter.println(MessageFormat.format( message+"\r\n", parameters ) );
		        } else {
		            mWriter.println(message+"\r\n");
		        }
	        }catch(Exception ex){
	        	Log.e( mTag, ex.getMessage());
	        }
	    }
	    else if( mWriter == null && isLoggingOn() && isLoggable(level)){
	    	this.mLog = createWriter(LogManager.LOG_FILE_NAME );
	    	
	        Date date = new Date();	        
	        try {
		        mWriter.print(mFormat.format(date) );	        
		        mWriter.print( "|" );
		        mWriter.print( level.toString() );
		        mWriter.print( "|" );
		        mWriter.print( getMethodInfo(4));
		        mWriter.print( "|" );
		        mWriter.print( Thread.currentThread().getName() );
		        mWriter.print( "|" );
		        if( parameters != null && parameters.length > 0 ) {
		            mWriter.println(MessageFormat.format( message+"\r\n", parameters ) );
		        } else {
		            mWriter.println(message+"\r\n");
		        }
	        }catch(Exception ex){
	        	Log.e( mTag, ex.getMessage());
	        }
	    }
	}

	/**
	 * @param context
	 * @throws IOException
	 */
	public void sendEmail(Context context) throws IOException {
		sendEmail(context, mSupportEmail, false);
	}
	
	/**
	 * @param context
	 * @param fatalError
	 * @throws IOException
	 */
	public void sendEmail(Context context, boolean fatalError) throws IOException {
		sendEmail(context, mSupportEmail, fatalError);
	}
	
	/**
	 * @param context
	 * @param emailAddress
	 * @param fatalError
	 * @throws IOException
	 */
	public void sendEmail(Context context, String emailAddress, boolean fatalError) throws IOException {
		close();
		
	    File[] logs = mLog.getParentFile().listFiles( 
	    new FileFilter() {
	        @Override
	        public boolean accept(File pathname) {
	            return pathname.getName().endsWith(".log");
	        }
	    });

	    File temp = zipLogFiles(logs);
	    String[] mailto = { emailAddress };
	    Intent sendIntent = new Intent(Intent.ACTION_SEND);
	    sendIntent.setType("application/zip");
	    sendIntent.putExtra(Intent.EXTRA_EMAIL, mailto);
	    sendIntent.putExtra(Intent.EXTRA_STREAM, Uri.fromFile(temp) );	    
	    sendIntent.setType("text/plain");
	    if (fatalError) {
		    sendIntent.putExtra(Intent.EXTRA_SUBJECT, mTag + " (Fatal): Log File Attached");
		    sendIntent.putExtra(Intent.EXTRA_TEXT, "A user has experienced a fatal error and has attached a log containing relevant information.");
	    	Intent newTaskIntent = Intent.createChooser(sendIntent, "Fatal Error: Send Logs to Support");
	    	newTaskIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
	    	context.startActivity(newTaskIntent);
	    } else {
		    sendIntent.putExtra(Intent.EXTRA_SUBJECT, mTag + " : Log File Attached");
		    sendIntent.putExtra(Intent.EXTRA_TEXT, "A user has requested you look at some logs.");
	    	context.startActivity(Intent.createChooser(sendIntent, "Send Logs To Support"));
	    }	    	        
	}	

	/**
	 * @param logs
	 * @return
	 * @throws IOException
	 */
	private File zipLogFiles(File[] logs) throws IOException {
		File dir = new File( Environment.getExternalStorageDirectory(),appDir);		
		File zipfile = new File(dir, zippedLogFile);	    		
		
	    ZipOutputStream stream = new ZipOutputStream( new FileOutputStream(zipfile) );
	    try {
	        for( File log : logs ) {
	            ZipEntry entry = new ZipEntry( log.getName() );
	            stream.putNextEntry( entry );
	            copy( stream, log );
	            stream.closeEntry();
	        }
	        stream.finish();
	        return zipfile;
	    } finally {
	        stream.close();
	    }
	}

	/**
	 * @param stream
	 * @param file
	 * @throws IOException
	 */
	private void copy(OutputStream stream, File file) throws IOException {
	    InputStream istream = new FileInputStream( file );
	    try {
	        byte[] buffer = new byte[8096];
	        int length = 0;
	        while( (length = istream.read( buffer )) >= 0 ) {
	            stream.write( buffer, 0, length );
	        }
	    } finally {
	        istream.close();
	    }
	}
	
	/**
	 * @param depth
	 * @return
	 */
	public static String getMethodInfo(final int depth)
	{
		String methodInfo = "";
		try {
			final StackTraceElement[] ste = Thread.currentThread().getStackTrace();
			return methodInfo=ste[1 + depth].getClassName()+":"+ste[1 + depth].getMethodName();
		}catch(Exception ex){
			return methodInfo;
		}			    
	}
}