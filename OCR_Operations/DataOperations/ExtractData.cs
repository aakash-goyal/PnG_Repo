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
        private List<DataPointDefinition> GetDataPointDefinitions(int cpeEntryId)
        {
            var cpe_DevEntities = new cpe_devEntities1();
            var cpeEntry = from cpedata in cpe_DevEntities.CpeEntries
                           where cpedata.Id == cpeEntryId
                           select cpedata;
            var cpeEntrydata = cpeEntry.ToList().First();
            var dataset = from dataPoint in cpe_DevEntities.DataPointDefinitions
                          where (dataPoint.CpeDefinitionId == cpeEntrydata.CpeDefinitionId && (dataPoint.VariantId == cpeEntrydata.CpeDefinitionVariantId))
                          select dataPoint;
            return dataset.ToList();
        }
        private CpeEntryDataPointValue CreateCpeEntryDataPoint(string value, int dataPointId, int cpeDefinitionId, int cpeEntryId)
        {
            //If value is not found then assign null to value
            if (value.Equals(ErrorText))
            {
                value = "";
            }
            //to handle possible common errors in Values
            // Create CpeEntryDataPoint object with the values obtained
            CpeEntryDataPointValue cpeEntryDataPointValue = new CpeEntryDataPointValue
            {
                DataPointDefinitionId = dataPointId,
                Value = value,
                IsBlobValue = false,  // Not saving any file for now
                CpeDefinitionId = cpeDefinitionId,
                CPEEntryId = cpeEntryId
            };
            return cpeEntryDataPointValue;
        }
        private string GetMeasurementValue(string label, int value_length)
        {
            int index_Of_Field = OCRText.IndexOf(label);
            int countOfField = label.Length;
            while (index_Of_Field == -1)
            {
                label = label.Remove(countOfField - 1, 1);
                countOfField = label.Length;
                index_Of_Field = OCRText.IndexOf(label);
                //Error Handle if label not exist 
                if (countOfField <= 2)
                {
                    return ErrorText;
                }
            }
            string value = OCRText.Substring(index_Of_Field + countOfField + 2, value_length); // add +2 for /r/n characters
            return value;
        }
    }
}
