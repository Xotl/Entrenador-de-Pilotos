/**
 * SesionDeEntrenamiento
 *
 * @module      :: Model
 * @description :: A short summary of how this model works and what it represents.
 * @docs		:: http://sailsjs.org/#!documentation/models
 */

var incrementarCantidadDeSesionesRealizadas = function(sesion) {

	Usuario.findOne(sesion.usuario_id).exec(function(err, usuario) {

		usuario.totalDeSesionesRealizadas++;
		delete usuario.password;
		usuario.save(function(err) {
			if (err) return console.log(err);
		});
	});
},

actualizarPromedioGeneral = function(sesion) {

	SesionDeEntrenamiento.find({usuario_id:sesion.usuario_id}).exec(function(err, sesiones) {

		if (err) {
			return console.log(err);
		}

		var sumatoria = 0, totalParaPromedio = 0;
		for (var i = sesiones.length - 1; i >= 0; i--) {

			if (sesiones[i].CalificacionGeneral > 0) {// Sólo usa las sesiones evaluadas
				totalParaPromedio++;
				sumatoria += sesiones[i].CalificacionGeneral;
			}
		}

		Usuario.update(sesion.usuario_id, {promedioGeneral: sumatoria / totalParaPromedio}).exec(function(err) {
			if (err) {
				return console.log(err);
			}
		});// Fin Usuario.update

	});// Fin this.find
};

module.exports = {

	attributes: {
		/* e.g.
		nickname: 'string'
		*/
	},

	afterCreate: function(sesion, cb) {
		incrementarCantidadDeSesionesRealizadas(sesion);
		cb();
	},

	afterUpdate: function(sesion, cb) {
		actualizarPromedioGeneral(sesion);
		cb();
	},
};
