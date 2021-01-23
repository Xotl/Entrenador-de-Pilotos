using Entrenamiento.Nucleo;
namespace Entrenamiento.Nucleo.Instrumentos
{
    class GAS_PRODUCER : Instrumento
    {
        public GAS_PRODUCER()
            : base(NombresDeInstrumentos.Gas_Producer, TiposDeIntrumentos.Aguja)
        {
        }
        
        protected override bool seEncuentraEnAdvertencia(ValoresDeInstrumento valores)
        {
            return false;
        }
        
        protected override bool seEncuentraEnAlerta(ValoresDeInstrumento valores)
        {
            if (valores[0] > 105)
                return true;
            
            return false;
        }

        protected override ValoresDeInstrumento valoresMaximos()
        {
            ValoresDeInstrumento max = new ValoresDeInstrumento();
            max.Cantidad = 1;// Cantidad de valores por instrumento
            max[0] = 107;
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

    //tambien teine dos ahujas la pequeña muestra incrmentos de 1% en la precensia de gas 
    // y la otra incrementos en 2% 

    // la pequeña va de 0-9
    // la garnde de 0 a 105 de 61 a 103 es verde y > a eso es rojo
}

