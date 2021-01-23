using System;

namespace Entrenamiento.Nucleo
{
    public static class Instrumentacion
    {
        /// <summary>
        /// Obtiene la cantidad de valores necesarios para representar correctamente un instrumento dado.
        /// </summary>
        /// <param name="instrumento">Nombre del instrumento del que se quiere conocer la cantidad de valores.</param>
        /// <returns>Cantidad de valores.</returns>
        public static int ObtenerCantidadDeValoresDelInstrumento(NombresDeInstrumentos instrumento)
        {
            switch (instrumento)
            {
                case NombresDeInstrumentos.Dual_Tach:
                case NombresDeInstrumentos.Fuel_Preassure_Loadmeter:
                case NombresDeInstrumentos.Engine_Oil_Temp_Press:
                case NombresDeInstrumentos.XMSN_Oil_Temp_Press:
                case NombresDeInstrumentos.Inclinometer:
                case NombresDeInstrumentos.TURN_AND_SLIP:
                    return 2;

                default:
                    return 1;
            }
        }

        /// <summary>
        /// Obtiene un objeto de valores en cero que puede representar correctamente un instrumento dado.
        /// </summary>
        /// <param name="instrumento">Nombre del instrumento que se quiere representar.</param>
        /// <returns>Objeto con el que se pueden representar los valores del instrumento.</returns>
        public static ValoresDeInstrumento ObtenerValoresVaciosDeInstrumento(NombresDeInstrumentos instrumento)
        {
            ValoresDeInstrumento valores = new ValoresDeInstrumento();
            valores.Cantidad = Instrumentacion.ObtenerCantidadDeValoresDelInstrumento(instrumento);
            return valores;
        }

