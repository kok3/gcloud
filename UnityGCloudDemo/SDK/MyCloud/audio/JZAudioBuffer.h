//
//  JZAudioBuffer.h
//  Live
//
//  Created by 周振宇 on 15/7/15.
//  Copyright (c) 2015年 juzi. All rights reserved.
//

#import <Foundation/Foundation.h>

typedef NS_ENUM(NSInteger, JZAudioBuffer_state) {
    JZAudiobufferState_Empty = 0,
    JZAudioBufferState_Load = 1,
    JZAudioBufferState_InUse = 2
};
@interface JZAudioBuffer : NSObject

@property (nonatomic) uint8_t* data;
@property (nonatomic) size_t size;
@property (nonatomic) UInt32 numChannels;
@property (atomic) int state;

- (void)reset;
@end
