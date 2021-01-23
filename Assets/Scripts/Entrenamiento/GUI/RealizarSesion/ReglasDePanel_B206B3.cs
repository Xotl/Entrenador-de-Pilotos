using System;
using Entrenamiento.Nucleo;

namespace Entrenamiento.GUI.RealizarSesion
{
    class ReglasDePanel_B206B3 : ReglasDePanelDeInstrumentos
    {
        protected override void inicializarReglas()
        {
            this.AgregarRegla(NombresDeInstrumentos.Gas_Producer, reglaGasProducer);
            this.AgregarRegla(NombresDeInstrumentos.Dual_Tach, reglaDualTach);
            this.AgregarRegla(NombresDeInstrumentos.XMSN_Oil_Temp_Press, reglaXMNS_Oil_Temp);
            this.AgregarRegla(NombresDeInstrumentos.Fuel_Quantity, reglaFuelQty);
        }

        private void reglaGasProducer(ValoresDeInstrumento valores)
        {
            if (valores[0] < 55)
            {
                this.ModificarValor(NombresDeInstrumentos.Eng_Out_Light, new ValoresDeInstrumento(1));
            }
            else
            {
                this.ModificarValor(NombresDeInstrumentos.Eng_Out_Light, new ValoresDeInstrumento(0));
            }
        }

        private void reglaDualTach(ValoresDeInstrumento valores)
        {
            if (valores[0] < 90)
            {
                this.ModificarValor(NombresDeInstrumentos.Rotor_Low_RPM_Light, new ValoresDeInstrumento(1));
            }
            else
            {
                this.ModificarValor(NombresDeInstrumentos.Rotor_Low_RPM_Light, new ValoresDeInstrumento(0));
            }
        }

        private void reglaXMNS_Oil_Temp(ValoresDeInstrumento valores)
        {
            if (valores[0] < 30)
            {
                this.ModificarValor(NombresDeInstrumentos.Trans_Oil_Press_Light, new ValoresDeInstrumento(1));
            }
            else
            {
                this.ModificarValor(NombresDeInstrumentos.Trans_Oil_Press_Light, new ValoresDeInstrumento(0));
            }

            if (valores[1] > 110)
            {
                this.ModificarValor(NombresDeInstrumentos.Trans_Oil_Temp_Light, new ValoresDeInstrumento(1));
            }
            else
            {
                this.ModificarValor(NombresDeInstrumentos.Trans_Oil_Temp_Light, new ValoresDeInstrumento(0));
            }
        }

        private void reglaFuelQty(ValoresDeInstrumento valores)
        {
            if (valores[0] < 15)
            {
                this.ModificarValor(NombresDeInstrumentos.Fuel_Low, new ValoresDeInstrumento(1));
            }
            else
            {
                this.ModificarValor(NombresDeInstrumentos.Fuel_Low, new ValoresDeInstrumento(0));
            }
        }
    }
}
