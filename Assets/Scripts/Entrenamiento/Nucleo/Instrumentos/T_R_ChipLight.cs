using Entrenamiento.Nucleo;
namespace Entrenamiento.Nucleo.Instrumentos
{
    class T_R_ChipLight : Instrumento
    {
        public T_R_ChipLight()
            : base(NombresDeInstrumentos.TR_Chip_Light, TiposDeIntrumentos.Indicador_Luminoso)
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

        // no teiene valores es una iman que al atraer algo cierra un circuito y activa la luz  este esta en la caja de cambios del rotor
    }
}

