using Entrenamiento.Nucleo;

namespace Entrenamiento.Nucleo.Instrumentos
{
    class Dual_Tach_206L3 : Instrumento
    {
        /**
         *  El primer valor (valor[0]) corresponde a la aguja Rotor.
         *  El segundo valor (valor[1]) corresponde a la aguja Turbina.
         */

        public Dual_Tach_206L3()
            : base(NombresDeInstrumentos.Dual_Tach, TiposDeIntrumentos.DobleAguja)
        {
        }

        protected override bool seEncuentraEnAdvertencia(ValoresDeInstrumento valores)
        {
            if (valores[1] > 101 && valores[1] <= 103)
                return true;

            return false;
        }

        protected override bool seEncuentraEnAlerta(ValoresDeInstrumento valores)
        {
            if (valores[0] < 90 || valores[0] > 107 || valores[1] < 97 || valores[1] > 100)
                return true;

            return false;
        }

        protected override ValoresDeInstrumento valoresMaximos()
        {
            ValoresDeInstrumento max = new ValoresDeInstrumento();
            max.Cantidad = 2;// Cantidad de valores por instrumento
            max[0] = 120;
            max[1] = 120;
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
}
