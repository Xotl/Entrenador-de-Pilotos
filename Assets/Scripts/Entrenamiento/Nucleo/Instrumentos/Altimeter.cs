using Entrenamiento.Nucleo;

namespace Entrenamiento.Nucleo.Instrumentos
{
    class Altimeter : Instrumento
    {
        public Altimeter()
            : base(NombresDeInstrumentos.Altimeter, TiposDeIntrumentos.Aguja)
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
            max[0] = 100000;
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

    //The barometric pressure altimeter presents
//an altitude reading in feet above mean sea
//level (MSL) based on the relationship between
//the static air pressure and the barometric
//setting on the altimeter. The barometric
//setting may be adjusted to reflect the current
//barometric pressure corrected to sea level in
//inches of mercury or in millibars, depending
//on the instrument installed.
}
