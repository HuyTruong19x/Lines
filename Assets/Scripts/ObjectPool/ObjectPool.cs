using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    [SerializeField]
    private List<GameObject> _objectTemplates = new List<GameObject>();
    private List<GameObject> _objects = new List<GameObject>();

    public GameObject TakeObject(string i_tag)
    {
        for(int i = 0; i < _objects.Count; i++)
        {
            if(_objects[i].tag == i_tag && !_objects[i].activeSelf)
            {
                _objects[i].SetActive(true);
                return _objects[i];
            }    
        }    
        for(int i = 0; i < _objectTemplates.Count; i++)
        {
            if(_objectTemplates[i].gameObject.tag == i_tag)
            {
                GameObject obj = Instantiate(_objectTemplates[i], transform);
                _objects.Add(obj);
                obj.SetActive(true);
                return obj;
            }    
        }    
        return null;
    }    
}
