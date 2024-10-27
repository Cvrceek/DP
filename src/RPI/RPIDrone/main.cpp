#include <iostream>
#include <vector>
#include <thread>
#include <chrono>
#include "XBeeSerialPort.h" 
#include <wiringPi.h>
#include "CytronSmartDriveDuo.h"



int main() {

    wiringPiSetupGpio();

    CytronSmartDriveDuo mainMotors(16, 17, 20, 27);

    //CytronSmartDrive m1(16,17);
    //CytronSmartDrive m2(20,27);

    //m1.setMotor(0);
    //m2.setMotor(0);

    
    XBeeSerialPort serial("/dev/ttyUSB0");








    serial.openPort();

    serial.setDataReceivedHandler([&mainMotors](const XbeeFrame& frameData) {
        std::cout << "Received frame: ";
        //for (auto byte : frameData) {
        //    printf("%02X ", byte);
        //    std::string result(frameData.begin() + 5, frameData.end());

        //    // Výpis výsledného řetězce
        //    std::cout << "Data: " << result << std::endl;
        //}

        //std::string result(frameData.begin() + 5, frameData.end());


        int dir_r = static_cast<int>(frameData.data[5]);
        int right = static_cast<int>(frameData.data[6]);
        int dir_l = static_cast<int>(frameData.data[7]);
        int left =  static_cast<int>(frameData.data[8]);


       // mainMotors.setMotors(dir_l, dir_r, left, right);

        // Výpis výsledného řetězce
        std::cout << "Complete frame: " << static_cast<int>(dir_r) << " " << static_cast<int>(left) << " " << static_cast<int>(right) << std::endl;
        std::cout << std::endl;
        });

    serial.startReading();  // Spustí čtení ve vlákně

    //// Simulace nějaké práce v hlavním vlákně
    //std::this_thread::sleep_for(std::chrono::seconds(10));


    while (true) {
        sleep(10);
    }
    //serial.stopReading();  // Zastaví čtení a ukončí vlákno

    //serial.closePort();

    return 0;
}