#include "XBeeSerialPort.h"

// Konstruktor
XBeeSerialPort::XBeeSerialPort(const std::string& portName, int baudRate, int dataBits, int stopBits, char parity)
    : portName(portName), fd(-1), reading(false) {
    memset(&tty, 0, sizeof(tty));
}

// Destruktor
XBeeSerialPort::~XBeeSerialPort() {
    stopReading();
    closePort();
}

// Otevření sériového portu
void XBeeSerialPort::openPort() {
    fd = open(portName.c_str(), O_RDWR | O_NOCTTY | O_NDELAY);
    if (fd == -1) {
        std::cerr << "Error opening port" << std::endl;
        return;
    }
    else {
        std::cout << "Port opened" << std::endl;
    }

    if (tcgetattr(fd, &tty) != 0) {
        std::cerr << "Error getting termios attributes" << std::endl;
        closePort();
        return;
    }
    else
    {
        std::cout << "Port setted" << std::endl;
    }

    tty.c_cflag = B9600 | CS8 | CLOCAL | CREAD;
    tty.c_iflag = IGNPAR;
    tty.c_oflag = 0;
    tty.c_lflag = 0;

    /* Apply the settings */
    tcflush(fd, TCIFLUSH);

    tcsetattr(fd, TCSANOW, &tty); // Apply the settings
}

// Zavření sériového portu
void XBeeSerialPort::closePort() {
    if (fd != -1) {
        close(fd);
        fd = -1;
    }
}

// Nastavení handleru pro zpracování přijatých dat
void XBeeSerialPort::setDataReceivedHandler(DataReceivedCallback callback) {
    dataReceivedHandler = callback;
}

// Hlavní smyčka pro čtení ve vlákně
void XBeeSerialPort::readingLoop() {
    while (reading) {
        readData();
        usleep(100000);  // Pauza 50 ms mezi kontrolou dostupných dat
    }
}

// Čtení dat ze sériového portu
void XBeeSerialPort::readData() {
    uint8_t buffer[256];
    int bytesAvailable = 0;
    ioctl(fd, FIONREAD, &bytesAvailable);

    if (bytesAvailable > 0) {
        int len = read(fd, buffer, sizeof(buffer));
        if (len > 0) {
            dataStack.insert(dataStack.end(), buffer, buffer + len);
            //0x7E
            while (dataStack.size() > 0) {
                // Najdi start delimiter (0x7E)
                auto startIt = std::find(dataStack.begin(), dataStack.end(), 0x7E);
                if (startIt == dataStack.end()) {
                    // Žádný start delimiter
                    return;
                }

                // Odstranění předcházejících dat před start delimiterem
                dataStack.erase(dataStack.begin(), startIt);

                if (dataStack.size() < 3) {
                    // Nejsou dostatečná data pro délku rámce
                    return;
                }

                // Zjištění délky rámce
                uint8_t lengthMSB = dataStack[1];
                uint8_t lengthLSB = dataStack[2];
                int length = (lengthMSB << 8) | lengthLSB;

                // Kontrola, jestli máme dostatek dat pro celý rámec
                if (dataStack.size() < length + 4) { // Start delimiter + Length + Frame + Checksum
                    return;
                }

                // Zkopírování dat rámce
                std::vector<uint8_t> frameData(dataStack.begin() + 3, dataStack.begin() + 3 + length);
                // Zjištění kontrolního součtu
                uint8_t checksum = dataStack[length + 3];
                if (checksum != calculateChecksum(frameData)) {
                    std::cerr << "Checksum mismatch" << std::endl;
                    dataStack.erase(dataStack.begin(), dataStack.begin() + length + 4); // Odstranění neplatného rámce
                    continue;
                }

                // Předání dat zpět prostřednictvím callbacku
                if (dataReceivedHandler) {
                    XbeeFrame frameDto;
                    frameDto.startDelimiter = dataStack[0];
                    frameDto.msb = dataStack[1];
                    frameDto.lsb = dataStack[2];
                    frameDto.length = length;
                    frameDto.data = frameData;
                    frameDto.frameType = dataStack[3];
                    frameDto.checkSum = dataStack[length + 3];
                    dataReceivedHandler(frameDto);
                }
                dataStack.erase(dataStack.begin(), dataStack.begin() + length + 4);
            }
        }
    }
    else {
        //std::cerr << "NO data" << std::endl;
    }
}

// Výpočet kontrolního součtu
uint8_t XBeeSerialPort::calculateChecksum(const std::vector<uint8_t>& frameData) {
    int sum = 0;
    for (uint8_t byte : frameData) {
        sum += byte;
    }
    return 0xFF - (sum & 0xFF);
}

// Spustí čtení ve vlákně
void XBeeSerialPort::startReading() {
    std::cout << "Start reading" << std::endl;
    reading = true;
    readerThread = std::thread(&XBeeSerialPort::readingLoop, this);

}

// Zastaví čtení a ukončí vlákno
void XBeeSerialPort::stopReading() {
    reading = false;
    if (readerThread.joinable()) {
        readerThread.join();  // Počkej, až vlákno dokončí
    }
}
