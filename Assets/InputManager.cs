using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public GameObject binary;
    public GameObject digits;
    public GameObject input;

    private void Awake()
    {
        StartCoroutine(WaitForGameObjects());

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void RefInputs()
    {
        binary = transform.Find("binary").gameObject;
        digits = transform.Find("digits").gameObject;
        input = transform.Find("input").gameObject;

        Debug.Log("Inputs Received");
    }

    private IEnumerator WaitForGameObjects()
    {
        while (binary == null || digits == null || input == null)
        {
            RefInputs();
            yield return null;
        }

        // The game objects have been found,  dependent code can execute now
        SetBinaryActive();
    }


    private void Start()
    {
        StartCoroutine(WaitForGameObjects());
    }

    public void SetNumpadActive()
    {
        binary.SetActive(false);
        digits.SetActive(true);
        input.SetActive(false);
    }

    public void SetBinaryActive()
    {
        binary.SetActive(true);
        digits.SetActive(false);
        input.SetActive(false);
    }

    public void SetInputActive()
    {
        binary.SetActive(false);
        digits.SetActive(false);
        input.SetActive(true);
    }
}