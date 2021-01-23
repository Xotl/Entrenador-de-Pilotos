using Entrenamiento.Nucleo;

namespace Entrenamiento.Nucleo.Instrumentos
{
    class Torquemeter_B3_y_L3 : Instrumento
    {
        public Torquemeter_B3_y_L3()
            : base(NombresDeInstrumentos.TorqueMeter, TiposDeIntrumentos.Aguja)
        {
        }

        protected override bool seEncuentraEnAdvertencia(ValoresDeInstrumento valores)
        {
            if (valores[0] > 85)
                return true;

            return false;
        }

        protected override bool seEncuentraEnAlerta(ValoresDeInstrumento valores)
        {
            if (valores[0] > 100)
                return true;

            return false;
        }

        protected override ValoresDeInstrumento valoresMaximos()
        {
            ValoresDeInstrumento max = new ValoresDeInstrumento();
            max.Cantidad = 1;// Cantidad de valores por instrumento
            max[0] = 120;
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
}

