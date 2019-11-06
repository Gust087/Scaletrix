using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCar : MonoBehaviour {

    //public float speed = 0;
    //public float max_speed = 300f;
    //public KeyCode button_accelerate;
    public Transform pos_guide;

    //private Rigidbody rb;
    //private Vector3 direction;


    private void Start()
    {
        //rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        transform.LookAt(pos_guide.position);
        //direction = (pos_guide.transform.position - rb.transform.position).normalized;

        //if (Input.GetKey(button_accelerate))
        //{
        //    speed = speed == 0 ? 30f : speed;
        //    Acelerar();
        //}
        //else
        //{
        //    Desacelerar();
        //}
    }
    //void Acelerar()
    //{
    //    speed += speed < max_speed ? 20f : 0;
    //    Mover();
    //}

    //void Desacelerar()
    //{
    //    speed -= speed > 0 ? 20f : 0;
    //    Mover();
    //}
    //void Mover()
    //{
    //    rb.MovePosition(rb.transform.position + direction * speed * Time.deltaTime);
    //}
    //void Descarrilar()
    //{
    //    rb.AddForce(rb.transform.position + rb.transform.forward * speed);
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "guide")
    //    {
    //        //Destroy(other.gameObject);
    //        Descarrilar();
    //    }
    //}
}
