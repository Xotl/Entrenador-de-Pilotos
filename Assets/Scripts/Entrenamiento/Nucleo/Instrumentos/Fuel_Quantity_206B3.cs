using Entrenamiento.Nucleo;
namespace Entrenamiento.Nucleo.Instrumentos
{
    /*
     *  Este es para el modelo 206-B3
     *  Se mide en galones
     */
    class Fuel_Quantity_206B3 : Instrumento
    {
        public Fuel_Quantity_206B3()
            : base(NombresDeInstrumentos.Fuel_Quantity, TiposDeIntrumentos.Aguja)
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
            max[0] = 90;
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
