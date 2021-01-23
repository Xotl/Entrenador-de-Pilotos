using System;
using System.Collections.Generic;

namespace Entrenamiento.Nucleo
{
    public class ValoresDeInstrumento : Comunicacion_JSON.IToJsonFx, ICloneable
    {
        /// <summary>
        /// Necesario para gestionar los valores.
        /// </summary>
        private List<float> _Valores;

        /// <summary>
        /// Se desencadena cuando cambia alguno de los valores.
        /// </summary>
        public event EventHandler AlCambiarUnValor;

        /// <summary>
        /// Se desencadena cuando cambia la cantidad de elementos en _Valores.
        /// </summary>
        public event EventHandler AlCambiarLaCantidad;

        public ValoresDeInstrumento(params float[] valores)
        {
            this.initialize(valores);
        }

        public ValoresDeInstrumento()
        {
            this.initialize(null);
        }

        private void initialize(float[] valores)
        {
            this._Valores = new List<float>();
            this._Valores.Add(0);

            if (valores != null)
            {
                this.Cantidad = valores.Length;
                for (int i = 0; i < valores.Length; i++)
                {
                    this[i] = valores[i];
                }
            }
        }

        /// <summary>
        /// Obtiene o establece la cantidad de valores existentes. Debe ser mayor o igual a 1.
        /// </summary>
        public int Cantidad
        {
            get
            {
                return this._Valores.Count;
            }
            set
            {
                if (value != this.Cantidad)
                {
                    if (value < 1)
                        throw new ArgumentOutOfRangeException("Asignación de Cantidad", "No se le puede asignar un valor menor que 1.");

                    if (value > this.Cantidad)
                    {
                        while (value > this.Cantidad)
                            this._Valores.Add(0);
                    }
                    else
                    {// value es menor que this.Cantidad
                        while(this.Cantidad > value)
                            this._Valores.RemoveAt(this.Cantidad - 1);
                    }

                    this.eventoAlCambiarLaCantidad(new EventArgs());
                }
            }
        }

        public float this[int Key]
        {
            get
            {
                return this._Valores[Key];
            }
            set
            {
                if(value != this._Valores[Key])
                {
                    this._Valores[Key] = value;
                    this.eventoAlCambiarUnValor(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarLaCantidad.
        /// </summary>
        private void eventoAlCambiarLaCantidad(EventArgs e)
        {
            if (this.AlCambiarLaCantidad != null)
                this.AlCambiarLaCantidad(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarUnValor.
        /// </summary>
        private void eventoAlCambiarUnValor(EventArgs e)
        {
            if (this.AlCambiarUnValor != null)
                this.AlCambiarUnValor(this, e);
        }

        public static bool operator ==(ValoresDeInstrumento x, ValoresDeInstrumento y)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(x, y))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)x == null) || ((object)y == null))
            {
                return false;
            }
            
            return x.esIgual(y);
        }

        public static bool operator !=(ValoresDeInstrumento x, ValoresDeInstrumento y)
        {

            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(x, y))
            {
                return false;
            }

            // If one is null, but not both, return false.
            if (((object)x == null) || ((object)y == null))
            {
                return true;
            }

            if (x.Cantidad != y.Cantidad)
                return true;

            for (int i = 0; i < x.Cantidad; i++)
            {
                if (x[i] != y[i])
                    return true;
            }

            return false;
        }

        public static implicit operator float[](ValoresDeInstrumento x)
        {
            return x._Valores.ToArray();
        }

        public static implicit operator ValoresDeInstrumento(float[] x)
        {
            ValoresDeInstrumento v = new ValoresDeInstrumento();
            v.Cantidad = x.Length;
            for (int i = 0; i < v.Cantidad; i++)
            {
                v[i] = x[i];
            }
            return v;
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            ValoresDeInstrumento p = obj as ValoresDeInstrumento;
            if ((System.Object)p == null)
            {
                return false;
            }

            return this.esIgual(p);
        }

        public bool Equals(ValoresDeInstrumento p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return this.esIgual(p);
        }

        /// <summary>
        /// Compara si el objeto actual es igual al otro.
        /// </summary>
        private bool esIgual(ValoresDeInstrumento obj)
        {
            if (this.Cantidad != obj.Cantidad)
                return false;

            for (int i = 0; i < this.Cantidad; i++)
            {
                if (this[i] != obj[i])
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this._Valores.GetHashCode();
        }

        public override string ToString() 
        {
            string s = "[";

            foreach (float f in this._Valores)
                s+= f + ", ";

            if (s[s.Length - 1] == ' ' && s[s.Length - 2] == ',')
                s = s.Substring(0, s.Length - 2) + "]";
            else
                s += "]";

            return s;
        }

        /// <summary>
        /// Devuelve un arreglo en notación Json. Ejemplo: "[1,2,3]"
        /// </summary>
        /// <returns></returns>
        public string toJSON()
        {
            System.Text.StringBuilder json = new System.Text.StringBuilder();
            json.Append("[");
            
            int count = this.Cantidad;
            foreach (float valor in this._Valores)
            {
                json.Append(valor + (--count == 0 ? "" : ","));
            }
            json.Append("]");
            
            return json.ToString();
        }

        public object Clone()
        {
            ValoresDeInstrumento clon = new ValoresDeInstrumento();
            clon.Cantidad = this.Cantidad;
            for (int i = 0; i < clon.Cantidad; i++)
            {
                clon[i] = this[i];
            }
            return clon;
        }
    }
}