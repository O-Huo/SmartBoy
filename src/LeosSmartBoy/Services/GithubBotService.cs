using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using LeosSmartBoy.Models;
using Newtonsoft.Json;


namespace LeosSmartBoy.Services
{
    public class GithubBotService
    {
        private string _githubID;
        private string _githubSecret;
        private HttpClient _httpClient;
        public GithubBotService(string githubID, string githubSecret)
        {
            //Init GithubBot with RoommateX account
            Console.WriteLine("Init GithubBot here");
            _githubID = githubID;
            _githubSecret = githubSecret;
            var byteArray = Encoding.ASCII.GetBytes(_githubID+':'+_githubSecret);
            var header = new AuthenticationHeaderValue("Basic",Convert.ToBase64String(byteArray));
            _httpClient = new HttpClient{BaseAddress = new Uri("https://api.github.com")};
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
            _httpClient.DefaultRequestHeaders.Authorization = header;
            //GetIssue();
            Test();
            //CreateComment("hahahaha");
        }

        private async Task Test()
        {
            string issueID = await CreateIssue("test2", "heklrhoiethyioe");
            Console.WriteLine(issueID);
            CreateComment(issueID, "hahaha");
        }

        private async Task GetIssue()
        {               
            HttpResponseMessage response = await _httpClient.GetAsync("/issues");
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
            HttpResponseMessage response = await _httpClient.PostAsync("/repos/RoommateX/test/issues", httpContent);
            var text = response.Content.ReadAsStringAsync().Result;
            var definition = new {number = ""};
            var textJson = await Task.Run(()=>JsonConvert.DeserializeAnonymousType(text, definition));
            return textJson.number;
        }

        private async Task CreateComment(string issueID, string commentBody)
        {
            var comment = new Comment{
                body = commentBody
            };
            var stringComment = await Task.Run(()=>JsonConvert.SerializeObject(comment));
            var httpContent = new StringContent(stringComment, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync("/repos/RoommateX/test/issues/" + issueID + "/comments", httpContent);
            var text = response.Content.ReadAsStringAsync().Result;
        }
    }        
}
