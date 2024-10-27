#pragma once

#include <iostream>
#include <vector>
#include <thread>
#include <atomic>
#include <cstring>
#include <unistd.h>
#include <fcntl.h>
#include <termios.h>
#include <sys/ioctl.h>
#include <functional>
#include <algorithm>
#include "XbeeFrame.h"

class XBeeSerialPort {
public:
    using DataReceivedCallback = std::function<void(const XbeeFrame&)>;

    XBeeSerialPort(const std::string& portName, int baudRate = B9600, int dataBits = CS8, int stopBits = 1, char parity = 'N');
    ~XBeeSerialPort();

    void openPort();
    void closePort();
    void setDataReceivedHandler(DataReceivedCallback callback);
    void startReading();  // Spustí čtení ve vlákně
    void stopReading();   // Zastaví čtení a ukončí vlákno

private:
    std::string portName;
    int fd;
    struct termios tty;
    DataReceivedCallback dataReceivedHandler;
    std::vector<char> dataStack;
    std::atomic<bool> reading;  // Pro řízení čtení ve vlákně
    std::thread readerThread;   // Vlákno pro čtení

    void readData();      // Interní metoda pro čtení dat
    void readingLoop();   // Hlavní smyčka pro vlákno
    uint8_t calculateChecksum(const std::vector<uint8_t>& frameData);
};

