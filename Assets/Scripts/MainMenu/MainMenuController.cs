using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    NotificationController notifications;

    [SerializeField] private string username = "";
    [SerializeField] private string password = "";
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject registerPanel;

    private void Start()
    {
        notifications = NotificationController.instance;
    }

    public void SubmitUsername(string newUsernameText)
    {
        username = newUsernameText;
    }

    public void SubmitPassword(string newPasswordText)
    {
        password = newPasswordText;
    }

    public void LoginUser()
    {
        if (DataBaseController.CheckIfUserExists(username) && DataBaseController.CheckIfPasswordCorrect(username, password))
        {
            PlayerPrefs.SetString("LoggedUser", username);
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            //Debug.Log("USUARIO O CONTRASEÑA INCORRECTOS");
            notifications.GenerateNotification("Incorrect username or password.");
        }
    }

    public void RegisterUser()
    {
        string trimmedPassword = password.Trim(' ');

        if (DataBaseController.CheckIfUserExists(username))
        {
            //Debug.Log("ESTE USUARIO YA EXISTE");
            notifications.GenerateNotification("User already exists.");
            return;
        }

        if (trimmedPassword.Length < 8)
        {
            //Debug.Log("La contraseña debe tener MINIMO 8 caracteres");
            notifications.GenerateNotification("Password must be at least 8 characters long.");
            return;
        }

        CharacterPersonalization editor = FindObjectOfType<CharacterPersonalization>();

        DataBaseController.InsertNewUserIntoDB(username, password, editor.GetEyeID(), editor.GetColor());
        ChangePanels();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChangePanels()
    {
        if (registerPanel.activeSelf)
        {
            registerPanel.SetActive(false);
            mainPanel.SetActive(true);
        }
        else
        {
            registerPanel.SetActive(true);
            mainPanel.SetActive(false);
        }

        foreach (TMP_InputField i in FindObjectsOfType<TMP_InputField>())
        {
            i.text = "";
        }
    }
}
