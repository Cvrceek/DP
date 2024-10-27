#include "CytronSmartDriveDuo.h"
#include "CytronSmartDrive.h"

/// <summary>
/// CTOR pro motor driver 
/// </summary>
/// <param name="m1_pwm">pin dle wiringPI gpio</param>
/// <param name="m1_dir">pin dle wiringPI gpio</param>
/// <param name="m2_pwm">pin dle wiringPI gpio</param>
/// <param name="m2_dir">pin dle wiringPI gpio</param>
CytronSmartDriveDuo::CytronSmartDriveDuo(int m1_pwm, int m1_dir, int m2_pwm, int m2_dir) : m1(m1_pwm, m1_dir), m2(m2_pwm, m2_dir) 
{

}

/// <summary>
/// Nastavi otacky motoru, dle predanych hodnot
/// </summary>
/// <param name="direction"></param>
/// <param name="m1_speed">left</param>
/// <param name="m2_speed">right</param>
void CytronSmartDriveDuo::setMotors(int m1_direction, int m2_direction, int m1_speed, int m2_speed) {
	
	//kvuli bagu primo na dirveru, nutne nastavit jednu hodnotu vzdy na zapornou
	if (m1_direction == 0 && m2_direction == 0) {
		m1.setMotor(m1_speed);
		m2.setMotor(-1 * m2_speed);
	}
	if (m1_direction == 1 && m2_direction == 1) {
		m1.setMotor(-1 * m1_speed);
		m2.setMotor(m2_speed);
	}
}

