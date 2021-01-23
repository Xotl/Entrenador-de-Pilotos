/**
 * LoginController
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
   *    `/login/index`
   *    `/login`
   */
    index: function (req, res) {
        
        if(req.session.authenticated) {
            var redirect = req.param('redirect');
            if(!redirect)
                redirect = '/';
            return res.redirect(redirect);
        }
        
        return res.view({
            title: 'LogIn - Entrenador de pilotos',
            redirect: req.param('redirect')
        });
    },
    
    auth: function (req, res) {
        
        var redirect = req.param('redirect');
        if(!redirect) {
            redirect = '/';
        }
        
        var user = req.param('user');
        var password = req.param('password');
        
        if (user && password) {
            
            Usuario.findOne({
                username: user
            })
            .exec(function(err, usuario) {

                if (err) {
                    return console.log(err);
                }
                
                if (!usuario) {
                    console.log('Usuario no encontrado');// Debug
                } else {// Usuario encontrado
                    var crypto = require('crypto');
                    var shasum = crypto.createHash('sha1');
                    shasum.update(password);
                    shasum = shasum.digest('hex');
                    
                    if (shasum === usuario.password) {// Usuario autenticado
                        req.session.authenticated = true;
                        req.session.user = usuario;
                    } else {
                        console.log("Password no coincide");// Debug
                    }
                }
                
                return res.redirect(redirect);
            });
        }
        else {
            return res.redirect(redirect);
        }
    },
    
    registro: function (req, res) {
        
        var user = req.param('user');
        var pass1 = req.param('password');
        var pass2 = req.param('pass');
        var tipo = req.param('tipo');
        
        if (user && pass1 && pass2 && (pass1 === pass2) && tipo) {
            Usuario.create({
                password: pass1,
                username: user,
                tipo: tipo
            }).exec(function(err, usuario) {
                if(err) {
                    return console.log(err);
                }
                    
                return res.view({
                    title: 'Registrado [' + user + '] - Entrenador de pilotos',
                });
            });
            
        } else {
            return res.view({
                title: 'Registro - Entrenador de pilotos',
            });
        }
    },


  /**
   * Overrides for the settings in `config/controllers.js`
   * (specific to LoginController)
   */
  _config: {}

  
};
