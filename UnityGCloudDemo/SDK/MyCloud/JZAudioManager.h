//
//  JZAudioManager.h
//  Unity-iPhone
//
//  Created by 周振宇 on 15/11/29.
//
//

#import <Foundation/Foundation.h>

#define  Null 0
#define  RegisterMic  1
#define  MicSamples  2
#define  DegisterMic  3
#define  MicSamples2  4
#define  VoiceAnchor  7
#define  MESSAGE_SHIELD_VOIVE  8
#define  MESSAGE_BACK_SHIELD_VOIVE  9

@interface JZAudioManager : NSObject

+ (JZAudioManager *)sharedInstance;

- (void)setIP:(NSString*)params;
- (void)setup;
- (void)registerMic:(NSString *)roomIdandteamId;
- (void)deRegisterMic:(NSString *)roomId;
- (void)start;
- (void)stop;
- (void)becomeActive;
- (void)shieldSpeakMic:(NSString *)MIDandUID;
- (void)backShieldSpeakMic:(NSString *)MIDandUID;
- (void)setRecordModel:(BOOL)params;
@property (nonatomic) NSInteger port;
@property (nonatomic, strong) NSString *host;
@property (nonatomic) UInt64 playerId;
@property (nonatomic) UInt32 _inNumberFrames;
@property (nonatomic) bool _isLiving;
@property (nonatomic) bool _isRecordSpeak;
@property (nonatomic) bool _isCheckMicOpen;
@property (nonatomic) UInt32 _isMicModel;
@property (nonatomic, strong) NSString *_urlParamString;
@property (nonatomic) bool _isNewSetUDPIP;
@end
