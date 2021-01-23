angular.module('EntrenadorApp')
.filter('filtroUsuarios', function() {
	return function(arreglo, objFiltro) {
		if (!arreglo || (!objFiltro.tipo && objFiltro.texto.length === 0)) {
			return arreglo;
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
		
		var final = [];
		angular.forEach(arreglo, function(valor){
			if ((!patt || patt.test(valor.nombre + ' ' + valor.username)) &&
					(!objFiltro.tipo || objFiltro.tipo === valor.tipo)) {
				final.push(valor);
			}
		});
		return final;
	};
})
.controller('administrarUsuariosCtlr', ['$scope', '$http', '$rootScope', function($scope, $http, $rootScope) {

	var alertasContainer = $('#alertas-usuario-modal');
	
	var alertaMsg = function(msg) {
		msg = $('<div data-alert class="alert-box alert radius">' + msg + '<a href="#" class="close">&times;</a></div>');
		alertasContainer.append(msg).foundation('alert', 'init');
	};
	
	var actualizarUsuarios = function() {
		$scope.select(null);// Limpio la selección
		$http.get('/api/usuario').success(function(data) {
			$scope.Usuarios = data;
		})
		.error(function(data, status, headers, config) {
			$('#Error-modal').foundation('reveal', 'open');
		});
	};
	
	$scope.txtFiltro = '';
	$scope.editUsuario = null;
	$scope.FilaSeleccionada = null;
	$scope.UsuarioSeleccionado = null;
	$scope.tituloForm = 'Sin título';
	$scope.msgBox = {
		Titulo: 'No hay título',
		Mensaje: 'Este mensaje desaparecerá automáticamente una vez que se haya completado la acción.'
	};
	$scope.objFiltro = {
		tipo: false,
		ordenacion: {},
		texto: []
	};
	
	$scope.$watch('txtFiltro', function(newValue, oldValue) {
		$scope.objFiltro.tipo = false;
		$scope.objFiltro.texto = [];
		newValue = newValue.trim();
		if (!newValue) {
			return;
		}
		
		var match = newValue.match(/\balumnos?\b/i);
		if (match) {
			newValue = newValue.replace(match, '');// Limpio la entrada
			$scope.objFiltro.tipo = 'Alumno';
		}
		else {
			match = newValue.match(/\bentrenador(?:es)?\b/i);
			if (match) {
				newValue = newValue.replace(match, '');// Limpio la entrada
				$scope.objFiltro.tipo = 'Entrenador';
			}
		}
		
		newValue = newValue.trim();
		if (newValue) {
			$scope.objFiltro.texto = newValue.split(/[\s,.:;\-+]+/);
		}
		
		$scope.select(null);// Limpio la selección
	});
	
	$scope.select = function(usuario) {
		$scope.UsuarioSeleccionado = usuario;
		$scope.FilaSeleccionada = (usuario ? usuario.id : usuario);
	};
	$scope.usrForm = function(titulo, usuario) {
		$scope.editUsuario = angular.copy(usuario);
		$scope.tituloForm = titulo;
		alertasContainer.empty();// Limpio alertas anteriores
	};
	$scope.filaDblClick = function(titulo, usuario) {
		$scope.usrForm(titulo, usuario);
		$('#usuario-modal').foundation('reveal', 'open');
	};
	$scope.guardarUsuario = function() {

		alertasContainer.empty();// Limpio alertas anteriores
		if (!$scope.editUsuario) {
			return;// No hay usuario. Un error extraño.
		}
		
		
		// Quito espacios en campos de texto
		
		if($scope.editUsuario.nombre) {
			$scope.editUsuario.nombre = $scope.editUsuario.nombre.trim();
		}
		
		if($scope.editUsuario.username) {
			$scope.editUsuario.username = $scope.editUsuario.username.trim();
		}
		
		if ($scope.editUsuario.password) {
			$scope.editUsuario.password = $scope.editUsuario.password.trim();
		}
		
		if ($scope.editUsuario.password2) {
			$scope.editUsuario.password2 = $scope.editUsuario.password2.trim();
		}
		
		
		// Comienzan las validaciones
		
		if (!$scope.editUsuario.nombre) {
			alertaMsg('Debe escribir el nombre del alumno.');
			return;
		}
		
		if ($scope.editUsuario.nombre.length < 7 || $scope.editUsuario.nombre.split(' ').length < 2) {
			alertaMsg('¿En serio ese es el nombre completo?, por favor escriba nombre y apellido del alumno.');
			return;
		}
		
		if (!$scope.editUsuario.username || $scope.editUsuario.username.length < 5) {
			alertaMsg('El nombre de usuario de tener al menos 5 caracteres.');
			return;
		}
		
		if ($scope.editUsuario.nuevo && !$scope.editUsuario.password) {
			alertaMsg('Debe escribir la contraseña.');
			return;
		}
		
		if ($scope.editUsuario.password && $scope.editUsuario.password.length < 6) {
			alertaMsg('La contraseña debe ser de al menos 6 caracteres de largo.');
			return;
		}
		
		if ($scope.editUsuario.password !== $scope.editUsuario.password2) {
			alertaMsg('Las contraseñas no coinciden. Por favor, vuelva a escribirlas.');
			return;
		}
		
		if (!$scope.editUsuario.tipo) {
			alertaMsg('Debe seleccionar el tipo de usuario.');
			return;
		}
		
		

		// Pasó la validación, es momento de guardar.
		
		$('#msgBox-modal').one('opened.fndtn.reveal', function () {
			$http.post('/api/usuario/guardar', {data:$scope.editUsuario}).success(function(data) {
				if (data.usuarioDuplicado) {
					$('#usuario-duplicado-modal').one('closed.fndtn.reveal', function () {
						$('#usuario-modal').foundation('reveal', 'open');
					});
					$('#usuario-duplicado-modal').foundation('reveal', 'open');
				}
				else {
					actualizarUsuarios();
					$('#msgBox-modal').foundation('reveal', 'close');
				}
			}).
			error(function(data, status, headers, config) {
				$('#Error-modal').foundation('reveal', 'open');
			});
		});
		
		$scope.msgBox.Titulo = 'Guardando, por favor espere...';
		$('#msgBox-modal').foundation('reveal', 'open');
	};
	
	$scope.borrarUsuario = function(cancelar) {
		if (cancelar) {// Usado en el botón de "No, mejor no"
			$('#borrar-usuario-modal').foundation('reveal', 'close');
			return;
		}
		
		if ($scope.UsuarioSeleccionado) {
			$('#msgBox-modal').one('opened.fndtn.reveal', function () {
				$http.post('/api/usuario/borrar', {id:$scope.UsuarioSeleccionado.id}).success(function(data) {
					actualizarUsuarios();
					$('#msgBox-modal').foundation('reveal', 'close');
				}).
				error(function(data, status, headers, config) {
					$('#Error-modal').foundation('reveal', 'open');
				});
			});
			
			$scope.msgBox.Titulo = 'Borrando usuario, por favor espere...';
			$('#msgBox-modal').foundation('reveal', 'open');
		}
	};
	
	actualizarUsuarios();
}]);