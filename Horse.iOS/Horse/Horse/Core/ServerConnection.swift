//
//  ServerConnection.swift
//  Horse
//
//  Created by Trejon House on 2/28/18.
//  Copyright © 2018 Trejon House. All rights reserved.
//

import Foundation
import Socket

public class ServerConnection{
    private static var inet:String
    private static var displayName:String
    private static var InetAddress:String
    private static var ServerSocket:Socket
    private static let port = 5400
    public static var Ready:Bool = false
    private static var MessagesOut =  Queue<String>()
    private static var MessagesIn =  Queue<String>()
    
    public init(_inet: String, name: String){
        inet = _inet
        displayName = name
    }
}
