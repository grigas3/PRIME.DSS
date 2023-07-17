using System;
using System.Collections.Generic;
using System.Text;

namespace PRIME.Core.Common.Interfaces
{
    public interface IVariable
    {
        /// <summary>
        /// Code
        /// </summary>
        string Code { get; }
        /// <summary>
        /// Variable Namespace
        /// </summary>
        string CodeNameSpace { get; }

        /// <summary>
        /// Name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// If not numeric possible values for the variable
        /// </summary>
        List<string> Values { get;  }
        
    }
}
