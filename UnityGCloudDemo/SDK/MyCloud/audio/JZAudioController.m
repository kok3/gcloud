//
//  JZAudioCapture.m
//  Live
//
//  Created by 周振宇 on 15/6/2.
//  Copyright (c) 2015年 juzi. All rights reserved.
//

#import "JZAudioController.h"
#import <AudioToolbox/AudioToolbox.h>
#import <AVFoundation/AVFoundation.h>
#import "JZAudioSession.h"
#import "JZAppDelegateConfig.h"
#import "JZAudioCircleBuffer.h"
#import "JZQueueManager.h"
#import "JZAudioManager.h"
//#import "DCRejectionFilter.h"
#define kOutputBus 0
#define kInputBus 1
JZAudioController* audioController;

@interface JZAudioController()
{
    uint8_t* _receiveBuffer;
    size_t _bufferLen;
}

@property (nonatomic) double sampleRate;

@property (nonatomic) double ioBufferDuration;

@property (nonatomic) UInt32 maxBufferSize;
@property  BOOL isSetup;

@property (nonatomic) JZAudioCircleBuffer* circleBuffer;
//@property (nonatomic) DCRejectionFilter* filter;

@end

static OSStatus recordingCallback(void *inRefCon,
                                  AudioUnitRenderActionFlags *ioActionFlags,
                                  const AudioTimeStamp *inTimeStamp,
                                  UInt32 inBusNumber,
                                  UInt32 inNumberFrames,
                                  AudioBufferList *ioData) {
    
    AudioBuffer buffer;
    
    buffer.mNumberChannels = 1;
    buffer.mDataByteSize = inNumberFrames * 2;
    buffer.mData = malloc( inNumberFrames * 2);
    
    // Put buffer in a AudioBufferList
    AudioBufferList bufferList;
    bufferList.mNumberBuffers = 1;
    bufferList.mBuffers[0] = buffer;
    
    // Then:
    // Obtain recorded samples
    
    OSStatus status;
    
    status = AudioUnitRender(audioController.audioUnit,
                             ioActionFlags,
                             inTimeStamp,
                             inBusNumber,
                             inNumberFrames,
                             &bufferList);
//    [capture checkStatus:status];
    //    cStatus(status);
    // Now, we have the samples we just read sitting in buffers in bufferList
    // Process the new data
    //    capture.filter->ProcessInplace((Float32*) bufferList.mBuffers[0].mData, inNumberFrames);
    [audioController processAudio:&bufferList pts:inTimeStamp->mSampleTime];
    //    cStatus(status);
    // release the malloc'ed data in the buffer we created earlier
    
    free(bufferList.mBuffers[0].mData);
    
    return noErr;
}

static OSStatus playbackCallback(void *inRefCon,
                                 AudioUnitRenderActionFlags *ioActionFlags,
                                 const AudioTimeStamp *inTimeStamp,
                                 UInt32 inBusNumber,
                                 UInt32 inNumberFrames,
                                 AudioBufferList *ioData) {
    [JZAudioManager sharedInstance]._inNumberFrames =inNumberFrames;
    for (int i=0; i < ioData->mNumberBuffers; i++) {
        AudioBuffer buffer = ioData->mBuffers[i];
        
               // NSLog(@"get audio buffer data %i",inNumberFrames);
        
        JZAudioBuffer *tempBuffer = [audioController.circleBuffer shiftBuffer];
        
//         indicate how much data we wrote in the buffer
        if ( tempBuffer && tempBuffer.size > 0) {
            memset(buffer.mData, 0, buffer.mDataByteSize);
            UInt32 size = min(buffer.mDataByteSize, tempBuffer.size);
            buffer.mDataByteSize = size;
            memcpy(buffer.mData, tempBuffer.data, size);
            //NSLog(@"playback%i",size);
            [audioController.circleBuffer resetBuffer:tempBuffer];
        }else
        {
            memset(buffer.mData, 0, buffer.mDataByteSize);
            //            NSLog(@"empty audio data");
        }
//        memcpy(buffer.mData, audioController.backBuffer.mData, audioController.backBuffer.mDataByteSize);
    }
    return noErr;
}

