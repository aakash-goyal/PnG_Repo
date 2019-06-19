using OCR_Operations.DataAcess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_Operations.DataOperations
{
    partial class ExtractData
    {
        const string ErrorText = "Not Found";
        public string OCRText { get; set; }
        private List<DataPointDefinition> GetDataPointDefinitions(int cpeDefId)
        {
            var cpe_DevEntities = new cpe_devEntities();
            var dataset = from dataPoint in cpe_DevEntities.DataPointDefinitions
                          where dataPoint.CpeDefinitionId == cpeDefId
                          select dataPoint;
            return dataset.ToList();
        }
    }
}
