#include "DevicesManager.h"
#include "../Cytron.Custom/CytronSmartDrive.h"
#include "../Cytron.Custom/CytronSmartDriveDuo.h"

DevicesManager::DevicesManager() : deviceMotor(23, 24), mainMotors(6,22, 5, 27) {
	servosDriver.init(1, 0x40);
	servosDriver.setPWMFreq(50);

}

void DevicesManager::setDeviceMotor(int direction, int speed) {
	deviceMotor.setMotor(speed);
}
void DevicesManager::setMainMotors(int m1_direction, int m2_direction, int m1_speed, int m2_speed) {
	mainMotors.setMotors(m1_direction, m2_direction, m1_speed, m2_speed);
}
void DevicesManager::setServo(int value) {
	servosDriver.setPWM(0, 0, (150 + (value * 450) / 100));
}                          