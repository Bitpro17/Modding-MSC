using UnityEngine;
using System.Collections.Generic;
using MSCLoader;

namespace BaleSpike
{
    internal class HayBaleLogic : MonoBehaviour
    {
        Transform spike;
        Collider spikeCollider;
        Transform haybales;
        void Start ()
        {
            spike = GameObject.Find("KEKMET(350-400psi)/Frontloader/ArmPivot/Arm/LoaderPivot/Loader").transform.GetChild(3);
            spikeCollider = spike.GetComponent<Collider>();
            transform.parent = spike.parent;
            transform.localPosition = new Vector3(1.2f, 0f, -0.09f);
            transform.localEulerAngles = Vector3.zero;

            BoxCollider trigger = gameObject.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(0.03f, 0.5f, 0.01f);

            haybales = GameObject.Find("HayBales").transform;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.name.Contains("haybale"))
                return;
            spikeCollider.enabled = false;

            other.transform.parent = transform;

            Rigidbody rb = other.GetComponent<Rigidbody>();
            lockedBodies.Add(new RigidbodyHolder(rb));

        }

        void OnTriggerExit(Collider other)
        {
            if (!other.name.Contains("haybale"))
                return;

            //Physics.IgnoreCollision(spikeCollider, other.GetComponent<Collider>(), false);
            spikeCollider.enabled = true;

            other.transform.parent = haybales;
            //other.transform.localPosition += new Vector3(-1f, 0f, 0f);

            Rigidbody rb = other.GetComponent<Rigidbody>();
            Free(rb);
        }

        List<RigidbodyHolder> lockedBodies = new List<RigidbodyHolder>();

        class RigidbodyHolder
        {
            public Rigidbody rigidbody;
            public Transform transform;
            public Vector3 startLocalPos;
            public Quaternion startLocalRot;

            public RigidbodyHolder(Rigidbody rb)
            {
                this.rigidbody = rb;
                transform = rb.transform;
                startLocalPos = transform.localPosition;
                startLocalRot = transform.localRotation;
            }
        }

        void Free(Rigidbody rb)
        {
            foreach(var holder in lockedBodies)
            {
                if (holder.rigidbody == rb)
                {
                    lockedBodies.Remove(holder);
                    break;
                }    
            }
        }

        void FixedUpdate()
        {
            foreach (var holder in lockedBodies)
            {
                Rigidbody rb = holder.rigidbody;
                Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
                localVelocity.y = 0;
                localVelocity.z = 0;
                if (localVelocity.x < 0)
                {
                    localVelocity.x += 0.1f * Time.fixedDeltaTime;
                    if (localVelocity.x > 0)
                        localVelocity.x = 0;
                }
                else
                {
                    localVelocity.x -= 0.1f * Time.fixedDeltaTime;
                    if (localVelocity.x > 0)
                        localVelocity.x = 0;
                }

                if (holder.transform.localPosition.x < -0.24f)
                {
                    holder.transform.localPosition = new Vector3(-0.24f, holder.startLocalPos.y, holder.startLocalPos.z);
                    localVelocity.x = 0;
                }
                else
                    holder.transform.localPosition = new Vector3(holder.transform.localPosition.x, holder.startLocalPos.y, holder.startLocalPos.z);

                rb.velocity = transform.TransformDirection(localVelocity);
                //rb.transform.localPosition = Vector3.zero;//new Vector3(rb.transform.localPosition.x, holder.startLocalPos.y, holder.startLocalPos.z);


                holder.transform.localRotation = holder.startLocalRot;
            }
        }
    }
}
