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
        private string GetTargetValue(string label, int value_length)
        {
            int index_Of_Field = OCRText.IndexOf(label);
            int countOfField = label.Length;
            while (index_Of_Field == -1)
            {
                label = label.Remove(countOfField - 1, 1);
                countOfField = label.Length;
                index_Of_Field = OCRText.IndexOf(label);
                if (countOfField <= 0)
                {
                    return ErrorText;
                }
            }
            string value = OCRText.Substring(OCRText.IndexOf("\r\n", (index_Of_Field + countOfField + 2)) + 2, value_length); // add +2 for /r/n characters
            return value;
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
                if (countOfField <= 0)
                {
                    return ErrorText;
                }
            }
            string value = OCRText.Substring(index_Of_Field + countOfField + 2, value_length); // add +2 for /r/n characters
            return value;
        }
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
