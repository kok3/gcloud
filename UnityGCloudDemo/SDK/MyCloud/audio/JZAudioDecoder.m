//
//  JZAudioDecoder.m
//  Live
//
//  Created by 周振宇 on 15/6/10.
//  Copyright (c) 2015年 juzi. All rights reserved.
//

#import "JZAudioDecoder.h"
#import "JZAudioBuffer.h"
#import "JZAppDelegateConfig.h"
@interface JZAudioDecoder()
{
    decoder_audio_handler _handler;
}

@property (nonatomic, assign)    UInt32 maxOutSizePerPacket;

@property (nonatomic, assign)    BOOL isStarted;

@property (nonatomic) AudioConverterRef converter;

@property (nonatomic) EncoderProperties encoderProps;

@property (nonatomic) MagicCookie *magicCookie;

@property (nonatomic) void  *outBuffer;

@property (nonatomic) uint8_t* currentBuffer;

@property (nonatomic) UInt32 bytesToEncode;

@property (nonatomic) UInt32 mChannels;

@property (nonatomic) AudioStreamPacketDescription *packetDesc;

@property (nonatomic) AudioStreamBasicDescription sourceASBD;

@property (nonatomic) AudioStreamBasicDescription destASBD;

@end
@implementation JZAudioDecoder

- (instancetype)initWithEncoderProperties:(EncoderProperties)props packetHandler:(decoder_audio_handler)handler
{
    self = [super init];
    if (self) {
        _handler = handler;
        self.encoderProps = props;
        
        _sourceASBD.mSampleRate = audioSampleRate;
        _sourceASBD.mFormatID = kAudioFormatMPEG4AAC;
        _sourceASBD.mChannelsPerFrame = 1;
        UInt32 size = sizeof(_sourceASBD);
        OSStatus status = AudioFormatGetProperty(kAudioFormatProperty_FormatInfo, 0, NULL, &size, &_sourceASBD);
        if (status != noErr) {
            NSLog(@"AudoFormatGetProperty kAudioFormatProperty_FormatInfo error:%d", status);
            return nil;
        }
        
        _destASBD.mSampleRate = audioSampleRate;
        _destASBD.mFormatID = kAudioFormatLinearPCM;
        _destASBD.mFormatFlags = kAudioFormatFlagIsSignedInteger | kAudioFormatFlagIsPacked;
        _destASBD.mFramesPerPacket = 1;
        _destASBD.mChannelsPerFrame = 1;
        _destASBD.mBitsPerChannel = 16;
        _destASBD.mBytesPerPacket = 2;
        _destASBD.mBytesPerFrame = 2;
        
        
        //        AudioClassDescription* descrip = [self getAudioClassDescriptionWithType:_destASBD.mFormatID fromManufacturer:kAppleSoftwareAudioCodecManufacturer];
        status = AudioConverterNew(&_sourceASBD, &_destASBD, &_converter);
        //        status = AudioConverterNewSpecific(&_sourceASBD, &_destASBD, 1, descrip, &_converter);
        if (status != noErr || !_converter) {
            NSLog(@"create audo converter error:%d", status);
            return nil;
        }
        
        UInt32 origBitRate = 0;
        size = sizeof(origBitRate);
        AudioConverterGetProperty(_converter, kAudioConverterEncodeBitRate, &size, &origBitRate);
//        NSLog(@"default encode bitrate:%d", origBitRate);
        //        UInt32 outputBitRate = props.bitrate;
        //        size = sizeof(outputBitRate);
        //        AudioConverterSetProperty(_converter, kAudioConverterEncodeBitRate, size, &outputBitRate);
        
//        UInt32 cookieSize = 0;
//        AudioConverterGetPropertyInfo(_converter, kAudioConverterCompressionMagicCookie, &cookieSize, NULL);
//        
//        char* cookie = (char *)calloc(1, cookieSize);
//        AudioConverterGetProperty(_converter, kAudioConverterCompressionMagicCookie, &cookieSize, cookie);
//        
//        self.magicCookie = malloc(sizeof(MagicCookie));
//        self.magicCookie->byteSize = cookieSize;
//        self.magicCookie->data = cookie;
        
        UInt32 maxOutputSizePerPacket = 0;
        UInt32 dataSize = sizeof(maxOutputSizePerPacket);
        AudioConverterGetProperty(_converter,
                                  kAudioConverterPropertyMaximumOutputPacketSize,
                                  &dataSize,
                                  
                                  &maxOutputSizePerPacket);
        _maxOutSizePerPacket = maxOutputSizePerPacket;
        
        self.outBuffer = malloc(self.maxOutSizePerPacket * 1024);
        
        self.packetDesc = (AudioStreamPacketDescription *)calloc(1, sizeof(AudioStreamPacketDescription));
    }
    return self;

}

