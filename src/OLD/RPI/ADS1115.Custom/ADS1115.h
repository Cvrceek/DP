///original dostupny na https://github.com/DFRobot/DFRobot_ADS1115/blob/master/DFRobot_ADS1115.cpp
///odstraneno arduino
/// pridan koeficient dle privadeneho napeti na desku

#ifndef ADS1115_H
#define ADS1115_H

#include <cstdint>
#include <iostream>
#include <thread>
#include <chrono>
#include <fcntl.h>
#include <unistd.h>
#include <sys/ioctl.h>
#include <linux/i2c-dev.h>

class ADS1115 {
public:
    explicit ADS1115(uint8_t i2cAddress);
    ~ADS1115();

    bool checkADS1115();

    void setLowThreshold(int16_t threshold);
    int16_t getLowThreshold();

    void setHighThreshold(int16_t threshold);
    int16_t getHighThreshold();

    void setGain(uint8_t value);
    void setOSMode(uint8_t value);
    void setCompQue(uint8_t value);
    void setCompLat(uint8_t value);
    void setCompPol(uint8_t value);
    void setCompMode(uint8_t value);
    void setRate(uint8_t value);
    void setMode(uint8_t value);
    void setMux(uint8_t value);

    uint16_t readVoltage(uint8_t channel);

private:
    int ads_fd;
    uint8_t ads_i2cAddress;
    int16_t ads_lowthreshold = 0;
    int16_t ads_highthreshold = 0;
    int ads_conversionDelay = 8;
    //float coefficient = 0.0625;
    //float coefficient = 0.1526; 
    float coefficient = 0.2646;   //6.144/16383 
    //float coefficient = 0.30519440;  // 5/16383
    int16_t readAdsReg(uint8_t reg);
    void writeAdsReg(uint8_t reg, uint16_t value);

    static constexpr uint8_t DFROBOT_ADS1115_POINTER_CONFIG = 0x01;
    static constexpr uint8_t DFROBOT_ADS1115_POINTER_CONVERT = 0x00;
    static constexpr uint8_t DFROBOT_ADS1115_POINTER_LOWTHRESH = 0x02;
    static constexpr uint8_t DFROBOT_ADS1115_POINTER_HITHRESH = 0x03;
};

#endif // ADS1115_H
