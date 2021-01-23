angular.module('EntrenadorApp')
.filter('filtroAlumnos', function() {
    return function(input, args) {console.time('filtroAlumnos');
        args = args.trim();
        if (args === '') return input;
        var out = [], patt = '', i = 0;

        args = args.split(/[ ,]/g);
        for (i = 0; i < args.length; i++) {
            patt += '(?=.*' + args[i].toLowerCase() + ')';
        }
        
        // Fix para que busque con acentos.
        var mapObj = {a:"[aá]", e:"[eé]", i:"[ií]", o:"[oó]", u:"[uúü]"};
        patt = patt.replace(/[aeiou]/gi, function(matched){
            return mapObj[matched];
        });
        
        patt = new RegExp(patt, 'i');
        for (i = 0; i < input.length; i++) {
            if (patt.test(input[i].nombre + ' ' + input[i].username)) {
                out.push(input[i]);
            }
        }console.timeEnd('filtroAlumnos');
        return out;
    };
})
.filter('calMinF', function() {
    return function(objEnum, calMax) {
        if (calMax === null) return objEnum;
        var out = {};
        for (var prop in objEnum) {
            if (objEnum[prop] < calMax) out[prop] = objEnum[prop];
        }
        return out;
    };
})
.filter('calMaxF', function() {
    return function(objEnum, calMin) {
        if (calMin === null) return objEnum;
        var out = {};
        for (var prop in objEnum) {
            if (objEnum[prop] > calMin) out[prop] = objEnum[prop];
        }
        return out;
    };
})
.filter('dosDecimales', function() {
    return function(input) {
        return Number(input.toString().match(/^\d+(?:\.\d{0,2})?/));
    };
})
.filter('promedioEnTexto', function() {

    var calificacionesEnum = {
        No_calificado: 0,
        Malo: 40,
        Insuficiente: 60,
        Suficiente: 80,
        Excelencia: 100,
    },
    convertirPromedioEnTextoDeEvaluacion = function(promedio) {

        if (promedio === 0) {
            return 'No evaluado';
        }

        for (var prop in calificacionesEnum) {
            if (promedio <= calificacionesEnum[prop]) {
                return prop.replace('_', ' ');
            }
        }
    };

    return function(input) {
        return convertirPromedioEnTextoDeEvaluacion(input);
    };
})
.controller('reportesCtlr', ['$scope', '$http', function($scope, $http) {
    $scope.calificacionesEnum = {
        No_calificado: 0,
        Malo: 40,
        Insuficiente: 60,
        Suficiente: 80,
        Excelencia: 100,
    };
    $scope.Alumnos = [];
    $scope.Escenarios = {};
    $scope.modelosAeronaves = ['B-206L4','B-206L3','B-206B3'];
    $scope.Filtros = {
        calMin: null,
        calMax: null,
        modelo: null,
        escenario: null,
        search: '',
    };
    $scope.HayFiltros = function() {
        return $scope.Filtros.calMin || $scope.Filtros.calMax || $scope.Filtros.modelo || $scope.Filtros.escenario_id;
    };
    $scope.aplicarFiltros = function() {
        var filtros = {
            calMin: $scope.Filtros.calMin,
            calMax: $scope.Filtros.calMax,
            modelo: $scope.Filtros.modelo,
        };
        if ($scope.Filtros.escenario) 
            filtros.escenario_id = $scope.Filtros.escenario.id;
        buscarUsuarios(filtros);
    };

    var loadingModal = $('#Loading-modal');
    var initialize = function() {

        buscarUsuarios({});
        $http.post('/api/escenario/').success(function(data) {
            $scope.Escenarios = data;
        }).
        error(function() {
            $('#Error-modal').foundation('reveal', 'open');
        });
    };
    var buscarUsuarios = function(filtros) {

        loadingModal.one('opened.fndtn.reveal', function () {
            $http.post('/entrenador/busquedaUsuarios', {filtros:filtros}).success(function(data) {
                loadingModal.foundation('reveal', 'close');
                $scope.Alumnos = data;
            }).
            error(function() {
                $('#Error-modal').foundation('reveal', 'open');
            });
        });
        loadingModal.foundation('reveal', 'open');
    };

    initialize();
}]);