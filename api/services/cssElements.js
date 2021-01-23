
module.exports = {
    
    elementos: [], // Arreglo donde se guardan los elementos.
    
    agregar: function(ruta, sinVariaciones) {
        
        if (!sinVariaciones && ruta.substr(-4, 4) !== '.css'){
                ruta += '.css';
        }
                
        if (this.elementos.indexOf(ruta) === -1) {
            this.elementos.push(ruta);
        }
    },
    
    colocar: function() {
        
        var final = '';
        for (var elem in this.elementos) {
            final += '<link rel="stylesheet" href="' + this.elementos[elem] + '"/>';
        }
        this.elementos.length = 0;
        return final;
    },
};
