#include <iostream>
#include <vector>
#include <thread>
#include <chrono>
#include "XBeeSerialPort.h" 
#include <wiringPi.h>
#include "../Cytron.Custom/CytronSmartDriveDuo.h"
#include "../Cytron.Custom/CytronSmartDrive.h"
#include "DevicesManager.h"
//#include "../ADS1115.Custom/ADS1115.h"
#include <ads1115.h>


#define ADS_ADDR 0x48  // I2C adresa ADS1115 (default 0x48)
#define BASE 100       // WiringPi BASE hodnota

int main() {

  
    //CytronSmartDrive deviceMotor(23, 24);
    //CytronSmartDriveDuo mainMotors(16, 17, 27, 20);


      if (wiringPiSetupGpio() == -1) {
        printf("Chyba při inicializaci WiringPi\n");
        return 1;
    }

    //CytronSmartDrive m1(16,17);
    //CytronSmartDrive m2(20,27);

    //m1.setMotor(0);
    //m2.setMotor(0);

      ads1115Setup(BASE, ADS_ADDR);  // Inicializace ADS1115

      while (1) {
          for (int i = 0; i < 4; i++) {  // Projdi všechny 4 vstupy
              int adc_value = analogRead(BASE + i);
              float voltage = (adc_value / 32767.0) * 4.8;// 6.144;  // Gain = 0 → ±6.144V rozsah

              printf("AIN%d: ADC: %d, Napětí: %.3fV\n", i, adc_value, voltage);
          }
          printf("\n");
          delay(1000);  // Pauza 1 sekunda
      }

      return 0;
      
      
      
      
      
      
      
      
      
      
      
      
      
      
      
      
      
      DevicesManager devicesManager;









      //ADS1115 analogSenzorsManager(0x48);

      //if (!analogSenzorsManager.checkADS1115()) {
      //    std::cerr << "ADS1115 not detected!" << std::endl;
      //}


      //analogSenzorsManager.setMux(0x04);
      ////analogSenzorsManager.setGain(0);         // Zisk ±6.144V (možno změnit: 0 = ±6.144V, 1 = ±4.096V, 2 = ±2.048V, ...)
      //analogSenzorsManager.setOSMode(1);       // Jednorázový režim (1 = spuštění jednorázové konverze)
      //analogSenzorsManager.setCompQue(3);      // Komparátor vypnut
      //analogSenzorsManager.setCompLat(0);      // Nelatentní režim
      //analogSenzorsManager.setCompPol(0);      // Komparátor s nízkou úrovní
      //analogSenzorsManager.setCompMode(0);     // Tradiční komparátor
      //analogSenzorsManager.setRate(4);         // Rychlost vzorkování (4 = 128SPS)
      //analogSenzorsManager.setMode(1);         // Jednorázový režim měření


      //while (true) {
      //    uint16_t voltage = analogSenzorsManager.readVoltage(0); // Čtení z kanálu AIN0
      //    std::cout << "Napětí na pinu AIN0: " << voltage << " mV" << std::endl;
      //    uint16_t voltage1 = analogSenzorsManager.readVoltage(1); // Čtení z kanálu AIN0
      //    std::cout << "Napětí na pinu AIN1: " << voltage1 << " mV" << std::endl;
      //    uint16_t voltage2 = analogSenzorsManager.readVoltage(2); // Čtení z kanálu AIN0
      //    std::cout << "Napětí na pinu AIN2: " << voltage2 << " mV" << std::endl;
      //    uint16_t voltage3 = analogSenzorsManager.readVoltage(3); // Čtení z kanálu AIN0
      //    std::cout << "Napětí na pinu AIN3: " << voltage3 << " mV" << std::endl;

      //    std::this_thread::sleep_for(std::chrono::seconds(1)); // Pauza 1 sekunda
      //}



    
    XBeeSerialPort serial("/dev/ttyUSB0");








    serial.openPort();

    serial.setDataReceivedHandler([&devicesManager](const XbeeFrame& frameData) {
        std::cout << "Received frame: ";
        //for (auto byte : frameData) {
        //    printf("%02X ", byte);
        //    std::string result(frameData.begin() + 5, frameData.end());

        //    // Výpis výsledného řetězce
        //    std::cout << "Data: " << result << std::endl;
        //}

        //std::string result(frameData.begin() + 5, frameData.end());

        int type = static_cast<int>(frameData.data[5]);
        int dir_r = static_cast<int>(frameData.data[6]);
        int right = static_cast<int>(frameData.data[7]);
        int dir_l = static_cast<int>(frameData.data[8]);
        int left =  static_cast<int>(frameData.data[9]);

        if (type == 1)
            devicesManager.setMainMotors(static_cast<int>(dir_l), static_cast<int>(dir_r), static_cast<int>(left), static_cast<int>(right));
        else if (type == 2)
            devicesManager.setDeviceMotor(0, static_cast<int>(frameData.data[6]));
        else if (type == 3)
            devicesManager.setServo(static_cast<int>(frameData.data[6]));

        // Výpis výsledného řetězce
        std::cout << "Complete frame: " << static_cast<int>(dir_l) << " " << static_cast<int>(dir_r) << " " << static_cast<int>(left) << " " << static_cast<int>(right) << std::endl;
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