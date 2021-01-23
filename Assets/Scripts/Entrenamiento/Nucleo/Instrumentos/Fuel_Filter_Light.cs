using Entrenamiento.Nucleo;

namespace Entrenamiento.Nucleo.Instrumentos
{
    class Fuel_Filter_Light : Instrumento
    {
        public Fuel_Filter_Light()
            : base(NombresDeInstrumentos.Fuel_Filter_Light, TiposDeIntrumentos.Indicador_Luminoso)
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

//    // El indicador luminoso COMBUSTIBLE FILTRO precaución 
//cuando el filtro de combustible del fuselaje se encuentra en una 
//condición de bypass inminente (aproximadamente 1 
//Diferencial PSI). El filtro de combustible del fuselaje se 
//derivación a aproximadamente 4 diferencial PSI
}
