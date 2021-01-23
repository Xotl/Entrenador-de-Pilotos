using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Entrenamiento.Nucleo;
using Interfaz.Utilities;

namespace Entrenamiento.GUI
{
    public class EditorDeEscenariosController : MonoBehaviour
    {
        private enum panelDeEdicion
        {
            Editor_de_Sintoma,
            Editor_de_Solucion,
            Panel_de_No_Seleccion,
        }

        #region Campos publicos

        /// <summary>
        /// Mensaje de texto que aparecerá en medio de la pantalla.
        /// </summary>
        public GUIText msgText;

        /// <summary>
        /// Fondo del mensaje de texto que aparecerá en medio de la pantalla.
        /// </summary>
        public GUITexture msgTextFondo;

        /// <summary>
        /// Scroll principal que contiene las etapas con sus síntomas y sus soluciones.
        /// </summary>
        public ScrollControl VisorDeEtapas;

        /// <summary>
        /// Prefab para instanciar el botón de nueva etapa.
        /// </summary>
        public BotonController PrefabBotonNuevaEtapa;

        public Instrumentos.PanelDeInstrumentosController PanelDeInstrumentos;
        public Interruptores.PanelDeInterruptoresController PanelDeInterruptores;

        /// <summary>
        /// Espaciado que existe entre cada bloque de etapas.
        /// </summary>
        public const float ESPACIADO_ENTRE_ETAPAS = 0.2f;

        /// <summary>
        /// Caja de texto que permite editar el nombre del escenario.
        /// </summary>
        public CajaDeTexto txtNombreDeEscenario;

        /// <summary>
        /// Botón de nueva etapa en el scroll.
        /// </summary>
        public BotonController btnNuevaEtapa;

        /// <summary>
        /// Prefab que representa un bloque de etapa y que se instanciará por cada etapa.
        /// </summary>
        public Transform EtapaNuevaPrefab;

        /// <summary>
        /// Prefab que representa una solución.
        /// </summary>
        public BotonController SolucionPrefab;

        /// <summary>
        /// Distancia entre el centro del mesh de una solución y otra.
        /// </summary>
        public const float DISTANCIA_ENTRE_SOLUCIONES = 1f;

        /// <summary>
        /// Distancia de espaciado que existe entre el sintomas y la primer solución de un bloque de etapa.
        /// </summary>
        public const float ESPACIADO_SINTOMA_SOLUCION = 1f;

        /// <summary>
        /// Espaciado que existe entre el último elemento de un bloque de etapa y el separador.
        /// </summary>
        public const float ESPACIO_SEPARADOR_SOLUCIONES = 0.8f;

        /// <summary>
        /// Prebab con el área donde se editan los valores de un instrumento.
        /// </summary>
        public GameObject EditorDeSintomas;
        
        /// <summary>
        /// Texto que se muestra cuando no se ha seleccionado nada.
        /// </summary>
        public GameObject PanelDeNoSeleccion;

        /// <summary>
        /// Botón que guarda los cambios en el escenario.
        /// </summary>
        public BotonController btnGuardar;

        /// <summary>
        /// Botón que mueve al usuario a la vista anterior.
        /// </summary>
        public BotonController btnRegresar;

        #endregion


        #region Campos privados

        /// <summary>
        /// Asociación existente entre cada etapa del escenario y cada bloque de etapa en la interfaz.
        /// </summary>
        private DiccionarioBidireccional<Etapa, Transform> _relEtapasObjetos;

        /// <summary>
        /// Asociación existente entre cada solución del escenario y cada botón de solución en la interfaz.
        /// </summary>
        private DiccionarioBidireccional<Entrenamiento.Nucleo.Solucion, BotonController> _relSolucionesObjetos;

        /// <summary>
        /// Delegado usado para actualizar los síntomas relacionados con una etapa determinada.
        /// </summary>
        private System.Action<Sintoma[]> ActualizarSintomasDelegate = null;

