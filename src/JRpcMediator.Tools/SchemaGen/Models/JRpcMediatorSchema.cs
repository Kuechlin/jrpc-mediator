using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRpcMediator.Tools.SchemaGen.Models
{
    public class JRpcMediatorSchema
    {
        public Dictionary<string, RequestSchema> Requests { get; set; }
        public Dictionary<string, TypeSchema> Types { get; set; }
        public Dictionary<string, EnumSchema> Enums { get; set; }

        public JRpcMediatorSchema(Dictionary<string, RequestSchema> requests, Dictionary<string, TypeSchema> types, Dictionary<string, EnumSchema> enums)
        {
            Requests = requests;
            Types = types;
            Enums = enums;
        }
    }
}
