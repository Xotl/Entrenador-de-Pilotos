using System.Collections.Generic;

public class DiccionarioBidireccional<TUno, TDos>
{
    private Dictionary<TUno, TDos> DiccionarioUno;
    private Dictionary<TDos, TUno> DiccionarioDos;

    public DiccionarioBidireccional()
    {
        this.DiccionarioUno = new Dictionary<TUno, TDos>();
        this.DiccionarioDos = new Dictionary<TDos, TUno>();
    }

    /// <summary>
    /// Agrega una nueva definición al diccionario.
    /// </summary>
    public void Add(TUno objeto1, TDos objeto2)
    {
        this.Agregar(objeto1, objeto2);
    }

    /// <summary>
    /// Agrega una nueva definición al diccionario.
    /// </summary>
    public void Add(TDos objeto2, TUno objeto1)
    {
        this.Agregar(objeto1, objeto2);
    }

    private void Agregar(TUno objeto1, TDos objeto2)
    {
        if (this.DiccionarioUno.ContainsKey(objeto1) || this.DiccionarioDos.ContainsKey(objeto2))
        {
            throw new IndiceDuplicadoException("Uno de los elementos ya se encuentra en el diccionario.");
        }

        this.DiccionarioUno.Add(objeto1, objeto2);
        this.DiccionarioDos.Add(objeto2, objeto1);
    }

    /// <summary>
    /// Remueve una nueva definición del diccionario.
    /// </summary>
    public bool Remove(TUno objeto1)
    {
        TDos objeto2 = this.DiccionarioUno[objeto1];
        return this.DiccionarioUno.Remove(objeto1) && this.DiccionarioDos.Remove(objeto2);
    }

    /// <summary>
    /// Remueve una nueva definición del diccionario.
    /// </summary>
    public bool Remove(TDos objeto2)
    {
        TUno objeto1 = this.DiccionarioDos[objeto2];
        return this.DiccionarioUno.Remove(objeto1) && this.DiccionarioDos.Remove(objeto2);
    }

    /// <summary>
    /// Quita todas las definiciones en el diccionario.
    /// </summary>
    public void Clear()
    {
        this.DiccionarioUno.Clear();
        this.DiccionarioDos.Clear();
    }

    public override string ToString()
    {
        string res = "(";

        foreach(TDos o2 in this.DiccionarioUno.Values)
        {
            res += o2 + " | " + this.DiccionarioDos[o2] + ",";
        }

        res = res.Substring(0, res.Length - 1) + ")";
        return res;
    }

    public TUno this[TDos objeto2]
    {
        get
        {
            return this.DiccionarioDos[objeto2];
        }
    }

    public TDos this[TUno objeto1]
    {
        get
        {
            return this.DiccionarioUno[objeto1];
        }
    }

    /// <summary>
    /// Obtiene el número de definiciones en el diccionario.
    /// </summary>
    public int Count
    {
        get
        {
            return this.DiccionarioUno.Count;
        }
    }
}
