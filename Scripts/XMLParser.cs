using System.Xml;
using System.Xml.Linq;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class XMLParser : MonoBehaviour
{
    // La structure de données pour stocker les questions/réponses
    // sera la suivante : {question1:[(reponse, true/false)], question2:[(reponse, true/false)]}
    private Dictionary<string, List<Tuple<string, bool>>> questions = new Dictionary<string, List<Tuple<string, bool>>>();

    /// <summary>
    /// Ajoute une question dans le dictionnaire de données.
    /// Exemple de <paramref name="reponses"/> : [(reponse1, false), (reponse2, false), (reponse3, true), (reponse4, false)]
    /// </summary>
    /// <param name="question">String de la question</param>
    /// <param name="reponses">Liste des reponses pour cette question</param>
    private void add_question(string question, List<Tuple<string, bool>> reponses)
    {
        questions.Add(question, reponses);
    }

    /// <summary>
    /// Fonction qui récupère le Dictionnaire des questions pour utilisation dans un autre fichier 
    /// </summary>
    /// <returns>Dictionnaire de données</returns>
    public Dictionary<string, List<Tuple<string, bool>>> GetDictionary()
    {
        return questions;
    }

    /// <summary>
    /// Fonction qui récupère le fichier et le place dans le dictionnaire de données
    /// </summary>
    /// <param name="path">Chemain d'accès du fichier</param>
    public void parse_XML(string path)
    {
        if (!System.IO.File.Exists(path))
        {
            Debug.LogError("Fichier XML introuvable");
            return;
        }
        XDocument doc = XDocument.Load(path);
        foreach (XElement question in doc.Element("root").Elements("question"))
        {
            List<Tuple<string, bool>> reps = new List<Tuple<string, bool>>();
            foreach (XElement reponse in question.Elements("answer"))
            {
                if (reponse.Attribute("correct") != null && reponse.Attribute("correct").Value == "true")
                {
                    reps.Add(new Tuple<string, bool>(reponse.Value, true));
                }
                else
                {
                    reps.Add(new Tuple<string, bool>(reponse.Value, false));
                }
            }
            add_question(question.Element("title").Value, reps);
        }
    }
}
