using UnityEngine;

public class EnableFunctions : MonoBehaviour {


    private static bool red;
    private static bool green;
    private static bool blue;
    private static bool yellow;

    // Use this for initialization
    void Start () {
        red = false;
        green = false;
        blue = false;
        yellow = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartRace()
    {
        /*chequeo qué autos están y se acomodan segun el circuito que se eligió para correr*/
        int circuito_actual = GameManager.actual_GM.getActualCircuit();
        switch (circuito_actual)
        {
            case 1:
                //se juega en la pista 8
                if (red)
                {
                    UI_manager.actual_UI.setCarPos(1, new Vector3(50.57f, 89.13f, 162.49f));
                    UI_manager.actual_UI.setCarRot(1, new Quaternion(0, 210f, 0, 0));
                }
                if (green) //negro
                {
                    UI_manager.actual_UI.setCarPos(2, new Vector3(54.63f, 89.1f, 160f));
                    UI_manager.actual_UI.setCarRot(2, new Quaternion(0, 210f, 0, 0));
                }
                break;
            case 2:
                //se juega en la pista de los 100 años
                if (red)
                {
                    UI_manager.actual_UI.setCarPos(1, new Vector3(21.7f, 26.94f, 278.57f));
                    UI_manager.actual_UI.setCarRot(1, new Quaternion(0, 360f, 0, 0));
                }
                if (green) //negro
                {
                    UI_manager.actual_UI.setCarPos(1, new Vector3(26.32f, 26.94f, 278.57f));
                    UI_manager.actual_UI.setCarRot(1, new Quaternion(0, 360f, 0, 0));
                }
                break;
        }
        /*+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++*/
        UI_manager.actual_UI.setIsIn321(true);
        UI_manager.actual_UI.InitialScreenDone();
        
        GameManager.actual_GM.GetComponent<AudioSource>().Play();
    }

    public void changeCircuit()
    {
        GameManager.actual_GM.changeActualCircuit();
    }
    public void RedOK()
    {
        UI_manager.actual_UI.Sel1OK(!red);
        red = !red;
    }
    public void GreenOK()
    {
        UI_manager.actual_UI.Sel2OK(!green);
        green = !green;
    }
    public void BlueOK()
    {
        UI_manager.actual_UI.Sel3OK(!blue);
        blue = !blue;
    }
    public void YellowOK()
    {
        UI_manager.actual_UI.Sel4OK(!yellow);
        yellow = !yellow;
    }

}
