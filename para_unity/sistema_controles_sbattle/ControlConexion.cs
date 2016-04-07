using UnityEngine;
using System.Collections;

public class ControlConexion : MonoBehaviour, ISuscriptor {

    public UnityEngine.Networking.NetworkConnection netcon;
    int accionPrevia = -1;

    Personaje personaje;
    bool suscripto;

    // Use this for initialization
    void Awake()
    {
        personaje = GetComponent<Personaje>();
    }

    void Start() {
        suscripto = ServidorControlRemoto.Suscribirse(netcon,this);
    }
    void OnDestroy()
    {
        if(suscripto)ServidorControlRemoto.Desuscribirse(netcon);
    }

    public void OnAccion(MsjDireccion msjdir)
    {
        bool vibrar = false;

        if (msjdir.id == 0)
        {
            vibrar = !personaje.IntentarFlashear(Posicion.PosicionEspecifica(msjdir.direccion), Random.Range(0, 3));
        }
        else if (msjdir.id == 1)
        {
            vibrar = !personaje.IntentarBloquear(msjdir.direccion);
        }

        if (vibrar)
        {
            //
        }
    }
}
