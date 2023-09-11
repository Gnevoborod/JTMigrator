namespace JTMigrator
{

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Старт миграции");
            Console.WriteLine("Получение списка задач в JIRA IT1");
            GetTasksFromITOne getTasks=new GetTasksFromITOne();
            getTasks.DownloadAllTasks();
            if(!getTasks.isOk)
            {
                Console.WriteLine("Что-то пошло не так. См.логи.");
                return;
            }
            PrintAll(getTasks.tasks);
            //ToDo
            Console.WriteLine("Список задач получен.\nНачинаем миграцию.");
            //ToDo
            //Migrator migrator = new Migrator(getTasks.tasks);
            //migrator.UploadTasks();
        }

        public static void PrintAll(List<TaskToClone> tasks)
        {
            foreach (TaskToClone task in tasks)
            {
                Console.WriteLine(task.summary + ":\n" + task.description);
                Console.WriteLine("Epic: "+task.epic+". Labels:"+task.labels +". Component:"+task.component);
                Console.WriteLine("Parent:" + task.parentID + ". Type:" + task.type);
                Console.WriteLine(("\n").PadRight(48,'-'));
            }
        }
    }
}