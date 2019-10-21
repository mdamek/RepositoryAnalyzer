﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace AnalyzeManager.Models
{
    public class Node
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("children")]
        public List<Node> Children { get; set; }
    }
}
