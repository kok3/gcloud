//
//  JZILbcEncoder.h
//  ILbcDemo
//
//  Created by 周振宇 on 15/11/28.
//  Copyright (c) 2015年 Jared. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface JZILbcEncoder : NSObject

- (void)onGetAudioData:(uint8_t *)data size:(size_t)size;
- (void)ilbcDecode:(uint8_t *)data size:(size_t)size;
@end
