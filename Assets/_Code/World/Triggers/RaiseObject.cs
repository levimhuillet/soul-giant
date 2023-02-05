using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseObject : MonoBehaviour
{
    [SerializeField] private GameObject m_ToRaise;
    [SerializeField] private float m_RaiseSpeed;

    private enum RiseState { 
        Idle,
        Rising
    }

    private RiseState m_State;

    private void Start() {
        m_State = RiseState.Idle;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            m_State = RiseState.Rising;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_State == RiseState.Rising) {
            m_ToRaise.transform.position += new Vector3(0, m_RaiseSpeed * Time.deltaTime, 0);
        }
    }
}
