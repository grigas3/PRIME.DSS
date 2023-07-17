using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using PRIME.Core.Common.Interfaces;
using PRIME.Core.DSS.Dexi;

namespace PRIME.Core.DSS
{
    /// <summary>
    /// DSS Mapping Class
    /// </summary>
    public class DSSConfig:IValueMapping
    {
        /// <summary>
        /// DSS Name
        /// </summary>
        [Description("DSS Code")]
        [JsonRequired]
        public string Code { get; set; }

        /// <summary>
        /// DSS Name
        /// </summary>
        [Description("DSS name")]
        [JsonRequired]
        public string Name { get; set; }

        /// <summary>
        /// DSS Version
        /// </summary>
        [Description("Version of the DSS Model")]
        [JsonRequired]
        public string Version { get; set; }

        /// <summary>
        /// Dexi File
        /// </summary>
        [Description("Dexi File Reference")]
        [JsonRequired]
        public string DexiFile { get; set; }

        /// <summary>
        /// Value Mappings
        /// </summary>
        [Description("Input")]
        public List<DSSValueMapping> Input { get; set; }

        /// <summary>
        /// Aggregation Period Days (Default 30)
        /// </summary>
        [Description("Aggregation Period Days(Default 30)")]
        [JsonRequired]
        public int AggregationPeriodDays { get; set; }

        /// <summary>
        /// Dexi Attribute Value
        /// </summary>
        public string DexiOutputValue { get; set; }

        #region Helpers


        public static DSSConfig CreateFromModel(string modelfile,string code)
        {
            var model = new Model(modelfile);
            DSSConfig config = new DSSConfig();
            config.DexiFile = modelfile;
            config.Code = code;
            config.Name = code;
            config.Version = "1.0";
            config.Input = new List<DSSValueMapping>();
            
            foreach (var c in model.Basic)
            {


                var mapping = new DSSValueMapping()
                {
                    
                    Name=c.Name,
                    Code=c.Name,
                    ValueType= "categorical",
                    CategoryMapping =new DSSCategoricalValueMappingList(){ }

                };
                if (c.ScaleSize == 0)
                {
                    config.Input.Add(mapping);
                    continue;
                }

                for (int i = 0; i < c.ScaleSize; i++)
                {

                    mapping.CategoryMapping.Add(new DSSCategoricalValueMapping()
                    {

                        Name=c.Scale[i].Name,
                        Value=i
                        
                    });
                    

                }

                config.Input.Add(mapping);
            }


            return config;

        }


        /// <summary>
        /// Load From File
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static DSSConfig FromString(string config)
        {
            DSSConfig ret = null;
            try
            {
                ret = JsonConvert.DeserializeObject<DSSConfig>(config);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ret;
        }

        #endregion

        public int GetValue(string variable, double v)
        {

            if (variable == null)
                return 0;
            var c = Input.FirstOrDefault(e => e.Code == variable);

            if (c == null)
                return 0;


            int vi;
            if (c.NumericBins != null)
            {
                vi=c.GetNumericMapping(v);
                
            }
            //else if (c.ValueType == "categorical")
            //{
            //    if (v<=0.0001)
            //    {
            //        var cmap=c.CategoryMapping.FirstOrDefault(e => e.Value == 0);
            //        if (cmap != null)
            //            return cmap.Name;

            //    }
            //}
            else
            {
                vi =(int)Math.Round(v,0);
            }


            return vi;

        }
    }
}