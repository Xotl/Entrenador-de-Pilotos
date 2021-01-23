using Entrenamiento.Nucleo;
namespace Entrenamiento.Nucleo.Instrumentos
{
    class TURBINE_OUTLET_TEMPERATURE_206B3 : Instrumento
    {
        public TURBINE_OUTLET_TEMPERATURE_206B3()
            : base(NombresDeInstrumentos.Turbine_Outlet_Temp, TiposDeIntrumentos.Aguja)
        {
        }
        
        protected override bool seEncuentraEnAdvertencia(ValoresDeInstrumento valores)
        {
            if (valores[0] > 738)
                return true;

            return false;
        }
        
        protected override bool seEncuentraEnAlerta(ValoresDeInstrumento valores)
        {
            if (valores[0] >= 810)
                return true;

            return false;
        }

        protected override ValoresDeInstrumento valoresMaximos()
        {
            ValoresDeInstrumento max = new ValoresDeInstrumento();
            max.Cantidad = 1;// Cantidad de valores por instrumento
            max[0] = 1000;
            return max;
        }

        protected override ValoresDeInstrumento valoresMinimos()
        {
            ValoresDeInstrumento min = new ValoresDeInstrumento();
            min.Cantidad = 1;// Cantidad de valores por instrumento
            min[0] = 0;
            return min;
        }
    }

    // tiene una pequeña flecha roja entre 8.2 y 8.3 y un punto rojo en 9.4
    // se podra usar en adventencia durante 10 segudnos para inciar elevacion y aterrizaje
}

