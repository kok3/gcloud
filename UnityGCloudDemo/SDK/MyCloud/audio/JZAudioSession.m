//
//  JZAudioSession.m
//  Live
//
//  Created by 周振宇 on 15/6/3.
//  Copyright (c) 2015年 juzi. All rights reserved.
//

#import "JZAudioSession.h"
#import <AVFoundation/AVFoundation.h>
#import "JZAppDelegateConfig.h"
#import "JZAudioManager.h"

@implementation JZAudioSession

+ (instancetype) sharedInstance
{
    static dispatch_once_t predict;
    static JZAudioSession* instance;
    dispatch_once(&predict, ^{
        instance = [[JZAudioSession alloc] init];
    });
    return instance;
}

- (instancetype)init
{
    self = [super init];
    if (self) {
//        [self setupAudioSession];
    }
    return self;
}

- (void)setupRecordSession:(double)sampleRate bufferSize:(UInt32)bufferSize
{
    NSError *error;
    
    self.sampleRate = sampleRate;
    AVAudioSession *session = [AVAudioSession sharedInstance];
    
    [session setCategory:AVAudioSessionCategoryPlayAndRecord withOptions:AVAudioSessionCategoryOptionDefaultToSpeaker  error:&error];
    
    if([JZAudioManager sharedInstance]._isMicModel == 2)
    {
        [session setPreferredSampleRate:audioSampleRate error:&error];
    }
    
    self.ioBufferDuration = audioIOBufferDuration;  //为了让编码每次获得大于1024样本,由于aac至少需要1024个样本一个packet
    [session setPreferredIOBufferDuration:self.ioBufferDuration error:&error];
    
    [session setActive:YES error:&error];
    
   self.sampleRate = [session sampleRate];
    self.ioBufferDuration = [session IOBufferDuration];
    self.bufferSize = (UInt32)(self.ioBufferDuration * self.sampleRate);
    NSLog(@"set preferred samplerate:%f preferred IObuffer Duration:%f", self.sampleRate, self.ioBufferDuration);
    
}

- (void)setupPlaySession:(double)sampleRate bufferSize:(UInt32)bufferSize
{
    NSError *error;
    
    self.sampleRate = sampleRate;
    AVAudioSession *session = [AVAudioSession sharedInstance];
    
    //    [session setCategory:AVAudioSessionCategoryPlayAndRecord error:&error];
    [session setCategory:AVAudioSessionCategoryAmbient error:&error];
    //    [session overrideOutputAudioPort:AVAudioSessionPortOverrideNone error:&error];
    
    if([JZAudioManager sharedInstance]._isMicModel == 2)
    {
        [session setPreferredSampleRate:audioSampleRate error:&error];
    }
    
    self.ioBufferDuration = audioIOBufferDuration;  //为了让编码每次获得大于1024样本,由于aac至少需要1024个样本一个packet
    [session setPreferredIOBufferDuration:self.ioBufferDuration error:&error];
    
    //   [session setMode:AVAudioSessionModeVoiceChat error:&error];
    [session setActive:YES error:&error];
    
    self.sampleRate = [session sampleRate];
    self.ioBufferDuration = [session IOBufferDuration];
    self.bufferSize = (UInt32)(self.ioBufferDuration * self.sampleRate);
    NSLog(@"set preferred samplerate:%f preferred IObuffer Duration:%f", self.sampleRate, self.ioBufferDuration);
}

- (void)clearSession
{
      NSError *error;
    [[AVAudioSession sharedInstance] setActive:false error:&error];
}

- (void)setupAudioSession
{
    
//    self.ioBufferDuration = 0.05;
//    [session setPreferredIOBufferDuration:self.ioBufferDuration error:&error];
//    self.ioBufferDuration = [session IOBufferDuration];
//    NSLog(@"set preferred IObuffer Duration:%@", error.description);
//    
//    int result = 0;
//    // Get the set of available inputs. If there are no audio accessories attached, there will be
//    // only one available input -- the built in microphone.
//    NSArray* inputs = [session availableInputs];
//    
//    // Locate the Port corresponding to the built-in microphone.
//    AVAudioSessionPortDescription* builtInMicPort = nil;
//    for (AVAudioSessionPortDescription* port in inputs)
//    {
//        if ([port.portType isEqualToString:AVAudioSessionPortBuiltInMic])
//        {
//            builtInMicPort = port;
//            break;
//        }
//    }
//    
//    // Print out a description of the data sources for the built-in microphone
//    NSLog(@"There are %u data sources for port :\"%@\"", (unsigned)[builtInMicPort.dataSources count], builtInMicPort);
//    NSLog(@"%@", builtInMicPort.dataSources);
//    
//    // loop over the built-in mic's data sources and attempt to locate the front microphone
//    AVAudioSessionDataSourceDescription* frontDataSource = nil;
//    for (AVAudioSessionDataSourceDescription* source in builtInMicPort.dataSources)
//    {
//        if ([source.orientation isEqual:AVAudioSessionOrientationFront])
//        {
//            frontDataSource = source;
//            break;
//        }
//    } // end data source iteration
//    
//    if (frontDataSource)
//    {
//        NSLog(@"Currently selected source is \"%@\" for port \"%@\"", builtInMicPort.selectedDataSource.dataSourceName, builtInMicPort.portName);
//        NSLog(@"Attempting to select source \"%@\" on port \"%@\"", frontDataSource, builtInMicPort.portName);
//        
//        // Set a preference for the front data source.
//        error = nil;
//        result = [builtInMicPort setPreferredDataSource:frontDataSource error:&error];
//        if (!result)
//        {
//            // an error occurred. Handle it!
//            NSLog(@"setPreferredDataSource failed");
//        }
//    }
//    
//    // Make sure the built-in mic is selected for input. This will be a no-op if the built-in mic is
//    // already the current input Port.
//    error = nil;
//    result = [session setPreferredInput:builtInMicPort error:&error];
//    if (!result)
//    {
//        // an error occurred. Handle it!
//        NSLog(@"setPreferredInput failed");
//    }
    
}

@end