@implementation JZAudioController

- (void)start
{
    OSStatus status = AudioOutputUnitStart(_audioUnit);
    [self checkStatus:status];
    
    
}

- (void)stop
{
    if (_audioUnit) {
        OSStatus status = AudioOutputUnitStop(_audioUnit);
        [self checkStatus:status];
    }
}

- (void)destroy
{
    [self stop];
    self.isSetup = false;
    AudioUnitUninitialize(_audioUnit);
    AudioComponentInstanceDispose(_audioUnit);
  //  free(_tempBuffer.mData);
    _audioUnit = NULL;
    
    [_circleBuffer reset];
    _circleBuffer = NULL;

}

- (instancetype)initWithIsCapture:(BOOL)isCapture;
{
    self = [super init];
    self.sampleRate = audioSampleRate;
    self.maxBufferSize = 1024;
    self.isCapture = isCapture;
    
    self.circleBuffer = [JZQueueManager sharedInstance].circleBuffer;
    _receiveBuffer = calloc(1, 1024);
    _bufferLen = 0;
  //  _filter = new DCRejectionFilter;
    NSError* error = nil;
    
  //定义remoteIo组件
    OSStatus status;
    AudioComponentDescription desc;
    desc.componentType = kAudioUnitType_Output;
//    if (isCapture) {
//    desc.componentSubType = kAudioUnitSubType_VoiceProcessingIO;
//    }else
//    {
        desc.componentSubType = kAudioUnitSubType_RemoteIO;
//    }

    desc.componentFlags = 0;
    desc.componentFlagsMask = 0;
    desc.componentManufacturer = kAudioUnitManufacturer_Apple;
    
    //获得组件
    AudioComponent inputComponent = AudioComponentFindNext(NULL, &desc);
    
    //获得audio unit
    status = AudioComponentInstanceNew(inputComponent, &_audioUnit);
    [self checkStatus:status];
    
    //enable recode
    UInt32 flagOne = 1;
    UInt32 flagZero = 0;
    status = AudioUnitSetProperty(_audioUnit, kAudioOutputUnitProperty_EnableIO, kAudioUnitScope_Input, kInputBus, &flagOne, sizeof(flagOne));
    [self checkStatus:status];
    
    //disable playback
//    if (isCapture) {
//        status = AudioUnitSetProperty(_audioUnit, kAudioOutputUnitProperty_EnableIO, kAudioUnitScope_Output, kOutputBus, &flagZero, sizeof(flagZero));

//    }else
//    {
        status = AudioUnitSetProperty(_audioUnit, kAudioOutputUnitProperty_EnableIO, kAudioUnitScope_Output, kOutputBus, &flagOne, sizeof(flagOne));
//    }
    [self checkStatus:status];
    
//    if (isCapture) {
//        status = AudioUnitSetProperty(_audioUnit,
//                                      kAUVoiceIOProperty_VoiceProcessingEnableAGC,
//                                      kAudioUnitScope_Global,
//                                      1,
//                                      &flagZero,
//                                      sizeof(flagZero));
//        [self checkStatus:status];
//
//    }
    
    //vpio mute掉输出的话， recording基本为无声

//    status = AudioUnitSetProperty(_audioUnit, kAUVoiceIOProperty_MuteOutput, kAudioUnitScope_Global, kOutputBus, &flagOne, sizeof(flagOne));
//    [self checkStatus:status];
//    status = AudioUnitSetProperty(_audioUnit,
//                                  kAUVoiceIOProperty_DuckNonVoiceAudio,
//                                  kAudioUnitScope_Global,
//                                  1,
//                                  &flagZero,
//                                  sizeof(flagZero));
//    [self checkStatus:status];

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
    status = AudioUnitSetProperty(_audioUnit, kAudioUnitProperty_StreamFormat, kAudioUnitScope_Output, kInputBus, &ASBD, sizeof(ASBD));
    [self checkStatus:status];
    
    status = AudioUnitSetProperty(_audioUnit, kAudioUnitProperty_StreamFormat, kAudioUnitScope_Input, kOutputBus, &ASBD, sizeof(ASBD));
    [self checkStatus:status];
    
    //input callback
    AURenderCallbackStruct callbackStruct;
    callbackStruct.inputProc = recordingCallback;
    callbackStruct.inputProcRefCon = (__bridge void *)(self);
    status = AudioUnitSetProperty(_audioUnit, kAudioOutputUnitProperty_SetInputCallback, kAudioUnitScope_Global, kInputBus, &callbackStruct, sizeof(callbackStruct));
    [self checkStatus:status];
    
//    play callback
    callbackStruct.inputProc = playbackCallback;
    callbackStruct.inputProcRefCon = (__bridge void*)self;
    status = AudioUnitSetProperty(_audioUnit, kAudioUnitProperty_SetRenderCallback, kAudioUnitScope_Global, kOutputBus, &callbackStruct, sizeof(callbackStruct));
    
    //diable system allocate
   //disable buffer alloction for the recorder
    status = AudioUnitSetProperty(_audioUnit, kAudioUnitProperty_ShouldAllocateBuffer, kAudioUnitScope_Output, kInputBus, &flagZero, sizeof(flagZero));
    
    
    _tempBuffer.mNumberChannels = 1;
    _tempBuffer.mDataByteSize = self.maxBufferSize;
    _tempBuffer.mData = calloc(1, _tempBuffer.mDataByteSize);
    
    _backBuffer.mNumberChannels = 1;
    
    status = AudioUnitInitialize(_audioUnit);
    [self checkStatus:status];
    return self;
}



