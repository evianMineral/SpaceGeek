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
            frFRResource.SkillName = "Faits scientifiques Américain";
            frFRResource.GetFactMessage = "Voici une anecdote spatiale : ";
            frFRResource.HelpMessage = " Vous pouvez dire \"donne moi un fait scientifique\" ou vous pouvez dire arrête... comment puis-je vous aider?";
            frFRResource.HelpReprompt = "Vous pouvez dire \"donne moi un fait scientifique\" pour commencer";
            frFRResource.StopMessage = "Au revoir !";
            frFRResource.Facts.Add("Une année sur Mercure ne dure que 88 jours.");
            frFRResource.Facts.Add("Bien qu'elle soit plus éloignée du Soleil, Vénus connaît des températures plus élevées que Mercure.");
            frFRResource.Facts.Add("Vénus tourne dans le sens inverse des aiguilles d'une montre, peut-être à cause d'une collision passée avec un astéroïde.");
            frFRResource.Facts.Add("Sur Mars, le Soleil apparaît à peu près deux fois moins grand que sur Terre.");
            frFRResource.Facts.Add("La Terre est la seule planète qui ne porte pas le nom d'un dieu.");
            frFRResource.Facts.Add("Jupiter a le jour le plus court de toutes les planètes.");
            frFRResource.Facts.Add("La Voie lactée entrera en collision avec la galaxie d'Andromède dans environ 5 milliards d'années.");
            frFRResource.Facts.Add("Le Soleil contient 99,86 % de la masse du système solaire.");
            frFRResource.Facts.Add("Le Soleil est une sphère presque parfaite.");
            frFRResource.Facts.Add("Une éclipse solaire totale peut se produire une fois tous les 1 à 2 ans. Cela en fait un événement rare.");
            frFRResource.Facts.Add("Saturne rayonne deux fois et demie plus d'énergie dans l'espace qu'elle n'en reçoit du soleil.");
            frFRResource.Facts.Add("La température à l'intérieur du Soleil peut atteindre 15 millions de degrés Celsius.");
            frFRResource.Facts.Add("La Lune s'éloigne d'environ 3,8 cm de notre planète chaque année.");

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
