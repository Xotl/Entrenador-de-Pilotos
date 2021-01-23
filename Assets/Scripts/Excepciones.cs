using System;

class NoHayElementosException: Exception
{
    public NoHayElementosException(string message)
        : base(message)
    {
    }
}

class PosicionInvalidaException : Exception
{
    public PosicionInvalidaException(string message)
        : base(message)
    {
    }
}

class IndiceDuplicadoException : Exception
{
    public IndiceDuplicadoException(string message)
        : base(message)
    {
    }

    public IndiceDuplicadoException()
    {
    }
}