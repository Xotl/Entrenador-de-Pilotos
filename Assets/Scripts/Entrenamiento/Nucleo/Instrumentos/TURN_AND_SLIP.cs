using Entrenamiento.Nucleo;
namespace Entrenamiento.Nucleo.Instrumentos
{
    class TURN_AND_SLIP : Instrumento
    {
        public TURN_AND_SLIP()
            : base(NombresDeInstrumentos.TURN_AND_SLIP, TiposDeIntrumentos.Turn_and_Slip)
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
            max[0] = 100;
            max[1] = 100;
            return max;
        }

        protected override ValoresDeInstrumento valoresMinimos()
        {
            ValoresDeInstrumento min = new ValoresDeInstrumento();
            min.Cantidad = 2;// Cantidad de valores por instrumento
            min[0] = -100;
            min[1] = -100;
            return min;
        }
    }

    // no se tienen valores e emergencia o alerta este istrumento contine dos independientes:
    // la aguja: Muestra si esta girando y hacia que lado y la velocidad a la que lo hace "velocidad angular" 
    //           cuando la aguja esta total mente cargada hacia un lado se dice  que esta en celocidad de giro estandar normalmente de 3° por segundo
    //          muestra el movimiendoto de Izquierda o derecha 
    // la bola: se activa con los pedales, muestra el movimento de adelante atras "Punta, cola" 
    // http://www.manualvuelo.com/INS/INS28.html
}

