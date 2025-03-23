#pragma once

#include <vector>
#include <cstdint>

//https://www.digi.com/resources/documentation/Digidocs/90001942-13/concepts/c_api_frame_structure.htm?tocpath=XBee%20API%20mode%7C_____2

class XbeeFrame
{
public:
	uint8_t startDelimiter;
	uint8_t msb;
	uint8_t lsb;
	uint8_t frameType;
	std::vector<uint8_t> data;
	uint8_t checkSum;
	uint16_t length;
	XbeeFrame parseFromVector(const std::vector<uint8_t>& values);
};