- (void)processAudio:(AudioBufferList *)bufferList pts:(double)pts
{
    if (!self.isStreaming) {
        return;
    }
    AudioBuffer sourceBuffer = bufferList->mBuffers[0];
    if (_tempBuffer.mDataByteSize != sourceBuffer.mDataByteSize) {
        free(_tempBuffer.mData);
        _tempBuffer.mDataByteSize = sourceBuffer.mDataByteSize;
        _tempBuffer.mData = calloc(1, sourceBuffer.mDataByteSize);
    }

    memcpy(_tempBuffer.mData, bufferList->mBuffers[0].mData, bufferList->mBuffers[0].mDataByteSize);
    if (self.streamerDelegate && self.isCapture) {
//        AudioBuffer* sendBuffer = (AudioBuffer *)calloc(1, sizeof(AudioBuffer));
//        sendBuffer->mDataByteSize = sourceBuffer.mDataByteSize;
//        sendBuffer->mData = calloc(1, sourceBuffer.mDataByteSize);
//        memcpy(sendBuffer->mData, sourceBuffer.mData, sourceBuffer.mDataByteSize);
        [self.streamerDelegate onGetAudioBuffer:&_tempBuffer pts:pts];
//        NSLog(@"current pcm size:%d", _tempBuffer.mDataByteSize);
    }
}

- (void)setBackBufferValue:(uint8_t *)data size:(size_t)size
{
    if (_backBuffer.mDataByteSize != size) {
        free(_backBuffer.mData);
        _backBuffer.mDataByteSize = size;
        _backBuffer.mData = calloc(1, size);
    }
    memcpy(_backBuffer.mData, data, size);
}

- (void)onGetPCMData:(uint8_t *)data size:(size_t)size mChannels:(uint8_t)mChannels
{
    if (size == 0) {
        return;
    }
    UInt32 inNumberFramesValue = [JZAudioManager sharedInstance]._inNumberFrames * 2;
    memcpy(_receiveBuffer + _bufferLen, data, size);
    _bufferLen += size;
    while (_bufferLen >= inNumberFramesValue) {
        [_circleBuffer appendBuffer:_receiveBuffer size:inNumberFramesValue];
        _bufferLen -= inNumberFramesValue;
        memcpy(_receiveBuffer, _receiveBuffer + inNumberFramesValue, _bufferLen);
    }

//    [self setBackBufferValue:data size:size];
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
