using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorModel : MonoBehaviour
{
    public GameObject ClosedModel;
    public GameObject OpenModel;
	
    private GameObject _currentModelObject;

    public void Start()
    {
        _currentModelObject = transform.Find("Door Open").gameObject;
    }

    public void Close()
    {
        // Gets the current model if unknown
        if (_currentModelObject == null)
            _currentModelObject = OpenModel;

        // Destroys the open model
        Destroy(_currentModelObject);

        // Creates the closed model
        _currentModelObject = Instantiate(ClosedModel, this.transform);
    }

    public void Open()
    {
        // Gets the current model if unknown
        if (_currentModelObject == null)
            _currentModelObject = ClosedModel;

        // Destroys the closed model
        Destroy(_currentModelObject);

        // Creates the open model
        _currentModelObject = Instantiate(OpenModel, this.transform);
    }
}
