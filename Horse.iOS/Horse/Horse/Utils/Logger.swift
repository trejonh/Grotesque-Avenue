//
//  Logger.swift
//  Horse
//
//  Created by Trejon House on 2/27/18.
//  Copyright © 2018 Trejon House. All rights reserved.
//

import Foundation
import SwiftyBeaver
public class Logger {
    private static let logger = SwiftyBeaver.self
    private static let logFile = NSSearchPathForDirectoriesInDomains(.documentDirectory, .userDomainMask, true)[0]+"/horse_log.log"
    
    public static func initLogger(){
        let console = ConsoleDestination()
        console.format = "$DHH:mm:ss$d $C$L$c: $M"
        let file = FileDestination()
        file.format = "$Dyyyy-MM-dd HH:mm:ss.SSS$d $C$L$c: $M"
        //let documents = NSSearchPathForDirectoriesInDomains(.documentDirectory, .userDomainMask, true)[0]
        file.logFileURL = URL(fileURLWithPath: logFile)
        //let writePath = documents//documents.stringByAppendingPathComponent("file.plist")
        #if DEBUG
            file.asynchronously = false
        #endif
        logger.addDestination(console)
        logger.addDestination(file)
    }
    
    public static func info(message: String){
        logger.info(message)
    }
    
    public static func info(params: Any){
        logger.info(params)
    }
    
    public static func error(message: String, exception: NSError){
        logger.error([message, exception.localizedDescription])
    }
    
    public static func warning(message: Any){
        logger.warning(message)
    }
    
    public static func debug(message: String, params: Any){
        logger.debug([message, params])
    }
    
    public static func getLogs()->NSData{
        do{
            let file = NSData(contentsOfFile: logFile)
            return file!
        }
    }
}
