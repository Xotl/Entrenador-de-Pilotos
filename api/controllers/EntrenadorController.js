/**
 * EntrenadorController
 *
 * @module      :: Controller
 * @description	:: A set of functions called `actions`.
 *
 *                 Actions contain code telling Sails how to respond to a certain type of request.
 *                 (i.e. do stuff, then send some JSON, show an HTML page, or redirect to another URL)
 *
 *                 You can configure the blueprint URLs which trigger these actions (`config/controllers.js`)
 *                 and/or override them with custom routes (`config/routes.js`)
 *
 *                 NOTE: The code you write here supports both HTTP and Socket.io automatically.
 *
 * @docs        :: http://sailsjs.org/#!documentation/controllers
 */

module.exports = {
		
	
	/**
	 * Action blueprints:
	 *    `/entrenador/index`
	 *    `/entrenador`
	 */
	 index: function (req, res) {
		
		
		return res.view({
				nombreDelEntrenador: (req.session.user.nombre || req.session.user.username || 'Cosme Fulanito'),
		});
	},


	/**
	 * Action blueprints:
	 *    `/entrenador/escenarios`
	 */
	 escenarios: function (req, res) {
			 
		var escenario_id = req.param('id');
		
		if (escenario_id) {// Edición de escenario
				
				var data = {
						nombre: (req.session.user.nombre || req.session.user.username || 'Cosme Fulanito'),
						escenario_id: escenario_id,
				};
				
				return res.view('404', {// Como el layout no tiene vista se manda el 404 en caso de que algo raro ocurra
						layout: 'unity_layout',
						UnityApp: {
								action: 'EditorDeEscenarios',
								data: data,
								backURL: '/entrenador/escenarios',
						}
				});
		}
		else {// Selección de algún escenario a editar
				
				return res.view();
		}
	},


	/**
	 * Action blueprints:
	 *    `/entrenador/sesiones`
	 */
	sesiones: function (req, res) {
		var id = req.param('id');
		var evaluacion = req.param('data');
		
		if (id) {// Evaluación de sesión de entrenamiento
				
				if (evaluacion) {// Se guarda la evaluación de la sesión de entrenamiento
						
					SesionDeEntrenamiento.update({id: id}, JSON.parse( evaluacion )).exec(function(err) {

						if (err) {
								return console.log(err);
						}
						
						return res.json({
								Guardado: true
						});
					});
				}
				else {// Reproductor de sesión
		
					var data = {
							entrenador_id: req.session.user.id,
							sesion_de_entrenamiento_id: id,
					};
					
					return res.view('404', {// Como el layout no tiene vista se manda el 404 en caso de que algo raro ocurra
						layout: 'unity_layout',
						UnityApp: {
								action: 'ReproductorDeSesion',
								data: data,
								backURL: '/entrenador/sesiones',
						}
					});
				}
		}
		else {// Selección de sesión
		
			return res.view({});
		}
	},
	
	
	/**
	 * Action blueprints:
	 *    `/entrenador/sesiones`
	 */
	administrar: function (req, res) {
	
		return res.view({});
	},


	/**
	 * Action blueprints:
	 *    `/entrenador/reportes`
	 */
	 reportes: function (req, res) {
		
	 	var id = req.param('id');
	 	if (id) {

	 		Usuario.findOne(id).exec(function(err, usuario) {
	 			req.session.alumno = usuario;
	 			sails.controllers.alumno.index(req, res);
	 		});
	 	}
	 	else {
	 		
			return res.view({
			});
	 	}
	},

	busquedaUsuarios: function (req, res) {

		var filtros = req.param('filtros'),
			condiciones = { where: {} };

		if (filtros && (filtros.modelo || filtros.escenario_id)) {

			if (filtros.modelo) 
				condiciones.where.ModeloDeAeronave = modelosDeAeronaves.nameToIndex(filtros.modelo);

			if (filtros.escenario_id)
				condiciones.where.escenario_id = filtros.escenario_id;

			SesionDeEntrenamiento.find(condiciones).exec(function(err, sesiones) {

				if (err)
					return console.log(err);
				
				var listaIndexadaUsuariosID = {}, listaUsuariosID = [];
				for (var i = sesiones.length - 1; i >= 0; i--) {

					if (!listaIndexadaUsuariosID.hasOwnProperty(sesiones[i].usuario_id)) {
						listaIndexadaUsuariosID[sesiones[i].usuario_id] = {
							promedioGeneral: 0,
							totalDeSesionesRealizadas: 0,
							totalSesionesEvaluadas: 0,
						};
						listaUsuariosID.push(sesiones[i].usuario_id);
					}

					listaIndexadaUsuariosID[sesiones[i].usuario_id].totalDeSesionesRealizadas++;

					if (sesiones[i].CalificacionGeneral > 0) {
						listaIndexadaUsuariosID[sesiones[i].usuario_id].promedioGeneral += sesiones[i].CalificacionGeneral;
						listaIndexadaUsuariosID[sesiones[i].usuario_id].totalSesionesEvaluadas++;
					}
				}

				Usuario.find(listaUsuariosID).exec(function(err, usuarios) {

					if (err)
						return console.log(err);

					var calMin = 0, calMax = 100;

					if (filtros.calMin) {
						calMin = filtros.calMin;
					}

					if (filtros.calMax) {
						calMax = filtros.calMax;
					}


					for (var i = usuarios.length - 1; i >= 0; i--) {
						usuarios[i].totalDeSesionesRealizadas = listaIndexadaUsuariosID[usuarios[i].id].totalDeSesionesRealizadas;
						usuarios[i].promedioGeneral = 
							listaIndexadaUsuariosID[usuarios[i].id].promedioGeneral / listaIndexadaUsuariosID[usuarios[i].id].totalSesionesEvaluadas;

						if (usuarios[i].promedioGeneral < calMin || usuarios[i].promedioGeneral > calMax) {
							usuarios.splice(i, 1);
						}
					}
					
					// Ordeno según el promedio de forma ascendente
					usuarios.sort(function(a, b) {
						return a.promedioGeneral - b.promedioGeneral;
					});

					return res.json(usuarios);
				});
			});
		}
		else {

			if (filtros) {

				if (filtros.calMin) {
					condiciones.where.promedioGeneral = {
						'>=': filtros.calMin
					};
				}

				if (filtros.calMin) {

					if (condiciones.where.promedioGeneral)
						condiciones.where.promedioGeneral['<='] = filtros.calMax;
					else
						condiciones.where.promedioGeneral = {'<=': filtros.calMax};
				}
			}

			condiciones.sort = 'promedioGeneral ASC';
			condiciones.where.tipo = 'Alumno';
			Usuario.find(condiciones).exec(function(err, usuarios) {
				
				if (err)
					return console.log(err);

				return res.json(usuarios);
			});
		}
	},


	/**
	 * Overrides for the settings in `config/controllers.js`
	 * (specific to EntrenadorController)
	 */
	_config: {}

	
};
