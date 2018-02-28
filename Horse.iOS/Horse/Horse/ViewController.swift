//
//  ViewController.swift
//  Horse
//
//  Created by Trejon House on 2/26/18.
//  Copyright © 2018 Trejon House. All rights reserved.
//

import UIKit
import MessageUI

class ViewController: UIViewController, MFMailComposeViewControllerDelegate, UITextFieldDelegate{

    @IBOutlet weak var serverName: UITextField!
    @IBOutlet weak var playerName: UITextField!
    @IBOutlet weak var scrollView: UIScrollView!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view, typically from a nib.
        serverName.delegate = self
        playerName.delegate = self
        NotificationCenter.default.addObserver(self, selector: #selector(keyboardWillShow), name:NSNotification.Name.UIKeyboardWillShow, object: nil)
        NotificationCenter.default.addObserver(self, selector: #selector(keyboardWillHide), name:NSNotification.Name.UIKeyboardWillHide, object: nil)
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }
    
    func textFieldShouldClear(_ textField: UITextField) -> Bool {
        textField.resignFirstResponder()
        return true
    }

    func textFieldShouldReturn(_ textField: UITextField) -> Bool {
        self.view.endEditing(true)
        return false
    }
    
    @objc func keyboardWillShow(notification:NSNotification){
        
        var userInfo = notification.userInfo!
        var keyboardFrame:CGRect = (userInfo[UIKeyboardFrameBeginUserInfoKey] as! NSValue).cgRectValue
        keyboardFrame = self.view.convert(keyboardFrame, from: nil)
        
        var contentInset:UIEdgeInsets = self.scrollView.contentInset
        contentInset.bottom = keyboardFrame.size.height
        scrollView.contentInset = contentInset
    }
    
    @objc func keyboardWillHide(notification:NSNotification){
        let contentInset:UIEdgeInsets = UIEdgeInsets.zero
        scrollView.contentInset = contentInset
    }

    @IBAction func joinGame(_ sender: UIButton) {
        Logger.info(message: "button pressed")
        Logger.info(params: [serverName.text, playerName.text])
        //sendLogs()
        UIHelper.showLoadingDialog(message: "Im loading", vc: self)
    }
    
    
    public func sendLogs(){
        if !MFMailComposeViewController.canSendMail() {
            Logger.warning(message: "Mail services are not available")
            return
        }
        sendEmail()
    }
    
    private func sendEmail() {
        let composeVC = MFMailComposeViewController()
        composeVC.mailComposeDelegate = self
        // Configure the fields of the interface.
        composeVC.setToRecipients(["coding.with.casa@gmail.com"])
        composeVC.setSubject("Horse iOS Logs")
        composeVC.setMessageBody("Hello this is my message body!", isHTML: false)
        composeVC.addAttachmentData(Logger.getLogs() as Data, mimeType: "text/plain", fileName: "horse_log")
        // Present the view controller modally.
        self.present(composeVC, animated: true, completion: nil)
    }
    
    func mailComposeController(controller: MFMailComposeViewController, didFinishWithResult result: MFMailComposeResult, error: NSError?) {
        
        // Dismiss the mail compose view controller.
        controller.dismiss(animated: true, completion: nil)
    }
    
}

