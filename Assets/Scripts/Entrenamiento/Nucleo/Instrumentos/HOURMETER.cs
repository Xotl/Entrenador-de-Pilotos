using Entrenamiento.Nucleo;
namespace Entrenamiento.Nucleo.Instrumentos
{
    class HOURMETER : Instrumento
    {
        public HOURMETER()
            : base(NombresDeInstrumentos.HOURMETER, TiposDeIntrumentos.Desconocido)
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
    //El contador de horas est� montado en el mamparo de popa 
//del compartimento de la bater�a. El contador va 
//alimentado por el bus 28 VDC a trav�s de un 1-amp 
//interruptor remoto que obtiene la energ�a de 
//el disyuntor PRECAUCI�N 5 amperios. la 
//sistema se activa cuando el productor de gas 
//RPM (N1) es mayor que 55%.
    
}

