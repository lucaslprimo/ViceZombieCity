using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollPhysics : MonoBehaviour
{
    [SerializeField] GameObject ragdoll;
    [SerializeField] GameObject animatedBody;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CopyPosition(ragdoll.transform, animatedBody.transform);
    }

    private void CopyPosition( Transform sourceTransform , Transform destinationTransform)
    {
        for(int i = 0; i < sourceTransform.childCount; i++)
        {
            var source = sourceTransform.transform.GetChild(i);

            var destination = destinationTransform.transform.GetChild(i);

            destination.transform.rotation = source.transform.rotation;
            destination.transform.position = source.transform.position;

            CopyPosition(source, destination);
        }
    }
}
