using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSelector : MonoBehaviour
{
    private bool selected = false;
    public Material selected_material;
    public Material default_material;
    private Renderer renderer;

    void Start() {
        renderer = GetComponent<Renderer>();
        renderer.enabled = true;
        renderer.material = default_material;
    }

    public void ChangeColor() {
        selected = !selected;
        if (selected) renderer.material = selected_material;
        else renderer.material = default_material;
    }
}
