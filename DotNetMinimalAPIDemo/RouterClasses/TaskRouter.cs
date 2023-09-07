using DotNetMinimalAPIDemo.EntityClasses;
using DotNetMinimalAPIDemo.Components;
using Task = DotNetMinimalAPIDemo.EntityClasses.Task;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace DotNetMinimalAPIDemo.RouterClasses
{
    public class TaskRouter : RouterBase
    {
        IDictionary<int, List<Task>> myDictionary = new Dictionary<int, List<Task>>();
        public TaskRouter(ILogger<TaskRouter> logger)
        {
            UrlFragment = "task";
            Logger = logger;
            myDictionary.Add(-1, new List<Task> { new Task { ID = 1, Name = "Task_1", Owner = "", HasChildren = true } });
            myDictionary.Add(-2, new List<Task> { new Task { ID = 2, Name = "Task_2", Owner = "", HasChildren = true } });
            myDictionary.Add(1, new List<Task> { new Task { ID = 4, Name = "Step_1_1", Owner = "anonymous", HasChildren = false }, new Task { ID = 5, Name = "Step_1_2", Owner = "anonymous", HasChildren = false } });
            myDictionary.Add(2, new List<Task> { new Task { ID = 6, Name = "Step_2_1", Owner = "Joe", HasChildren = false }, new Task { ID = 7, Name = "Step_2_2", Owner = "Bob", HasChildren = true } });
            myDictionary.Add(7, new List<Task> { new Task { ID = 8, Name = "Step_2_2_1", Owner = "Bob & Alice", HasChildren = false }, new Task { ID = 9, Name = "Step_2_2_2", Owner = "Bob & Max", HasChildren = false } });
        }

        protected virtual IDictionary<int, List<Task>> GetRootTree()
        {
            IDictionary<int, List<Task>> rootDictionary = new Dictionary<int, List<Task>>();
            foreach (int key in myDictionary.Keys)
            {
                if (key < 0)
                {
                    rootDictionary.Add(key, myDictionary[key]);
                }
            }
            return rootDictionary;
        }

        protected virtual IDictionary<int, List<Task>> GetChildren(int id)
        {
            IDictionary<int, List<Task>> childDictionary = new Dictionary<int, List<Task>>();
            foreach (int key in myDictionary.Keys)
            {
                if (key == id)
                {
                    childDictionary.Add(key, myDictionary[key]);
                }
            }
            return childDictionary;
        }
        protected virtual void AddChild(int id, Task task)
        {
            bool isNewEntry = true;
            foreach (int key in myDictionary.Keys)
            {
                if (key == id)
                {
                    isNewEntry = false;
                    Random random = new();
                    task.ID = random.Next();
                    myDictionary[key].Add(task);
                }
            }
            if (isNewEntry == true)
            {
                int parentId = -99;
                foreach (int key in myDictionary.Keys)
                {
                    foreach (Task currentTask in myDictionary[key])
                    {
                        if (currentTask.ID == id)
                        {
                            parentId = key;
                        }
                    }
                }
                if (!String.IsNullOrEmpty(parentId.ToString()))
                {
                    foreach (Task currentTask in myDictionary[parentId])
                    {
                        if (currentTask.ID == id)
                        {
                            Random random = new();
                            task.ID = random.Next();
                            myDictionary.Add(id, new List<Task> { task });
                            currentTask.HasChildren = true;
                        }
                    }
                }

            }
            return;
        }

        public override void AddRoutes(WebApplication app)
        {
            app.MapGet("/rootTree", () => GetRootTree());
            app.MapGet($"/children/{{id:int}}", (int id) => GetChildren(id));
            app.MapPost($"/add/{{id:int}}", async delegate (HttpContext context)
            {
                using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    var id = Convert.ToInt32(context.Request.RouteValues["id"]);
                    string task = await reader.ReadToEndAsync();
                    var finalTask = JsonSerializer.Deserialize<Task>(task);
                    AddChild(id, finalTask);


                }
            });
        }
    }


}
