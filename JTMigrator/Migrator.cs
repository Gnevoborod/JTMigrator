using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
    *pid=11503 - проект
    issuetype=10201 -тип задачи
    *atl_token=BXQZ-C67O-KVR0-SWI7_6b29186b8cf1be8efdf658925a45510367f01129_lin - какой0то токен
    summary=Тестовая задача -заголовок
    *priority=10002 -приоритет
    *versions=13274 -релиз (Версия)
    *fixVersions=13274 -исправить в версии
    components=11166 -компонент
    description=Создание тестовой задачи не через интерфейс жиры -описание задачи
    labels=Stage1 -метка
    *issuelinks-linktype=has+story -тип связи 
    issuelinks-issues=CUR-97 -ссылка на сторю
    customfield_10101=key:CUR-3 -ссылка на эпик
 */


namespace JTMigrator
{
    public class Migrator
    {
        string sCustomersEndPoint = "https://oneproject.it-one.ru/jira/secure/QuickCreateIssue.jspa";
        HttpClient client = new HttpClient();
        public bool isOk = true;
        public List<TaskToClone> tasksToClone;
        Task task;
        List<KeyValuePair<string, string>> Stories = null; //маппинг ссылок на стори. В IT1 это, например, CUR-4, в жире заказчика это будет что-то иное
       List<KeyValuePair<string,string>> Epics = null;//маппинг ссылок на эпики. в IT1 это, например, key:CUR-3, в жире заказчика будет что-то иное после key:

        //блок констант которые надо взять из жиры заказчика. ПЕРЕД ЗАПУСКОМ надо внести изменения в них и в данные в методе 
        //PrepareTask
        //КРОМЕ ТОГО
        //значение переменной customfield_10101 в запросе к жире 100% будет другим! его тоже подменить

        string pid = "11503";
        string atl_token= "BXQZ-C67O-KVR0-SWI7_d05cbacfcb8b39ab76e41bf926fd5be988f57a67_lin";//должен соответствовать токену из кукисов
        string priority = "10002";
        string versions= "13274", fixVersions = "13274";
        string issuelinks_linktype = "has story";
         
        public Migrator(List<TaskToClone> tasksToClone)
        {
            this.tasksToClone = tasksToClone;
        }

