using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class UButton
{
    public Button button;
    public ULabel label;
    public GameObject gameObject;

    public string Name { get; private set; }

    public UButton(string buttonName)
    {
        var buttonObject = GameObject.Find(buttonName);
        if (buttonObject != null)
        {
            InitWithGameObject(buttonObject);
        }
    }

    public UButton(GameObject buttonObject)
    {
        InitWithGameObject(buttonObject);
    }

    private void InitWithGameObject(GameObject buttonObject)
    {
        gameObject = buttonObject;
        button = buttonObject.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogWarning("Object is not a button - " + buttonObject.name);
            return;
        }
        label = new ULabel(buttonObject.transform.GetChild(0).gameObject);

        Name = buttonObject.name;
    }

    public void SetText(string newText)
    {
        label?.SetText(newText);
    }

    public void OnClick(UnityAction action)
    {
        button.onClick.AddListener(action);
    }
}

public class ULabel: UObject
{
    public TextMeshProUGUI textMP;
    public Text text;
    public GameObject gameObject;

    private string initialText;

    public LabelMode Mode = LabelMode.None;

    public ULabel(string labelName)
    {
        var labelObject = GameObject.Find(labelName);
        if (labelObject != null)
        {
            InitWithGameObject(labelObject);
        }
    }

    public ULabel(GameObject gameObject)
    {
        InitWithGameObject(gameObject);
    }

    private void InitWithGameObject(GameObject labelObject)
    {
        gameObject = labelObject;

        textMP = labelObject.GetComponentInChildren<TextMeshProUGUI>();
        if (textMP != null)
        {
            Mode = LabelMode.TextMeshPro;
        }
        else
        {
            text = labelObject.GetComponentInChildren<Text>();
            if (text != null)
            {
                Mode = LabelMode.Standard;
            }
        }
    }

    public void SetText(string newText)
    {
        if (gameObject == null)
        {
            return;
        }

        switch (Mode)
        {
            case LabelMode.Standard:
                text.text = newText;
                break;
            case LabelMode.TextMeshPro:
                textMP.text = newText;
                break;
            default:
                break;
        }
    }

    public string Text()
    {
        switch (Mode)
        {
            case LabelMode.Standard:
                return text.text;
            case LabelMode.TextMeshPro:
                return textMP.text;
            default:
                return null;
        }
    }

    public void Localize(bool shouldLocalize = true)
    {
        if (shouldLocalize)
        {
            //turn on localization
            ULocalization.LanguageChanged += UpdateLanguage;
            initialText = Text();
            UpdateLanguage();
        }
        else
        {
            //turn it off
            ULocalization.LanguageChanged -= UpdateLanguage;
            SetText(initialText);
        }
    }

    public void Localize(string key)
    {
        initialText = key;
        ULocalization.LanguageChanged += UpdateLanguage;
        UpdateLanguage();
    }

    public void UpdateLanguage()
    {
        var text = ULocalization.LocalizedStringForKey(initialText);
        SetText(text);
    }
}

public enum LabelMode
{
    Standard, TextMeshPro, None
}

public class UImage
{
    private Image image;
    public GameObject gameObject;

    public UImage(string imageName)
    {
        var imageObject = GameObject.Find(imageName);
        if (imageObject != null)
        {
            InitWithGameObject(imageObject);
        }
    }

    public UImage(GameObject gameObject)
    {
        InitWithGameObject(gameObject);
    }

    private void InitWithGameObject(GameObject imageObject)
    {
        gameObject = imageObject;

        image = imageObject.GetComponent<Image>();
    }

    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }
}

//TODO: Think about how to implement this!
public abstract class UObject
{
    public virtual void WillBeRemoved() { }
}