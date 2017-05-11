//
//  JZAppDelegateConfig.h
//  Live
//
//  Created by zhangzhiyi on 15/5/7.
//  Copyright (c) 2015年 juzi. All rights reserved.
//

#ifndef Live_JZAppDelegateConfig_h
#define Live_JZAppDelegateConfig_h

#define iPhoneUDID  [[UIDevice currentDevice] identifierForVendor].UUIDString
#define APPDELEGATE ((JZAppDelegate*)[[UIApplication sharedApplication] delegate])

#define  audioIsStereo  0
#define  audioSampleSize 16
#define  audioSampleRate 8000
#define  ascType 4
#define  audioBitrate 64000
#define  audioIOBufferDuration (128.0/audioSampleRate)

#ifndef max
#define max( a, b ) ( ((a) > (b)) ? (a) : (b) )
#endif

#ifndef min
#define min( a, b ) ( ((a) < (b)) ? (a) : (b) )
#endif
/*
 * 宏定义结束
 */

#endif