        ///// <summary>
        ///// Delegado usado para actualizar las soluciones relacionadas con una etapa determinada.
        ///// </summary>
        //private System.Action<Solucion> ActualizarSolucionesDelegate = null;

        #endregion


        #region Propiedades

        private Entrenamiento.Nucleo.Escenario _Escenario;
        /// <summary>
        /// Obtiene o establece el escenario que se edita.
        /// </summary>
        public Entrenamiento.Nucleo.Escenario Escenario
        {
            get
            {
                return this._Escenario;
            }
            set
            {
                if (this._Escenario != value)
                {
                    if (this._Escenario != null)
                        this.LimpiarEventosEnEscenario(this._Escenario);

                    this._relEtapasObjetos.Clear();
                    this._relSolucionesObjetos.Clear();
                    
                    this._Escenario = value;

                    if (this._Escenario != null)
                    {
                        // Eventos relacionados al Escenario
                        this._Escenario.AlAgregarEtapa += new System.EventHandler<EtapaEnEscenarioEventArgs>(this.Escenario_AlAgregarEtapa);
                        this._Escenario.AlQuitarEtapa += new System.EventHandler<EtapaEnEscenarioEventArgs>(this.Escenario_AlQuitarEtapa);

                        if (string.IsNullOrEmpty(this._Escenario.Nombre))
                        {
                            this.txtNombreDeEscenario.Texto = "Nombre del escenario";
                        }
                        else
                            this.txtNombreDeEscenario.Texto = this._Escenario.Nombre;

                        this.ActualizarEscenarioEnVisorDeEtapas();
                    }
                    else
                    {// Agrega un escenario vacío
                        throw new System.NotImplementedException();
                    }
                }
            }
        }

        //private Entrenamiento.Nucleo.Instrumento[] _Instrumentos;
        private Entrenamiento.Nucleo.Interruptor[] _Interruptores;

        #endregion


        #region Eventos de Unity

        private void Start()
        {
            //this.InicializarVisorDeEtapas();
            this.MostrarPanelDeEdicion(panelDeEdicion.Panel_de_No_Seleccion);

            object data = EspacioGlobal.Sesion.Data;
            if (data == null)
            {
                Escenario nuevo = new Escenario();
                Etapa etapa = new Etapa();
                etapa.Nombre = "Etapa 1";
                nuevo.AgregarEtapa(etapa);

                this.Escenario = nuevo;
            }
            else
            {
                this.Escenario = data as Escenario;
            }
        }

        private void Awake()
        {
            // Inicializaciones
            this._relEtapasObjetos = new DiccionarioBidireccional<Etapa, Transform>();
            this._relSolucionesObjetos = new DiccionarioBidireccional<Solucion, BotonController>();
            //this._Interruptores = Aeronaves.ObtenerInterruptores(ModelosDeHelicoptero.B206L3);
            this._Interruptores = this.PanelDeInterruptores.InterruptoresDelPanel;

            // Eventos de interfaz
            this.txtNombreDeEscenario.AlCambiarTexto += this.txtNombreDeEscenario_AlCambiarTexto;
            this.PanelDeInstrumentos.AlEditarSintoma += this.PanelDeInstrumentos_AlEditarSintoma;
            this.btnGuardar.Click += this.btnGuardar_Click;
            this.btnRegresar.Click += this.btnRegresar_Click;
        }

        #region Eventos dependientes del Awake

        private void PanelDeInstrumentos_AlEditarSintoma(object sender, EditorDeSintomaController.SintomaEditadoEventArgs e)
        {
            if (this.ActualizarSintomasDelegate != null)
            {
                this.ActualizarSintomasDelegate(this.PanelDeInstrumentos.Sintomas);
            }
        }

        #endregion

        #endregion


        #region Eventos del objeto Escenario y sus dependencias

