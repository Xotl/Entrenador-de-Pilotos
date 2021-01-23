using System;

namespace Entrenamiento.Nucleo
{
    /*
        eng out - warning
        battery hot - warning

        engine chip - caution
        Fuel pump - caution
        Fuel filter - caution
        Trans oil press - caution
        Trans oil temp - caution
        trans chip - caution
        rotor low rpm - caution
        battery temp - caution
        t/r Chip - caution
        AF fuel filter - caution
     */

    public enum NombresDeInstrumentos
    {
        Desconocido = -2,
        Ambiente,
        AirSpeed,
        Altimeter,
        Inclinometer,
        TorqueMeter,
        Battery_Hot_Light,
        Fuel_Low,
        Dual_Tach,
        Engine_Oil_Temp_Press,
        Fuel_Quantity,
        Gas_Producer,
        Fuel_Preassure_Loadmeter,
        Turbine_Outlet_Temp,
        XMSN_Oil_Temp_Press,
        Clock,
        AF_Fuel_filter_Light,
        Fuel_Filter_Light,
        Engine_Chip_Light,
        Trans_Oil_Press_Light,
        Eng_Out_Light,
        Fuel_Pump_Light,
        TR_Chip_Light,
        Trans_Chip_Light,
        Battery_Temp_Light,
        Trans_Oil_Temp_Light,
        Rotor_Low_RPM_Light,
        Gen_Fail,
        Baggage_Door,

        // De aquí para abajo son las del Chino
        Litter_Door_Open_Light,
        R_Fuel_Pump_Light,
        L_Fuel_Pump_Light,
        Battery_RLY_Light,
        TURN_AND_SLIP,
        VERTICAL_SPEED,
        DIRECTIONAL_GYRO,
        MAGNETIC_COMPASS,
        TRANSMISSION_OIL_PRESSURE_TEMPERATURE,
        LOADMETER,
        OUTSIDE_AIR_TEMPERATURE_GAUGE,
        DIGITAL_CHRONOMETER,
        HOURMETER,
        FUEL_PRESSURE_DC_LOADMETER,
    }
}