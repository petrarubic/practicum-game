using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumberChange : Trigger {
    public int randomNumber;
    public int randomNumber2;
    public GameObject doors;
    public GameObject numberManager3;
    public TMP_Text number1;
    public TMP_Text number2;
    public TMP_Text number3;
    public TMP_Text number4;
    public TMP_Text number5;
    public TMP_Text number6;
    public Transform numbers;
    public Transform[] placeholders;
    public float placeholderTimer = 0;
    public float timer = 0;
    public float timer2 = 0;
    public int brojac = 0;
    //public bool correct = false;
    public bool firstDigit = false;
    public bool firstDigitCorrect = false;
    public bool secondDigit = false;
    public bool secondDigitCorrect = false;
    public bool secondUnlocked = false;
    public bool resetFirst = false;

    public override void ResetTrigger() {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        number1.text = "1";
        number2.text = "2";
        number3.text = "5";
        number4.text = "6";
        number5.text = "7";
        number6.text = "9";
        numbers.transform.position = placeholders[brojac].position;
    }

    // Update is called once per frame
    void Update()
    {
        placeholderTimer += Time.deltaTime;
        if (!firstDigit)
        {
            timer += Time.deltaTime;

            if (timer > 1f)
            {
                randomNumber = Random.Range(1, 7);
                number2.text = randomNumber.ToString();
                if (randomNumber < 5 && randomNumber > 1)
                {
                    firstDigitCorrect = true;
                }
                else
                {
                    firstDigitCorrect = false;
                }
                timer = 0;
            }
        }
        if (!secondDigit)
        {
            timer2 += Time.deltaTime;
            if (timer2 > 0.7f)
            {
                randomNumber2 = Random.Range(3, 9);
                number5.text = randomNumber2.ToString();
                if (randomNumber2 < 9 && randomNumber2 > 6)
                {
                    secondDigitCorrect = true;
                }
                else
                {
                    secondDigitCorrect = false;
                }
                timer2 = 0;
            }
            

        }
        if (placeholderTimer > 3f && !secondDigit)
        {
            brojac++;
            if (brojac >= placeholders.Length)
            {
                brojac = 0;
            }
            numbers.transform.position = placeholders[brojac].position;
            numbers.transform.eulerAngles += new Vector3(0, 90, 0);
            placeholderTimer = 0;
            
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
            {
                Debug.Log(hit.collider.gameObject.tag);
                if (hit.collider.gameObject.tag == "Numbers")
                {
                    if (secondUnlocked && secondDigitCorrect == false)
                    {
                        firstDigit = false;
                        secondDigit = false;
                        secondUnlocked = false;
                        firstDigitCorrect = false;
                    }

                    if (secondUnlocked)
                    {
                        secondDigit = true;
                        isTriggered = true;
                        OnTrigger();
                        if (doors) {
                            doors.transform.position += new Vector3(0, 5, 0);
                        }
                        numberManager3.SetActive(true);
                        Destroy(this);
                    }

                    if (firstDigitCorrect)
                    {
                        firstDigit = true;
                        secondUnlocked = true;
                        resetFirst = false;
                    }
                }

            }


        }
    }
}
