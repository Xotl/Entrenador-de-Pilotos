/**
 * Usuaios
 *
 * @module      :: Model
 * @description :: A short summary of how this model works and what it represents.
 * @docs		:: http://sailsjs.org/#!documentation/models
 */

var encriptarPassword = function(password) {
    var crypto = require('crypto');
    var shasum = crypto.createHash('sha1');
    shasum.update(password);
    return shasum.digest('hex');
};

module.exports = {

    attributes: {
        username: {
          type: 'string',
          required: true
        },
        password: {
          type: 'string',
          required: true
        },
        tipo: {
          type: 'string',
          required: true
        },
        nombre: {
          type: 'string',
          defaultsTo: 'Sin nombre'
        },
        
        toJSON: function() {
            var obj = this.toObject();
            delete obj.password;
            return obj;
        }
    },
    
    beforeCreate: function(values, next) {
        values.promedioGeneral = 0;
        values.totalDeSesionesRealizadas = 0;
        values.password = encriptarPassword(values.password);
        next();
    },
    
    beforeUpdate : function(values, next) {
        if (values.password) {
            values.password = encriptarPassword(values.password);
        }
        next();
    },
};
