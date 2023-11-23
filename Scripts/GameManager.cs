using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private XMLParser xml_parser; // Reférence au script XMLParser
    private UiController ui_controller; // Référence au controlleur d'UI
    private int question_counter = 0; // Compteur de question
    private List<string> list_question; // Liste des String des questions
    private Dictionary<string, List<Tuple<string, bool>>> dict_question; // Dictionnaire de données des questions
    public bool is_showing_awnsers = false; // Booléen utilisé pour savoir si le participant est en train de répondre ou si
                                            // il vérifie ses réponses
    void Start()
    {
        xml_parser = gameObject.GetComponent<XMLParser>();
        ui_controller = gameObject.transform.parent.Find("UIDocument").GetComponent<UiController>();
        ui_controller.ui_init();

        string xml_path = Application.streamingAssetsPath + "/qcmFile.xml";
        xml_parser.parse_XML(xml_path);
        dict_question = xml_parser.GetDictionary();
        list_question = new List<string>(dict_question.Keys);
        next_question();
        ui_controller.switch_button();
    }

    /// <summary>
    /// Affiche la prochaine questions sur l'UI
    /// </summary>
    public void next_question()
    {
        ui_controller.switch_button();
        if (question_counter == list_question.Count)
        {
            question_counter = 0;
        }
        is_showing_awnsers = false;
        ui_controller.set_question(list_question[question_counter], question_counter + 1);
        set_labbels(list_question[question_counter]);
        question_counter++;
    }

    /// <summary>
    /// Affiche toutes les reponses de la question entré en parametre sur l'UI
    /// </summary>
    /// <param name="question">String de la question</param>
    public void set_labbels(string question)
    {
        int nb_label = ui_controller.get_nb_toggles();
        int count = 0;
        foreach (Tuple<string, bool> ques in dict_question[question])
        {
            ui_controller.set_label_toggle(ques.Item1, count);
            count++;
        }
        for (int i = count; i < nb_label; i++)
        {
            ui_controller.hide_toggle(i);
        }
    }

    /// <summary>
    /// Valide la question donnée avec une list[bool] contenant true si la checkbox est coché, false sinon
    /// </summary>
    /// <param name="reps">Liste des booléens</param>
    /// <returns>true si la question est bien répondu, false sinon</returns>
    public bool validate(List<bool> reps)
    {
        int nb_toggles = dict_question[list_question[question_counter - 1]].Count;
        is_showing_awnsers = true;
        bool res = true;
        ui_controller.switch_button();
        for (int i = 0; i < nb_toggles; i++)
        {
            if (dict_question[list_question[question_counter - 1]][i].Item2 == true)
            {
                ui_controller.set_correct(i);
                if (reps[i] == false)
                {
                    res = false;
                }
            }
            else
            {
                ui_controller.set_incorrect(i);
                if (reps[i] == true)
                {
                    res = false;
                }
            }
        }
        return res;
    }
}
