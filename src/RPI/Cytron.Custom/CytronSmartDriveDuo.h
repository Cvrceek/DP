#pragma once
#include "CytronSmartDrive.h"
class CytronSmartDriveDuo
{
private: 
	CytronSmartDrive m1;
	CytronSmartDrive m2;

public:
	CytronSmartDriveDuo(int m1_pwm, int m1_dir, int m2_pwm, int m2_dir);
	void setMotors(int m1_direction, int m2_direction, int m1_speed, int m2_speed);
};

