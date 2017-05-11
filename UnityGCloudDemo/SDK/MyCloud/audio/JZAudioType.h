//
//  JZAudioType.h
//  Live
//
//  Created by 周振宇 on 15/6/10.
//  Copyright (c) 2015年 juzi. All rights reserved.
//

#ifndef Live_JZAudioType_h
#define Live_JZAudioType_h

#import "JZAudioBuffer.h"
typedef struct EncoderProperties_
{
    Float64 samplingRate;
    UInt32  inChannels;
    UInt32  outChannels;
    UInt32  frameSize;
    UInt32  bitrate;
} EncoderProperties;

/* Structure to keep the magic cookie */
typedef struct MagicCookie_
{
    void *data;
    int byteSize;
} MagicCookie;

typedef int (^encoder_audio_handler)(uint8_t* data, size_t size, double pts);
typedef int (^encoder_audio_headHandler)(uint8_t* data, size_t size, double pts);

typedef int (^decoder_audio_handler)(uint8_t* data, size_t size, UInt32 mChannels, double pts);

#endif

