using System;

namespace Entrenamiento.Nucleo
{
    public class Aeronaves
    {
        /// <summary>
        /// Obtiene los interrupotres de una areonave.
        /// </summary>
        /// <param name="modelo">Modelo de la aeronave.</param>
        public static Entrenamiento.Nucleo.Interruptor[] ObtenerInterruptores(ModelosDeHelicoptero modelo)
        {
            Interruptor[] interruptores = null;
            switch (modelo)
            {
                case ModelosDeHelicoptero.B206L3:
                    EstadosDeInterruptores[] estados = 
                        new EstadosDeInterruptores[]{ EstadosDeInterruptores.Abajo, EstadosDeInterruptores.Abajo };
                    interruptores = new Interruptor[3];
                    interruptores[0] = new Interruptor(NombresDeInterruptores.Defog_blower_switch, estados);
                    interruptores[1] = new Interruptor(NombresDeInterruptores.Engine_anti_icing, estados);
                    interruptores[2] = new Interruptor(NombresDeInterruptores.Hydraulic_system_switch, estados);
                    break;
                case ModelosDeHelicoptero.B206B3:
                    throw new System.NotImplementedException();
                    //break;

                case ModelosDeHelicoptero.B206L4:
                    throw new System.NotImplementedException();
                    //break;
            }
            return interruptores;
        }

        /// <summary>
        /// Obtiene los instrumentos de una areonave.
        /// </summary>
        /// <param name="modelo">Modelo de la aeronave.</param>
        public static Entrenamiento.Nucleo.Instrumento[] ObtenerIntrumentos(Entrenamiento.Nucleo.ModelosDeHelicoptero modelo)
        {
            Instrumento[] instrumentos = null;
            switch (modelo)
            {
                case ModelosDeHelicoptero.B206L3:
                    instrumentos = new Instrumento[2];
                    instrumentos[0] = new Entrenamiento.Nucleo.Instrumentos.AirSpeed();
                    instrumentos[1] = new Entrenamiento.Nucleo.Instrumentos.Altimeter();
                    break;
                case ModelosDeHelicoptero.B206B3:
                    throw new System.NotImplementedException();
                    //break;

                case ModelosDeHelicoptero.B206L4:
                    throw new System.NotImplementedException();
                    //break;
            }
            return instrumentos;
        }
    }
}