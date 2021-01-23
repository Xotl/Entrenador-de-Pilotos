/**
 * UsuarioController
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
   *    `/usuario/index`
   *    `/usuario`
   */
    index: function (req, res) {
    
        var id = req.param('id');
        var condiciones = req.param('data');
        
        if (condiciones) {
            condiciones = JSON.parse( condiciones );
        }
        else {
            condiciones = { where: {} };
        }
        
        if (id) {
            condiciones.where.id = id;
        }
        
        Usuario.find(condiciones).exec(function(err, usuarios) {
            
            if (err) {
                return console.log(err);
            }
            
            return res.json(usuarios);
        });
    },

/**
   * Action blueprints:
   *    `/usuario/borrar`
   */
    borrar: function (req, res) {
        
        var id = req.param('id');
        
        if (id) {
            
            SesionDeEntrenamiento.destroy({ usuario_id: id }).exec(function(err) {

                if (err) {
                    return console.log(err);
                }
                
                // Las Sesiones de entrenamiento han sido borradas.
                Usuario.destroy({ id: id }).exec(function(err) {
                    
                    if (err) {
                        return console.log(err);
                    }
                    
                    // Usuario y sesiones borradas
                    return res.json({status:true});
                });
            });
        }
        else {
            return res.json({status:false});
        }
    },


  /**
   * Action blueprints:
   *    `/usuario/guardar`
   */
    guardar: function (req, res) {
    
        var id = req.param('id');
        var data = req.param('data');
        
        if (data) {
            
            if (typeof data === 'string')
                data = JSON.parse(data);
            
            if(!id && data.id) {
                id = data.id;
            }
        }
        else {
            return res.json({guardado:false});
        }
        
        
        
        // Quito espacios en campos de texto
        
        if(data.nombre) {
            data.nombre = data.nombre.trim();
        }
        
        if(data.username) {
            data.username = data.username.trim();
        }
        
        if (data.password) {
            data.password = data.password.trim();
        }
        
        
        // Comienzan las validaciones
        
        if (!data.nombre) {
            console.log('Debe escribir el nombre del alumno.');
            return res.json({guardado:false});
        }
        
        if (!data.username || data.username.length < 5) {
            console.log('El nombre de usuario de tener al menos 5 caracteres.');
            return res.json({guardado:false});
        }
        
        if (!data.password && !id) {
            console.log('Debe escribir la contraseña.');
            return res.json({guardado:false});
        }
        
        if (data.password && data.password.length < 6) {
            console.log('La contraseña debe ser de al menos 6 caracteres de largo.');
            return res.json({guardado:false});
        }
        
        if (!data.tipo) {
            console.log('Debe seleccionar el tipo de usuario.');
            return res.json({guardado:false});
        }
        else if (data.tipo !== 'Entrenador' && data.tipo !== 'Alumno') {
            console.log('Tipo de usuario inválido. "' + data.tipo + '" no es un tipo válido.');
            return res.json({guardado:false});
        }
        
        
        // Pasó la validación, es momento de guardar.
        
        Usuario.findOne({username: data.username}).exec(function(err, usuario) {
            
            if (err) {
                return console.log(err);
            }
            
            if(usuario && usuario.id !== id) {
                console.log('Este nombre de usuario no está disponible.');
                return res.json({guardado:false, msg:'El nombre de usuario no está disponible.', usuarioDuplicado:true});
            }
            
            var datos = {
                nombre: data.nombre,
                username: data.username,
                tipo: data.tipo,
            };
            
            if(data.password) {
                datos.password = data.password;
            }
            
            if (id) {// Edición de usuario
                
                Usuario.update({id: id}, datos, function(err, usuario) {
                    
                    if (err) {
                        return console.log(err);
                    }
                    
                    return res.json(usuario);
                });
            }
            else {// Usuario nuevo
                
                Usuario.create(datos).exec(function(err, usuario) {
                    
                    if (err) {
                        return console.log(err);
                    }
                    return res.json(usuario);
                });
            }
        });
    },




  /**
   * Overrides for the settings in `config/controllers.js`
   * (specific to UsuarioController)
   */
  _config: {}

  
};
