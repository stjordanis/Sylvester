﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
namespace Sylvester.NLU.Wit.Models
{
    public class Meaning : WitApiResponse
    {
        /// <summary>
        /// Either the ID you passed to the API or an ID generated by the API
        /// </summary>
      
        public string msg_id { get; set; }

        /// <summary>
        /// Either the text sent in the q argument or the transcript of the speech input. This value should be used only for debug as Wit.ai focuses on entities.
        /// </summary>
        public string text { get; set; }

        public List<Intent> intents { get; set; }
        
        public Dictionary<string, List<Entity>> entities { get; set; }

        public Dictionary<string, List<Trait>> traits { get; set; }
    }
}
