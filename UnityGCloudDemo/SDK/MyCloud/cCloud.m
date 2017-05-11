#include "cCloud.h"
#import "JZAudioManager.h"



#if defined(__cplusplus)
extern "C"
{
#endif
    void cStartSpeaker()
    {
        NSLog(@"cStartSpeaker111");
        [[JZAudioManager sharedInstance] setIP:(NSString*)@"192.168.94.250:5656 | 1234"];
        [[JZAudioManager sharedInstance] setup];
        [[JZAudioManager sharedInstance] start];
    }

    void cStopSpeaker()
    {
        [[JZAudioManager sharedInstance] stop];
        NSLog(@"cStopSpeaker222");
    }

    void cStartMic()
    {
        [[JZAudioManager sharedInstance] registerMic: @"123|456|789"];
        
        NSLog(@"cStartMic333");
    }

    void cStopMic()
    {
        [[JZAudioManager sharedInstance] deRegisterMic: @"ppdzroom123456"];
        NSLog(@"cStopMic444");
    }

    void cEnterRoom()
    {
        NSLog(@"cEnterRoom");
        [[JZAudioManager sharedInstance] setup];
    }

    void cQuitRoom()
    {
        NSLog(@"cQuitRoom");
    }

#if defined(__cplusplus)
}
#endif
