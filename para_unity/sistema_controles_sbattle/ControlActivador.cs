using UnityEngine;
using System.Collections;

public class ControlActivador : MonoBehaviour
{

    public int id;
    // Use this for initialization
    void Awake()
    {
        if (Registro.cantidad >= 0 && Registro.cantidad > id)
        {
            GetComponent<Personaje>().enabled = true;
            ControlRegistrado conreg = Registro.Get(id);
            if (conreg.tipoDeControl == TipoDeControl.XInput)
            {
                ControlXInput input = gameObject.AddComponent<ControlXInput>();
                input.pindex = conreg.idXInput;
            }
            else if (conreg.tipoDeControl == TipoDeControl.Conexion)
            {
                ControlConexion input = gameObject.AddComponent<ControlConexion>();
                input.netcon = conreg.idConexion;
            }
            else if (conreg.tipoDeControl == TipoDeControl.Teclado)
            {
                ControlTeclado input = gameObject.AddComponent<ControlTeclado>();
            }
        }
#if UNITY_EDITOR
        else if (Registro.cantidad == 0 && id == 0)
        {
            GetComponent<Personaje>().enabled = true;
            ControlTeclado input = gameObject.AddComponent<ControlTeclado>();
        }
#endif
        else
        {
            GetComponent<Animator>().enabled = GetComponent<SpriteRenderer>().enabled = false;
        }
    }

}