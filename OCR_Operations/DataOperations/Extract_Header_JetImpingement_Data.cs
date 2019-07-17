using OCR_Operations.DataAcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_Operations.DataOperations
{
    partial class ExtractData
    {
        public bool Headbox_JetImpingement(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.DataSetDefinitionId == 119)
                {
                    int labelIndex = GetLabelIndex("Jet that contact Forming Roll");
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("Jet Appearance"))       //As this value not available in form
                    {
                        continue;
                    }
                    else if (dataPointDefinition.Title.Contains("% Impingement"))          //Check if impingement is filled in form
                    {
                        string jetContactLabel = "Jet that contact Forming Roll";
                        string jcfrValue = GetLabelValue_RefinerPlate(jetContactLabel, labelIndex);
                        jcfrValue = RemoveGeneralError_SteamHood(jcfrValue);
                        string jetThicknessLabel = "Total Jet Thickness";
                        string tjtValue = GetLabelValue_RefinerPlate(jetThicknessLabel, labelIndex);
                        tjtValue = RemoveGeneralError_SteamHood(tjtValue);
                        if (tjtValue != "0" && tjtValue != "" && jcfrValue != "")
                        {
                            value = (Convert.ToDouble(jcfrValue) / Convert.ToDouble(tjtValue) * 100).ToString();
                        }
                        value = ErrorText;
                    }
                    else
                    {
                        value_Label = dataPointDefinition.Title;
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                }
                else
                {
                    continue;
                }
                // Add to Data List
                cpeDataList.Add(CreateCpeEntryDataPoint(value, dataPointDefinition.Id, cpeDefinitionId, cpeEntryId));
            }
            //Insert list of cpeentrydatapointvalues in to database using datainsertion class
            bool success = dataInsertion.Insert(cpeDataList);

            return success;
        }
    }
}
