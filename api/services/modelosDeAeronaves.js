var modelos = [
	'B-206L4',
	'B-206L3',
	'B-206B3',
];

module.exports = {
	
	/**
	 * Obtiene un array con todos los modelos de aeronaves.
	 * @return {Array} Array de string's con los modelos.
	 */
	obtener: function() {
		return modelos;
	},

	/**
	 * Ejecuta el la función dada por cada uno de los modelos de aeronaves.
	 * @param  {Function} cb Función a ejecutar por cada uno de los modelos. 
	 *                       Recibe 2 argumentos, el primero es un string con el nombre del modelo, y el segundo 
	 *                       es el índice del modelo.
	 * @return
	 */
	forEach: function(cb) {

		for (var i = 0; i < modelos.length; i++) {
			cb(modelos[i], i);
		}
	},

	/**
	 * Conviete un indice de modelo en el nombre de la aeronave.
	 * @param  {Number} index Indice de la enumeración.
	 * @return {String}       Nombre de la aeronave.
	 */
	indexToName: function(index) {
		return modelos[index];
	},

	/**
	 * Devuelve el valor de la enumeración para dicho modelo de aeronave.
	 * @param  {String} modelo Nombre del modelo de la aeronave.
	 * @return {Number}        Valor del modelo de la aeronave ó -1 en caso de no existir.
	 */
	nameToIndex: function(modelo) {

		for (var i = modelos.length - 1; i >= 0; i--) {
			if (modelos[i] === modelo)
				return i;
		}

		return -1;
	},
};