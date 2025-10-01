# EmailAgent


# 📬 EmailAgentSWB

An intelligent email agent for businesses, built with Blazor Server, MudBlazor, Hugging Face, and MySQL ERP integration.

## 🚀 Features

- 📥 Reads incoming emails via IMAP (Strato)
- 🧠 Analyzes content using Hugging Face or OpenAI
- 🗂 Links emails with ERP data (products, orders, customers)
- ✍️ Generates automatic reply suggestions
- ✅ UI for approving, editing, and sending responses

## 🧱 Technology Stack

| Component          | Technology                          |
|--------------------|--------------------------------------|
| UI                 | Blazor Server + MudBlazor            |
| Backend            | ASP.NET Core (.NET 7)                |
| Database           | MySQL (via EF Core & Pomelo)         |
| NLP / AI           | Hugging Face API / OpenAI API        |
| Email Integration  | IMAP via MailKit                     |
| ERP Access         | Scaffolded DbContext from MySQL      |

## 🖥 Screenshots

<img width="1713" height="948" alt="image" src="https://github.com/user-attachments/assets/4f4d955b-acd0-4343-995a-92049173fb1e" />

## 📦 Setup

### Requirements

- Visual Studio 2022+
- .NET 7 SDK
- Strato IMAP credentials
- Access to MySQL ERP database
- API key for Hugging Face or OpenAI

### Installation

bash
````
git clone https://github.com/your-username/EmailAgentSWB.git
cd EmailAgentSWB
Database Connection
````
Edit appsettings.json:
````
json
"ConnectionStrings": {
  "DefaultConnection": "Server=xxx.xxx.xxx.xxx;User=username;Password=***;Database=database;Port=3306;CharSet=utf8;"
}
````
Scaffold DbContext
powershell
````
Scaffold-DbContext "Server=...;Database=..." Pomelo.EntityFrameworkCore.MySql -OutputDir Data -Context AppDbContext -Force
````

🤖 AI Service: ModelAIService
The generic ModelAIService supports:

Hugging Face (facebook/bart-large-cnn) for text summarization

OpenAI (GPT-4o, GPT-5) for full email response generation

Example:

csharp
var reply = await modelAIService.GeneriereAntwort(email.Body);
🔐 API tokens should be managed via IConfiguration or User Secrets, not hardcoded.

📨 Email Reading with MailKit
csharp
````
using MailKit.Net.Imap;
using MailKit.Security;
using MimeKit;

public void ReadEmails() {
    using var client = new ImapClient();
    client.Connect("imap.provaider.de", 993, SecureSocketOptions.SslOnConnect);
    client.Authenticate("user@domain.de", "password");
    var inbox = client.Inbox;
    inbox.Open(FolderAccess.ReadOnly);
    var message = inbox.GetMessage(0);
}
````
🖼 UI with MudBlazor
The EmailAgent.razor page includes:

A table displaying received emails

A textbox with the AI-generated reply

Buttons for “Send Reply”, “Edit”, “Archive”

📄 License
````
MIT License
````
📂 Importing a Database as DbContext in Blazor
To generate the Entity Framework Core DbContext and models from an existing MySQL database, follow these steps:

1. Configure the connection string
Edit your appsettings.json and add a connection string (replace with your own values):

json
````
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;User=YOUR_USER;Password=YOUR_PASSWORD;Database=YOUR_DATABASE;Port=3306;CharSet=utf8;"
}
````
⚠️ Important: Never commit real passwords or production credentials to GitHub. Use placeholders in appsettings.json and store secrets with User Secrets or environment variables.

2. Install required NuGet packages
From the Package Manager Console in Visual Studio:

powershell
````
Install-Package Pomelo.EntityFrameworkCore.MySql
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.EntityFrameworkCore.Design
````
3. Scaffold the DbContext
Run the following command in the Package Manager Console (replace placeholders with your own values):
powershell
````
Scaffold-DbContext "Server=YOUR_SERVER;User=YOUR_USER;Password=YOUR_PASSWORD;Database=YOUR_DATABASE;Port=3306;CharSet=utf8;" Pomelo.EntityFrameworkCore.MySql -OutputDir Models -Context AppDbContext -Force
````
-OutputDir Models → generates all entity classes inside a Models folder

-Context AppDbContext → creates the AppDbContext class

-Force → overwrites existing files if they already exist

4. Result
This will generate:

AppDbContext.cs → your EF Core context class

One .cs file per table in your database inside the Models folder