        private void Escenario_AlQuitarEtapa(object sender, EtapaEnEscenarioEventArgs e)
        {
            // Elimino sus eventos asignados
            e.Etapa.AlAgregarSolucion -= this.Etapa_AlAgregarSolucion;
            e.Etapa.AlQuitarSolucion -= this.Etapa_AlQuitarSolucion;
            e.Etapa.AlCambiarNombre -= this.Etapa_AlCambiarNombre;

            // Detruye el bloque y todas sus relaciones
            foreach (Solucion s in e.Etapa.Soluciones)
                this._relSolucionesObjetos.Remove(s);
            this.VisorDeEtapas.EliminarElemento(this._relEtapasObjetos[e.Etapa].gameObject);
            this._relEtapasObjetos.Remove(e.Etapa);

            // Renombra todas las etapas
            for (int i = 0; i < this.Escenario.Etapas.Length; i++)
                this.Escenario.Etapas[i].Nombre = "Etapa " + (i + 1);
        }

        private void Escenario_AlAgregarEtapa(object sender, EtapaEnEscenarioEventArgs e)
        {
            this.AgregarEtapaAlVisorDeEtapas(e.Etapa);
        }

        private void Etapa_AlCambiarNombre(object sender, System.EventArgs e)
        {
            Etapa etapa = (Etapa)sender;
            TextMesh texto = this._relEtapasObjetos[etapa].Find("Etapa Nombre").GetComponent<TextMesh>();
            texto.text = etapa.Nombre;
        }

        private void Etapa_AlQuitarSolucion(object sender, SolucionEnEtapaEventArgs e)
        {
            throw new System.NotImplementedException("Revisa el EditorDeEscenariosController");
        }

        private void Etapa_AlAgregarSolucion(object sender, SolucionEnEtapaEventArgs e)
        {
            this.AgregarSolucionAlVisorDeEtapas(e.Solucion, (Etapa)sender);
        }

        #endregion


        #region Métodos generales de la vista

        private void btnRegresar_Click(object sender, System.EventArgs e)
        {
            Application.ExternalCall("Regresar", string.Empty);
        }

        private void btnGuardar_Click(object sender, System.EventArgs e)
        {
            if (this.msgText.gameObject.activeInHierarchy)
                return;
            else
                this.msgText.gameObject.SetActive(true);

            
            this.msgText.text = "Guardando...";
            System.Action<string> cb = (error) =>
            {
                if (string.IsNullOrEmpty(error))
                {// Se guardó correctamente
                    this.msgText.text = "¡Guardado!";
                    this.StartCoroutine(this.DesvanecerMSgText(2));
                }
                else
                {// Ocurrió algún error.
                    this.msgText.text = "No se pudo guardar.\nVerifique su conexión e intente de nuevo.";
                    this.StartCoroutine(this.DesvanecerMSgText(4));
                }
            };
            this.Escenario.Guardar(cb);
        }

        /// <summary>
        /// Corutina que desvanece el msgText gradualmente.
        /// </summary>
        private IEnumerator DesvanecerMSgText(float segundos)
        {
            yield return new WaitForSeconds(segundos);

            Color original = this.msgText.color;
            Color originalFondo = this.msgTextFondo.color;

            Color aux = original;
            Color auxFondo = originalFondo;

            while (aux.a > 0)
            {
                aux.a = aux.a - Time.deltaTime * 2;
                auxFondo.a = auxFondo.a - Time.deltaTime * 2;
                this.msgText.color = aux;
                this.msgTextFondo.color = auxFondo;
                yield return null;
            }

            this.msgText.color = original;
            this.msgTextFondo.color = originalFondo;

            this.msgText.gameObject.SetActive(false);
        }

        private void txtNombreDeEscenario_AlCambiarTexto(object sender, System.EventArgs e)
        {
            this.Escenario.Nombre = this.txtNombreDeEscenario.Texto;
        }

        #endregion


        #region Funciones genéricas y útiles

