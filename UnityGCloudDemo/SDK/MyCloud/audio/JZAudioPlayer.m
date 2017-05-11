//
//  JZAudioCapture.m
//  Live
//
//  Created by 周振宇 on 15/6/2.
//  Copyright (c) 2015年 juzi. All rights reserved.
//

#import "JZAudioPlayer.h"
#import <AudioToolbox/AudioToolbox.h>
#import <AVFoundation/AVFoundation.h>
#import "JZAudioSession.h"
#import "JZAppDelegateConfig.h"
#import "JZQueueManager.h"
#import "JZAudioCircleBuffer.h"
#import "JZQueueManager.h"
#import "JZAudioDecoder.h"
#define kOutputBus 0
#define kInputBus 1
JZAudioPlayer* audioPlayer;


@interface JZAudioPlayer()

@property (nonatomic) double sampleRate;

@property (nonatomic) double ioBufferDuration;

@property (nonatomic) UInt32 maxBufferSize;

@property (nonatomic) JZAudioCircleBuffer* circleBuffer;

@property (nonatomic) JZAudioDecoder* decoder;

@property  BOOL isStarted;

//@property (nonatomic) DCRejectionFilter* filter;

@end

static OSStatus playbackCallback(void *inRefCon,
                                 AudioUnitRenderActionFlags *ioActionFlags,
                                 const AudioTimeStamp *inTimeStamp,
                                 UInt32 inBusNumber,
                                 UInt32 inNumberFrames,
                                 AudioBufferList *ioData) {
    for (int i=0; i < ioData->mNumberBuffers; i++) {
        AudioBuffer buffer = ioData->mBuffers[i];
        
//        NSLog(@"get audio buffer data");

        JZAudioBuffer *tempBuffer = [audioPlayer.circleBuffer shiftBuffer];
        
            // indicate how much data we wrote in the buffer
        if ( tempBuffer && tempBuffer.size > 0) {
             UInt32 size = min(buffer.mDataByteSize, tempBuffer.size);
             buffer.mDataByteSize = size;
             memcpy(buffer.mData, tempBuffer.data, size);
            [audioPlayer.circleBuffer resetBuffer:tempBuffer];
        }else
        {
            memset(buffer.mData, 0, buffer.mDataByteSize);
//            NSLog(@"empty audio data");
        }
    }
    return noErr;
}

@implementation JZAudioPlayer

- (void)start
{
    
    OSStatus status = AudioOutputUnitStart(_audioUnit);
    [self checkStatus:status];
    self.isStarted = true;
}

- (void)stop
{
    OSStatus status = AudioOutputUnitStop(_audioUnit);
    [self checkStatus:status];
    self.isStarted = false;
}

- (void)destroy
{
    [self stop];
        
    AudioUnitUninitialize(_audioUnit);
    AudioComponentInstanceDispose(_audioUnit);
    _audioUnit = NULL;
    [_circleBuffer reset];
    _circleBuffer = NULL;
    [_decoder destroy];
    _decoder = NULL;
//    free(_tempBuffer.mData);
}

- (instancetype)init
{
    self = [super init];
    self.sampleRate = audioSampleRate;
    self.circleBuffer = [JZQueueManager sharedInstance].circleBuffer;
//    [[JZAudioSession sharedInstance] setupPlaySession:self.sampleRate bufferSize:self.maxBufferSize];
    
    //  _filter = new DCRejectionFilter;
    OSStatus status;
    NSError* error = nil;
    
    EncoderProperties p;
    p.samplingRate = audioSampleRate;
    p.bitrate      = audioBitrate;
    _decoder = [[JZAudioDecoder alloc] initWithEncoderProperties:p packetHandler:^int(uint8_t *data, size_t size, UInt32 mChannels, double pts) {
        [self onGetPCMData:data size:size mChannels:mChannels];
        return 1;
    }];
    
    //定义remoteIo组件
    AudioComponentDescription desc;
    desc.componentType = kAudioUnitType_Output;
    desc.componentSubType = kAudioUnitSubType_RemoteIO;
    desc.componentFlags = 0;
    desc.componentFlagsMask = 0;
    desc.componentManufacturer = kAudioUnitManufacturer_Apple;
    
    //获得组件
    AudioComponent inputComponent = AudioComponentFindNext(NULL, &desc);
    
    //获得audio unit
    status = AudioComponentInstanceNew(inputComponent, &_audioUnit);
    [self checkStatus:status];
    
    //enable record
    UInt32 flagOne = 1;
    UInt32 flagZero = 0;
//    status = AudioUnitSetProperty(_audioUnit, kAudioOutputUnitProperty_EnableIO, kAudioUnitScope_Input, kInputBus, &flagOne, sizeof(flagOne));
//    [self checkStatus:status];
    
    //enable playback
    status = AudioUnitSetProperty(_audioUnit, kAudioOutputUnitProperty_EnableIO, kAudioUnitScope_Output, kOutputBus, &flagOne, sizeof(flagOne));
    [self checkStatus:status];
    
    //asbd
    AudioStreamBasicDescription ASBD;
    ASBD.mSampleRate = self.sampleRate;
    ASBD.mFormatID = kAudioFormatLinearPCM;
    ASBD.mFormatFlags = kAudioFormatFlagIsSignedInteger | kAudioFormatFlagIsPacked;
    ASBD.mFramesPerPacket = 1;
    ASBD.mChannelsPerFrame = 1;
    ASBD.mBitsPerChannel = 16;
    ASBD.mBytesPerPacket = 2;
    ASBD.mBytesPerFrame = 2;
    ASBD.mChannelsPerFrame = 1;
    ASBD.mReserved = 0;
    
    //设置asbd
    status = AudioUnitSetProperty(_audioUnit, kAudioUnitProperty_StreamFormat, kAudioUnitScope_Input, kOutputBus, &ASBD, sizeof(ASBD));
        [self checkStatus:status];
    
    //play callback
    AURenderCallbackStruct callbackStruct;
    callbackStruct.inputProc = playbackCallback;
        callbackStruct.inputProcRefCon = (__bridge void*)self;
        status = AudioUnitSetProperty(_audioUnit, kAudioUnitProperty_SetRenderCallback, kAudioUnitScope_Global, kOutputBus, &callbackStruct, sizeof(callbackStruct));
    
    //diable system allocate
    //disable buffer alloction for the recorder
    status = AudioUnitSetProperty(_audioUnit, kAudioUnitProperty_ShouldAllocateBuffer, kAudioUnitScope_Output, kInputBus, &flagZero, sizeof(flagZero));
    
    status = AudioUnitInitialize(_audioUnit);
    [self checkStatus:status];
    return self;
}


- (void)onGetAudioPacket:(uint8_t *)packet size:(size_t)size pts:(double)pts
{
    if (!_isStarted) {
        return;
    }
    uint8_t *data = calloc(1, size);
    memcpy(data, packet, size);
    dispatch_async(dispatch_get_main_queue(), ^{
        [_decoder putAudioBuffer:data size:size mChannels:1 pts:pts];
    });
}

- (void)onGetPCMData:(uint8_t *)data size:(size_t)size mChannels:(uint8_t)mChannels
{
    if (!_isStarted) {
        return;
    }
    if (size == 0) {
        return;
    }
    [_circleBuffer appendBuffer:data size:size];
}

- (void)checkStatus:(int)status
{
    if (status != noErr) {
        NSError *error = [NSError errorWithDomain:NSOSStatusErrorDomain code:status userInfo:nil];
        NSLog(@"error:%@", error.localizedDescription);
        return;
    }
}
@end
