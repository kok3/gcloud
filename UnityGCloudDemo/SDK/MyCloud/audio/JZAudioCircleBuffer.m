//
//  JZAudioCircleBuffer.m
//  Live
//
//  Created by 周振宇 on 15/7/15.
//  Copyright (c) 2015年 juzi. All rights reserved.
//

#import "JZAudioCircleBuffer.h"
#import "JZAudioBuffer.h"

@interface JZAudioCircleBuffer()
{
    NSMutableArray *_audioBuffers;
    NSMutableArray *_audioConsumeBuffers;
}

@end
@implementation JZAudioCircleBuffer

- (instancetype)initWithCapacity:(UInt32)capacity maxBufferSize:(UInt32)bufferSize;
{
    self = [super init];
    if (self) {
        _audioBuffers = [[NSMutableArray alloc] initWithCapacity:capacity];
        _audioConsumeBuffers = [[NSMutableArray alloc] init];
        for (UInt32 i = 0; i < capacity; i++) {
            JZAudioBuffer* buffer = [[JZAudioBuffer alloc] init];
            buffer.data = calloc(1, bufferSize);
            buffer.numChannels = 1;
            buffer.size = 0;
            [_audioBuffers addObject:buffer];
        }
    }
    return self;
}

- (JZAudioBuffer *)getEmptyBuffer
{
    for (UInt32 i = 0; i < _audioBuffers.count; i++) {
        JZAudioBuffer* buffer = _audioBuffers[i];
        if (buffer.state == JZAudiobufferState_Empty) {
            return buffer;
        }
    }
    return NULL;
}

- (void)appendBuffer:(uint8_t *)data size:(size_t)size
{
    @synchronized(self)
    {
        JZAudioBuffer* buffer = [self getEmptyBuffer];
        if (buffer == NULL) {
            return;
        }
        buffer.size = size;
        memcpy(buffer.data, data, size);
        [_audioConsumeBuffers addObject:buffer];
        buffer.state = JZAudioBufferState_Load;
    }
}

- (void)resetBuffer:(JZAudioBuffer *)buffer
{
    @synchronized(self)
    {
        buffer.state = JZAudiobufferState_Empty;
        memset(buffer.data, 0, buffer.size);
        buffer.size = 0;
    }
}

- (JZAudioBuffer *)shiftBuffer;
{
    JZAudioBuffer *buffer = NULL;
    @synchronized(self)
    {
        if (_audioConsumeBuffers.count > 0) {
            buffer = [_audioConsumeBuffers firstObject];
            buffer.state = JZAudioBufferState_InUse;
            [_audioConsumeBuffers removeObject:buffer];
        }
    }
    return buffer;
}

- (UInt32)availableBytesLen
{
    return 0;
}

- (void)reset
{
    for (UInt32 i = 0; i < _audioBuffers.count; i++) {
        JZAudioBuffer* buffer = _audioBuffers[i];
        [self resetBuffer:buffer];
    }
    [_audioConsumeBuffers removeAllObjects];
}

- (void)destroy
{
    for (UInt32 i = 0; i < _audioBuffers.count; i++) {
        JZAudioBuffer* buffer = _audioBuffers[i];
        free(buffer.data);
    }
    [_audioBuffers removeAllObjects];
    _audioBuffers = NULL;
}
@end
