/**
 * EscenariosController
 *
 * @module      :: Controller
 * @description :: A set of functions called `actions`.
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
   *    `/escenarios/index`
   *    `/escenarios`
   */
	index: function (req, res) {
		
		var id = req.param('id');
		var condiciones = req.param('data');

		if (condiciones) {
			condiciones = JSON.parse( condiciones );
		}
		else {
			condiciones = { where: {}, sort: 'Nombre ASC' };
		}
		
		if (id) {
			condiciones.where.id = id;
		}
		
		Escenario.find(condiciones).exec(function(err, escenarios) {
			
			if (err) {
				return console.log(err);
			}
			
			return res.json(escenarios);
		});
	},


  /**
   * Action blueprints:
   *    `/escenarios/guardar`
   */
	guardar: function (req, res) {
	
		var id = req.param('id');
		var data = req.param('data');
		
		if (data) {
			data = JSON.parse(data);
			
			if(!id && data.id) {
				id = data.id;
			}
		}
		else {
			return res.json({});
		}
		
		
		if (id) {
			
			Escenario.update({id: id}, data).exec(function(err, escenario) {
				
				if (err) {
					return console.log(err);
				}
				
				return res.json(escenario);
			});
		}
		else {
			
			Escenario.create(data).exec(function(err, escenario) {
				if (err) {
					return console.log(err);
				}
				return res.json(escenario);
			});
		}
	},

	borrar: function (req, res) {

		var id = req.param('id');
		if (id) {

			Escenario.destroy(id).exec(function(err) {
				
				if (err) {
					return console.log(err);
				}

				return res.json({status:true});
			});
		}
	},


  /**
   * Overrides for the settings in `config/controllers.js`
   * (specific to EscenariosController)
   */
  _config: {}

  
};
