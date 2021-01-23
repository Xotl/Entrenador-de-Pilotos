using Entrenamiento.Nucleo;
namespace Entrenamiento.Nucleo.Instrumentos
{
    class FUEL_PRESSURE_DC_LOADMETER_206L3 : Instrumento
    {
        /**
         *  El primer valor (valor[0]) indica % de carga(?).
         *  El segundo valor (valor[1]) indica la presión en PSI.
         */

        public FUEL_PRESSURE_DC_LOADMETER_206L3()
            : base(NombresDeInstrumentos.Fuel_Preassure_Loadmeter, TiposDeIntrumentos.DobleAguja)
        {
        }
        
        protected override bool seEncuentraEnAdvertencia(ValoresDeInstrumento valores)
        {
            return false;
        }

        protected override bool seEncuentraEnAlerta(ValoresDeInstrumento valores)
        {
            if (valores[0] > 90 || valores[1] > 25 || valores[1] < 4)
                return true;

            return false;
        }

        protected override ValoresDeInstrumento valoresMaximos()
        {
            ValoresDeInstrumento max = new ValoresDeInstrumento();
            max.Cantidad = 2;// Cantidad de valores por instrumento
            max[0] = 100;
            max[1] = 30;
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

    // es de doble aguja  la carga escala en 10
    //lado derecho 0 - 10 y marca roja en 10
    // IZq. precion 0 - 3 verde o correcto de 1 a 2.5 
}

