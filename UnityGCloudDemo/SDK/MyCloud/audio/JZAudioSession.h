//
//  JZAudioSession.h
//  Live
//
//  Created by 周振宇 on 15/6/3.
//  Copyright (c) 2015年 juzi. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface JZAudioSession : NSObject
+ (instancetype) sharedInstance;

@property (nonatomic) double sampleRate;
@property (nonatomic) double ioBufferDuration;
@property (nonatomic) UInt32 bufferSize;

- (void)setupRecordSession:(double)sampleRate bufferSize:(UInt32)bufferSize;

- (void)setupPlaySession:(double)sampleRate bufferSize:(UInt32)bufferSize;

- (void)clearSession;
@end
