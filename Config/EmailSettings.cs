﻿namespace EmailAgentSWB.Config
{
    public class EmailSettings
    {
        public string ImapServer { get; set; } = string.Empty;
        public int ImapPort { get; set; }
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
