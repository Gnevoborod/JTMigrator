using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTMigrator
{
    public class TaskToClone
    {
        public int type;
        public string parentID { get; set; }//связь со сторёй
        public string summary { get; set; }
        public string description { get; set; }
        public string epic { get; set; }
        public string component { get; set; }
        public string labels { get; set; }
        public string createdfrom { get; set; }//а это просто номер задачи откуда клонировали
    }
}
