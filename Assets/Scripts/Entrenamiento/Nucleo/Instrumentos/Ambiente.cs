using Entrenamiento.Nucleo;

namespace Entrenamiento.Nucleo.Instrumentos
{
    class Ambiente : Instrumento
    {
        public Ambiente()
            : base(NombresDeInstrumentos.Ambiente, TiposDeIntrumentos.Ambiente)
        {
        }

        protected override bool seEncuentraEnAdvertencia(ValoresDeInstrumento valores)
        {
            return false;
        }

        protected override bool seEncuentraEnAlerta(ValoresDeInstrumento valores)
        {
            TiposDeAmbiente ambiente = (TiposDeAmbiente)System.Convert.ToInt32(valores[0]);

            if (ambiente == TiposDeAmbiente.Normal)
                return false;

            return true;
        }

        protected override ValoresDeInstrumento valoresMaximos()
        {
            ValoresDeInstrumento max = new ValoresDeInstrumento();
            max.Cantidad = 1;// Cantidad de valores por instrumento
            max[0] = System.Enum.GetValues(typeof(TiposDeAmbiente)).Length - 1;
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
