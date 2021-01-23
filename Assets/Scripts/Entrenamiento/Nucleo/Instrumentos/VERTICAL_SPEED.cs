using Entrenamiento.Nucleo;
namespace Entrenamiento.Nucleo.Instrumentos
{
    class VERTICAL_SPEED : Instrumento
    {
        public VERTICAL_SPEED()
            : base(NombresDeInstrumentos.VERTICAL_SPEED, TiposDeIntrumentos.Aguja)
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
            max[0] = 4000;
            return max;
        }

        protected override ValoresDeInstrumento valoresMinimos()
        {
            ValoresDeInstrumento min = new ValoresDeInstrumento();
            min.Cantidad = 1;// Cantidad de valores por instrumento
            min[0] = -4000;
            return min;
        }
    }

    // este no tiene valores maximos y minimos  solo indica el incremento o decremento del aeronave en vertical  0 a 4000 pies 
}

