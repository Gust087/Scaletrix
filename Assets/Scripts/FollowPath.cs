using UnityEngine;
using System.Collections;
using System;

public class FollowPath : MonoBehaviour
{
    public Transform[] points_eight_circuit;
    public Transform[] points_one_hundred_circuit;

    private Transform[] points;

    private bool points_initialized = false;

    public KeyCode accelerate_button;

    private Vector3 heading;
    private Vector3 direction;
    private Vector3 direction_fc;
    private Vector3 pos_guide;
    private Vector3 pos_derail;

    private Vector3 point_A; //Punto A para Lerp
    private Vector3 point_B; //Punto B para Lerp

    Rigidbody rb;

    int current_index = 0; //Index para mover en puntos
    float t; //Factor tiempo de Lerp
    float factorT; //Factor de movimiento
    float distance_next = 10; // Distancia al siguiente punto

    float m; //masa del vehículo
    float R; //Radio, distancia al centro del campo centrífugo
    float Fr; //Valor máximo de fuerza de rozamiento dado por el producto del coeficiente de rozamiento estático por la reacción del plano
    float N; //Peso del vehículo (m*g) 
    float U = 0.2f; //Fuerza de rozamiento del vehículo en movimiento
    float drag = 2.5f; //Arrastre del vehículo en inercia

    float max_speed = 6;
    float acceleration = 2;
    float current_speed = 0;

    private bool enabled_run = true;
    private bool derail = false;
    private bool return_race = false;

    /*variables para controlar posicion en carrera*/
    private int laps_in_race = 0;
    private int next_follow_point = 0;
    /*++++++++++++++++++++++++++++++++++++++++++++*/

    private AudioSource audio_source_car;
    float sound_vel = 1;
    float prev_vol = 0;

    private EnableFunctions EF;

    void Start()
    {
        t = 1f;
        //CalculatePoint(points, current_index);
        rb = GetComponent<Rigidbody>();
        m = rb.mass;
        U = rb.angularDrag;
        drag = rb.drag;
        N = m * 9.8f;
        Fr = U * N;

        audio_source_car = gameObject.GetComponent<AudioSource>();
        EF = new EnableFunctions();
    }

    void FixedUpdate()
    {
        if (UI_manager.actual_UI.raceStarted() && !UI_manager.actual_UI.getIfShowingPositions() && !UI_manager.actual_UI.getIsIn321())
        {
            switch (accelerate_button.ToString())
            {
                case "R":
                    if (GameManager.getMoveR())
                    {
                        CalculateVel(GameManager.getVelR(), GameManager.getMoveR());
                    }
                    break;
                case "N":
                    if (GameManager.getMoveN())
                    {
                        CalculateVel(GameManager.getVelN(), GameManager.getMoveN());
                    }
                    break;
            }
            if (Input.GetKey(accelerate_button))
            {
                CalculateVel(1f, true); //se le pasa hardcodeado 1f, que es la velocidad maxima del pulsador, para poder jugar con las teclas
            }
            else
            {
                CalculateVel(1f, false);
            }
        }
        else if(UI_manager.actual_UI.getIfShowingPositions())
        {
            //cuando se muestran las posiciones, los autitos siguen acelerando solos a una velocidad baja
            if(current_speed < (max_speed / 8))
                CalculateVel(.5f, true);
            else
                CalculateVel(0f, false);
        }
    }

    private void updPointsOfCar()
    {
        switch (GameManager.actual_GM.getActualCircuit())
        {
            case 1:
                points = points_eight_circuit;
                break;
            case 2:
                points = points_one_hundred_circuit;
                break;
        }
        CalculatePoint(points, 0);

        points_initialized = true;
    }

    void CalculateVel(float i, bool state)
    {
        if (state && !derail && enabled_run) //|| GameManager.getMove()
        {
            if (!points_initialized)
                updPointsOfCar();

            if (current_speed > max_speed)
            {
                Derail();
            }
            else
            {
                Accelerate(i);
            }
        }
        else if (current_speed <= 0 && !return_race && !enabled_run)//&& rb.velocity == new Vector3(0, 0, 0))
        {
            //pos_derail = rb.transform.position; //Vector3.Lerp(point_A, point_B, t);
            return_race = true;
        }
        else if (return_race)
        {
            Return();
        }
        else
        {
            Decelerate();
        }
        
    }

