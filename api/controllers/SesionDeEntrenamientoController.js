/**
 * SesionDeEntrenamientoController
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
   *    `/sesiondeentrenamiento/index`
   *    `/sesiondeentrenamiento`
   */
    index: function (req, res) {
        
        var id = req.param('id'),
			condiciones = req.param('data'),
			obtencionSimple = req.param('simple');
		
		if (condiciones) {
			condiciones = JSON.parse( condiciones );
		}
		else {
			condiciones = { where: {}, sort: 'createdAt ASC' };
		}
		
		if (id) {
			condiciones.where.id = id;
		}
		
		SesionDeEntrenamiento.find(condiciones).exec(function(err, sesionesDeEntrenamiento) {
			
			if (err) {
				return console.log(err);
			}
			

			var auxID, listaUsuariosID = [], listaEscenariosID = [], listaUsuariosIndexada = {}, listaEscenariosIndexada = {};

			// Obtención de id´s de usuarios y de escenarios.
			for (var i = sesionesDeEntrenamiento.length - 1; i >= 0; i--) {

				auxID = sesionesDeEntrenamiento[i].usuario_id;
				if (!listaUsuariosIndexada.hasOwnProperty(auxID)) {

					listaUsuariosID.push(sesionesDeEntrenamiento[i].usuario_id);
					listaUsuariosIndexada[auxID] = null;
				}
				

				auxID = sesionesDeEntrenamiento[i].escenario_id;
				if (!listaEscenariosIndexada.hasOwnProperty(auxID)) {

					listaEscenariosID.push(sesionesDeEntrenamiento[i].escenario_id);
					listaEscenariosIndexada[auxID] = null;
				}
			}


			var async = require('async');
			async.parallel([
				function(cb) {
					Usuario.find({id:listaUsuariosID}).exec(cb);
				},
				function(cb) {
					Escenario.find({id:listaEscenariosID}).exec(cb);
				},
			],
			function(err, results) {// Callback parallel
			
				if (err) {
					return console.log(err);
				}
				
				var i, simplificarDatos = function() {};

				if (obtencionSimple) {
					simplificarDatos = function(sesion) {
						delete sesion.GrabacionDeInstrumentacion;
						delete sesion.GrabacionDeInterruptores;
						delete sesion.Hitos;
						
						if (sesion.Escenario)// En caso de no haber sido eliminado el escenario
							delete sesion.Escenario.Etapas;
					};
				}

				// Indexo la lista de usuarios
				for (i = results[0].length - 1; i >= 0; i--) {
					listaUsuariosIndexada[results[0][i].id] = results[0][i];
				}

				// Indexo la lista de escenarios
				for (i = results[1].length - 1; i >= 0; i--) {
					listaEscenariosIndexada[results[1][i].id] = results[1][i];
				}

				// Asigno los datos a las sesiones
				for (i = sesionesDeEntrenamiento.length - 1; i >= 0; i--) {
					sesionesDeEntrenamiento[i].Usuario = listaUsuariosIndexada[sesionesDeEntrenamiento[i].usuario_id];
					sesionesDeEntrenamiento[i].Escenario = listaEscenariosIndexada[sesionesDeEntrenamiento[i].escenario_id];
					delete sesionesDeEntrenamiento[i].usuario_id;
					simplificarDatos(sesionesDeEntrenamiento[i]);
				}
				
				return res.json(sesionesDeEntrenamiento);
			});// Fin Parallel

		});// Fin SesionDeEntrenamiento.find
    },


  /**
   * Action blueprints:
   *    `/sesiondeentrenamiento/guardar`
   */
    guardar: function (req, res) {
    
        var data = req.param('data');
        
        if (data) {
            data = JSON.parse(data);
            
            if(req.session.authenticated) {
				data.usuario_id = req.session.user.id;
            }
            else {
                data.usuario_id = 'ID de Prueba';
            }
            
            data.Fecha = new Date();// Obtiene la fecha actual.
            data.Fecha.setTime(data.Fecha.getTime() - data.TiempoDeFinalizacion);// Le restamos el tiempo de simulación
        }
        else {
            return res.json({});
        }
        
        SesionDeEntrenamiento.create(data).exec(function(err, sesionDeEntrenamiento) {
            if (err) {
                return console.log(err);
            }
            return res.json(sesionDeEntrenamiento);
        });
    },




  /**
   * Overrides for the settings in `config/controllers.js`
   * (specific to SesionDeEntrenamientoController)
   */
  _config: {}

  
};
