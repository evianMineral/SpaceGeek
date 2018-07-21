using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Alexa.NET.Response;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace SpaceGeek
{
    public class Function
    {
        public List<FactResource> GetResources()
        {
            List<FactResource> resources = new List<FactResource>();
            FactResource frFRResource = new FactResource("fr-FR");
            frFRResource.SkillName = "Faits scientifiques Am�ricain";
            frFRResource.GetFactMessage = "Voici une anecdote spatiale : ";
            frFRResource.HelpMessage = " Vous pouvez dire \"donne moi un fait scientifique\" ou vous pouvez dire arr�te... comment puis-je vous aider?";
            frFRResource.HelpReprompt = "Vous pouvez dire \"donne moi un fait scientifique\" pour commencer";
            frFRResource.StopMessage = "Au revoir !";
            frFRResource.Facts.Add("Une ann�e sur Mercure ne dure que 88 jours.");
            frFRResource.Facts.Add("Bien qu'elle soit plus �loign�e du Soleil, V�nus conna�t des temp�ratures plus �lev�es que Mercure.");
            frFRResource.Facts.Add("V�nus tourne dans le sens inverse des aiguilles d'une montre, peut-�tre � cause d'une collision pass�e avec un ast�ro�de.");
            frFRResource.Facts.Add("Sur Mars, le Soleil appara�t � peu pr�s deux fois moins grand que sur Terre.");
            frFRResource.Facts.Add("La Terre est la seule plan�te qui ne porte pas le nom d'un dieu.");
            frFRResource.Facts.Add("Jupiter a le jour le plus court de toutes les plan�tes.");
            frFRResource.Facts.Add("La Voie lact�e entrera en collision avec la galaxie d'Androm�de dans environ 5 milliards d'ann�es.");
            frFRResource.Facts.Add("Le Soleil contient 99,86 % de la masse du syst�me solaire.");
            frFRResource.Facts.Add("Le Soleil est une sph�re presque parfaite.");
            frFRResource.Facts.Add("Une �clipse solaire totale peut se produire une fois tous les 1 � 2 ans. Cela en fait un �v�nement rare.");
            frFRResource.Facts.Add("Saturne rayonne deux fois et demie plus d'�nergie dans l'espace qu'elle n'en re�oit du soleil.");
            frFRResource.Facts.Add("La temp�rature � l'int�rieur du Soleil peut atteindre 15 millions de degr�s Celsius.");
            frFRResource.Facts.Add("La Lune s'�loigne d'environ 3,8 cm de notre plan�te chaque ann�e.");

            resources.Add(frFRResource);
            return resources;
        }

        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            SkillResponse response = new SkillResponse();
            response.Response = new ResponseBody();
            response.Response.ShouldEndSession = false;
            IOutputSpeech innerResponse = null;
            ILambdaLogger log = context.Logger;
            log.LogLine($"Skill Request Object:");
            log.LogLine(JsonConvert.SerializeObject(input));

            List<FactResource> allResources = GetResources();
            FactResource resource = allResources.FirstOrDefault();

            if (input.GetRequestType() == typeof(LaunchRequest))
            {
                log.LogLine($"Default LaunchRequest made: 'Alexa, open Science Facts");
                innerResponse = new PlainTextOutputSpeech();
                (innerResponse as PlainTextOutputSpeech).Text = EmitNewFact(resource, true);
            }
            else if (input.GetRequestType() == typeof(IntentRequest))
            {
                IntentRequest intentRequest = (IntentRequest)input.Request;

                switch (intentRequest.Intent.Name)
                {
                    case "AMAZON.CancelIntent":
                        log.LogLine($"AMAZON.CancelIntent: send StopMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.StopMessage;
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.StopIntent":
                        log.LogLine($"AMAZON.StopIntent: send StopMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.StopMessage;
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.HelpIntent":
                        log.LogLine($"AMAZON.HelpIntent: send HelpMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.HelpMessage;
                        break;
                    case "GetFactIntent":
                        log.LogLine($"GetFactIntent sent: send new fact");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = EmitNewFact(resource, false);
                        break;
                    case "GetNewFactIntent":
                        log.LogLine($"GetFactIntent sent: send new fact");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = EmitNewFact(resource, false);
                        break;
                    default:
                        log.LogLine($"Unknown intent: " + intentRequest.Intent.Name);
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = resource.HelpReprompt;
                        break;
                }
            }

            response.Response.OutputSpeech = innerResponse;
            response.Version = "1.0";
            log.LogLine($"Skill Response Object...");
            log.LogLine(JsonConvert.SerializeObject(response));

            return response;
        }

        public string EmitNewFact(FactResource resource, bool withPreface)
        {
            Random r = new Random();
            if (withPreface)
                return resource.GetFactMessage + resource.Facts[r.Next(resource.Facts.Count)];
            return resource.Facts[r.Next(resource.Facts.Count)];
        }

        ///// <summary>
        ///// A simple function that takes a string and does a ToUpper
        ///// </summary>
        ///// <param name="input"></param>
        ///// <param name="context"></param>
        ///// <returns></returns>
        //public string FunctionHandler(string input, ILambdaContext context)
        //{
        //    return input?.ToUpper();
        //}
    }

    public class FactResource
    {
        public string Language { get; set; }
        public string SkillName { get; set; }
        public List<string> Facts { get; set; }
        public string GetFactMessage { get; set; }
        public string HelpMessage { get; set; }
        public string HelpReprompt { get; set; }
        public string StopMessage { get; set; }

        public FactResource(string language)
        {
            this.Language = language;
            this.Facts = new List<string>();
        }
    }
}
