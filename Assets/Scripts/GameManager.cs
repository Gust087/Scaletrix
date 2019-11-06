using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Linq;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager actual_GM;

    private int actual_circuit = 1; //1=pista 8 | 2=100 años

    public RawImage sprite_btn_change_circuit;

    public GameObject eight_circuit;
    public GameObject one_hundred_circuit;

    public GameObject positions;

    private string buffer { get; set; }

    SerialPort serialPort = new SerialPort("COM8", 9600); //Inicializamos el puerto serie

    private static bool moveR = false;
    private static bool moveN = false;
    private static bool first_cycle = true;

    private static float standBy_value_R = 0; // valor de pulsador Rojo sin estar presionado
    private static float velR = 1;
    private static float standBy_value_N = 0;
    private static float velN = 1;

    private EnableFunctions EF;
    private bool R_is_pressing = false;
    private bool N_is_pressing = false;

    // Use this for initialization
    void Start () {

        actual_GM = this;
        EF = new EnableFunctions();

        serialPort.ReadTimeout = 1000; //Establecemos el tiempo de espera cuando una operación de lectura no finaliza
        serialPort.Open(); //Abrimos una nueva conexión de puerto serie

        StartCoroutine(Arduino_co()); //corutina que va leyendo los valores del serial port de la placa
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    public IEnumerator Arduino_co() //probar llamar un metodo normal desde un invoke, en lugar de una corutina
    {
        while(true)
        {
            if (serialPort.IsOpen) //comprobamos que el puerto esta abierto
            {
                try //utilizamos el bloque try/catch para detectar una posible excepción.
                {
                    //print(serialPort.ReadLine().ToString());

                    string actual_line = serialPort.ReadLine().ToString();
                    //print(actual_line);

                    if (first_cycle)
                    {
                        standBy_value_R = float.Parse(actual_line.Substring(1, 4)) - 50f; //se da un margen
                        standBy_value_N = float.Parse(actual_line.Substring(6, 4)) - 50f;
                        first_cycle = false;

                        print("Rojo inicial: " + standBy_value_R.ToString() + " - Verde inicial: " + standBy_value_N.ToString());
                    }

                    //print("Rojo: " + DecibelToLinear(float.Parse(actual_line.Substring(1, 4)) / 1000f).ToString() + " - Negro: " + actual_line.Substring(6, 4));

                    float actualR = float.Parse(actual_line.Substring(1, 4));
                    float actualN = float.Parse(actual_line.Substring(6, 4));

                    if (actualR < standBy_value_R)
                    {
                        //el pulsador Rojo esta siendo presionado
                        if (UI_manager.actual_UI.raceStarted())
                        {
                            moveR = true;
                            velR = 1f;
                            velR = velR - (actualR / 1000);
                        }
                        else if(!UI_manager.actual_UI.getIfShowingPositions() && !UI_manager.actual_UI.getIsIn321())
                        {
                            //se está en menu principal
                            if(!R_is_pressing)
                                StartCoroutine(selectPlayer(1));
                        }
                            
                    }
                    else
                        moveR = false;
                    

                    if (actualN < standBy_value_N)
                    {
                        //el pulsador Negro esta siendo presionado
                        if (UI_manager.actual_UI.raceStarted())
                        {
                            moveN = true;
                            velN = 1f;
                            velN = velN - (actualN / 1000);
                        }
                        else if (!UI_manager.actual_UI.getIfShowingPositions() && !UI_manager.actual_UI.getIsIn321())
                        {
                            //se está en menu principal
                            if (!N_is_pressing)
                                StartCoroutine(selectPlayer(2));
                        }

                    }
                    else
                        moveN = false;
                    
                }
                catch (Exception e)
                {
                    print("catch" + e.Message);
                }

            }

            yield return new WaitForSeconds(.000015f); //funca excelente con tiempo delay(30) en arduino
        }
    }

    private IEnumerator selectPlayer(int n)
    {
        switch(n)
        {
            case 1:
                EF.RedOK();
                R_is_pressing = true;
                break;
            case 2:
                EF.GreenOK();
                N_is_pressing = true;
                break;
            case 3:

                break;
            case 4:

                break;
        }
        yield return new WaitForSeconds(.3f);

        switch (n)
        {
            case 1:
                R_is_pressing = false;
                break;
            case 2:
                N_is_pressing = false;
                break;
            case 3:

                break;
            case 4:

                break;
        }
    }

    public static bool getMoveR()
    {
        return moveR;
    }
    public static bool getMoveN()
    {
        return moveN;
    }

    public static float getVelR()
    {
        //return velR + 4f;
        return convertLogToDec(velR + 4f, standBy_value_R);
    }
    public static float getVelN()
    {
        return velN + 4f;
    }

    private static float convertLogToDec(float log_val, float stand_by_val)
    {
        var rdo = log_val - (1f - (stand_by_val / 1000));
        return rdo;
    }

    public int getActualCircuit()
    {
        return actual_circuit;
    }

    public AudioSource getGameManagerAudioSource()
    {
        return gameObject.GetComponent<AudioSource>();
    }

    public void changeActualCircuit()
    {
        if (actual_circuit < 2)
            actual_circuit++;
        else
            actual_circuit = 1;

        switch(actual_circuit)
        {
            case 1:
                sprite_btn_change_circuit.texture = Resources.Load<Texture>("images/circuit_01");
                one_hundred_circuit.SetActive(false);
                eight_circuit.SetActive(true);
                break;
            case 2:
                sprite_btn_change_circuit.texture = Resources.Load<Texture>("images/circuit_02");
                eight_circuit.SetActive(false);
                one_hundred_circuit.SetActive(true);
                break;
        }
    }

    public void showPositionsOfCars()
    {


        //ya se deshabilito el acelerador de los autos, se tiene la vuelta y posicion en pista de cada auto.
        //se debe mostrar una grilla con las posiciones de los autos que corrieron
        //print("vueltas: " + GameObject.Find("verde").transform.GetChild(0).gameObject.GetComponent<FollowPath>().getLapsInRace());

        /*int laps_amarillo = GameObject.Find("amarillo").transform.GetChild(1).gameObject.GetComponent<FollowPath>().getLapsInRace();
        int laps_azul = GameObject.Find("azul").transform.GetChild(1).gameObject.GetComponent<FollowPath>().getLapsInRace();*/
        int laps_negro = GameObject.Find("verde").transform.GetChild(1).gameObject.GetComponent<FollowPath>().getLapsInRace();
        int laps_rojo = GameObject.Find("rojo").transform.GetChild(1).gameObject.GetComponent<FollowPath>().getLapsInRace();

        /*int nfp_amarillo = GameObject.Find("amarillo").transform.GetChild(1).gameObject.GetComponent<FollowPath>().getNextFollowPoint();
        int nfp_azul = GameObject.Find("azul").transform.GetChild(1).gameObject.GetComponent<FollowPath>().getNextFollowPoint();*/
        int nfp_negro = GameObject.Find("verde").transform.GetChild(1).gameObject.GetComponent<FollowPath>().getNextFollowPoint();
        int nfp_rojo = GameObject.Find("rojo").transform.GetChild(1).gameObject.GetComponent<FollowPath>().getNextFollowPoint();

        /*string aux_amarillo = laps_amarillo.ToString() + "." + nfp_amarillo.ToString();
        float nro_amarillo = float.Parse(aux_amarillo);

        string aux_azul = laps_azul.ToString() + "." + nfp_azul.ToString();
        float nro_azul = float.Parse(aux_azul);*/

        string aux_negro = laps_negro.ToString() + "." + nfp_negro.ToString();
        float nro_negro = float.Parse(aux_negro);

        string aux_rojo = laps_rojo.ToString() + "." + nfp_rojo.ToString();
        float nro_rojo = float.Parse(aux_rojo);

        //float[] resultados = { nro_amarillo, nro_azul, nro_negro, nro_rojo };
        float[] resultados = { 0, 0, nro_negro, nro_rojo };

        Array.Sort<float>(resultados); //quedan ordenados de menor a mayor
        //print(resultados[0].ToString() +" - "+ resultados[1].ToString());

        positions.SetActive(true);

        Text nombre_auto01 = GameObject.Find("nombre_auto01").GetComponent<Text>();
        Text vueltas_auto01 = GameObject.Find("vueltas_auto01").GetComponent<Text>();
        Text nombre_auto02 = GameObject.Find("nombre_auto02").GetComponent<Text>();
        Text vueltas_auto02 = GameObject.Find("vueltas_auto02").GetComponent<Text>();
        Text nombre_auto03 = GameObject.Find("nombre_auto03").GetComponent<Text>();
        Text vueltas_auto03 = GameObject.Find("vueltas_auto03").GetComponent<Text>();
        Text nombre_auto04 = GameObject.Find("nombre_auto04").GetComponent<Text>();
        Text vueltas_auto04 = GameObject.Find("vueltas_auto04").GetComponent<Text>();

        /*se muestran las posiciones*/
        vueltas_auto01.text = resultados[3].ToString();
        /*if (nro_amarillo == resultados[3])
        {
            nombre_auto01.text = "AMARILLO";
        }
        if (nro_azul == resultados[3])
        {
            nombre_auto01.text = "AZUL";
        }*/
        if (nro_negro == resultados[3])
        {
            nombre_auto01.text = "VERDE";
            GameObject.Find("verde").transform.GetChild(0).gameObject.SetActive(true); //se muestra el trofeito arriba
        }
        if (nro_rojo == resultados[3])
        {
            nombre_auto01.text = "ROJO";
            GameObject.Find("rojo").transform.GetChild(0).gameObject.SetActive(true);
        }

        vueltas_auto02.text = resultados[2].ToString();
        /*if (nro_amarillo == resultados[2])
            nombre_auto02.text = "AMARILLO";
        if (nro_azul == resultados[2])
            nombre_auto02.text = "AZUL";*/
        if (nro_negro == resultados[2])
            nombre_auto02.text = "VERDE";
        if (nro_rojo == resultados[2])
            nombre_auto02.text = "ROJO";

        UI_manager.actual_UI.setStartRace(false);
        UI_manager.actual_UI.setIfShowingPositions(true);
    }
}