        /// <summary>
        /// Calcula las dimensiones de un objeto dado y sus hijos.
        /// </summary>
        /// <param name="objeto">Objeto del que se desean las dimensiones.</param>
        /// <returns>Dimensiones del objeto junto con sus hijos.</returns>
        private Bounds CalcularDimensionesDeObjeto(GameObject objeto)
        {
            Bounds bounds = new Bounds(objeto.transform.position, Vector3.zero);
            Renderer[] elementos = objeto.GetComponentsInChildren<Renderer>();
            if (elementos.Length > 0)
            {// Calcula las dimensiones
                bounds = new Bounds(elementos[0].bounds.center, elementos[0].bounds.size);
                for (int i = 1; i < elementos.Length; i++)
                {
                    bounds.Encapsulate(elementos[i].bounds);
                }
            }
            return bounds;
        }

        /// <summary>
        /// Le quita todos los eventos relacionados a esta vista a un escenario.
        /// </summary>
        /// <param name="escenario">Escenario que se quiere limpiar.</param>
        private void LimpiarEventosEnEscenario(Escenario escenario)
        {
            escenario.AlAgregarEtapa -= Escenario_AlAgregarEtapa;
            escenario.AlQuitarEtapa -= Escenario_AlQuitarEtapa;

            foreach (Etapa etapa in escenario.Etapas)
            {
                etapa.AlAgregarSolucion -= Etapa_AlAgregarSolucion;
                etapa.AlQuitarSolucion -= Etapa_AlQuitarSolucion;
                etapa.AlCambiarNombre -= Etapa_AlCambiarNombre;
            }
        }

        #endregion


        #region Métodos relacionados al Scroll de Etapas

        /// <summary>
        /// Limpia el visor de etapas y lo actualiza con el escenario de la propiedad Escenario.
        /// </summary>
        private void ActualizarEscenarioEnVisorDeEtapas()
        {
            this.VisorDeEtapas.Limpiar();
            this.InicializarVisorDeEtapas();

            if (this.Escenario != null && Escenario.Etapas != null)
            {
                foreach (Etapa etapa in Escenario.Etapas)
                {
                    this.AgregarObjetoEtapaAlVisorDeEtapas(etapa);
                    foreach (Solucion solucion in etapa.Soluciones)
                    {
                        this.AgregarObjetoSolucionAlVisorDeEtapas(solucion, etapa);
                    }
                    this.ReacomodarContenidoDeUnaEtapa(etapa);
                }
            }

            this.ReacomodarEtapasEnVisorDeEtapas();
        }

        /// <summary>
        /// Inicializa el Visor de etapas agregando el botón de nueva etapa, entre otras cosas.
        /// </summary>
        private void InicializarVisorDeEtapas()
        {
            this.btnNuevaEtapa = Instantiate(this.PrefabBotonNuevaEtapa) as BotonController;
            this.VisorDeEtapas.AgregarElemento(this.btnNuevaEtapa.gameObject, new Vector2(ESPACIO_SEPARADOR_SOLUCIONES, -0.7f));
            this.btnNuevaEtapa.Click += new System.EventHandler(this.btnNuevaEtapa_Click);
        }

        /// <summary>
        /// Agrega una etapa al visor de etapas y reacomoda las etapas.
        /// </summary>
        /// <param name="etapa">Etapa que se desea agregar.</param>
        private void AgregarEtapaAlVisorDeEtapas(Etapa etapa)
        {
            this.AgregarObjetoEtapaAlVisorDeEtapas(etapa);
            this.ReacomodarEtapasEnVisorDeEtapas();
        }

        /// <summary>
        /// Agrega una solución a una etapa y reacomoda el contenido del visor.
        /// </summary>
        /// <param name="solucion">Solución a agregar.</param>
        /// <param name="etapa">Etapa a la que pertenece la solución.</param>
        private void AgregarSolucionAlVisorDeEtapas(Solucion solucion, Etapa etapa)
        {
            this.AgregarObjetoSolucionAlVisorDeEtapas(solucion, etapa);
            this.ReacomodarContenidoDeUnaEtapa(etapa);
            this.ReacomodarEtapasEnVisorDeEtapas();
        }

