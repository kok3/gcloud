//
//  JZAudioDecoder.h
//  Live
//
//  Created by 周振宇 on 15/6/10.
//  Copyright (c) 2015年 juzi. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "JZAudioType.h"
#import <AudioToolbox/AudioToolbox.h>
#import "JZAudioBuffer.h"
@interface JZAudioDecoder : NSObject

- (instancetype)initWithEncoderProperties:(EncoderProperties)props  packetHandler:(decoder_audio_handler)handler;
- (void)putAudioBuffer:(uint8_t *)buffer size:(size_t)size mChannels:(UInt32)mChannles pts:(double)pts;
- (void)destroy;

@end
