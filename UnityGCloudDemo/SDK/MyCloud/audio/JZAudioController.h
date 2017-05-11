//
//  JZAudioStreamer.h
//  Live
//
//  Created by 周振宇 on 15/6/2.
//  Copyright (c) 2015年 juzi. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AudioUnit/AudioUnit.h>

@protocol JZAudioControllerDelegate <NSObject>

- (void)onGetAudioBuffer:(AudioBuffer *)buffer pts:(double)pts;

@end
@interface JZAudioController : NSObject

@property (nonatomic) AudioComponentInstance audioUnit;

@property (nonatomic) AudioBuffer tempBuffer;

@property (nonatomic) AudioBuffer backBuffer;

@property (nonatomic) BOOL isStreaming;

@property BOOL isCapture;
- (void) processAudio: (AudioBufferList*) bufferList pts:(double)pts;

@property (nonatomic, weak) id<JZAudioControllerDelegate> streamerDelegate;


- (void)start;
- (void)stop;
- (void)destroy;
- (instancetype)initWithIsCapture:(BOOL)isCapture;
- (void)setBackBufferValue:(uint8_t *)data size:(size_t)size;

- (void)onGetPCMData:(uint8_t *)data size:(size_t)size mChannels:(uint8_t)mChannels;
@end

extern JZAudioController* audioController;