    private void Derail()
    {
        enabled_run = false;
        derail = true;
        current_speed /= drag;
        print(gameObject.name);
        /*gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<Rigidbody>().drag = 0; 
        gameObject.GetComponent<Rigidbody>().mass = 10000;*/

        /*se modifican las propiedades del rigidbody del padre del Cube (es decir, del objeto car completo)*/
        rb.isKinematic = false;
        rb.drag = 0; //si no se le setea en 0 el drag, se mueve lentamente
        rb.mass = 10000; //se agrega masa para que el auto pese mas y caiga más rapidamente
        Decelerate();
    }

    void Accelerate(float vel_pulsador)
    {
        //print(vel_pulsador);

        current_speed += current_speed < (max_speed) ? acceleration * Mathf.Abs(Time.deltaTime * vel_pulsador) : 0;
        Move();
        
    }

    void Decelerate()
    {
        current_speed -= current_speed > 0 ? acceleration * Mathf.Abs(Time.deltaTime) * 3f : 0;
        if(current_speed > 0) Move();
        else if(current_speed < 0) current_speed = 0;
        
    }

    void Return()
    {
        if (current_speed == 0)
        {
            var a = Vector3.Distance(rb.transform.position, pos_guide);
            if (Vector3.Distance(rb.transform.position, pos_guide) <= 15)
            {
                enabled_run = true;
                derail = false;
                rb.drag = drag; //si no se le setea en 0 el drag, se mueve lentamente
                rb.mass = m; //se agrega masa para que el auto pese mas y caiga más rapidamente
                rb.useGravity = true;
                rb.isKinematic = true;
                return_race = false;
            }
            else
            {
                pos_guide = Vector3.Lerp(point_A, point_B, t);
                rb.position = pos_guide;
                return_race = true;
                //
                //direction = (pos_guide - rb.transform.position).normalized;
                //rb.transform.LookAt(pos_guide);
                //rb.MovePosition(rb.transform.position + direction * 5);
            }
        }
    }

    void Move()
    {
        if (!derail)
        {
            if (Vector3.Distance(pos_guide, rb.transform.position) < distance_next)
            {
                current_index++;
                //se chequea si es el ultimo tramo
                if (current_index == points.Length - 1)
                {
                    current_index = 0;
                    next_follow_point = 0;
                    laps_in_race++;
                }
                next_follow_point = current_index; //next_follow_point se usa para sacar en que parte de la pista quedo el auto cuando termino la carrera
                CalculatePoint(points, current_index);
            }

            pos_guide = Vector3.Lerp(point_A, point_B, t);
            direction = (pos_guide - rb.transform.position).normalized;
            rb.transform.LookAt(pos_guide);
            rb.MovePosition(rb.transform.position + direction * current_speed);
        }
        else
        {
            rb.MovePosition(rb.transform.position + direction * current_speed);
        }

        //print((current_speed / 10).ToString());

        if (!UI_manager.actual_UI.getIfShowingPositions()) //sonido del auto si no se esta mostrando las posiciones
        {
            sound_vel = sound_vel - .3f;
            if (!audio_source_car.isPlaying)
            {
                sound_vel = 1;
                StartCoroutine(playCarSound());
            }
            //audio_source_car.pitch = sound_vel;

            float sound_vol = (current_speed / 10);
            if (sound_vol < .1f)
            {
                if (prev_vol < sound_vol)
                    audio_source_car.volume = sound_vol + (.1f - sound_vol);
                else
                    audio_source_car.volume = 0;
            }
            else
                audio_source_car.volume = sound_vol;

            prev_vol = sound_vol;
        }
    }

    IEnumerator playCarSound()
    {
        audio_source_car.Play();
        yield return new WaitForSeconds(.3f);
        audio_source_car.Stop();
    }

    void CalculatePoint(Transform[] Lapt, int index)
    {
        point_A = Lapt[index].position;
        point_B = Lapt[index + 1].position;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "campo_centrifugo")
        {
            heading = transform.position - other.transform.position; // Apunta  del centro al auto
            R = heading.magnitude;
            direction_fc = heading / R; // Dirección de empuje hacia afuer normalizada
            direction_fc.y = 0;

            //Debug.Log("Velocidad max: " + max_speed + " Vel Actual: " + current_speed + " Dir: " + direction + "FC: " + direction_fc);

            max_speed = Mathf.Sqrt(Fr * R / m);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "campo_centrifugo")
        {
            max_speed = 10;
        }

    }

    public int getLapsInRace()
    {
        return laps_in_race;
    }

    /*TODO: getNextFollowPoint() devuelve el numero del siguiente punto a seguir en la pista. sirve para estimar que porcentaje de la vuelta actual ya tenia completada*/
    public int getNextFollowPoint()
    {
        return next_follow_point;
    }
}
