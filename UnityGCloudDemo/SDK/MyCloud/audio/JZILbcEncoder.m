//
//  JZILbcEncoder.m
//  ILbcDemo
//
//  Created by 周振宇 on 15/11/28.
//  Copyright (c) 2015年 Jared. All rights reserved.
//

#import "JZILbcEncoder.h"
#import "audiowrapper.h"
#import "JZAudioController.h"
#import "JZUDPClient.h"J
#import "JZAudioPlayer.h"
#import "JZAudioManager.h"


#define EncodedBufferSize 200;
@implementation JZILbcEncoder

- (instancetype)init
{
    self = [super init];
    if (self) {
        initiLbc(30);
    }
    return self;
}

- (void)onGetAudioData:(uint8_t *)samples size:(size_t)size
{
//    NSLog(@"encode sample number:%zu", size);
    uint8_t * bdata = (uint8_t *)malloc(200);
    int encodeSize = encode(samples, bdata);
//    NSLog(@"encoded data size:%d", encodeSize);
    
    
 
    //[udpSocket sendData:data toHost:host port:port withTimeout:-1 tag:tag];
    //tag++;
    char msgType = MicSamples2;
    if ([JZAudioManager sharedInstance]._isLiving == true) {
        msgType = VoiceAnchor;
    }
    UInt64 playerId = [JZAudioManager sharedInstance].playerId;
    NSMutableData *data = [[NSMutableData alloc] init];
    [data appendBytes:&msgType length:1];  //添加一个消息类型头， 1byte
    //[data appendBytes:&playerId length:8]; //添加一个playerid 头部  8byte
    [data appendBytes:bdata length:encodeSize];
    [[JZUDPClient sharedInstance] send:data];
    
    NSString *samplesData = [NSString stringWithFormat:@"1"];
    UnitySendMessage("Main Camera", "RecordSamples", [samplesData UTF8String]);
    free(bdata);
}

- (void)ilbcDecode:(uint8_t *)data size:(size_t)size
{
    uint8_t* adata = (uint8_t *)malloc(1024);
    int backSize = decode(data, adata, 1);
    //NSLog(@"back  size:%d", backSize);
    [audioController onGetPCMData:adata size:backSize mChannels:1];
    
    free(adata);
    
    NSString *samplesData = [NSString stringWithFormat:@"1"];
    UnitySendMessage("Main Camera", "PlaySamples", [samplesData UTF8String]);
}
@end
