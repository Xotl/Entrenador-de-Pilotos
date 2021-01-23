angular.module('EntrenadorApp')
.controller('EditarEscenariosCtlr', ['$scope', '$http', function($scope, $http) {
    var actualizarEscenarios = function() {
	    $http.get('/api/escenario').success(function(data) {
	        $scope.Escenarios = data;
	    }).
	    error(function() {
	    	$('#Error-modal').foundation('reveal', 'open');
	    });
	};
	$scope.msgBox = {
        Titulo: 'No hay título',
        Mensaje: 'Este mensaje desaparecerá automáticamente una vez que se haya completado la acción.'
    };
    $scope.escenarioSeleccionado = null;
	$scope.borrarEscenario = function(id) {
		if (!id) {// Usuario presionó el botón de "No, mejor no"
            $('#borrar-escenario-modal').foundation('reveal', 'close');
        	$scope.escenarioSeleccionado = null;
            return;
        }

		$('#msgBox-modal').one('opened', function () {
            $http.post('/api/escenario/borrar', {id:id}).success(function(data) {
                actualizarEscenarios();
                $('#msgBox-modal').foundation('reveal', 'close');
            }).
            error(function(data) {
                $('#Error-modal').foundation('reveal', 'open');
            });
        });
        
        $scope.msgBox.Titulo = 'Borrando escenario, por favor espere...';
        $('#msgBox-modal').foundation('reveal', 'open');
	};
	actualizarEscenarios();
}]);