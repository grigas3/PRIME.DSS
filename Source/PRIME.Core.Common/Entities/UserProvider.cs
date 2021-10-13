using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PRIME.Core.Common.Entities;
using PRIME.Core.Web.Entities;

namespace PRIME.Core.Context.Entities
{
    public class UserProvider:BaseEntity
    {
        public int UserId { get; set; }
        public int CDSSProviderId { get; set; }
        public string Settings { get; set; }
    }

    
}
