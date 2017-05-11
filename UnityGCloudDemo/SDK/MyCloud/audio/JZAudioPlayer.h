//
//  JZAudioPlayer.h
//  Live
//
//  Created by 周振宇 on 15/6/9.
//  Copyright (c) 2015年 juzi. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AudioUnit/AudioUnit.h>

@interface JZAudioPlayer : NSObject

- (void)start;
- (void)stop;
- (void)destroy;

@property (nonatomic) AudioComponentInstance audioUnit;

@property (nonatomic) AudioBuffer tempBuffer;

- (void)onGetAudioPacket:(uint8_t *)packet size:(size_t)size pts:(double)pts;
@end

extern JZAudioPlayer* audioPlayer;