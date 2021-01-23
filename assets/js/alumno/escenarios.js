angular.module('EntrenadorApp')
.controller('mostrarEscenariosCtlr', ['$scope', '$http', function($scope, $http) {
    $scope.escenarioSeleccionado = null;
    $scope.Aeronave = null;
    $http({method: 'GET', url: '/api/escenario'})
    .success(function(data, status, headers, config) {
        $scope.Escenarios = data;
    }).
    error(function(data, status, headers, config) {
    	$('#Error-modal').foundation('reveal', 'open');
    });
}]);