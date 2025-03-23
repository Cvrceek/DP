#pragma once

#include <wiringPi.h>
#include <softPwm.h>
#include <stdio.h>
#include <iostream>
class CytronSmartDrive
{
private:
	int pinPWM, pinDIR;

public:
	CytronSmartDrive(int pwm, int dir);
	void setMotor(int speed);
	void stopMotor();
};

