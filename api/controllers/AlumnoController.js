/**
 * AlumnoController
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



/**
 * Convierte un tiempo dado en milisegundos a tiempo en texto que se puede mostrar en las estadísticas del alumno.
 * @param  {Integer} tiempo  Tiempo en Milisegundos
 * @return {String}          Cadena con texto en el formato "Xh Xm Xs".
 */
var convertirTiempoEnTextoDeSeion = function(tiempo) {

    var texto = '', aux = 0;

    aux = parseInt(tiempo / 3600000);
    if (aux !== 0) {
        texto += aux + 'h ';
        tiempo = tiempo - aux * 3600000;
    }

    aux = parseInt(tiempo / 60000);
    texto += aux + 'm ';
    tiempo = tiempo - aux * 60000;

    texto += parseInt(tiempo / 1000) + 's';
    return texto;
};

module.exports = {


    index: function (req, res) {
        
        var alumno_id, nombreDelAlumno;

        if (req.session.user.tipo === 'Entrenador') {

            if (!req.session.alumno) {
                return res.redirect('/');
            }
            
            alumno_id = req.session.alumno.id;
            nombreDelAlumno = (req.session.alumno.nombre || req.session.alumno.username || 'Cosme Fulanito');
        }
        else {
            alumno_id = req.session.user.id;
            nombreDelAlumno = (req.session.user.nombre || req.session.user.username || 'Cosme Fulanito');
        }

        SesionDeEntrenamiento.find({
            where: {
                usuario_id: alumno_id
            },
            sort: 'Fecha ASC'
        }).exec(function(err, sesiones) {

            if (err) {
                return console.log(err);
            }

            var tiempoTotal = 0, cantidadEvaluaciones = 0, promedioEvaluaciones = 0,
            estadisticas = {
                todasLasSesiones: [],
                IDs_escenarios_unicos: [],// Lista con todos los id's de los escenarios únicos.
                conteoEscenarios: {},// Objeto con el conteo de las sesiones según el escenario. Están ordenas por ID.
                promedioAeronave: [],// Objeto con promedios generales según el modelo de aeronave. Los índices corresponden a la enumeración de modelos de Unity.
            };
            


            // ****************** Definición de clase Sesión ******************

            /**
             * Clase para representar cada sesión individual
             * @param  {Uuid} escenario_id ID del escenario donde se realizó la sesión.
             * @param  {Number} calificacion Calificación de la sesión.
             * @param  {Date string} fecha Fecha de realización.
             * @param  {Number} modelo Modelo de la aeronave.
             */
            sesionClass = function(escenario_id, calificacion, fecha, modelo) {
                this.calificacion = calificacion;
                this.escenario_id = escenario_id;
                this.fecha = fecha;
                this.nombre = 'No obtenido';
                this.modelo = modelosDeAeronaves.indexToName(modelo);
                this.modeloNumerico = modelo;
            };


            sesionClass.prototype.asignarNombre = function() {
                this.nombre = estadisticas.conteoEscenarios[this.escenario_id].nombre;
                delete this.asignarNombre;
            };
            // **************** Fin Definición de clase Sesión ****************



            // Asignación de modelos a "estadisticas.promedioAeronave"
            modelosDeAeronaves.forEach(function(modelo) {
                estadisticas.promedioAeronave.push({ 
                    modelo: modelo, 
                    promedio: 0,
                    cantidadEvaluadas: 0,
                });
            });


            for (var i = 0; i < sesiones.length; i++) {

                // Creación del objeto para organizar datos según id´s de escenario únicos
                if (!estadisticas.conteoEscenarios.hasOwnProperty(sesiones[i].escenario_id)) {

                    estadisticas.IDs_escenarios_unicos.push(sesiones[i].escenario_id);
                    estadisticas.conteoEscenarios[sesiones[i].escenario_id] = {
                        cantidad: 0,// Cantidad de sesiones realizadas en dicho escenario
                        nombre: 'Eliminado',// Nombre del escenario
                        cantidadEvaluadas: 0,// Cantidad de sesiones evaluadas de dicho escenario
                        promedio: 0,// Promedio general de cada escenario
                    };
                }

                // Obtención de calificación por sesión individual
                estadisticas.todasLasSesiones.push(new sesionClass(
                    sesiones[i].escenario_id,
                    sesiones[i].CalificacionGeneral,
                    sesiones[i].Fecha,
                    sesiones[i].ModeloDeAeronave
                ));

                // Conteo de cada sesión en dicho escenario
                estadisticas.conteoEscenarios[sesiones[i].escenario_id].cantidad++;
                
                // Sumatoria para el tiempo total
                tiempoTotal += sesiones[i].TiempoDeFinalizacion;

                
                if (sesiones[i].CalificacionGeneral !== 0) {
                    
                    // Obtención de datos para el promedio general del alumno
                    cantidadEvaluaciones++;
                    promedioEvaluaciones += sesiones[i].CalificacionGeneral;

                    // Obtención de datos para el promedio de cada escenario
                    estadisticas.conteoEscenarios[sesiones[i].escenario_id].cantidadEvaluadas++;
                    estadisticas.conteoEscenarios[sesiones[i].escenario_id].promedio += 
                        sesiones[i].CalificacionGeneral;

                    // Obtención de datos para el promedio según aeronave
                    estadisticas.promedioAeronave[sesiones[i].ModeloDeAeronave].cantidadEvaluadas++;
                    estadisticas.promedioAeronave[sesiones[i].ModeloDeAeronave].promedio += sesiones[i].CalificacionGeneral;
                }
            }
            
            // Se finaliza el cálculo del promedio según modelo de aeronave
            for (var indice = estadisticas.promedioAeronave.length - 1; indice >= 0; indice--) {
                
                if (estadisticas.promedioAeronave[indice].cantidadEvaluadas) {
                    estadisticas.promedioAeronave[indice].promedio = 
                        (estadisticas.promedioAeronave[indice].promedio / estadisticas.promedioAeronave[indice].cantidadEvaluadas);
                }
            }

            Escenario.find({id: estadisticas.IDs_escenarios_unicos}).exec(function(err, escenarios) {

                if (err) {
                    return console.log(err);
                }
                
                for (var i = escenarios.length - 1; i >= 0; i--) {

                    // Asigno los nombres de los escenarios
                    estadisticas.conteoEscenarios[escenarios[i].id].nombre = escenarios[i].Nombre;

                    // Se finaliza el cálculo del promedio según escenario
                    if (estadisticas.conteoEscenarios[escenarios[i].id].cantidadEvaluadas !== 0) {
                        estadisticas.conteoEscenarios[escenarios[i].id].promedio = 
                            estadisticas.conteoEscenarios[escenarios[i].id].promedio / estadisticas.conteoEscenarios[escenarios[i].id].cantidadEvaluadas;
                    }
                }

                // Asigno los nombres de los escenarios de cada sesión individual
                for (var index = estadisticas.todasLasSesiones.length - 1; index >= 0; index--) {
                    estadisticas.todasLasSesiones[index].asignarNombre();
                }

                return res.view('Alumno/index', {
                    nombreDelAlumno: nombreDelAlumno,
                    cantSesiones: sesiones.length,
                    tiempoTotal: convertirTiempoEnTextoDeSeion(tiempoTotal),
                    desempenoGeneral: calificacionesService.convertirPromedioEnTextoDeEvaluacion(promedioEvaluaciones / cantidadEvaluaciones),
                    estadisticas: estadisticas,
                    esEntrenador: req.session.user.tipo === 'Entrenador',
                });

            });// Cierre de Escenario.exec

        });// Cierre de SesionDeEntrenamiento.exec

    },
  
    escenarios: function (req, res) {
    
        var id = req.param('id');
        if (id) {// Lo mando a realizar la sesión de dicho escenario
        
            var modelo = req.param('Aeronave');
            if (!modelo) {
                modelo = 0;
            }
            console.log('Modelo de aeronave: ' + modelo);

            return res.view('404', {// Como el layout no tiene vista se manda el 404 en caso de que algo raro ocurra
                layout: 'unity_layout',
                UnityApp: {
                    action: 'RealizarSesion',
                    data: {
                        escenario_id: id,
                        modelo_de_aeronave: modelo,
                    },
                    backURL: '/alumno',
                }
            });
        }
    
        // Lo mando a que seleccione un escenario
        return res.view({});
    },
    
    observaciones: function (req, res) {
        
        var id = req.param('id');
        var observaciones = req.param('data');
        
        if(id && observaciones) {// Guardar las observaciones
            
            Usuario.update({id: id}, {observaciones: observaciones}).exec(function(err) {
                
                if (err) {
					return console.log(err);
				}
				
				return res.json({
                    Guardado: true
                });
            });
        }
        else {// No se puede guardar.
        
            return res.json({
                Guardado: false
            });
        }
    },

    /**
     * Búsqueda de sesiones exclusivo para el index de Alumnos
     * @param  {[type]} req [description]
     * @param  {[type]} res [description]
     * @return {[type]}     [description]
     */
    busquedaDeSesiones: function (req, res) {

        var fechaInicio = req.param('fechaInicio'),
            fechaFin = req.param('fechaFin'),
            modelo = req.param('modelo'),
            escenario = req.param('escenario'),
            condiciones = {},
            escenariosCondition = {},
            listaIndexadaEscenariosIDs = {};


        if (req.session.user.tipo === 'Entrenador')
            condiciones.usuario_id = req.session.alumno.id;
        else
            condiciones.usuario_id = req.session.user.id;
        

        if (modelo || modelo === 0)
            condiciones.ModeloDeAeronave = modelo;

        if (fechaInicio) {

            condiciones.Fecha = {
                $gte: new Date(fechaInicio),
                $lt: null,
            };

            if (fechaFin) {
                condiciones.Fecha.$lt = new Date(fechaFin);
            }
            else {
                condiciones.Fecha.$lt = new Date(fechaInicio);
            }
            
            // Le agrego 23h 59h 59s de tiempo a la fecha máxima para establecer el rango correcto.
            condiciones.Fecha.$lt.setTime(condiciones.Fecha.$lt.getTime() + ((24 * 60 * 60 * 1000) - 1));
        }
    
        if (escenario) {
            escenariosCondition.Nombre = new RegExp(escenario, 'i');
        }

        Escenario.find(escenariosCondition).exec(function(err, escenarios) {

            if (err) {
                return console.log(err);
            }

            var ListaEscenariosIDs = [];
            for (var i = escenarios.length - 1; i >= 0; i--) {

                delete escenarios[i].Etapas;// No se necesitan las etapas.
                ListaEscenariosIDs.push(escenarios[i].id);
                listaIndexadaEscenariosIDs[escenarios[i].id] = escenarios[i];
            }

            if (ListaEscenariosIDs)
                condiciones.escenario_id = ListaEscenariosIDs;
            

            SesionDeEntrenamiento.find(condiciones).exec(function(err, sesiones) {

                if (err) {
                    return console.log(err);
                }

                for (var i = sesiones.length - 1; i >= 0; i--) {
                    
                    // Asigno los datos del escenario
                    if (listaIndexadaEscenariosIDs.hasOwnProperty(sesiones[i].escenario_id))
                        sesiones[i].Escenario = listaIndexadaEscenariosIDs[sesiones[i].escenario_id];
                    else
                        sesiones[i].Escenario = { Nombre: 'Eliminado' };

                     sesiones[i].Evaluacion = calificacionesService.convertirPromedioEnTextoDeEvaluacion(sesiones[i].CalificacionGeneral);

                    // Quito los datos que no se usarán
                    delete sesiones[i].GrabacionDeInstrumentacion;
                    delete sesiones[i].GrabacionDeInterruptores;
                    delete sesiones[i].Hitos;
                }

                return res.json(sesiones);
            });
        });
    },


  /**
   * Overrides for the settings in `config/controllers.js`
   * (specific to AlumnoController)
   */
  _config: {}

  
};
