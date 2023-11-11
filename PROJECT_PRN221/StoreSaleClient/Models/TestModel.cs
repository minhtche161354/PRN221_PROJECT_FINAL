using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreSaleClient.Models
{
    public class TestModel
    {
        public int testModelID { get; set; }
        public string testModelString { get; set; }
        public List<string> testModelList { get; set; }

        public TestModel(int testModelID, string testModelString)
        {
            this.testModelID = testModelID;
            this.testModelString = testModelString;
            this.testModelList = new List<string>()
            {
                "lmao",
                "khong em",
                "123 ngoi sao"
            };
        }
    }
}
