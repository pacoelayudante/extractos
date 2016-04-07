using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;
using System.Collections;

public class Menu : MonoBehaviour
{
    public string escenaJuego = "x";
    public float tiempoHoldPlay = 1;
    public GameObject[] panel;
    public Button botServidor, botTest;//, botTeclado;
    public Texture icoTeclado, icoXInput, icoSmartf, icoDisponible;
    public Sprite[] chars;

    RawImage[] imagenControl;
    Image[] imagenChar;
    ControlRegistrado[] controles;
    float cuentaHoldPlay = -1;
    bool testActivo;

    void Awake()
    {
        imagenChar = new Image[panel.Length];
        imagenControl = new RawImage[panel.Length];
        for (int i = 0; i < imagenChar.Length; i++)
        {
            imagenChar[i] = panel[i].GetComponentsInChildren<Image>()[1];
            imagenControl[i] = panel[i].GetComponentInChildren<RawImage>();
        }
    }

    public void ActivarServidor()
    {
        ServidorControlRemoto.Activar();
    }

    public void ActivarTest()
    {
        if (testActivo)
        {
            botTest.GetComponentInChildren<Text>().text = "Activar Test Latencia";
            MsjTest msj = new MsjTest();
            msj.accion = CodigoMensaje.Finalizar;
            ServidorControlRemoto.Enviar(TipoMensaje.Test, msj);
            testActivo = false;
        }
        else
        {
            botTest.GetComponentInChildren<Text>().text = "Desactivar Test Latencia";
            MsjTest msj = new MsjTest();
            msj.accion = CodigoMensaje.Iniciar;
            ServidorControlRemoto.Enviar(TipoMensaje.Test, msj);
            testActivo = true;
        }
    }

    public void ActivarTeclado(bool activar)
    {
        if (Registro.hayTeclado != activar)
        {
            if (activar) Registro.Registrar();
            else Registro.Borrar();
        }
    }

    public void Play()
    {
        if (Registro.cantidad > 0) UnityEngine.SceneManagement.SceneManager.LoadScene(escenaJuego);
    }

    void Update()
    {
        if (botServidor.interactable)
        {
            if (ServidorControlRemoto.activo) botServidor.interactable = false;
            else if (Input.GetKeyDown(KeyCode.S)) ServidorControlRemoto.Activar();
        }
        else if (!botTest.interactable) botTest.interactable = true;

        if (Input.GetKeyDown(KeyCode.T)) ActivarTeclado(!Registro.hayTeclado);
        if (Input.GetKeyDown(KeyCode.Return)) Play();

        foreach (PlayerIndex pi in (PlayerIndex[])System.Enum.GetValues(typeof(PlayerIndex)))
        {
            GamePadState gps = GamePad.GetState(pi);
            if (gps.IsConnected)
            {
                if (gps.Buttons.Start == ButtonState.Pressed)
                {
                    Registro.Registrar(pi);
                    if (cuentaHoldPlay < 0) cuentaHoldPlay = 0;
                }
                else if (gps.Buttons.Start == ButtonState.Released)
                {
                    if (cuentaHoldPlay >= 0) cuentaHoldPlay = -1;
                }
                if (gps.Buttons.Back == ButtonState.Pressed)
                {
                    if (Registro.Contiene(pi)) Registro.Borrar(pi);
                }
            }
            else
            {
                if (Registro.Contiene(pi)) Registro.Borrar(pi);
            }
        }

        if (cuentaHoldPlay >= 0)
        {
            cuentaHoldPlay += Time.deltaTime;
            if (cuentaHoldPlay >= tiempoHoldPlay) Play();
        }

        for (int i = 0; i < panel.Length; i++)
        {
            if (i > Registro.cantidad)
            {
                if (panel[i].activeSelf)
                {
                    panel[i].SetActive(false);
                }
            }
            else if (i == Registro.cantidad)
            {
                if (!panel[i].activeSelf)
                {
                    panel[i].SetActive(true);
                }
                if (imagenChar[i].sprite != null)
                {
                    imagenControl[i].texture = icoDisponible;
                    imagenChar[i].sprite = null;
                }
            }
            else
            {
                ControlRegistrado reg = Registro.Get(i);
                if (reg.tipoDeControl == TipoDeControl.Teclado) imagenControl[i].texture = icoTeclado;
                else if (reg.tipoDeControl == TipoDeControl.XInput) imagenControl[i].texture = icoXInput;
                else if (reg.tipoDeControl == TipoDeControl.Conexion) imagenControl[i].texture = icoSmartf;

                if (imagenChar[i].sprite == null) imagenChar[i].sprite = chars[0];
            }
        }
    }
}