using System;

namespace Entrenamiento.Estadisticas
{
    public enum TiposDeHitos
    {
        Interruptor_no_esperado,
        Advertencia_de_instrumento,
        Alerta_de_instrumento,
        Inicio_de_escenario,
        Inicio_de_etapa,
        Interruptor_esperado_pero_estado_incorrecto,
        Fin_de_etapa,
    }
}