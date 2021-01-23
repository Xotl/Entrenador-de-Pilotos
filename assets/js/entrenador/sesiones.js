angular.module('EntrenadorApp')
.filter('filtroSesiones', function() {
    return function(original, objFiltro) {
        
        if (!original || (!objFiltro.rangoFecha && !objFiltro.fecha && objFiltro.calificacion === 'T' && objFiltro.texto.length === 0)) {
            return original;
        }
        
        var patt = false;
        if (objFiltro.texto.length > 0) { var i; patt = '^';
            for (i = 0; i < objFiltro.texto.length; i++) {
                patt += '(?=.*' + objFiltro.texto[i].toLowerCase() + ')';
            }
            
            // Fix para que busque con acentos.
            var mapObj = {a:"[aá]", e:"[eé]", i:"[ií]", o:"[oó]", u:"[uúü]"};
            patt = patt.replace(/[aeiou]/gi, function(matched){
                return mapObj[matched];
            });
            
            patt = new RegExp(patt, 'i');
        }
        
        var final = [], criterioRangoFecha = false, criterioFecha = false, criterioCal = false, criterioTexto = false;
        angular.forEach(original, function(valor){

            criterioFecha = !objFiltro.fecha || (objFiltro.fecha.getFullYear() === valor.Fecha.getFullYear() &&
                objFiltro.fecha.getMonth() === valor.Fecha.getMonth() && objFiltro.fecha.getDay() === valor.Fecha.getDay());
                
            criterioRangoFecha = !objFiltro.rangoFecha || (valor.Fecha >= objFiltro.rangoFecha[0] &&
                 valor.Fecha <= objFiltro.rangoFecha[1]);
                
            criterioCal =  objFiltro.calificacion === 'T' || 
                (valor.CalificacionGeneral === 0 && objFiltro.calificacion === 'N') || 
                (valor.CalificacionGeneral !== 0 && objFiltro.calificacion === 'C');
            
            criterioTexto = objFiltro.texto.length === 0 || patt.test(valor.Usuario.nombre + ' ' + valor.Escenario.Nombre);
            
            if (criterioRangoFecha && criterioFecha && criterioCal && criterioTexto) {
                final.push(valor);
            }
        });
        
        return final;
    };
})
.controller('mostrarSesionesCtlr', ['$scope', '$http', function($scope, $http) {
    $scope.txtFiltro = '';
    $scope.objFiltro = {
        fecha: false,
        calificacion: 'N',
        texto: []
    };
    $scope.$watch('txtFiltro', function(newValue, oldValue) {
        $scope.objFiltro.fecha = false;
        $scope.objFiltro.rangoFecha = false;
        $scope.objFiltro.texto = [];
        $scope.objFiltro.calificacion = 'N';// Representa los NO calificados
        newValue = newValue.trim();
        if (!newValue) {
            return;
        }
        
        // Busca un rango de fechas
        var match = newValue.match(/\b\d\d?[-\\|\/](?:0?\d|1[012])[-\\|\/]\d\d(?:\d\d)?\s*(?:\.\.+|-)\s*\d\d?[-\\|\/](?:0?\d|1[012])[-\\|\/]\d\d(?:\d\d)?\b/);
        if (match) {
            newValue = newValue.replace(match, '');// Limpio la entrada
            match = match[0].split(/\s*(?:\.\.+|-)\s*/);
            $scope.objFiltro.rangoFecha = match;
            
            match = $scope.objFiltro.rangoFecha[0].split(/[-\\|\/]+/);
            if (match[2].length === 2) match[2] = parseInt(match[2], 10) + 2000;
            $scope.objFiltro.rangoFecha[0] = new Date(match[2], match[1] - 1, match[0]);
            
            match = $scope.objFiltro.rangoFecha[1].split(/[-\\|\/]+/);
            if (match[2].length === 2) match[2] = parseInt(match[2], 10) + 2000;
            $scope.objFiltro.rangoFecha[1] = new Date(match[2], match[1] - 1, match[0]);
            
            if ($scope.objFiltro.rangoFecha[0] > $scope.objFiltro.rangoFecha[1]) {
                match = $scope.objFiltro.rangoFecha[0];
                $scope.objFiltro.rangoFecha[0] = $scope.objFiltro.rangoFecha[1];
                $scope.objFiltro.rangoFecha[1] = match;
            }
            
            // Hago el límite superior se refiera al final del día
            $scope.objFiltro.rangoFecha[1].setTime($scope.objFiltro.rangoFecha[1].getTime() + 86399000);
        }
        else {// Busco una fecha única
            match = newValue.match(/\b\d\d?[-\\|\/](?:0?\d|1[012])[-\\|\/]\d\d(?:\d\d)?\b/);
            if (match) {
                newValue = newValue.replace(match, '');// Limpio la entrada
                match = match[0].split(/[-\\|\/]+/);
                if (match[2].length === 2) match[2] = parseInt(match[2], 10) + 2000;
                $scope.objFiltro.fecha = new Date(match[2], match[1] - 1, match[0]);
            }
        }
        
        // Filtro 'Todos'
        match = newValue.match(/\btod[oa]s*\b/i);
        if (match) {
            newValue = newValue.replace(match, '');// Limpio la entrada
            $scope.objFiltro.calificacion = 'T';// Representa Todos
        }
        else {// Filtro 'Calificación'
            match = newValue.match(/\bcalifica(?:ci[óo]n(?:es)*|d[oa]s?)\b/i);
            if (match) {
                newValue = newValue.replace(match, '');// Limpio la entrada
                $scope.objFiltro.calificacion = 'C';// Representa a los ya Calificados
            }
        }
        
        newValue = newValue.trim();
        if (newValue) {
            $scope.objFiltro.texto = newValue.split(/[\s,.:;\-+]+/);
        }
    });
    
    $scope.FilaSeleccionada = null;
    $scope.select = function(id) {
        $scope.FilaSeleccionada = id;
    };
    $scope.ContinuarClick = function() {
        location.href = '/entrenador/sesiones/' + $scope.FilaSeleccionada;
    };
    
    var obtenerSesiones = function() {
        
        $('#Loading-modal').one('opened.fndtn.reveal', function () {
            $http.post('/api/sesiondeentrenamiento', {simple:true}).success(function(data) {
                $('#Loading-modal').foundation('reveal', 'close');
                angular.forEach(data, function(valor){
                    valor.Fecha = new Date(valor.Fecha);
                    if (!valor.Escenario)// Si fue eliminado
                        valor.Escenario = {Nombre: 'Escenario Eliminado'};
                });
                $scope.Sesiones = data;
            }).
            error(function(data, status, headers, config) {
                $('#Error-modal').foundation('reveal', 'open');
            });
        });
        $('#Loading-modal').foundation('reveal', 'open');
    };

    obtenerSesiones();
}]);