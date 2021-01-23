using Entrenamiento.Nucleo;
namespace Entrenamiento.Nucleo.Instrumentos
{
    class DIGITAL_CHRONOMETER : Instrumento
    {
        public DIGITAL_CHRONOMETER()
            : base(NombresDeInstrumentos.DIGITAL_CHRONOMETER, TiposDeIntrumentos.Desconocido)
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
            throw new System.NotImplementedException();
        }

        protected override ValoresDeInstrumento valoresMinimos()
        {
            throw new System.NotImplementedException();
        }
    }

    //// indicador de horas es un reloj de cuneta hacia adelante maximo 8 dias (99.59 Hrs)
}

