angular.module('EntrenadorApp')
.controller('alumnoIndexCtlr', ['$scope', '$http', function($scope, $http) {

	var chartsData = null, historialEscenariosChart = null, historialAeronaveChart = null,

	getRgbaRandomColor = function (opacity) {
	    if (!opacity) opacity = 1;
	    var color = 'rgba(';
	    for (var i = 0; i < 3; i++ ) {
	        color += Math.floor(Math.random() * 255) + ',';
	    }
	    return color + opacity + ')';
	},

	rgbaOpacityChange = function(rgba, opacity) {
	    return rgba.replace(/rgba\((.+),(.+),(.+),(.+)\)/, 'rgba($1,$2,$3,'+ opacity + ')');
	},

	inicializarDatosDeGraficas = function(estadisticas) {

		chartsData = {
			conteoEscenariosData: [],
			escenariosPromedioData: [],
			historialAeronave: {},
			historialEscenarios: {
				todas: { labels: [], datasets: [{
		            fillColor: 'rgba(220,220,220,0.5)',
		            strokeColor: 'rgba(220,220,220,1)',
		            pointColor: 'rgba(220,220,220,1)',
		            pointStrokeColor: '#fff',
		            data: [],
		        }] }
			},
		    aeronavePromedioData: {
		        labels: [],
		        datasets: [{
		            fillColor: getRgbaRandomColor(),
		            strokeColor: getRgbaRandomColor(),
		            pointColor: getRgbaRandomColor(),
		            pointStrokeColor: getRgbaRandomColor(),
		            data: [],
		        }],
		    },
		};

		var i, color, auxModelo, fecha, auxCal, escenario_id;


		for (var prop in estadisticas.conteoEscenarios) {
		    color = getRgbaRandomColor();

		    // Obtención de datos para el conteo de sesiones según escenario.
		    chartsData.conteoEscenariosData.push({
		        value: estadisticas.conteoEscenarios[prop].cantidad,
		        color: color,
		        label : estadisticas.conteoEscenarios[prop].nombre,
		        // labelColor : 'white',
		        // labelFontSize : '12'
		    });

		    // Obtención de datos para el desempeño según escenario.
		    chartsData.escenariosPromedioData.push({
		        value: Number(
		            estadisticas.conteoEscenarios[prop].promedio.toString().match(/^\d+(?:\.\d{0,2})?/)
		        ),
		        color: color,
		        label : estadisticas.conteoEscenarios[prop].nombre,
		        labelColor : 'white',
		        labelFontSize : '12'
		    });


	    	$scope.escenarios.push({ 
	    		nombre: estadisticas.conteoEscenarios[prop].nombre,
	    		id: prop
	    	});


		    // Obtengo el listado de todos los ID's de los escenarios con el objeto listo 
		    // para la gráfica, aunque sin datos.
		    chartsData.historialEscenarios[prop] = { labels: [], datasets: [{
		        fillColor: rgbaOpacityChange(color, 0.6),
		        strokeColor: color,
		        pointColor: 'fff',
		        pointStrokeColor: color,
		        data: [],
		    }] };
		}


		for (i = estadisticas.promedioAeronave.length - 1; i >= 0; i--) {
		    color = getRgbaRandomColor();
		    auxModelo = estadisticas.promedioAeronave[i].modelo;

		    chartsData.aeronavePromedioData.labels.push(auxModelo);
		    chartsData.aeronavePromedioData.datasets[0].data.push(Number(
		        estadisticas.promedioAeronave[i].promedio.toString().match(/^\d+(?:\.\d{0,2})?/)
		    ));

		    if (!chartsData.historialAeronave.hasOwnProperty(auxModelo)) {
		    	
		    	$scope.modelosAeronaves.push(auxModelo);

		        chartsData.historialAeronave[auxModelo] = { 
		        	labels: [], 
		        	datasets: [{
			            fillColor: rgbaOpacityChange(chartsData.aeronavePromedioData.datasets[0].fillColor, 0.5),
			            strokeColor: chartsData.aeronavePromedioData.datasets[0].strokeColor,
			            pointColor: chartsData.aeronavePromedioData.datasets[0].pointColor,
			            pointStrokeColor: chartsData.aeronavePromedioData.datasets[0].pointStrokeColor,
			            data: [],
		        	}]
		    	};
		    }
		}


		for (i = 0; i < estadisticas.todasLasSesiones.length; i++) {

		    fecha = (new Date(estadisticas.todasLasSesiones[i].fecha)).toLocaleDateString();
		    auxCal = estadisticas.todasLasSesiones[i].calificacion;
		    escenario_id = estadisticas.todasLasSesiones[i].escenario_id;
		    auxModelo = estadisticas.todasLasSesiones[i].modelo;

		    // Obtención de calificación de cada sesión individual
		    chartsData.historialEscenarios.todas.labels.push(fecha);
		    chartsData.historialEscenarios.todas.datasets[0].data.push(auxCal);

		    // Obtención de calificación de cada sesión individual organizada según escenario
		    chartsData.historialEscenarios[escenario_id].labels.push(fecha);
		    chartsData.historialEscenarios[escenario_id].datasets[0].data.push(auxCal);

		    // Obtención de calificación de cada sesión individual organizada según aeronave
		    chartsData.historialAeronave[auxModelo].labels.push(fecha);
		    chartsData.historialAeronave[auxModelo].datasets[0].data.push(auxCal);
		}
	},

	actualizarHistorialEscenariosChart = function(data) {
		var escenario_id = 'todas', canvas = $('#historialEscenariosLineChart');
		if ($scope.escenarioSeleccionado)
			escenario_id = $scope.escenarioSeleccionado.id;

		// Fix para evitar que truene cuando sólo hay un punto
		if (data[escenario_id].datasets[0].data.length === 1) {
			data[escenario_id].datasets[0].data.unshift(0);
			data[escenario_id].labels.unshift('');
		}

		if (historialEscenariosChart) {
			historialEscenariosChart.clear();
			historialEscenariosChart.initialize(data[escenario_id]);
		}
		else {
			historialEscenariosChart = new Chart(canvas[0].getContext('2d')).Line(data[escenario_id], {bezierCurve: false});
			canvas.removeAttr('style');
		}
	},

	actualizarHistorialAeronaveChart = function(data) {
		if ($scope.modeloSeleccionado) {

			// Fix para evitar que truene cuando sólo hay un punto
			if (data[$scope.modeloSeleccionado].datasets[0].data.length === 1) {
				data[$scope.modeloSeleccionado].datasets[0].data.unshift(0);
				data[$scope.modeloSeleccionado].labels.unshift('');
			}

			if (historialAeronaveChart) {
				historialAeronaveChart.clear();
				historialAeronaveChart.initialize(data[$scope.modeloSeleccionado]);
			}
			else {
				var canvas = $('#historialAeronaveLineChart');
				historialAeronaveChart = new Chart(canvas[0].getContext('2d')).Line(data[$scope.modeloSeleccionado], {bezierCurve: false});
				canvas.removeAttr('style');
			}
		}
	},

	generarCharts = function(data) {

		$('canvas').each(function() {// Ajusto el tamaño de cada uno de los "Canvas"
	        var elem = $(this), ctx = this.getContext('2d');
	        ctx.canvas.width = elem.width();
	        ctx.canvas.height = elem.height();
	    });

		new Chart(document.getElementById('escenariosPieChart')
	        .getContext('2d'), { background: 'black', fontColor: 'white' }).Pie(data.conteoEscenariosData);
	    new Chart(document.getElementById('escenariosPromedioPolarChart')
	        .getContext('2d')).PolarArea(data.escenariosPromedioData);
	    new Chart(document.getElementById('aeronavePromedioPolarChart')
	        .getContext('2d')).Radar(data.aeronavePromedioData);

	    actualizarHistorialEscenariosChart(data.historialEscenarios);
	    actualizarHistorialAeronaveChart(data.historialAeronave);

	    $('canvas').removeAttr('style');
	},

	alertaMsg = function(msg) {
		msg = $('<div data-alert class="alert-box alert radius">' + msg + '<a href="#" class="close">&times;</a></div>');
		$('#alertas-container').append(msg).foundation('alert', 'init');
	};


	$scope.hayEstadisticas = true;
	$scope.modeloSeleccionado = null;
	$scope.escenarioSeleccionado = null;
	$scope.modelosAeronaves = [];
	$scope.escenarios = [];
	$scope.conteoEscenariosData = null;
	$scope.initialize = function(estadisticas) {
		
		$scope.$apply(function() {

			inicializarDatosDeGraficas(estadisticas);
			$scope.conteoEscenariosData = chartsData.conteoEscenariosData;
			$scope.hayEstadisticas = estadisticas.todasLasSesiones.length > 0;
			$scope.modeloSeleccionado = $scope.modelosAeronaves[0];

			if ($scope.hayEstadisticas) {

				generarCharts(chartsData);

				$scope.$watch('escenarioSeleccionado', function() {
					actualizarHistorialEscenariosChart(chartsData.historialEscenarios);
				});

				$scope.$watch('modeloSeleccionado', function() {
					actualizarHistorialAeronaveChart(chartsData.historialAeronave);
				});
			}

		});// Fin $apply

	};// Fin asignación initialize

	
	$scope.verSesion = function (sesion) {
		$scope.sesionSeleccionada = sesion;
	};
	$scope.sesionSeleccionada = null;
	$scope.Filtro = {
		fechaInicio: null,
		fechaFin: null,
		escenario: '',
		modelo: null,
		modelosAeronaves: ['B-206L4','B-206L3','B-206B3'],
	};
	$scope.sesionesEncontradas = null;
	$scope.buscarSesiones = function() {

		$('#alertas-container').empty();
		var filtroAEnviar = {}, match, regexFecha = /\b\d\d?[-\\|\/](?:0?\d|1[012])[-\\|\/]\d\d(?:\d\d)?\b/;

		if ($scope.Filtro.fechaInicio) {
			match = $scope.Filtro.fechaInicio.match(regexFecha);
			if (match) {
				match = match[0].split(/[-\\|\/]+/);
				if (match[2].length === 2) match[2] = parseInt(match[2], 10) + 2000;
				filtroAEnviar.fechaInicio = new Date(match[2], match[1] - 1, match[0]);
			}
			else {
				alertaMsg('La fecha no es válida.');
				return;
			}
		}

		if ($scope.Filtro.fechaFin) {

			if (!$scope.Filtro.fechaInicio) {
				alertaMsg('La fecha de fin de rango debe ir junto con una fecha de inicio.');
				return;
			}

			match = $scope.Filtro.fechaFin.match(regexFecha);
			if (match) {
				match = match[0].split(/[-\\|\/]+/);
				if (match[2].length === 2) match[2] = parseInt(match[2], 10) + 2000;
				filtroAEnviar.fechaFin = new Date(match[2], match[1] - 1, match[0]);

				if (filtroAEnviar.fechaInicio > filtroAEnviar.fechaFin) {
					alertaMsg('La fecha de fin de rango no puede ser menor que la fecha de inicio.');
					return;
				}
			}
			else {
				alertaMsg('La fecha de fin de rango no es válida.');
				return;
			}
		}

		$scope.Filtro.escenario = $scope.Filtro.escenario.trim();
		if ($scope.Filtro.escenario) {
			filtroAEnviar.escenario = $scope.Filtro.escenario;
		}

		if ($scope.Filtro.modelo !== null) {
			filtroAEnviar.modelo = $scope.Filtro.modelo;
		}

        
		$('#sesiones-loading-modal').one('opened.fndtn.reveal', function() {
			$http.post('/alumno/busquedaDeSesiones', filtroAEnviar).success(function(data) {
                $scope.sesionesEncontradas = data;
                $('#sesiones-resultados-modal').foundation('reveal', 'open');
            }).
            error(function(data, status, headers, config) {
                $('#Error-modal').foundation('reveal', 'open');
            });
		});
		$('#sesiones-loading-modal').foundation('reveal', 'open');
	};
	
	$('#sesiones-modal').on('open.fndtn.reveal', function() {
		$('#alertas-container').empty();
	});
}]);