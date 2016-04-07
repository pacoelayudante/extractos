/*******************************Algun otro archivo **************************************/
public class EjemploEscenarioCombate : MonoBehaviour {
    public CapitanNinja capitanNinja;

    CapitanNinjaDatos capNinjaDatos = new CapitanNinjaDatos();

    void Start()
    {
        if(capitanNinja)StartCoroutine(Combate.CapitanOleada(capitanNinja, capNinjaDatos));
    }
    
    void Update()
    {
        if (!capNinjaDatos.enEjecucion && capitanNinja)
        {
            if (Combate.HayDescanso()) StartCoroutine(Combate.CapitanOleada(capitanNinja, capNinjaDatos));
        }
    }
}

/****************************************pedazo de Combate.cs*******************************************************/

public static class Combate {

/* COSAS ... */

#region CAPITAN_NINJA
    static MotivoPausa pausaCapitanLocal = MotivoPausa.None;
    public static MotivoPausa motivoPausaCapitan
    {
        get { return pausaCapitanLocal; }
    }
    public static bool pausaCapitan
    {
        get { return pausaCapitanLocal > MotivoPausa.None; }
    }
    static public bool PausarCapitan(MotivoPausa motivo)
    {
        pausaCapitanLocal |= motivo;
        return pausaCapitan;
    }
    static public bool ContinuarCapitan(MotivoPausa motivo)
    {
        pausaCapitanLocal &= ~motivo;
        return pausaCapitan;
    }
    static public bool ContinuarCapitanForzado()
    {
        pausaCapitanLocal = MotivoPausa.None;
        return pausaCapitan;
    }

    public static IEnumerator CapitanOleada(CapitanNinja capStats, CapitanNinjaDatos datos)
    {
        datos.enEjecucion = true;
        float t = capStats.TiempoDescanso();
        List<NinjaCuerpo> posibles = new List<NinjaCuerpo>(ocultos.Count);
        foreach (Atacante a in ocultos) if (a.TipoDeAtacante() == AtacanteTipo.Ninja) posibles.Add((NinjaCuerpo)a);
        while (capStats.maximaInvocacionNinjas > 0 && posibles.Count > capStats.maximaInvocacionNinjas)
        {
            posibles.RemoveAt(Random.Range(0, posibles.Count));
        }
        while(t >= 0)
        {
            if (!pausaCapitan) t -= Time.deltaTime;
            yield return null;
        }
        t = 0;
        float tParaAtacar = posibles.Count * capStats.tiempoAparicion;
        foreach (NinjaCuerpo ninja in posibles)
        {
            if (ninja)
            {
                ninja.Revelar(t += capStats.tiempoAparicion);
                ninja.Preparar(tParaAtacar += capStats.TiempoEntreAtaques(), samuraiControl.cuerpo);
            }
        }
        datos.enEjecucion = false;
    }
#endregion
/* Mas COSAS ... */
}

[System.Flags]
public enum MotivoPausa
{
    None = 0,
    Etapa = 1 << 0,
    SamuraiCayendo = 1 << 1,
    SamuraiMuerto = 1 << 2,
    SamuraiEsquivando = 1 << 3,
    Especial = 1 << 4
}