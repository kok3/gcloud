//
//  JZQueueManager.h
//  Live
//
//  Created by 周振宇 on 15/7/13.
//  Copyright (c) 2015年 juzi. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "JZAudioCircleBuffer.h"

@interface JZQueueManager : NSObject
+ (JZQueueManager *)sharedInstance;

@property (nonatomic) dispatch_queue_t rtmpRecvQueue;

@property (nonatomic) dispatch_queue_t decodeAudioQueue;

@property (nonatomic) dispatch_queue_t filterWordQueue;

@property (nonatomic) JZAudioCircleBuffer *circleBuffer;

@end
