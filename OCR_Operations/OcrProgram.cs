using Newtonsoft.Json.Linq;
using OCR_Operations.DataOperations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_Operations
{
    public class OcrProgram
    {
        public bool InsertFileData(int cpeEntryId, int cpeDefinitionId, string filePath)
        {
            bool success;
            if (File.Exists(filePath))
            {
                OCR ocr = new OCR();
                var json = ocr.GetJSON(filePath);
                string ocrText = ocr.GetProperty(json);
                ExtractData extractData = new ExtractData();
                extractData.OCRText = ocrText;

                // To add new form definitionID and method to handle it
                switch (cpeDefinitionId)
                {
                    case 1:
                        success = extractData.Yankee_Doctor_Blade(cpeEntryId, cpeDefinitionId);
                        break;
                    case 17:
                        success = extractData.Yankee_Hood_Gap(cpeEntryId, cpeDefinitionId);
                        break;
                    case 27:
                        success = extractData.Glue_Turret_Data(cpeEntryId, cpeDefinitionId);
                        break;
                    case 32:
                        success = extractData.Wirebox_Ver2(cpeEntryId, cpeDefinitionId);
                        break;
                    case 34:
                        success = extractData.Wire_Section_Shower(cpeEntryId, cpeDefinitionId);
                        break;
                    default:
                        success = false;                                     //if wrong definitionId is given
                        break;
                }
                return success;
            }
            else
            {
                success = false; //if file is not found
                return success;
            }
        }
    }
}
