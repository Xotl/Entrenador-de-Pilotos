using Entrenamiento.Nucleo;
namespace Entrenamiento.Nucleo.Instrumentos
{
    class Trans_ChipLight : Instrumento
    {
        public Trans_ChipLight()
            : base(NombresDeInstrumentos.Trans_Chip_Light, TiposDeIntrumentos.Indicador_Luminoso)
        {
        }
        
        protected override bool seEncuentraEnAdvertencia(ValoresDeInstrumento valores)
        {
            if (valores[0] > 0)
                return true;

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

    // este no tiene valores pues se activa cuando se detectan particulas de metal en la transmision o en el aciete de transmicion 
    // basicamente es un iman que al atraer algo activan el foco
}

