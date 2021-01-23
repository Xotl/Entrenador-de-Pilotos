module.exports = {
    
    elementos: [], // Arreglo donde se guardan los elementos.
    
    agregar: function(ruta, sinVariaciones) {
        
        if (!sinVariaciones && ruta.substr(-3, 3) !== '.js') {
                ruta += '.js';
        }
        
        if (this.elementos.indexOf(ruta) === -1) {
            this.elementos.push(ruta);
        }
    },
    
    colocar: function() {
        
        var final = '';
        for (var elem in this.elementos) {
            final += '<script src="' + this.elementos[elem] + '"></script>';
        }
        this.elementos.length = 0;
        return final;
    },
};