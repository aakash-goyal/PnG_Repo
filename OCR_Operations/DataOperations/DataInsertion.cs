using OCR_Operations.DataAcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_Operations.DataOperations
{
    class DataInsertion
    {
        public Boolean Insert(List<CpeEntryDataPointValue> cpeEntryDataPointValues)
        {
            bool success = true;
            cpe_devEntities db = new cpe_devEntities();

            if (cpeEntryDataPointValues.Count == 0)               // If No data is extracted
            {
                return success;                          
            }

            foreach (var cpeEntryDataPintValue in cpeEntryDataPointValues)
            {
                db.CpeEntryDataPointValues.Add(cpeEntryDataPintValue);
            }
            try
            {
                db.SaveChanges();
            }
            catch(Exception e)
            {
                success = false;
            }

            return success;
        }
    }
}
