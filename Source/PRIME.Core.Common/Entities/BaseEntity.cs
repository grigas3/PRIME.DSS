using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PRIME.Core.Common.Entities
{
    public abstract class BaseEntity
    {/// <summary>
        /// DSS ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Modified By
        /// </summary>
        [StringLength(100)]
        public string ModifiedBy { get; set; }

        /// <summary>
        /// Created By
        /// </summary>
        [StringLength(100)]
        public string CreatedBy { get; set; }


        /// <summary>
        /// Created Date
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Modified Date
        /// </summary>
        public DateTime ModifiedDate { get; set; }
    }
}
