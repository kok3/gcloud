//
//  JZUDPClient.h
//  ILbcDemo
//
//  Created by 周振宇 on 15/11/29.
//  Copyright (c) 2015年 Jared. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol JZUDPClientDelegate <NSObject>

- (void)onRecvData:(NSData *)data;

@end
@interface JZUDPClient : NSObject

+ (JZUDPClient *)sharedInstance;

@property (nonatomic, weak) id<JZUDPClientDelegate> delegate;

- (void)send:(NSData *)data;

- (void)bingToPort;

@end