        /// <summary>
        /// Instancias un instrumentos a partir del nombre del instrumento.
        /// </summary>
        /// <param name="instrumento"></param>
        /// <returns></returns>
        public static Instrumento InstanciarInstrumentoPorNombre(NombresDeInstrumentos nombreDeInstrumento, ModelosDeHelicoptero modelo)    
        {
            Instrumento instrumento = null;
            switch (nombreDeInstrumento)
            {
                case NombresDeInstrumentos.AirSpeed:
                    instrumento = new Instrumentos.AirSpeed();
                    break;

                case NombresDeInstrumentos.Altimeter:
                    instrumento = new Instrumentos.Altimeter();
                    break;

                case NombresDeInstrumentos.Ambiente:
                    instrumento = new Instrumentos.Ambiente();
                    break;

                case NombresDeInstrumentos.Battery_RLY_Light:
                    instrumento = new Instrumentos.Battery_RLYLight();
                    break;

                case NombresDeInstrumentos.Battery_Hot_Light:
                    instrumento = new Instrumentos.BatteryHotLight();
                    break;

                case NombresDeInstrumentos.DIGITAL_CHRONOMETER:
                    instrumento = new Instrumentos.DIGITAL_CHRONOMETER();
                    break;

                case NombresDeInstrumentos.DIRECTIONAL_GYRO:
                    instrumento = new Instrumentos.DIRECTIONAL_GYRO();
                    break;

                case NombresDeInstrumentos.Dual_Tach:
                    switch (modelo)
                    {
                        case ModelosDeHelicoptero.B206B3:
                            instrumento = new Instrumentos.Dual_Tach_206B3();
                            break;

                        case ModelosDeHelicoptero.B206L3:
                            instrumento = new Instrumentos.Dual_Tach_206L3();
                            break;

                        case ModelosDeHelicoptero.B206L4:
                            instrumento = new Instrumentos.Dual_Tach_206L4();
                            break;
                    }
                    break;

                case NombresDeInstrumentos.Engine_Chip_Light:
                    instrumento = new Instrumentos.ENG_CHIPLight();
                    break;

                case NombresDeInstrumentos.Eng_Out_Light:
                    instrumento = new Instrumentos.ENG_OutLight();
                    break;

                case NombresDeInstrumentos.Engine_Oil_Temp_Press:
                    instrumento = new Instrumentos.ENGINE_OIL_PRESSURE_TEMPERATURE();
                    break;

                case NombresDeInstrumentos.Fuel_Filter_Light:
                    instrumento = new Instrumentos.Fuel_Filter_Light();
                    break;

                case NombresDeInstrumentos.Fuel_Preassure_Loadmeter:
                    switch (modelo)
                    {
                        case ModelosDeHelicoptero.B206B3:
                            instrumento = new Instrumentos.FUEL_PRESSURE_DC_LOADMETER_206B3();
                            break;

                        case ModelosDeHelicoptero.B206L3:
                            instrumento = new Instrumentos.FUEL_PRESSURE_DC_LOADMETER_206L3();
                            break;

                        case ModelosDeHelicoptero.B206L4:
                            instrumento = new Instrumentos.FUEL_PRESSURE_DC_LOADMETER_206L4();
                            break;
                    }
                    break;

                case NombresDeInstrumentos.Fuel_Quantity:
                    switch (modelo)
                    {
                        case ModelosDeHelicoptero.B206B3:
                            instrumento = new Instrumentos.Fuel_Quantity_206B3();
                            break;

                        case ModelosDeHelicoptero.B206L3:
                        case ModelosDeHelicoptero.B206L4:
                            instrumento = new Instrumentos.FUEL_QUANTITY();
                            break;
                    }
                    break;

                case NombresDeInstrumentos.Fuel_Low:
                    instrumento = new Instrumentos.FuelLowLight();
                    break;

                case NombresDeInstrumentos.Gas_Producer:
                    instrumento = new Instrumentos.GAS_PRODUCER();
                    break;

                case NombresDeInstrumentos.Gen_Fail:
                    instrumento = new Instrumentos.GEN_FailLight();
                    break;

                case NombresDeInstrumentos.HOURMETER:
                    instrumento = new Instrumentos.HOURMETER();
                    break;

                case NombresDeInstrumentos.Inclinometer:
                    instrumento = new Instrumentos.Inclinometer();
                    break;

                case NombresDeInstrumentos.L_Fuel_Pump_Light:
                    instrumento = new Instrumentos.L_Fuel_PumpLight();
                    break;

                case NombresDeInstrumentos.Litter_Door_Open_Light:
                    instrumento = new Instrumentos.Litter_Door_Open_Light();
                    break;

                case NombresDeInstrumentos.MAGNETIC_COMPASS:
                    instrumento = new Instrumentos.MAGNETIC_COMPASS();
                    break;

                case NombresDeInstrumentos.OUTSIDE_AIR_TEMPERATURE_GAUGE:
                    instrumento = new Instrumentos.OUTSIDE_AIR_TEMPERATURE_GAUGE();
                    break;

                case NombresDeInstrumentos.R_Fuel_Pump_Light:
                    instrumento = new Instrumentos.R_Fuel_PumpLight();
                    break;

                case NombresDeInstrumentos.Rotor_Low_RPM_Light:
                    instrumento = new Instrumentos.Rotor_Low_RPMLight();
                    break;

                case NombresDeInstrumentos.TR_Chip_Light:
                    instrumento = new Instrumentos.T_R_ChipLight();
                    break;

                case NombresDeInstrumentos.TorqueMeter:
                    switch (modelo)
                    {
                        case ModelosDeHelicoptero.B206B3:
                        case ModelosDeHelicoptero.B206L3:
                            instrumento = new Instrumentos.Torquemeter_B3_y_L3();
                            break;

                        case ModelosDeHelicoptero.B206L4:
                            instrumento = new Instrumentos.Torquemeter_L4();
                            break;
                    }
                    break;

                case NombresDeInstrumentos.Trans_Chip_Light:
                    instrumento = new Instrumentos.Trans_ChipLight();
                    break;

                case NombresDeInstrumentos.Trans_Oil_Press_Light:
                    instrumento = new Instrumentos.Trans_Oil_PressLight();
                    break;

                case NombresDeInstrumentos.Trans_Oil_Temp_Light:
                    instrumento = new Instrumentos.Trans_Oil_TempLight();
                    break;

                case NombresDeInstrumentos.Turbine_Outlet_Temp:
                    switch (modelo)
                    {
                        case ModelosDeHelicoptero.B206B3:
                            instrumento = new Instrumentos.TURBINE_OUTLET_TEMPERATURE_206B3();
                            break;

                        case ModelosDeHelicoptero.B206L3:
                        case ModelosDeHelicoptero.B206L4:
                            instrumento = new Instrumentos.TURBINE_OUTLET_TEMPERATURE_206L3_Y_206L4();
                            break;
                    }
                    
                    break;

                case NombresDeInstrumentos.TURN_AND_SLIP:
                    instrumento = new Instrumentos.TURN_AND_SLIP();
                    break;

                case NombresDeInstrumentos.VERTICAL_SPEED:
                    instrumento = new Instrumentos.VERTICAL_SPEED();
                    break;

                case NombresDeInstrumentos.XMSN_Oil_Temp_Press:
                    instrumento = new Instrumentos.TRANSMISSION_OIL_PRESSURE_TEMPERATURE();
                    break;
            }

            return instrumento;
        }

    }
}