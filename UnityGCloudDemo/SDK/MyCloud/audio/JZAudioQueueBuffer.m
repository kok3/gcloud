//
//  JZAudioQueueBuffer.m
//  Live
//
//  Created by 周振宇 on 15/7/14.
//  Copyright (c) 2015年 juzi. All rights reserved.
//

#import "JZAudioQueueBuffer.h"

@implementation JZAudioQueueBuffer

- (instancetype)initWithAudioQueue:(AudioQueueRef)audioQueue bufferSize:(UInt32)bufferSize
{
    self = [super init];
    if (self) {
        NSError *error;
        OSStatus status = AudioQueueAllocateBuffer(audioQueue, bufferSize, &_buffer);
        if (status != noErr) {
            error = [NSError errorWithDomain:NSOSStatusErrorDomain code:status userInfo:nil];
            NSLog(@"error:%@", error.localizedDescription);
        }
        _isInUse = false;
 
    }
    return self;
}

- (void)clear
{
    _isInUse = false;
    _bytesFilled = 0;
    _packetsFilled = 0;
}

- (AudioStreamPacketDescription *)packetDescArr
{
    return _pctDescArr;
}
@end
