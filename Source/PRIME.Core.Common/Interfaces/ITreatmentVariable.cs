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
    }
}
