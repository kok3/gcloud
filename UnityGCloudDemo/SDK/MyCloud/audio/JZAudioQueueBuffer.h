//
//  JZAudioQueueBuffer.h
//  Live
//
//  Created by 周振宇 on 15/7/14.
//  Copyright (c) 2015年 juzi. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>
#import <AudioToolbox/AudioToolbox.h>


@interface JZAudioQueueBuffer : NSObject
{
    AudioStreamPacketDescription _pctDescArr[1024];
}

@property (nonatomic) AudioQueueBufferRef buffer;

@property (nonatomic) BOOL isInUse;

@property (nonatomic)    size_t bytesFilled;
@property (nonatomic)    size_t packetsFilled;

@property (nonatomic) AudioStreamPacketDescription *packetDescArr;

- (instancetype)initWithAudioQueue:(AudioQueueRef)audioQueue bufferSize:(UInt32)bufferSize;
- (void)clear;
@end
