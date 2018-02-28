//
//  HorseCache.swift
//  Horse
//
//  Created by Trejon House on 2/27/18.
//  Copyright © 2018 Trejon House. All rights reserved.
//

import Foundation

public class HorseCache{
    private static var HORSE_CACHE = [String: Any]()
    
    public static func addItem(key: String, val: Any){
        HORSE_CACHE[key] = val
    }
    
    public static func getItem(key: String) -> Any?{
        return HORSE_CACHE.isEmpty ? nil : HORSE_CACHE[key] as Any
    }
    
    public static func removeItem(key: String) -> Any?{
        let toRet = HORSE_CACHE.isEmpty ? nil : HORSE_CACHE[key] as Any
        HORSE_CACHE[key] = nil
        return toRet
    }
}
