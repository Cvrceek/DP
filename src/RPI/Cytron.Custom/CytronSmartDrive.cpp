#include "CytronSmartDrive.h"
#include <wiringPi.h>
#include <softPwm.h>

/// <summary>
/// CTOR pro motor driver, Nastavit  wiringPiSetupGpio(); v Main!!!
/// </summary>
/// <param name="pwm">pin dle wiringPI gpio</param>
/// <param name="dir">pin dle wiringPI gpio</param>
CytronSmartDrive::CytronSmartDrive(int pwm, int dir) : pinPWM(pwm), pinDIR(dir)
{
    pinMode(pinPWM, OUTPUT);
    pinMode(pinDIR, OUTPUT);
    softPwmCreate(pinPWM, 0, 100); 
}

/// <summary>
/// Nastavi otacky motoru, if speed <0 => dir vzad
/// </summary>
/// <param name="speed"></param>
void CytronSmartDrive::setMotor(int speed)
{
    if (speed >= 0)
    {
        digitalWrite(pinDIR, HIGH);  
        softPwmWrite(pinPWM, speed); 
    }
    else
    {
        digitalWrite(pinDIR, LOW);  
        softPwmWrite(pinPWM, -speed); 
    }
}
/// <summary>
/// Stopne motor, smer nastaven dopredu
/// </summary>
void CytronSmartDrive::stopMotor() {
    digitalWrite(pinDIR, HIGH);
    softPwmWrite(pinPWM, 0);
}