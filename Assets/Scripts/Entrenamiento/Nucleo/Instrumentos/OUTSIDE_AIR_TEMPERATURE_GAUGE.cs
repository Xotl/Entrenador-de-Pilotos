using Entrenamiento.Nucleo;
namespace Entrenamiento.Nucleo.Instrumentos
{
    class OUTSIDE_AIR_TEMPERATURE_GAUGE : Instrumento
    {
        public OUTSIDE_AIR_TEMPERATURE_GAUGE()
            : base(NombresDeInstrumentos.OUTSIDE_AIR_TEMPERATURE_GAUGE, TiposDeIntrumentos.Aguja
            )
        {
        }

        protected override bool seEncuentraEnAdvertencia(ValoresDeInstrumento valores)
        {
            return false;
        }

        protected override bool seEncuentraEnAlerta(ValoresDeInstrumento valores)
        {
            return false;
        }

        protected override ValoresDeInstrumento valoresMaximos()
        {
            ValoresDeInstrumento max = new ValoresDeInstrumento();
            max.Cantidad = 1;// Cantidad de valores por instrumento
            max[0] = 50;
            return max;
        }

        protected override ValoresDeInstrumento valoresMinimos()
        {
            ValoresDeInstrumento min = new ValoresDeInstrumento();
            min.Cantidad = 1;// Cantidad de valores por instrumento
            min[0] = -10;
            return min;
        }
    }

    // este no tiene valores de alerta solo es un termometro para el aire del exterior mide tanto en °f como en °c
    //°c 0 - 50 y -10 -70
    // °f 0 - 120  -70
}

