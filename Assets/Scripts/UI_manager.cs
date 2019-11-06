using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_manager : MonoBehaviour {

    public GameObject initial_screen;
    public GameObject sel_1_ok;
    public GameObject sel_2_ok;
    public GameObject sel_3_ok;
    public GameObject sel_4_ok;

    public GameObject count_1;
    public GameObject count_2;
    public GameObject count_3;

    public GameObject lbl_countdown;

    public GameObject car_1;
    public GameObject car_2;
    public GameObject car_3;
    public GameObject car_4;

    public GameObject timer_gral;
    public GameObject race_timer;
    public float timeLeft = 180;


    [HideInInspector] public bool start_race = false;
    [HideInInspector] public bool is_in_321 = false;
    [HideInInspector] public bool showing_positions = false;

    public static UI_manager actual_UI;
    public static EnableFunctions EF;

    private AudioClip countdown_beep;
    private AudioClip race_finished_song;
    private bool final_song = false;
    private AudioClip main_menu_song;
    private bool mm_song = false;

    IEnumerator cr;
    private float seconds_remaining;

    // Use this for initialization
    public void Start() {
        actual_UI = this;
        EF = new EnableFunctions();
        countdown_beep = Resources.Load<AudioClip>("Sounds/countdown_beep");
        race_finished_song = Resources.Load<AudioClip>("Sounds/race_finished_song");
        main_menu_song = Resources.Load<AudioClip>("Sounds/main_menu_song");

        lbl_countdown.SetActive(false);

        cr = startCountdown();
    }

    // Update is called once per frame
    public void Update()
    {
        if (start_race /*&& !showing_positions && !is_in_321*/)
        {
            timeLeft -= Time.deltaTime;
            race_timer.gameObject.GetComponent<Text>().text = timeLeft.ToString("#.##");
            if (timeLeft < 5.00f && timeLeft > 2.00f && !final_song)
            {
                gameObject.GetComponent<AudioSource>().PlayOneShot(race_finished_song);
                final_song = true;
            }

            if (timeLeft < 0)
            {
                start_race = false;
                timer_gral.SetActive(false);
                GameObject.Find("GameManager").GetComponent<GameManager>().showPositionsOfCars();
            }
        }
        else if (!showing_positions && !is_in_321)
        {
            int count = 0;

            if (sel_1_ok.active)
                count++;
            if (sel_2_ok.active)
                count++;
            if (sel_3_ok.active)
                count++;
            if (sel_4_ok.active)
                count++;
            if (count >= 2)
            {
                //hay al menos 2 jugadores listos para jugar
                if (seconds_remaining == 5)
                {
                    lbl_countdown.SetActive(true);
                    StartCoroutine(cr);
                }
            }
            else
            {
                //hay menos de 2 jugadores listos
                stopCR();
            }

            if (final_song)
            {            
                gameObject.GetComponent<AudioSource>().Stop();
                final_song = false;
            }

            if(!gameObject.GetComponent<AudioSource>().isPlaying)
            {
                gameObject.GetComponent<AudioSource>().PlayOneShot(main_menu_song);
                mm_song = true;
            }
        }
        else
        {
            //mostrando las posiciones al final de la carrera

        }
    }

    public void InitialScreenDone()
    {
        initial_screen.gameObject.SetActive(false);
        StartCoroutine(Counter());
    }

    public void stopCR()
    {
        try
        {
            seconds_remaining = 5;
            lbl_countdown.SetActive(false);
            StopCoroutine(cr);
        }
        catch { }
    }

    private IEnumerator Counter()
    {
        stopCR();
        if (mm_song)
        {
            gameObject.GetComponent<AudioSource>().Stop();
            mm_song = false;
        }

        count_3.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        count_3.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(0.5f);
        count_2.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        count_2.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(0.5f);
        count_1.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        count_1.gameObject.SetActive(false);

        setIsIn321(false);
        setStartRace(true);

        timer_gral.SetActive(true);
    }

    private IEnumerator startCountdown()
    {
        while (true)
        {
            lbl_countdown.GetComponent<Text>().text = ((int)seconds_remaining).ToString();
            seconds_remaining--;
            if (seconds_remaining > 0 && seconds_remaining < 10)
                gameObject.GetComponent<AudioSource>().PlayOneShot(countdown_beep);

            if (seconds_remaining < 0)
            {
                lbl_countdown.SetActive(false);
                //se llego a cero, arranca la carrera
                EF.StartRace();
            }

            yield return new WaitForSecondsRealtime(1f);
        }
    }

    public void Sel1OK(bool red)
    {
        sel_1_ok.gameObject.SetActive(red);
        car_1.gameObject.SetActive(red);
    }
    public void Sel2OK(bool green)
    {
        sel_2_ok.gameObject.SetActive(green);
        car_2.gameObject.SetActive(green);
    }
    public void Sel3OK(bool blue)
    {
        sel_3_ok.gameObject.SetActive(blue);
        car_3.gameObject.SetActive(blue);
    }
    public void Sel4OK(bool yellow)
    {
        sel_4_ok.gameObject.SetActive(yellow);
        car_4.gameObject.SetActive(yellow);
    }

    public void setCarPos(int car_n, Vector3 pos)
    {
        switch(car_n)
        {
            case 1:
                car_1.gameObject.transform.position = pos;
                break;
            case 2:
                car_2.gameObject.transform.position = pos;
                break;
            case 3:
                car_3.gameObject.transform.position = pos;
                break;
            case 4:
                car_4.gameObject.transform.position = pos;
                break;
        }
    }

    public void setCarRot(int car_n, Quaternion rot)
    {
        switch (car_n)
        {
            case 1:
                car_1.gameObject.transform.rotation = rot;
                break;
            case 2:
                car_2.gameObject.transform.rotation = rot;
                break;
            case 3:
                car_3.gameObject.transform.rotation = rot;
                break;
            case 4:
                car_4.gameObject.transform.rotation = rot;
                break;
        }
    }

    public bool raceStarted()
    {
        return start_race;
    }
    public void setStartRace(bool val)
    {
        start_race = val;
    }

    public bool getIfShowingPositions()
    {
        return showing_positions;
    }
    public void setIfShowingPositions(bool val)
    {
        showing_positions = val;
    }

    public bool getIsIn321()
    {
        return is_in_321;
    }
    public void setIsIn321(bool val)
    {
        is_in_321 = val;
    }
}
