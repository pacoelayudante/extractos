using UnityEngine;
using System.Collections;

public class ControlTeclado : MonoBehaviour {

    [Header("Salto")]
    public KeyCode derecha = KeyCode.D;
    public KeyCode abajo = KeyCode.S;
    public KeyCode izquierda = KeyCode.A;
    public KeyCode arriba = KeyCode.W;
    [Header("Bloqueo")]
    public KeyCode B = KeyCode.RightArrow;
    public KeyCode A = KeyCode.DownArrow;
    public KeyCode X = KeyCode.LeftArrow;
    public KeyCode Y = KeyCode.UpArrow;

    int accionPrevia = -1;

    Personaje personaje;

    // Use this for initialization
    void Awake () {
        personaje = GetComponent<Personaje>();
	}
	
	// Update is called once per frame
	void Update () {
        int accionActual = -1;
        if (!Input.GetKey(B)
            && !Input.GetKey(A)
            && !Input.GetKey(X)
            && !Input.GetKey(Y))
            accionActual = -1;
        else
        {
            if (Input.GetKey(B)) accionActual = 0;
            else if (Input.GetKey(A)) accionActual = 1;
            else if (Input.GetKey(X)) accionActual = 2;
            else if (Input.GetKey(Y)) accionActual = 3;
        }

        if (Input.GetKey(derecha)) personaje.IntentarFlashear(Posicion.PosicionEspecifica(0), Random.Range(0, 3));//apuntaje);
        else if (Input.GetKey(abajo)) personaje.IntentarFlashear(Posicion.PosicionEspecifica(1), Random.Range(0, 3));//apuntaje);
        else if (Input.GetKey(izquierda)) personaje.IntentarFlashear(Posicion.PosicionEspecifica(2), Random.Range(0, 3));//apuntaje);
        else if (Input.GetKey(arriba)) personaje.IntentarFlashear(Posicion.PosicionEspecifica(3), Random.Range(0, 3));//apuntaje);
        else if (accionActual != -1 && accionActual != accionPrevia) personaje.IntentarBloquear(accionActual);

        accionPrevia = accionActual;

        if (Input.GetKeyDown(KeyCode.Return)) FindObjectOfType<Restart>().enabled = true;
    }
}
