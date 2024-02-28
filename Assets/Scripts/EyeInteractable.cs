using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{
    public bool IsHovered { get; set; }

    [SerializeField]
    private UnityEvent<GameObject> OnObjectHover;

    [SerializeField]
    private Material onHoverActiveMaterial;

    [SerializeField]
    private Material onHoverInactiveMaterial;

    private MeshRenderer meshRenderer;

    void Start() => meshRenderer = GetComponent<MeshRenderer>();


    void Update()
    {
        if(IsHovered)
        {
            meshRenderer.material = onHoverActiveMaterial;
            OnObjectHover.Invoke(gameObject);
        }
        else
        {
            meshRenderer.material = onHoverInactiveMaterial;
        }
        
    }
}
