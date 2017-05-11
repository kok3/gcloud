//
//  JZAudioManager.m
//  Unity-iPhone
//
//  Created by 周振宇 on 15/11/29.
//
//

#import "JZAudioManager.h"
#import "JZAudioController.h"
#import "JZAudioSession.h"
#import "JZAppDelegateConfig.h"
#import "JZILbcEncoder.h"
#import "JZAudioPlayer.h"
#import "JZUDPClient.h"
#import <AVFoundation/AVFoundation.h>


@interface JZAudioManager ()<JZAudioControllerDelegate, JZUDPClientDelegate>
{
    dispatch_queue_t _captureAudioQueue;
    JZILbcEncoder *_encoder;
    uint8_t* _buffer;
    size_t bufferLen;
    bool isInit;
    bool _isStartSound;
    bool _isRegister;
}

@end
@implementation JZAudioManager

+ (JZAudioManager *)sharedInstance
{
    static JZAudioManager *instance = nil;
    
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        instance = [[self alloc] init];
    });
    
    return instance;
}

- (instancetype)init
{
    self = [super init];
    if (self) {
    }
    return self;
}

- (void)setIP:(NSString*)params
{
    NSArray *pArr = [params componentsSeparatedByString:@"|"];
    NSString *ip_port = pArr[0];
    self.playerId = [pArr[1] doubleValue];
    pArr = [ip_port componentsSeparatedByString:@":"];
    self.host = pArr[0];
    self.port = [pArr[1] integerValue];
    
    NSLog(@"\n  host： %@", self.host);
    NSLog(@"\n  port： %ld", (long)self.port);
    
    if([JZAudioManager sharedInstance]._isNewSetUDPIP == true)
    {
        //*****
        [[JZUDPClient sharedInstance] bingToPort];
    }
}

- (void)setup
{
    if(isInit == false)
    {
        isInit = true;
        _buffer = calloc(1, 1024);
        bufferLen = 0;
        _encoder = [JZILbcEncoder new];
        [JZUDPClient sharedInstance].delegate = self;
        //_captureAudioQueue = dispatch_queue_create("com.juzi.live.audiocapture_capture", DISPATCH_QUEUE_SERIAL);
    }
    /*
    [[JZAudioSession sharedInstance] setupRecordSession:audioSampleRate bufferSize:1024];
    audioController = [[JZAudioController alloc] initWithIsCapture:true];
    audioController.streamerDelegate = self;
    [audioController start];
    
    [[JZAudioSession sharedInstance] setupPlaySession:audioSampleRate bufferSize:1024];
    */
    //[self start];
    
    if([JZAudioManager sharedInstance]._isMicModel == 2)
    {
        [self stop];
    }else{
        [self InitController];
        
        if (audioController) {
            
            audioController.isStreaming = false;
        }
    }
    
    
    
    NSLog(@"set up voice");
}

- (void)becomeActive
{
    if(isInit == true)
    {
        [self stop];
    }
}

- (void)InitController
{
    if (audioController) {
        [audioController destroy];
    }
    
    audioController = [[JZAudioController alloc] initWithIsCapture:true];
    audioController.streamerDelegate = self;
    [audioController start];

}

- (void)start
{
    if(![self canRecord] && [self _isCheckMicOpen])
    {
        [self showAlert];
        return;
    }
    
    if([JZAudioManager sharedInstance]._isMicModel != 1)
    {
        [[JZAudioSession sharedInstance] setupRecordSession:audioSampleRate bufferSize:1024];
    }
    
    [self InitController];
    
    
    if (audioController) {
        
        audioController.isStreaming = true;
    }
    
    
}

- (void)stop
{
    if([JZAudioManager sharedInstance]._isMicModel != 1)
    {
        [[JZAudioSession sharedInstance] setupPlaySession:audioSampleRate bufferSize:1024];
    }
    
    [self InitController];
    
    if (audioController) {
        
        audioController.isStreaming = false;
    }
}

- (void)registerMic:(NSString *)roomIdandteamId
{
    NSArray *pArr = [roomIdandteamId componentsSeparatedByString:@"|"];
    UInt32 rId =(UInt32)[pArr[0] integerValue];
    UInt32 tId =(UInt32)[pArr[1] integerValue];
    UInt64 guanzhanID = [pArr[2] doubleValue];
    
    NSMutableData *data = [[NSMutableData alloc] init];
    char micType = RegisterMic;
    UInt64 playerId = self.playerId;
    [data appendBytes:&micType length:1];
    [data appendBytes:&playerId length:8];
    [data appendBytes:&rId length:4];
    [data appendBytes:&tId length:4];
    if (guanzhanID>0) {
        [data appendBytes:&guanzhanID length:8];
    }
    [[JZUDPClient sharedInstance] send:data];
    _isRegister = true;
    NSLog(@"register data:%@", data);
}

