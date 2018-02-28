//
//  UIHelper.swift
//  Horse
//
//  Created by Trejon House on 2/28/18.
//  Copyright © 2018 Trejon House. All rights reserved.
//

import Foundation
import UIKit

public class UIHelper{
    public struct iPhoneDisplay{
        var x = 0;
        var y = 0;
        var nvHeightPortrait = 0;
        var nvHeightLandscape = 0;
    }
    
    //Screen Definitions
    private static let IS_IPAD:Bool = (UIDevice.current.userInterfaceIdiom == .pad)
    private static let IS_IPHONE:Bool = UIDevice.current.userInterfaceIdiom == .phone
    private static let IS_RETINA:Bool = (UIScreen.main.scale >= 2.0)
    
    private static let SCREEN_WIDTH = UIScreen.main.bounds.size.width
    private static let SCREEN_HEIGHT = UIScreen.main.bounds.size.height
    private static let SCREEN_MAX_LENGTH = (max(UIScreen.main.bounds.size.width, UIScreen.main.bounds.size.height))
    private static let SCREEN_MIN_LENGTH = (min(UIScreen.main.bounds.size.width, UIScreen.main.bounds.size.height))
    
    private static let IS_IPHONE_4_OR_LESS = (IS_IPHONE && SCREEN_MAX_LENGTH < 568.0)
    private static let IS_IPHONE_5 = (IS_IPHONE && SCREEN_MAX_LENGTH == 568.0)
    private static let IS_IPHONE_6 = (IS_IPHONE && SCREEN_MAX_LENGTH == 667.0)
    private static let IS_IPHONE_PLUS = (IS_IPHONE && SCREEN_MAX_LENGTH == 736.0)
    private static let IS_IPHONE_X  = (IS_IPHONE && SCREEN_MAX_LENGTH == 812.0)
    private static let  iPhoneX = iPhoneDisplay(x: 375, y: 812, nvHeightPortrait: 88, nvHeightLandscape: 44)
    private static let iPhonePlus = iPhoneDisplay(x: 414, y: 736, nvHeightPortrait: 88, nvHeightLandscape: 44)
    private static let iPhone6 = iPhoneDisplay(x: 375, y: 667, nvHeightPortrait: 88, nvHeightLandscape: 44)
    private static let iPhone5 = iPhoneDisplay(x: 320, y: 568, nvHeightPortrait: 88, nvHeightLandscape: 44)
    
    private static var alert:UIAlertController? = nil
    private static var loadingIndicator: UIActivityIndicatorView? = nil
    public static func showLoadingDialog(message: String, vc: UIViewController){
        if(alert == nil){
            self.alert = UIAlertController(title: nil, message: message, preferredStyle: .alert)
        }else{
            dissmissLoadingDialog()
            self.alert?.message = message
        }
        self.loadingIndicator = UIActivityIndicatorView(frame: CGRect(x: 10, y: 5, width: 50, height: 50))
        self.loadingIndicator?.hidesWhenStopped = true
        self.loadingIndicator?.activityIndicatorViewStyle = UIActivityIndicatorViewStyle.gray
        self.loadingIndicator?.startAnimating();
        alert?.view.addSubview(loadingIndicator!)
        HorseCache.addItem(key: "LoadingViewController", val: vc)
        vc.present(alert!, animated: true, completion: nil)
    }
    
    public static func dissmissLoadingDialog(){
        let vc = HorseCache.getItem(key: "LoadingViewController") as! ViewController
        self.loadingIndicator?.removeFromSuperview()
        self.loadingIndicator = nil
        vc.dismiss(animated: false, completion: nil)
    }
    
    public static func  getIphoneDisplay() -> iPhoneDisplay{
        if(IS_IPHONE_X){
            return iPhoneX;
        }
        if(IS_IPHONE_PLUS){
            return iPhonePlus;
        }
        if(IS_IPHONE_6){
            return iPhone6;
        }
        return iPhone5;
    }
}
