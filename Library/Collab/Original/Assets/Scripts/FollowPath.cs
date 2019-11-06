using UnityEngine;
using System.Collections;
using System;

public class FollowPath : MonoBehaviour
{
    public Transform[] points;
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

    float max_speed = 20;
    float acceleration = 10;
    float current_speed = 0;

    private bool enabled_run = true;
    private bool derail = false;
    private bool return_race = false;

    /*variables para controlar posicion en carrera*/
    private int laps_in_race = 0;
    private int next_follow_point = 0;
    /*++++++++++++++++++++++++++++++++++++++++++++*/

    void Start()
    {
        t = 1f;
        CalculatePoint(points, current_index);
        rb = GetComponent<Rigidbody>();
        m = rb.mass;
        U = rb.angularDrag;
        drag = rb.drag;
        N = m * 9.8f;
        Fr = U * N;
    }

    void FixedUpdate()
    {
        if(Input.GetKey(accelerate_button) && !derail && enabled_run && UI_manager.actual_UI.start_race) //|| GameManager.getMove()
        {
            if (current_speed > max_speed)
            {
                Derail();
            }
            else
            {
                Accelerate();
            }
        }
        else if (current_speed <= 0 && !return_race && !enabled_run )//&& rb.velocity == new Vector3(0, 0, 0))
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

    void Accelerate()
    {
        current_speed += current_speed < max_speed ? acceleration * Mathf.Abs(Time.deltaTime) : 0;
        Move();
    }

    void Decelerate()
    {
        current_speed -= current_speed > 0 ? acceleration * Mathf.Abs(Time.deltaTime) : 0;
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
            }
            else
            {
                pos_guide = Vector3.Lerp(point_A, point_B, t);
                rb.position = pos_guide;
                return_race = false;
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
            max_speed = 20;
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
