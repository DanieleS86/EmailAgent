using EmailAgentSWB.Models;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using MimeKit;
using System.Net.Mail;
using EmailAgentSWB.Config;
using Microsoft.Extensions.Options;

namespace EmailAgentSWB.Services
{
    public class EmailService
    {

        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task<List<EmailMessage>> GetEmailsAsync()
        {
            var emails = new List<EmailMessage>();

            using var client = new ImapClient();
            await client.ConnectAsync(_settings.ImapServer, _settings.ImapPort, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);

            var inbox = client.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadOnly);

            // Ottieni gli ultimi 10 messaggi con metadati
            int count = inbox.Count;
            int start = Math.Max(0, count - 10);
            var summaries = await inbox.FetchAsync(start, count - 1, MessageSummaryItems.Envelope | MessageSummaryItems.Flags | MessageSummaryItems.InternalDate | MessageSummaryItems.UniqueId);

            foreach (var summary in summaries)
            {
                if (summary.UniqueId.IsValid)
                {
                    var message = await inbox.GetMessageAsync(summary.UniqueId);
                    emails.Add(new EmailMessage
                    {
                        From = message.From.Mailboxes.FirstOrDefault()?.Address ?? "Unbekannt",
                        Subject = message.Subject,
                        DateReceived = summary.InternalDate?.DateTime ?? DateTime.MinValue,
                        Body = message.TextBody ?? "(Kein Textinhalt)",
                        Status = summary.Flags?.HasFlag(MessageFlags.Seen) == true ? "Gelesen" : "Ungelesen"
                    });
                }
            }

            await client.DisconnectAsync(true);
            return emails;
        }

    }

}
