using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UiController : MonoBehaviour
{
    private Label question_text; // Label de la question
    private Label result_label; // Label utilisé pour dire le résultat de la question au participant
    private Button valid_button; // Boutton de validation
    private Button quit_button; // Boutton pour quitter l'application
    private VisualElement question_card; // Boite qui contient les checkboxes
    private GroupBox reps_toggles; // Groupe où les checkboxes sont stockées
    private List<TemplateContainer> list_toggle = new List<TemplateContainer>(); // Liste des checkboxes

    private GameManager game_manager; // Reférence au fichier gamemanager

    /// <summary>
    /// Initialise toutes les variables pour l'UI
    /// </summary>
    public void ui_init()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        game_manager = gameObject.transform.parent.Find("GameManager").GetComponent<GameManager>();

        reps_toggles = root.Q<GroupBox>("reponses");
        question_text = root.Q<Label>("question-text");
        result_label = root.Q<Label>("result-text");
        valid_button = root.Q<Button>("valid-button");
        quit_button = root.Q<Button>("quit-button");
        question_card = root.Q<VisualElement>("question-card");

        for (int i = 0; i < reps_toggles.childCount; i++)
        {
            list_toggle.Add(reps_toggles.Q<TemplateContainer>($"rep{i}"));
        }

        valid_button.clicked += valid_button_pressed;
        quit_button.clicked += quit_button_pressed;
    }

    /// <summary>
    /// Fonction utilisé quand le boutton validé est pressé
    /// </summary>
    private void valid_button_pressed()
    {
        if (game_manager.is_showing_awnsers)
        {
            clear_UI();
            game_manager.next_question();
        }
        else
        {
            set_result(game_manager.validate(get_awnsers()));
        }
    }

    /// <summary>
    /// Fonction utilisé quand le boutton quitté est pressé
    /// </summary>
    private void quit_button_pressed()
    {
        Application.Quit();
    }

    /// <summary>
    /// Change le texte du boutton valider en "suivant" et inversement
    /// </summary>
    public void switch_button()
    {
        if (valid_button.text == "Valider")
        {
            valid_button.text = "Suivant";
        }
        else
        {
            valid_button.text = "Valider";
        }
    }

    /// <summary>
    /// Affiche la question dans l'UI 
    /// </summary>
    /// <param name="question">String a afficher pour la question</param>
    /// <param name="question_nb">Numéro de la question a afficher</param>
    public void set_question(string question, int question_nb)
    {
        question_text.text = $"Question {question_nb.ToString()} :\n{question}";
    }

    /// <summary>
    /// Change le texte de la checkbox renseigné
    /// </summary>
    /// <param name="rep">Nouveau String de la checkbox</param>
    /// <param name="index">Index de la checkbox sur l'écran</param>
    public void set_label_toggle(string rep, int index)
    {
        if (index >= list_toggle.Count || index < 0)
        {
            return;
        }
        list_toggle[index].Q<Toggle>().Q<Label>().text = rep;
    }

    /// <summary>
    /// Cache la checkbox de l'écran si il y a moins de possibilité de réponse que la checkbox
    /// </summary>
    /// <param name="index">Index de la checkbox sur l'écran</param>
    public void hide_toggle(int index)
    {
        if (index >= list_toggle.Count || index < 0)
        {
            return;
        }
        list_toggle[index].style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Affiche la coche de reponse si elle a été caché a la question précedente
    /// </summary>
    /// <param name="index">Index de la checkbox sur l'écran</param>
    public void show_toggle(int index)
    {
        if (index >= list_toggle.Count || index < 0)
        {
            return;
        }
        list_toggle[index].style.display = DisplayStyle.Flex;
    }

    /// <summary>
    /// Rafraichie l'écran pour faire de la place à une nouvelle question
    /// </summary>
    public void clear_UI()
    {
        set_default_color();
        result_label.style.display = DisplayStyle.None;
        for (int i = 0; i < list_toggle.Count; i++)
        {
            list_toggle[i].Q<Toggle>().value = false;
            list_toggle[i].Q<Label>().text = "Label";
            show_toggle(i);
        }
    }

    /// <summary>
    /// Retourne le nombre de checkboxes
    /// </summary>
    /// <returns>Nombre de checkbox dans l'UI</returns>
    public int get_nb_toggles()
    {
        return list_toggle.Count;
    }

    /// <summary>
    /// Met la checkbox a la couleur verte
    /// </summary>
    /// <param name="index">Index de la checkbox sur l'écran</param>
    public void set_correct(int index)
    {
        list_toggle[index].style.color = new Color(0, 0.8f, 0);
    }

    /// <summary>
    /// Met la checkbox n°index a la couleur rouge
    /// </summary>
    /// <param name="index">Index de la checkbox sur l'écran</param>
    public void set_incorrect(int index)
    {
        list_toggle[index].style.color = new Color(0.8f, 0, 0);
    }

    /// <summary>
    /// Met toutes les checkboxes a la couleur par defaut
    /// </summary>
    public void set_default_color()
    {
        foreach (TemplateContainer toggle in list_toggle)
        {
            toggle.style.color = new Color(0, 0, 0);
        }
    }

    /// <summary>
    /// Donne les reponses du participant
    /// </summary>
    /// <returns>Liste des réponses en booléens</returns>
    private List<bool> get_awnsers()
    {
        List<bool> res = new List<bool>();
        foreach (TemplateContainer toggle in list_toggle)
        {
            res.Add(toggle.Q<Toggle>().value);
        }
        return res;
    }

    /// <summary>
    /// Affiche le résultat de cette question au participant
    /// </summary>
    /// <param name="result">true si la question a été résuissit, false sinon</param>
    private void set_result(bool result)
    {
        result_label.style.display = DisplayStyle.Flex;
        if (result)
        {
            result_label.text = "Bien joué !";
            result_label.style.color = new Color(0, 0.8f, 0);
        }
        else
        {
            result_label.text = "Dommage !";
            result_label.style.color = new Color(0.8f, 0, 0);
        }
    }
}