- (void)putAudioBuffer:(uint8_t *)buffer size:(size_t)size mChannels:(UInt32)mChannles pts:(double)pts
{

    
    self.currentBuffer = buffer;
    self.bytesToEncode = size;
    self.mChannels = mChannles;
    
    //frames * channels * sampleSize
    UInt32 outBufferMaxSize = 1024*_destASBD.mChannelsPerFrame*_destASBD.mBytesPerFrame;
    UInt32 numOutputDataPackets = outBufferMaxSize /_maxOutSizePerPacket;
    AudioStreamPacketDescription outPacketDesc[1024];
    memset(_outBuffer, 0, outBufferMaxSize);
    
    AudioBufferList outBufferList;
    outBufferList.mNumberBuffers = 1;
    outBufferList.mBuffers[0].mNumberChannels = self.mChannels;
    outBufferList.mBuffers[0].mDataByteSize = outBufferMaxSize;
    outBufferList.mBuffers[0].mData = self.outBuffer;
    
    OSStatus status = AudioConverterFillComplexBuffer(_converter, decodeProc, (__bridge void *)(self), &numOutputDataPackets, &outBufferList, outPacketDesc);
    
    if (status != noErr) {
        NSLog(@"AudioConverterFillComplexBuffer error:%d", status);
        [self checkStatus:status];
        return;
    }
    
    free(self.currentBuffer);
//    size_t size = (size_t)outBufferList.mBuffers[0].mDataByteSize;
    //    NSLog(@"get encoded audio data size:%zu", size);
    
    
    if (_handler) {
        _handler(outBufferList.mBuffers[0].mData, outBufferList.mBuffers[0].mDataByteSize, outBufferList.mBuffers[0].mNumberChannels, pts);
    }
//    free(data);
}

OSStatus decodeProc(AudioConverterRef inAudioConverter, UInt32 *ioNumberDataPackets, AudioBufferList *ioData, AudioStreamPacketDescription **outDataPacketDescription, void *inUserData)
{
    JZAudioDecoder* decoder = (__bridge JZAudioDecoder *)inUserData;
    
    UInt32 maxPackets = decoder.bytesToEncode / decoder.maxOutSizePerPacket;
    if (*ioNumberDataPackets > maxPackets) {
        *ioNumberDataPackets = maxPackets;
    }
    
    
    ioData->mBuffers[0].mData = decoder.currentBuffer;
    ioData->mBuffers[0].mDataByteSize = decoder.bytesToEncode;
    ioData->mBuffers[0].mNumberChannels = decoder.mChannels;
    
    if (outDataPacketDescription) {
        decoder.packetDesc[0].mStartOffset = 0;
        decoder.packetDesc[0].mVariableFramesInPacket = 0;
        decoder.packetDesc[0].mDataByteSize = decoder->_bytesToEncode;
        (*outDataPacketDescription) = decoder.packetDesc;
    }
    
    if (decoder.bytesToEncode == 0) {
        return 1;
    }
    
    decoder.bytesToEncode = 0;
    return  noErr;
}

- (void)destroy
{
    if (_converter) {
        AudioConverterDispose(_converter);
        _handler = NULL;
        _converter = NULL;
        free(self.outBuffer);
    }
}

- (void)checkStatus:(int)status
{
    if (status != noErr) {
        NSError *error = [NSError errorWithDomain:NSOSStatusErrorDomain code:status userInfo:nil];
        NSLog(@"error:%@", error.description);
        return;
    }
}

@end
