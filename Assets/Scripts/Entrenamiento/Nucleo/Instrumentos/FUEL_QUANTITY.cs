using Entrenamiento.Nucleo;
namespace Entrenamiento.Nucleo.Instrumentos
{
    /*
     *  Este es para el modelo L3 y L4
     *  Se mide en Libras
     */
    class FUEL_QUANTITY : Instrumento
    {
        public FUEL_QUANTITY()
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
            max[0] = 800;
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

    // escala en 100  total 752.8 libras 
}

