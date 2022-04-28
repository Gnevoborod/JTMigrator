using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTMigrator
{
    public class Task
    {
        public enum IssueTypes { Epic=10000, Story=10001, Task=10201 };
        public string pid { get; set; }//проект
        public int issuetype { get; set; }//тип задачи 10201-задача, 10001 сторя, 10000 эпик
        public string atl_token { get; set; }//какой-то токен BXQZ - C67O - KVR0 - SWI7_6b29186b8cf1be8efdf658925a45510367f01129_lin
        public string summary { get; set; }//заголовок
        public string priotity { get; set; }//приоритет
        public string versions { get; set; }//релиз(Версия)
        public string fixVersions { get; set; }//исправить в версии
        public int? components { get; set; }//компонент
        public string description { get; set; }//описание задачи
        public string labels { get; set; }//
        public string issuelinks_linktype { get; set; }//issuelinks-linktype=has+story -тип связи
        public string issuelinks_issues { get; set; }//ссылка на сторю
        public string customfield_10101 { get; set; }//   key:CUR-3 -ссылка на эпик
    
        public Task()
        {

        }
        public Task(string pid,
                    int issuetype,
                    string atl_token, 
                    string summary,
                    string priotity,
                    string versions,
                    string fixVersions, 
                    int? components, 
                    string description,
                    string labels, 
                    string issuelinks_linktype,
                    string issuelinks_issues,
                    string customfield_10101)
        {
            this.pid = pid;
            this.issuetype = issuetype;
            this.atl_token = atl_token;
            this.summary = summary;
            this.priotity = priotity;
            this.versions = versions;
            this.fixVersions = fixVersions;
            this.components = components;
            this.description = description;
            this.labels = labels;
            this.issuelinks_linktype = issuelinks_linktype;
            this.issuelinks_issues = issuelinks_issues;
            this.customfield_10101 = customfield_10101;
        }
    }

}
