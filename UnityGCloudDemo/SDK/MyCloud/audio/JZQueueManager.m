//
//  JZQueueManager.m
//  Live
//
//  Created by 周振宇 on 15/7/13.
//  Copyright (c) 2015年 juzi. All rights reserved.
//

#import "JZQueueManager.h"

@implementation JZQueueManager

+ (JZQueueManager *)sharedInstance
{
    static JZQueueManager *instance = nil;
    
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
        _circleBuffer = [[JZAudioCircleBuffer alloc] initWithCapacity:24 maxBufferSize:2048];
    }
    return self;
}

- (dispatch_queue_t)decodeAudioQueue
{
    if (_decodeAudioQueue == NULL) {
        _decodeAudioQueue = dispatch_queue_create("com.juzi.live.decodeAudioQueue", DISPATCH_QUEUE_SERIAL);
    }
    return _decodeAudioQueue;
}

- (dispatch_queue_t)rtmpRecvQueue
{
    if (_rtmpRecvQueue == NULL) {
         _rtmpRecvQueue = dispatch_queue_create("com.juzi.live.rtmpRecvQueue", DISPATCH_QUEUE_SERIAL);
    }
    return _rtmpRecvQueue;
}

- (dispatch_queue_t)filterWordQueue
{
    if (_filterWordQueue == NULL) {
        _filterWordQueue = dispatch_queue_create("com.juzi.live.filterWordQueue", DISPATCH_QUEUE_SERIAL);
    }
    return _filterWordQueue;
}

- (JZAudioCircleBuffer *)circleBuffer
{
    if (_circleBuffer == NULL) {
        _circleBuffer = [[JZAudioCircleBuffer alloc] initWithCapacity:24 maxBufferSize:2048];
    }
    return _circleBuffer;
}

@end
