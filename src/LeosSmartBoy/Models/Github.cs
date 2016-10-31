using System.Collections.Generic;

namespace LeosSmartBoy.Models
{
    public class AuthorizationParam
    {
        public string scopes;
        public string note;
        public string note_url;
        public string client_id;
        public string client_secret;
        public string fingerprint;
    }
    public class Issue
    {
        public string title;
        public string body;
    }
    public class Comment
    {
        public string body;
    }
}