        public async void UploadTasks()
        {
            try
            {
                if (tasksToClone == null)
                    return;
                HttpResponseMessage httpResponseMessage;
                HttpRequestMessage httpRequestMessage;
                foreach (TaskToClone taskToClone in tasksToClone)
                {
                    Task currentTask = PrepareTask(taskToClone);

                    httpRequestMessage = new HttpRequestMessage();
                    httpRequestMessage.RequestUri = new Uri(sCustomersEndPoint);
                    httpRequestMessage.Headers.Add("Cookie", "atlassian.xsrf.token=BXQZ-C67O-KVR0-SWI7_d05cbacfcb8b39ab76e41bf926fd5be988f57a67_lin; JSESSIONID=0CD6F4296B59FBDFE1E9DDA0FBA93F71; jira.editor.user.mode=wysiwyg; _ym_uid=1645705398790172598; _ym_d=1650971296; _fbp=fb.1.1650971296721.450619859; JSESSIONID=03122E828DDF2F23504FCF51D15520B5; PHPSESSID=wSb5xb16X2VtqrhUyPh1Ca7C1G2oee5c; crowd.token_key=4YAXbmJ3HbKOVPPSeIPFvAAAAAAACoAFcGRvcm9raG92; com.luxoft.saml.autologin=true; XSRF-TOKEN=19482c51-328f-4f02-9db5-6f2f58c2bcb1");
                    //httpRequestMessage.Headers.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                    httpRequestMessage.Content= new StringContent("application/x-www-form-urlencoded; charset=UTF-8");
                    //==========================
                    //Подменить заголовки!
                    //==========================
                    httpRequestMessage.Headers.Add("X-AUSERNAME", "PDorokhov");
                    httpRequestMessage.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    httpRequestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:99.0) Gecko/20100101 Firefox/99.0");
                    httpRequestMessage.Headers.Add("Sec-Fetch-Dest", "empty");
                    httpRequestMessage.Headers.Add("Sec-Fetch-Mode", "cors");
                    httpRequestMessage.Headers.Add("Sec-Fetch-Site", "same-origin");
                    //собрали заголовки, начинаем собирать сообщение
                    var content = new List<KeyValuePair<string, string>>();
                    content.Add(new KeyValuePair<string, string>("pid", pid));
                    content.Add(new KeyValuePair<string, string>("issuetype", currentTask.issuetype.ToString()));
                    content.Add(new KeyValuePair<string, string>("atl_token", atl_token));
                    content.Add(new KeyValuePair<string, string>("summary", currentTask.summary));
                    content.Add(new KeyValuePair<string, string>("priority", priority));
                    content.Add(new KeyValuePair<string, string>("versions", versions));
                    content.Add(new KeyValuePair<string, string>("fixVersions", fixVersions));
                    content.Add(new KeyValuePair<string, string>("components", (currentTask.components == null ? "" : currentTask.components.ToString())));
                    content.Add(new KeyValuePair<string, string>("description", currentTask.description.Replace("\\r\\n","\r\n")));
                    content.Add(new KeyValuePair<string, string>("labels", currentTask.labels));
                    content.Add(new KeyValuePair<string, string>("issuelinks-linktype", currentTask.issuelinks_linktype));
                    if (currentTask.issuelinks_issues != null)
                        if (Stories != null)//подменяем сторю айтивановскую на клиентскую
                            currentTask.issuelinks_issues = Stories.SingleOrDefault(s => s.Key == currentTask.issuelinks_issues).Value;
                    if(currentTask.issuelinks_issues!=null)
                        content.Add(new KeyValuePair<string, string>("issuelinks-issues", currentTask.issuelinks_issues));//ссылка на сторю
                    if (currentTask.customfield_10101 != null)
                        if (Epics != null)//подменяем сторю айтивановскую на клиентскую
                            currentTask.customfield_10101 = "key:" + Epics.SingleOrDefault(s => s.Key == currentTask.customfield_10101).Value;
                    content.Add(new KeyValuePair<string, string>("customfield_10101", currentTask.customfield_10101));//ссылка на эпик

                    httpRequestMessage.Content = new FormUrlEncodedContent(content);
                    httpRequestMessage.Method = HttpMethod.Post;
                    httpResponseMessage = client.Send(httpRequestMessage);
                    string response = await httpResponseMessage.Content.ReadAsStringAsync();
                    if(response.Contains("Forbidden"))
                    {
                        Console.WriteLine("Fordbidden!");
                        return;
                    }
                    GetTasksFromITOne gtfit = new GetTasksFromITOne();
                    string issueKey = gtfit.FindElement(response, "\"issueKey\"", null, "\",");
                    if (issueKey != null)
                    {
                        switch (currentTask.issuetype)
                        {
                            case (int)Task.IssueTypes.Epic:
                                if(Epics==null)
                                    Epics=new List<KeyValuePair<string, string>>();
                                Epics.Add(new KeyValuePair<string, string>(taskToClone.epic, issueKey));
                                break;
                            case (int)Task.IssueTypes.Story:
                                if(Stories == null)
                                    Stories = new List<KeyValuePair<string, string>>(); 
                                Stories.Add(new KeyValuePair<string, string>(taskToClone.createdfrom, issueKey));
                                break;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public Task PrepareTask(TaskToClone taskToClone)
        {
            string component;
            int? cmp = null;
            int resultComp = -1 ;
            if (taskToClone.component != null)
            {
                component = taskToClone.component;
                Int32.TryParse(component,out resultComp);
                //=========================================
                //cmp - компонент. компоненты надо создать руками, затем подменить новыми значениями ниже.
                //=========================================
                cmp = cmp == 11165 ? 11165 : cmp == 11166 ? 11166 : 11164;
            }
            if (resultComp > 0)
                cmp = resultComp;
            Task task = new Task(pid, taskToClone.type, atl_token, taskToClone.summary, priority,versions,fixVersions,cmp,taskToClone.description
                ,taskToClone.labels,issuelinks_linktype,taskToClone.parentID,taskToClone.epic);

            return task;
        }
    }
}
