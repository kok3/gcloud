//
//  JZUDPClient.m
//  ILbcDemo
//
//  Created by 周振宇 on 15/11/29.
//  Copyright (c) 2015年 Jared. All rights reserved.
//

#import "JZUDPClient.h"
#import "GCDAsyncUdpSocket.h"
#import "JZAudioPlayer.h"
#import "JZAudioManager.h"
#import "UnityAppController.h"

@interface JZUDPClient()<GCDAsyncUdpSocketDelegate>
{
    GCDAsyncUdpSocket* _udpSocket;
    dispatch_queue_t _udpQueue;
    NSMutableString *log;
    uint16_t bingPort;
}
@end


#define FORMAT(format, ...) [NSString stringWithFormat:(format), ##__VA_ARGS__]
#define  UDPPort 7410
#define UDPHost @"192.168.0.109"
@implementation JZUDPClient

+ (JZUDPClient *)sharedInstance
{
    static JZUDPClient *instance = nil;
    
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        instance = [[self alloc] init];
    });
    
    return instance;
}

- (instancetype)init
{
    self = [super init];
    if (self) {
        [self setup];
    }
    return self;
}

- (void)setup
{
    _udpQueue = dispatch_queue_create("com.ztgame.qiuqiu.udpqueue", DISPATCH_QUEUE_SERIAL);
    _udpSocket = [[GCDAsyncUdpSocket alloc] initWithDelegate:self delegateQueue:_udpQueue];
    
    //bingPort = 7410;
    bingPort = 5757;
    if([JZAudioManager sharedInstance]._isNewSetUDPIP == false)
    {
        [self bingToPort];
    }
    
    NSLog(@"Ready");
}

- (void)bingToPort
{
    if([JZAudioManager sharedInstance]._isNewSetUDPIP == true)
    {
        _udpSocket = [[GCDAsyncUdpSocket alloc] initWithDelegate:self delegateQueue:_udpQueue];
    }
    
    NSError *error;
    for (int i=0; i<100; i++) {
        
        bool result = true;
        if(![_udpSocket bindToPort:bingPort error:&error])
        {
            NSLog(@"Error binding: %@", error);
            result = false;
        }
        
        bingPort++;
        
        if(result)
        {
            break;
        }
    }
    

    if(![_udpSocket beginReceiving:&error])
    {
        NSLog(@"Error receiving: %@", error);
    }
    
}

- (void)send:(NSData *)data
{
    [_udpSocket sendData:data toHost:[JZAudioManager sharedInstance].host port:[JZAudioManager sharedInstance].port withTimeout:-1 tag:0];
    
    NSLog(@"SENT : %@", data);
    
}

- (void)udpSocket:(GCDAsyncUdpSocket *)sock didSendDataWithTag:(long)tag
{
    // You could add checks here
//    NSLog(@"send data succ:%ld", tag);
}

- (void)udpSocket:(GCDAsyncUdpSocket *)sock didNotSendDataWithTag:(long)tag dueToError:(NSError *)error
{
    // You could add checks here
}

- (void)udpSocketDidClose:(GCDAsyncUdpSocket *)sock withError:(NSError *)error
{
    /*
    NSError *closeError;
    if (![_udpSocket bindToPort:UDPPort error:&closeError])
    {
        NSLog(@"Error binding: %@", closeError);
        return;
    }
    if (![_udpSocket beginReceiving:&closeError])
    {
        NSLog(@"Error receiving: %@", closeError);
        return;
    }
    */
    [self bingToPort];
    NSLog(@"Ready");
}


- (void)udpSocket:(GCDAsyncUdpSocket *)sock didReceiveData:(NSData *)data
      fromAddress:(NSData *)address
withFilterContext:(id)filterContext
{
//        NSString *host = nil;
//        uint16_t port = 0;
//        [GCDAsyncUdpSocket getHost:&host port:&port fromAddress:address];
    
//    NSLog(@"RECV: %@", data);
    uint8_t* bytes = [data bytes];
    BOOL isSamples2 = (bytes[0] == 4);
    BOOL isVoiceAnchor = (bytes[0] == 7);
    if (self.delegate && isSamples2) {
        bytes += 1;
        NSData *tData = [NSData dataWithBytes:bytes length:(data.length - 1)];
        [self.delegate onRecvData:tData];
//        NSLog(@"sample is good:%d", tData);
    }else if(self.delegate && isVoiceAnchor){
        if ([JZAudioManager sharedInstance]._isRecordSpeak == false) {
            bytes += 1;
            NSData *tData = [NSData dataWithBytes:bytes length:(data.length - 1)];
            [self.delegate onRecvData:tData];
        }
    }else{
        NSString *str = @"1";
        UnitySendMessage("Main Camera", "MicReceive", [str UTF8String]);
    }
}

@end