        /// <summary>
        /// Agrega una etapa al visor de etapas.
        /// </summary>
        /// <param name="etapa"></param>
        private void AgregarObjetoEtapaAlVisorDeEtapas(Etapa etapa)
        {
            etapa.AlAgregarSolucion += new System.EventHandler<SolucionEnEtapaEventArgs>(this.Etapa_AlAgregarSolucion);
            etapa.AlQuitarSolucion += new System.EventHandler<SolucionEnEtapaEventArgs>(this.Etapa_AlQuitarSolucion);
            etapa.AlCambiarNombre += new System.EventHandler(this.Etapa_AlCambiarNombre);

            Transform nuevoObjeto = Instantiate(this.EtapaNuevaPrefab, Vector3.zero, Quaternion.identity) as Transform;
            this._relEtapasObjetos.Add(etapa, nuevoObjeto);
            this.VisorDeEtapas.AgregarElemento(nuevoObjeto.gameObject, new Vector2(0, -0.3f));

            nuevoObjeto.Find("Etapa Nombre").GetComponent<TextMesh>().text = etapa.Nombre;
            nuevoObjeto.Find("NuevaSolucion").GetComponent<BotonController>().Click += new System.EventHandler(this.NuevaSolucion_Click);
            nuevoObjeto.Find("Sintoma").GetComponent<BotonController>().Click += new System.EventHandler(this.Sintoma_Click);
        }

        /// <summary>
        /// Agrega una solución a una etapa.
        /// </summary>
        /// <param name="solucion">Solución a agregar.</param>
        /// <param name="etapa">Etapa a la que pertenece la solución.</param>
        private void AgregarObjetoSolucionAlVisorDeEtapas(Solucion solucion, Etapa etapa)
        {
            Transform bloqueEtapa = this._relEtapasObjetos[etapa];
            BotonController botonSolucion =
                Instantiate(this.SolucionPrefab, Vector3.zero, this.SolucionPrefab.transform.rotation) as BotonController;

            this._relSolucionesObjetos.Add(solucion, botonSolucion);// Agrego la relación correspondiente.
            botonSolucion.Click += new System.EventHandler(this.botonSolucion_Click);

            this.VisorDeEtapas.AgregarElemento(botonSolucion.gameObject, Vector3.zero, bloqueEtapa);
        }

        /// <summary>
        /// Reacomoda las etapas junto con el botón de Nueva etapa dentro del Visor de etapas.
        /// </summary>
        private void ReacomodarEtapasEnVisorDeEtapas()
        {
            Transform objEtapa;
            Bounds bounds;
            float posX = 0;
            foreach(Etapa etapa in this.Escenario.Etapas)
            {
                objEtapa = this._relEtapasObjetos[etapa];
                objEtapa.localPosition = new Vector3(
                        posX,
                        objEtapa.localPosition.y,
                        objEtapa.localPosition.z
                    );

                bounds = this.CalcularDimensionesDeObjeto(objEtapa.gameObject);
                posX += bounds.size.x + ESPACIADO_ENTRE_ETAPAS;
            }

            // Reposicionado del botón de nueva solución
            this.btnNuevaEtapa.transform.localPosition = new Vector3(
                    posX + 0.8f,
                    this.btnNuevaEtapa.transform.localPosition.y,
                    this.btnNuevaEtapa.transform.localPosition.z
                );

            this.VisorDeEtapas.RecalcularBounds();
        }

        /// <summary>
        /// Reacomoda el contenido de una etapa dentro del visor de etapas.
        /// </summary>
        /// <param name="etapa">Etapa que se desea reacomodar.</param>
        private void ReacomodarContenidoDeUnaEtapa(Etapa etapa)
        {
            Transform bloque = this._relEtapasObjetos[etapa];// Bloque con el contenido
            Transform nuevaSolucionBtn = bloque.Find("NuevaSolucion");
            Transform botonSintoma = bloque.Find("Sintoma");
            Transform separador = bloque.Find("Separador");

            float posX = botonSintoma.localPosition.x + ESPACIADO_SINTOMA_SOLUCION;
            float posY = botonSintoma.localPosition.y;
            float posZ = botonSintoma.localPosition.z;

            foreach (Solucion s in etapa.Soluciones)
            {
                this._relSolucionesObjetos[s].transform.localPosition = new Vector3(posX, posY, posZ);
                posX += DISTANCIA_ENTRE_SOLUCIONES;
            }

            nuevaSolucionBtn.localPosition = new Vector3(posX, posY, posZ);
            separador.localPosition = new Vector3(posX + ESPACIO_SEPARADOR_SOLUCIONES, separador.localPosition.y, posZ);
        }   