- (void)deRegisterMic:(NSString *)roomId
{
    UInt32 rId =(UInt32)[roomId integerValue];
    NSMutableData *data = [[NSMutableData alloc] init];
    char micType = DegisterMic;
    UInt64 playerId = self.playerId;
    [data appendBytes:&micType length:1];
    [data appendBytes:&playerId length:8];
    [data appendBytes:&rId length:4];
    [[JZUDPClient sharedInstance] send:data];
    _isRegister = false;
    NSLog(@"unregister data:%@", data);
    //[self stop];
    //if (audioController) {
    //    [audioController destroy];
    //}
}

- (void)shieldSpeakMic:(NSString *)MIDandUID
{
    NSArray *pArr = [MIDandUID componentsSeparatedByString:@"|"];
    UInt64 mId = [pArr[0] doubleValue];
    UInt64 uId = [pArr[1] doubleValue];
    
    NSMutableData *data = [[NSMutableData alloc] init];
    char micType = MESSAGE_SHIELD_VOIVE;
    [data appendBytes:&micType length:1];
    [data appendBytes:&mId length:8];
    [data appendBytes:&uId length:8];
    
    [[JZUDPClient sharedInstance] send:data];
}

- (void)backShieldSpeakMic:(NSString *)MIDandUID
{
    NSArray *pArr = [MIDandUID componentsSeparatedByString:@"|"];
    UInt64 mId = [pArr[0] doubleValue];
    UInt64 uId = [pArr[1] doubleValue];
    
    NSMutableData *data = [[NSMutableData alloc] init];
    char micType = MESSAGE_BACK_SHIELD_VOIVE;
    [data appendBytes:&micType length:1];
    [data appendBytes:&mId length:8];
    [data appendBytes:&uId length:8];
    
    [[JZUDPClient sharedInstance] send:data];
}



- (void)onGetAudioBuffer:(AudioBuffer *)buffer pts:(double)pts
{
    //        NSLog(@"audio pcm data:");
    //    RTMP_LogHexString(RTMP_LOGERROR, (uint8_t*)buffer->mData, buffer->mDataByteSize);
    
    //dispatch_async(_captureAudioQueue, ^{
        memcpy(_buffer + bufferLen, buffer->mData, buffer->mDataByteSize);
        bufferLen += buffer->mDataByteSize;
        if (bufferLen < 480) {
            return;
        }
        uint8_t *temp = calloc(1, 480);
        memcpy(temp, _buffer, 480);
        [_encoder onGetAudioData:temp size:480];
//         [_encoder onGetAudioData:buffer->mData size:buffer->mDataByteSize];
        bufferLen -= 480;
        memcpy(_buffer, _buffer + 480, bufferLen);
    //});
}

- (void)onRecvData:(NSData *)data
{
    if(_isRegister==true)
    {
        [_encoder ilbcDecode:[data bytes] size:data.length];
    }
}

- (BOOL)canRecord
{
    __block BOOL bCanRecord = YES;
    if ([[[UIDevice currentDevice]systemVersion]floatValue] >= 7.0) {
        AVAudioSession *audioSession = [AVAudioSession sharedInstance];
        if ([audioSession respondsToSelector:@selector(requestRecordPermission:)]) {
            [audioSession performSelector:@selector(requestRecordPermission:) withObject:^(BOOL granted) {
                if (granted) {
                    bCanRecord = YES;
                } else {
                    bCanRecord = NO;
                }
            }];
        }
    }
    return bCanRecord;
}

- (void)showAlert
{
    UIAlertView *alertView = [[UIAlertView alloc]
                              initWithTitle:@"无法录音"
                              message:@"请在 设置-隐私-麦克风 选项中,允许球球大作战访问你的麦克风" delegate:self
                              cancelButtonTitle:@"取消"
                              otherButtonTitles:@"设置", nil];
    [alertView show];
    
    /*
     NSString* message = @"请在手机设置中开启定位服务以看到附近位置";
     UIAlertView *alertView = [[UIAlertView alloc]
     initWithTitle:@"定位服务未开启"
     message:message delegate:self
     cancelButtonTitle:@"取消"
     otherButtonTitles:@"设置", nil];
     
     [alertView show];*/
}
- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex
{
    [alertView dismissWithClickedButtonIndex:buttonIndex animated:YES];
    if (buttonIndex) {
        if ([[[UIDevice currentDevice]systemVersion]floatValue] >= 8.0) {
            NSURL *url = [NSURL URLWithString:UIApplicationOpenSettingsURLString];
            if ([[UIApplication sharedApplication] canOpenURL:url]) {
                [[UIApplication sharedApplication] openURL:url];
            }
        }else{
            [[UIApplication sharedApplication] openURL:[NSURL URLWithString:@"prefs:root=Sounds"]];
        }
        
        
        
    }
}

- (void)setRecordModel:(UInt32)params
{
    [JZAudioManager sharedInstance]._isMicModel = params;
    if([JZAudioManager sharedInstance]._isMicModel == 1)
    {
        [[JZAudioSession sharedInstance] setupRecordSession:audioSampleRate bufferSize:1024];
    }else if([JZAudioManager sharedInstance]._isMicModel == 0){
        [[JZAudioSession sharedInstance] setupPlaySession:audioSampleRate bufferSize:1024];
    }
}



@end
