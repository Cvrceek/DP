#pragma once
#include "../Cytron.Custom/CytronSmartDrive.h"
#include "../Cytron.Custom/CytronSmartDriveDuo.h"
#include "../PCA9685/PCA9685.h"

class DevicesManager
{
private:
	CytronSmartDrive deviceMotor;
	CytronSmartDriveDuo mainMotors;
	PCA9685 servosDriver;

public:
	DevicesManager();
	void setDeviceMotor(int direction, int speed);
	void setMainMotors(int m1_direction, int m2_direction, int m1_speed, int m2_speed);
	void setServo(int value);
};

