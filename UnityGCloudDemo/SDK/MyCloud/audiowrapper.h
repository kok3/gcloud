//
//  audiowrapper.h
//  ILbcDemo
//
//  Created by 周振宇 on 15/11/28.
//  Copyright (c) 2015年 Jared. All rights reserved.
//

#ifndef ILbcDemo_audiowrapper_h
#define ILbcDemo_audiowrapper_h
int encode(short *samples, unsigned char *data);

int decode(unsigned char *data, short *samples, int mode);

void initiLbc(int mode) ;
#endif
