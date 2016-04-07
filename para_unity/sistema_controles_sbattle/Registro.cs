using UnityEngine;
using UnityEngine.Networking;
using XInputDotNetPure;
using System.Collections;
using System.Collections.Generic;

public class ControlRegistrado
{
    public TipoDeControl tipoDeControl;
    public PlayerIndex idXInput;
    public NetworkConnection idConexion;
    public ControlRegistrado()
    {
        tipoDeControl = TipoDeControl.Teclado;
    }
    public ControlRegistrado(PlayerIndex index)
    {
        idXInput = index;
        tipoDeControl = TipoDeControl.XInput;
    }
    public ControlRegistrado(NetworkConnection conexion)
    {
        idConexion = conexion;
        tipoDeControl = TipoDeControl.Conexion;
    }
}

public enum TipoDeControl
{
    Teclado, XInput, Conexion
}

public class Registro {
    public static int cantidad { get { return registrados.Count; } }
    public static bool hayTeclado {
        get { return hayTecladoLocal; }
    }
    static bool hayTecladoLocal = false;

    static List<ControlRegistrado> registrados = new List<ControlRegistrado>();
    
    public static ControlRegistrado Get(int i)
    {
        //if (i >= 0 && i < cantidad) return registrados[i];
        //else return null;
        return registrados[i];
    }

    public static bool Contiene()
    {
        return hayTeclado;
    }
    public static bool Contiene(PlayerIndex xinput)
    {
        foreach (ControlRegistrado cr in registrados)
        {
            if (cr.tipoDeControl == TipoDeControl.XInput && cr.idXInput == xinput) return true;
        }
        return false;
    }
    public static bool Contiene(NetworkConnection conexion)
    {
        foreach (ControlRegistrado cr in registrados)
        {
            if (cr.tipoDeControl == TipoDeControl.Conexion && cr.idConexion.connectionId == conexion.connectionId) return true;
        }
        return false;
    }

    public static ControlRegistrado Registrar()
    {
        if (hayTeclado) return null;
        else
        {
            hayTecladoLocal = true;
            ControlRegistrado salida = new ControlRegistrado();
            registrados.Add(salida);
            return salida;
        }
    }
    public static ControlRegistrado Registrar(PlayerIndex xinput)
    {
        foreach (ControlRegistrado cr in registrados)
        {
            if (cr.tipoDeControl == TipoDeControl.XInput)
            {
                if (cr.idXInput == xinput) return null;
            }
        }
        ControlRegistrado salida = new ControlRegistrado(xinput);
        registrados.Add(salida);
        return salida;
    }
    public static ControlRegistrado Registrar(NetworkConnection conexion)
    {
        foreach (ControlRegistrado cr in registrados)
        {
            if (cr.tipoDeControl == TipoDeControl.Conexion)
            {
                if (cr.idConexion.connectionId == conexion.connectionId) return null;
            }
        }
        ControlRegistrado salida = new ControlRegistrado(conexion);
        registrados.Add(salida);
        return salida;
    }

    public static void Borrar()
    {
        for (int i = registrados.Count - 1; i >= 0; i--)
        {
            if (registrados[i].tipoDeControl == TipoDeControl.Teclado)
            {
                registrados.RemoveAt(i);
                break;
            }
        }
        hayTecladoLocal = false;
    }
    public static void Borrar(PlayerIndex xinput)
    {
        for (int i = registrados.Count - 1; i >= 0; i--)
        {
            if (registrados[i].tipoDeControl == TipoDeControl.XInput && registrados[i].idXInput == xinput)
            {
                registrados.RemoveAt(i);
                break;
            }
        }
    }
    public static void Borrar(NetworkConnection conexion)
    {
        for (int i = registrados.Count - 1; i >= 0; i--)
        {
            if (registrados[i].tipoDeControl == TipoDeControl.Conexion && registrados[i].idConexion.connectionId == conexion.connectionId)
            {
                registrados.RemoveAt(i);
                break;
            }
        }
    }
}
