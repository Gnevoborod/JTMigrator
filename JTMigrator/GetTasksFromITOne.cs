using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTMigrator
{
    internal class GetTasksFromITOne
    {
        string sITOneEndPoint = "https://oneproject.it-one.ru/jira/rest/api/latest/issue/CUR-";
        int iStartTaskNumber = 4;
        int iEndTaskNumber = 6;
        HttpClient client = new HttpClient();
        public bool isOk = true;
        public List<TaskToClone> tasks = new List<TaskToClone>();
        public async void DownloadAllTasks()
        {
            
            HttpResponseMessage httpResponseMessage;
            HttpRequestMessage httpRequestMessage;
            for (int i = iStartTaskNumber; i <= iEndTaskNumber; i++)
            {
                httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.RequestUri = new Uri(sITOneEndPoint + i.ToString());
                httpRequestMessage.Headers.Add("Cookie", "atlassian.xsrf.token=BXQZ-C67O-KVR0-SWI7_d05cbacfcb8b39ab76e41bf926fd5be988f57a67_lin; JSESSIONID=0CD6F4296B59FBDFE1E9DDA0FBA93F71; jira.editor.user.mode=wysiwyg; _ym_uid=1645705398790172598; _ym_d=1650971296; _fbp=fb.1.1650971296721.450619859; JSESSIONID=3B9FC0ED21DBB3F4BF249A393579EB6B; PHPSESSID=wSb5xb16X2VtqrhUyPh1Ca7C1G2oee5c; crowd.token_key=4YAXbmJ3HbKOVPPSeIPFvAAAAAAACoAFcGRvcm9raG92; com.luxoft.saml.autologin=true; XSRF-TOKEN=e3093560-102c-44fe-be84-d923b8656cee");   
                httpResponseMessage=client.Send(httpRequestMessage);
                if (httpResponseMessage != null)
                {
                    string content = await httpResponseMessage.Content.ReadAsStringAsync();
                    if (content.Contains("ЗАПРОС НЕ СУЩЕСТВУЕТ"))
                        continue;
                    TaskToClone taskToClone = new TaskToClone();
                    taskToClone.summary = FindElement(content, "\"summary\"", null, "\",");
                    taskToClone.description = FindElement(content, "\"created\"", "\"description\"", "\",");
                    string type = FindElement(content, "\"worklog\"", "\"id\"", "\",");
                    if(type != null)
                        Int32.TryParse(type, out taskToClone.type);
                    taskToClone.epic = FindElement(content, "\"customfield_10101\"", null, "\",");
                    taskToClone.component = FindElement(content, "\"components\"", "\"id\"", "\",");
                    taskToClone.labels = FindElement(content, "\"labels\"", null,"]");
                    taskToClone.createdfrom = "CUR-" + i.ToString();
                    taskToClone.parentID = FindElement(content, "\"outwardIssue\"", "\"key\"", "\","); 
                    tasks.Add(taskToClone);
                    Thread.Sleep(10);
                }
            }
        }

        public string FindElement(string source, string searching, string subsearch, string delimeter)
        {
            
            if (source == null)
                return null;
            if (searching == null)
                return null;
            int resultOffset = source.IndexOf(searching);
            if (resultOffset == -1)
                return null;
            string offset = source.Substring(resultOffset);
            if (offset == null)
                return null;
            if (offset[searching.Length + 1] == 'n')
                return null;
            int resultStart = searching.Length+2;
            if (offset.Substring(resultStart-1, 2) == "[]")
                    return null;
            
            if(subsearch != null)
            {
                int subOffset= offset.IndexOf(subsearch);
                if (subOffset == -1)
                    return null;
                offset=offset.Substring(subOffset);
                resultStart = subsearch.Length + 2;
                if (offset[resultStart-1] == 'n')
                    return null;
                //ищем null для всяких эпиков и сторей (а может и задач) где отсутствует описание
            }
            int resultFin = offset.IndexOf(delimeter);
            return offset.Substring(resultStart, resultFin-resultStart);
        }
    }
}
