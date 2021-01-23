using System;

namespace Comunicacion_JSON
{
    public interface IToJsonFx
    {
        /// <summary>
        /// Convierte el objeto en un string con formato JSON.
        /// </summary>
        /// <returns>Representación JSON de este objeto.</returns>
        string toJSON();
    }
}
