using Entrenamiento.Nucleo;
namespace Entrenamiento.Nucleo.Instrumentos
{
    class Litter_Door_Open_Light : Instrumento
    {
        public Litter_Door_Open_Light()
            : base(NombresDeInstrumentos.Litter_Door_Open_Light, TiposDeIntrumentos.Indicador_Luminoso)
        {
        }
        
        protected override bool seEncuentraEnAdvertencia(ValoresDeInstrumento valores)
        {
            return false;
        }
        
        protected override bool seEncuentraEnAlerta(ValoresDeInstrumento valores)
        {
            if (valores[0] > 0)
                return true;

            return false;
        }

        protected override ValoresDeInstrumento valoresMaximos()
        {
            ValoresDeInstrumento max = new ValoresDeInstrumento();
            max.Cantidad = 1;// Cantidad de valores por instrumento
            max[0] = 1;
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

    //no valores detecta puertas abiertas
}

