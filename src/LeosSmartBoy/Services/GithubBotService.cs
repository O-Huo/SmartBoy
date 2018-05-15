using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeosSmartBoy.Models;
using LeosSmartBoy.Managers;
using Newtonsoft.Json;


namespace LeosSmartBoy.Services
{
    public class GithubBotService
    {
        private string githubID;
        private string githubSecret;
        private HttpClient httpClient;
        private IStorageManager storageManager;
        public GithubBotService(string githubID, string githubSecret)
        {
            //Init GithubBot with RoommateX account
            Console.WriteLine("Init GithubBot here");
            this.githubID = githubID;
            this.githubSecret = githubSecret;
            var byteArray = Encoding.ASCII.GetBytes(githubID+':'+githubSecret);
            var header = new AuthenticationHeaderValue("Basic",Convert.ToBase64String(byteArray));
            httpClient = new HttpClient{BaseAddress = new Uri("https://api.github.com")};
            httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
            httpClient.DefaultRequestHeaders.Authorization = header;
            //GetIssue();
            //Test();
            //CreateComment("hahahaha");
        }

        public void AddStorageManager(IStorageManager storageManager)
        {
            this.storageManager = storageManager;
        }

        public async Task<string> WriteBill(Bill bill)
        {
            var users = storageManager.GetChatUsers(bill.ChatId);
            
            string commentBody = "Amount: " + bill.AmountString + "\nSelected Users: " + string.Join(", ", users.Where(u => bill.SharedWith.Contains(u.Id)).Select(u => u.LastName + ' ' + u.FirstName)) + "\nCreated By: " + users.FirstOrDefault(u => u.Id==bill.CreatedBy).LastName + ' ' + users.FirstOrDefault(u => u.Id==bill.CreatedBy).FirstName;
            string commentUrl = await CreateComment("16",commentBody);
            return commentUrl;
        }

        public async Task Test()
        {
            string issueID = await CreateIssue("test2", "heklrhoiethyioe");
            Console.WriteLine(issueID);
            //CreateComment(issueID, "hahaha");
        }

        private async Task GetIssue()
        {               
            HttpResponseMessage response = await httpClient.GetAsync("/issues");
            response.EnsureSuccessStatusCode();
            var text = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(text);
            Console.WriteLine("finish");
        }

        private async Task<string> CreateIssue(string issueTitle, string issueBody)
        {
            var issue = new Issue{
                title = issueTitle,
                body = issueBody
            };

            var stringIssue = await Task.Run(()=>JsonConvert.SerializeObject(issue));
            var httpContent =  new StringContent(stringIssue, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync("/repos/RoommateX/test/issues", httpContent);
            var text = response.Content.ReadAsStringAsync().Result;
            var definition = new {number = ""};
            var textJson = await Task.Run(()=>JsonConvert.DeserializeAnonymousType(text, definition));
            return textJson.number;
        }

        private async Task<string> CreateComment(string issueID, string commentBody)
        {
            var comment = new Comment{
                body = commentBody
            };
            var stringComment = await Task.Run(()=>JsonConvert.SerializeObject(comment));
            var httpContent = new StringContent(stringComment, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync("/repos/RoommateX/test/issues/" + issueID + "/comments", httpContent);
            var text = response.Content.ReadAsStringAsync().Result;
            var definition = new {html_url = ""};
            var textJson = await Task.Run(()=>JsonConvert.DeserializeAnonymousType(text, definition));
            return textJson.html_url;
        }
    }        
}
