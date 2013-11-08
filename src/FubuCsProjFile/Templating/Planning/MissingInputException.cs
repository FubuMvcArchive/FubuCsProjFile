using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FubuCore;

namespace FubuCsProjFile.Templating.Planning
{
    [Serializable]
    public class MissingInputException : Exception
    {
        private readonly IEnumerable<string> _inputNames;

        public MissingInputException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MissingInputException(IEnumerable<string> inputNames) : base("Required inputs {0} are missing".ToFormat(inputNames.Join(", ")))
        {
            _inputNames = inputNames;
        }

        public IEnumerable<string> InputNames
        {
            get { return _inputNames; }
        }
    }
}