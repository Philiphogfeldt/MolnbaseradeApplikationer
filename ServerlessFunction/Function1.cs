using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.AspNetCore.Http.Internal;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;
using System.Data.Common;

namespace ServerlessFunction
{
    public static class TodoApi
    {
        static List<Todo> items = new List<Todo>();

        [FunctionName("CreateTodo")]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")]HttpRequest req, ILogger log)
        {
            log.LogInformation("Creating a new todo list item");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);

            var todo = new Todo() { TaskDescription = input.TaskDescription };
            items.Add(todo);
            return new OkObjectResult(todo);
        }

        [FunctionName("GetTodos")]
        public static IActionResult GetTodos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")]HttpRequest req, ILogger log)
        {
            log.LogInformation("Getting todo list items");
            return new OkObjectResult(items);
        }

        [FunctionName("GetTodoById")]
        public static IActionResult GetTodoById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")]HttpRequest req, ILogger log, string id)

        {
            var todo = items.FirstOrDefault(x => x.Id == id);
            if(todo == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(todo);
        }

		[FunctionName("UpdateTodos")]
		public static async Task<IActionResult> UpdateTodo(
				   [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")] HttpRequest req,
				   ILogger log, string id)
		{
			var todo = items.FirstOrDefault(t => t.Id == id);
			if (todo == null)
			{
				return new NotFoundResult();
			}

			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			var updated = JsonConvert.DeserializeObject<TodoUpdateModel>(requestBody);

			todo.IsCompleted = updated.IsCompleted;
			if (!string.IsNullOrEmpty(updated.TaskDescription))
			{
				todo.TaskDescription = updated.TaskDescription;
			}

			return new OkObjectResult(todo);
		}

        [FunctionName("DeleteTodo")]
        public static IActionResult DeleteTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")]HttpRequest req, ILogger log, string id)
        {
            var todo = items.FirstOrDefault(x => x.Id == id);
            if(todo == null)
            {
                return new NotFoundResult();
            }
            items.Remove(todo);
            return new OkResult();
        }
	}
}
