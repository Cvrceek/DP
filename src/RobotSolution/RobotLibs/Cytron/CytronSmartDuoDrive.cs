using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotLibs.Cytron
{
    public class CytronSmartDuoDrive : IDisposable
    {
        private readonly CytronSmartDrive m1;
        private readonly CytronSmartDrive m2;

        public CytronSmartDuoDrive(int m1_pwm, int m1_dir, int m2_pwm, int m2_dir)
        {
            m1 = new CytronSmartDrive(m1_pwm, m1_dir);
            m2 = new CytronSmartDrive(m2_pwm, m2_dir);
        }
        public void SetMotors(int m1_direction, int m2_direction, int m1_speed, int m2_speed)
        {
            // Kvůli bugu na driveru je nutné jednu hodnotu vždy invertovat
            if (m1_direction == 0 && m2_direction == 0)
            {
                m1.SetMotor(m1_speed);
                m2.SetMotor(-m2_speed);
            }
            else if (m1_direction == 1 && m2_direction == 1)
            {
                m1.SetMotor(-m1_speed);
                m2.SetMotor(m2_speed);
            }
        }
        public void Dispose()
        {
            m1.Dispose();
            m2.Dispose();
        }
    }
}
