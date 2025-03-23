#include "ADS1115.h"

ADS1115::ADS1115(uint8_t i2cAddress) : ads_i2cAddress(i2cAddress) {
    char filename[20];
    snprintf(filename, 19, "/dev/i2c-1");
    ads_fd = open(filename, O_RDWR);
    if (ads_fd < 0) {
        std::cerr << "Error opening I2C device" << std::endl;
        return;
    }
    if (ioctl(ads_fd, I2C_SLAVE, ads_i2cAddress) < 0) {
        std::cerr << "Failed to set I2C address" << std::endl;
        return;
    }
}

ADS1115::~ADS1115() {
    if (ads_fd >= 0) {
        close(ads_fd);
    }
}

bool ADS1115::checkADS1115() {
    return ads_fd >= 0;
}

void ADS1115::setLowThreshold(int16_t threshold) {
    ads_lowthreshold = threshold;
    writeAdsReg(DFROBOT_ADS1115_POINTER_LOWTHRESH, ads_lowthreshold);
}
void ADS1115::setMux(uint8_t value) {
    int16_t conf = readAdsReg(DFROBOT_ADS1115_POINTER_CONFIG);
    conf &= ~((uint16_t)0x07 << 12);
    conf |= (uint16_t)value << 12;
    writeAdsReg(DFROBOT_ADS1115_POINTER_CONFIG, conf);
}
int16_t ADS1115::getLowThreshold() {
    return ads_lowthreshold;
}

void ADS1115::setHighThreshold(int16_t threshold) {
    ads_highthreshold = threshold;
    writeAdsReg(DFROBOT_ADS1115_POINTER_HITHRESH, ads_highthreshold);
}

int16_t ADS1115::getHighThreshold() {
    return ads_highthreshold;
}

void ADS1115::setGain(uint8_t value) {
    int16_t conf = readAdsReg(DFROBOT_ADS1115_POINTER_CONFIG);
    conf &= ~((uint16_t)0x07 << 9);
    conf |= (uint16_t)value << 9;
    writeAdsReg(DFROBOT_ADS1115_POINTER_CONFIG, conf);

    switch (value) {
    case 0: coefficient = 0.0625; break;
    case 1: coefficient = 0.1875; break;
    case 2: coefficient = 0.125; break;
    case 3: coefficient = 0.03125; break;
    case 4: coefficient = 0.015625; break;
    case 5: coefficient = 0.0078125; break;
    default: coefficient = 0.0625; break;
   }

    
}

void ADS1115::setOSMode(uint8_t value) {
    int16_t conf = readAdsReg(DFROBOT_ADS1115_POINTER_CONFIG);
    conf &= ~((uint16_t)0x01 << 15);
    conf |= (uint16_t)value << 15;
    writeAdsReg(DFROBOT_ADS1115_POINTER_CONFIG, conf);
}

void ADS1115::setCompQue(uint8_t value) {
    int16_t conf = readAdsReg(DFROBOT_ADS1115_POINTER_CONFIG);
    conf &= ~((uint16_t)0x03);
    conf |= (uint16_t)value;
    writeAdsReg(DFROBOT_ADS1115_POINTER_CONFIG, conf);
}

void ADS1115::setCompLat(uint8_t value) {
    int16_t conf = readAdsReg(DFROBOT_ADS1115_POINTER_CONFIG);
    conf &= ~((uint16_t)0x01 << 2);
    conf |= (uint16_t)value << 2;
    writeAdsReg(DFROBOT_ADS1115_POINTER_CONFIG, conf);
}

void ADS1115::setCompPol(uint8_t value) {
    int16_t conf = readAdsReg(DFROBOT_ADS1115_POINTER_CONFIG);
    conf &= ~((uint16_t)0x01 << 3);
    conf |= (uint16_t)value << 3;
    writeAdsReg(DFROBOT_ADS1115_POINTER_CONFIG, conf);
}

void ADS1115::setCompMode(uint8_t value) {
    int16_t conf = readAdsReg(DFROBOT_ADS1115_POINTER_CONFIG);
    conf &= ~((uint16_t)0x01 << 4);
    conf |= (uint16_t)value << 4;
    writeAdsReg(DFROBOT_ADS1115_POINTER_CONFIG, conf);
}

void ADS1115::setRate(uint8_t value) {
    int16_t conf = readAdsReg(DFROBOT_ADS1115_POINTER_CONFIG);
    conf &= ~((uint16_t)0x07 << 5);
    conf |= (uint16_t)value << 5;
    writeAdsReg(DFROBOT_ADS1115_POINTER_CONFIG, conf);
}

void ADS1115::setMode(uint8_t value) {
    int16_t conf = readAdsReg(DFROBOT_ADS1115_POINTER_CONFIG);
    conf &= ~((uint16_t)0x01 << 8);
    conf |= (uint16_t)value << 8;
    writeAdsReg(DFROBOT_ADS1115_POINTER_CONFIG, conf);
}

uint16_t ADS1115::readVoltage(uint8_t channel) {
    if (channel > 3) return 0;

    setMux(channel + 5);

    std::this_thread::sleep_for(std::chrono::milliseconds(ads_conversionDelay));

    int16_t voltage = readAdsReg(DFROBOT_ADS1115_POINTER_CONVERT);
    return static_cast<uint16_t>(voltage * coefficient);
}

int16_t ADS1115::readAdsReg(uint8_t reg) {
    uint8_t buf[2] = { 0 };
    if (write(ads_fd, &reg, 1) != 1) return -1;
    if (read(ads_fd, buf, 2) != 2) return -1;
    int16_t res = (int16_t)(buf[0] << 8) | buf[1];
    return res;
}

void ADS1115::writeAdsReg(uint8_t reg, uint16_t value) {
    uint8_t buffer[3] = { reg, static_cast<uint8_t>(value >> 8), static_cast<uint8_t>(value & 0xFF) };
    write(ads_fd, buffer, 3);
}
