using Entrenamiento.Nucleo;

namespace Entrenamiento.Nucleo.Instrumentos
{
    class Inclinometer : Instrumento
    {
        /**
         *  El primer valor (valor[0]) indica el Roll (derecha o izquierda).
         *  El segundo valor (valor[1]) indica el Pitch (Arriba o abajo).
         */

        public Inclinometer()
            : base(NombresDeInstrumentos.Inclinometer, TiposDeIntrumentos.Horizonte)
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
            max.Cantidad = 2;// Cantidad de valores por instrumento
            max[0] = 180;
            max[1] = 180;
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