        #endregion

        #region Eventos de los botones dentro del Visor de Etapas

        private void btnNuevaEtapa_Click(object sender, System.EventArgs e)
        {
            Etapa etapa = new Etapa();
            etapa.Nombre = "Etapa " + (this.Escenario.Etapas.Length + 1);
            this.Escenario.AgregarEtapa(etapa);
        }

        private void NuevaSolucion_Click(object sender, System.EventArgs e)
        {
            Transform boton = ((BotonController)sender).transform;
            Etapa etapa = this._relEtapasObjetos[boton.parent];
            Solucion solucion = new Solucion(this._Interruptores, new ParDeDatosInterruptorEstado[0]);
            etapa.AgregarSolucion(solucion);
        }

        private void Sintoma_Click(object sender, System.EventArgs e)
        {
            BotonController boton = (BotonController)sender;
            this.ActualizarSintomasDelegate = (s) => this._relEtapasObjetos[boton.transform.parent].Sintomas = s;
            this.MostrarPanelDeEdicion(panelDeEdicion.Editor_de_Sintoma);
            this.MostrarEditorDeSintoma(this._relEtapasObjetos[boton.transform.parent].Sintomas);
        }

        private void botonSolucion_Click(object sender, System.EventArgs e)
        {
            this.MostrarEditorDeSolucion(this._relSolucionesObjetos[(BotonController)sender]);
        }

        #endregion


        #region Métodos del Visor de Edición

        /// <summary>
        /// Muestra el panel para editar un síntoma.
        /// </summary>
        /// <param name="sintomas">Síntomas que se desean mostrar.</param>
        private void MostrarEditorDeSintoma(Entrenamiento.Nucleo.Sintoma[] sintomas)
        {
            this.MostrarPanelDeEdicion(panelDeEdicion.Editor_de_Sintoma);
            this.PanelDeInstrumentos.Sintomas = sintomas;
        }

        /// <summary>
        /// Muestra el panel para editar una solución.
        /// </summary>
        /// <param name="solucion">Solución que se quiere mostrar.</param>
        public void MostrarEditorDeSolucion(Solucion solucion)
        {
            this.MostrarPanelDeEdicion(panelDeEdicion.Editor_de_Solucion);
            this.PanelDeInterruptores.Solucion = solucion;
        }

        /// <summary>
        /// Muestra un panel de edición determinado.
        /// </summary>
        /// <param name="panel">Nombre del panel que se quiere mostrar</param>
        private void MostrarPanelDeEdicion(panelDeEdicion panel)
        {
            switch (panel)
            {
                case panelDeEdicion.Editor_de_Sintoma:
                    this.PanelDeInstrumentos.gameObject.SetActive(true);
                    this.PanelDeInterruptores.SetActive(false);
                    this.PanelDeNoSeleccion.gameObject.SetActive(false);
                    break;

                case panelDeEdicion.Editor_de_Solucion:
                    this.PanelDeInterruptores.SetActive(true);
                    this.PanelDeInstrumentos.gameObject.SetActive(false);
                    this.PanelDeNoSeleccion.gameObject.SetActive(false);
                    break;

                case panelDeEdicion.Panel_de_No_Seleccion:
                    this.PanelDeNoSeleccion.gameObject.SetActive(true);
                    this.PanelDeInterruptores.SetActive(false);
                    this.PanelDeInstrumentos.gameObject.SetActive(false);
                    break;
            }
        }

        #endregion
    }
}