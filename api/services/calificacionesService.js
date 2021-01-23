var calificacionesEnum = {
    No_calificado: 0,
    Malo: 40,
    Insuficiente: 60,
    Suficiente: 80,
    Excelencia: 100,
};

module.exports = {
	/**
	 * Convierte el promedio en una evaluación de la enumeración "calificacionesEnum".
	 * @param  {Number} promedio  Promedio del alumno.
	 * @return {String}           Texto de evaluación para mostrar.
	 */
	convertirPromedioEnTextoDeEvaluacion: function(promedio) {

	    if (promedio === 0) {
	        return 'No evaluado';
	    }

	    for (var prop in calificacionesEnum) {
	        if (promedio <= calificacionesEnum[prop]) {
	            return prop.replace('_', ' ');
	        }
	    }
	},
	
};