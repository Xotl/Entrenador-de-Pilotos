using Entrenamiento.Nucleo;
namespace Entrenamiento.Nucleo.Instrumentos
{
    class ENGINE_OIL_PRESSURE_TEMPERATURE : Instrumento
    {
        /**
         *  El primer valor (valor[0]) indica la presión en PSI.
         *  El segundo valor (valor[1]) indica la temperatura en C°.
         */

        public ENGINE_OIL_PRESSURE_TEMPERATURE()
            : base(NombresDeInstrumentos.Engine_Oil_Temp_Press, TiposDeIntrumentos.DobleAguja)
        {
        }
        protected override bool seEncuentraEnAdvertencia(ValoresDeInstrumento valores)
        {
            if (valores[0] < 90)
                return true;

            return false;
        }
        protected override bool seEncuentraEnAlerta(ValoresDeInstrumento valores)
        {
            if (valores[0] < 50 || valores[0] > 130 || valores[1] > 105)
                return true;

            return false;
        }

        protected override ValoresDeInstrumento valoresMaximos()
        {
            ValoresDeInstrumento max = new ValoresDeInstrumento();
            max.Cantidad = 2;// Cantidad de valores por instrumento
            max[0] = 150;
            max[1] = 150;
            return max;
        }

        protected override ValoresDeInstrumento valoresMinimos()
        {
            ValoresDeInstrumento min = new ValoresDeInstrumento();
            min.Cantidad = 2;// Cantidad de valores por instrumento
            min[0] = 0;
            min[1] = 0;
            return min;
        }
    }

    // doble Aguja muestra Temp y Presicon eslaca de 10 valores de 0 15
    //DER. 
        // Alerta 5 y < 9
        // correcto >= 9 < 13
    // Izq.  Correcto de 0  - 11
}

