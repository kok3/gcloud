//
//  JZAudioCircleBuffer.h
//  Live
//
//  Created by 周振宇 on 15/7/15.
//  Copyright (c) 2015年 juzi. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "JZAudioBuffer.h"



@interface JZAudioCircleBuffer : NSObject


- (instancetype)initWithCapacity:(UInt32)capacity maxBufferSize:(UInt32)bufferSize;

- (void)appendBuffer:(uint8_t *)buffer size:(size_t)size;
- (JZAudioBuffer *)shiftBuffer;
- (UInt32)availableBuffer;
- (void)resetBuffer:(JZAudioBuffer *)buffer;
- (void)destroy;
- (void)reset;
@end
