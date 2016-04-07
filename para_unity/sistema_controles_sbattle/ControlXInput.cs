using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class ControlXInput : MonoBehaviour {

    public PlayerIndex pindex;
    [Header("Vibracion")]
    public float duracionVibracion = .2f;
    public float fuerzaVibracion = 1;
    Personaje personaje;

    int accionPrevia = -1;
    bool vibrando = false;
    bool startWasPressed = false;
	// Use this for initialization
	void Awake() {
        personaje = GetComponent<Personaje>();
	}
	
	// Update is called once per frame
	void Update () {
        GamePadState gpad = GamePad.GetState(pindex);

        int accionActual = -1;
        if (gpad.Buttons.B == ButtonState.Released
            && gpad.Buttons.A == ButtonState.Released
            && gpad.Buttons.X == ButtonState.Released
            && gpad.Buttons.Y == ButtonState.Released)
            accionActual = -1;
        else
        {
            if (gpad.Buttons.B == ButtonState.Pressed) accionActual = 0;
            else if (gpad.Buttons.A == ButtonState.Pressed) accionActual = 1;
            else if (gpad.Buttons.X == ButtonState.Pressed) accionActual = 2;
            else if (gpad.Buttons.Y == ButtonState.Pressed) accionActual = 3;
        }

        bool vibrar = false;

        if (gpad.DPad.Right == ButtonState.Pressed || gpad.ThumbSticks.Left.X > 0) vibrar = !personaje.IntentarFlashear(Posicion.PosicionEspecifica(0), Random.Range(0, 3));//apuntaje);
        else if (gpad.DPad.Down == ButtonState.Pressed || gpad.ThumbSticks.Left.Y < 0) vibrar = !personaje.IntentarFlashear(Posicion.PosicionEspecifica(1), Random.Range(0, 3));//apuntaje);
        else if (gpad.DPad.Left == ButtonState.Pressed || gpad.ThumbSticks.Left.X < 0) vibrar = !personaje.IntentarFlashear(Posicion.PosicionEspecifica(2), Random.Range(0, 3));//apuntaje);
        else if (gpad.DPad.Up == ButtonState.Pressed || gpad.ThumbSticks.Left.Y > 0) vibrar = !personaje.IntentarFlashear(Posicion.PosicionEspecifica(3), Random.Range(0, 3));//apuntaje);
        else if (accionActual != -1 && accionActual != accionPrevia) vibrar = !personaje.IntentarBloquear(accionActual);

        accionPrevia = accionActual;

        if (vibrar) Vibrar();

        if (gpad.Buttons.Start == ButtonState.Released && startWasPressed) FindObjectOfType<Restart>().enabled = true;
        startWasPressed = gpad.Buttons.Start == ButtonState.Pressed;
    }

    public void Vibrar()
    {
        if(!vibrando)StartCoroutine(CoVibrar());
    }
    IEnumerator CoVibrar()
    {
        vibrando = true;
        GamePad.SetVibration(pindex, fuerzaVibracion, fuerzaVibracion);
        yield return new WaitForSeconds(duracionVibracion);
        GamePad.SetVibration(pindex, 0, 0);
        vibrando = false;
    }
}
