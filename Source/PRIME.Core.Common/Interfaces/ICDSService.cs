using PRIME.Core.Common.Models.CDS;
using System;
using System.Collections.Generic;
using System.Text;

namespace PRIME.Core.Common.Interfaces
{
    public interface ICDSService
    {

        List<Card> GetCards(IConditionRepository repo);
    }
}
