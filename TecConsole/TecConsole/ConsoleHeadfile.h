#ifndef _CONSOLEHEADFILE_H
#define _CONSOLEHEADFILE_H

#include <Windows.h>
//---------------------------------------------------------------------------
// Types and structs
//---------------------------------------------------------------------------
typedef struct _I2CReadData
{
	unsigned short nBus;
	unsigned short nSlaveAddress;
	unsigned short cpReadBuffer[10];/*XN_IO_MAX_I2C_BUFFER_SIZE*/
	unsigned short cpWriteBuffer[10];/*XN_IO_MAX_I2C_BUFFER_SIZE*/
	unsigned short nReadSize;
	unsigned short nWriteSize;
} I2CReadData;

typedef struct _TecData
{
	unsigned short m_SetPointVoltage;
	unsigned short m_CompensationVoltage;
	unsigned short m_TecDutyCycle; //duty cycle on heater/cooler
	unsigned short m_HeatMode; //TRUE - heat, FALSE - cool
	int m_ProportionalError;
	int m_IntegralError;
	int m_DerivativeError;
	unsigned short m_ScanMode; //0 - crude, 1 - precise
}TecData;

typedef struct _TecFastConvergenceData
{
	short     m_SetPointTemperature;  // set point temperature in celsius,
	// scaled by factor of 100 (extra precision)
	short     m_MeasuredTemperature;  // measured temperature in celsius,
	// scaled by factor of 100 (extra precision)
	int 	m_ProportionalError;    // proportional error in system clocks
	int 	m_IntegralError;        // integral error in system clocks
	int 	m_DerivativeError;      // derivative error in system clocks
	unsigned short 	m_ScanMode; // 0 - initial, 1 - crude, 2 - precise
	unsigned short    m_HeatMode; // 0 - idle, 1 - heat, 2 - cool
	unsigned short    m_TecDutyCycle; // duty cycle on heater/cooler in percents
	unsigned short	m_TemperatureRange;	// 0 - cool, 1 - room, 2 - warm
}TecFastConvergenceData;



#endif