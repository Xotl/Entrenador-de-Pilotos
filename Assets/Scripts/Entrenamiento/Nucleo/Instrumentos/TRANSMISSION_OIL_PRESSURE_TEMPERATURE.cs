using Entrenamiento.Nucleo;
namespace Entrenamiento.Nucleo.Instrumentos
{
    class TRANSMISSION_OIL_PRESSURE_TEMPERATURE : Instrumento
    {
        /**
         *  El primer valor (valor[0]) indica la presión en PSI.
         *  El segundo valor (valor[1]) indica la temperatura en C°.
         */

        public TRANSMISSION_OIL_PRESSURE_TEMPERATURE()
            : base(NombresDeInstrumentos.XMSN_Oil_Temp_Press, TiposDeIntrumentos.DobleAguja)
        {
        }
        
        protected override bool seEncuentraEnAdvertencia(ValoresDeInstrumento valores)
        {
            return false;
        }
        
        protected override bool seEncuentraEnAlerta(ValoresDeInstrumento valores)
        {
            if (valores[0] < 30 || valores[0] > 70 || valores[1] > 110)
                return true;

            return false;
        }

        protected override ValoresDeInstrumento valoresMaximos()
        {
            ValoresDeInstrumento max = new ValoresDeInstrumento();
            max.Cantidad = 2;// Cantidad de valores por instrumento
            max[0] = 150;
            max[1] = 150;
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

    // este es de doble aguja 
    // Izquierda mide temperatura del aceite °C
    // Derecha mide pre Precion  PSI
    // 0 - 15  escala visual en ambas agujas
    //  PSI correcta 5 - 7 por arriba o abja advertencia 
    // °C correcto correcto > 2 < 11 
    // lo pongo asi porque no se como vayas a manejar este istrumento creo que lo mas facil seria hacer dos independientes y solo vusualmente kedaran el el mismo  


}

