# 💰🔐 Digital Wallet API

A secure RESTful API for managing digital wallets and user-to-user transfers, with JWT-based authentication.
![image](https://github.com/user-attachments/assets/0cb19c32-957f-4bba-8cd9-4037c956b5e6)

## 📋 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (or LocalDB)
- Visual Studio 2022 / VS Code (optional)

## 🚀 Setup

1. Clone the repository:

   ```bash
   git clone [repository-url]
   cd [project-folder]
   ```

2. Configure your database connection:

   ```json
   // appsettings.json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=DigitalWallet;Trusted_Connection=True;"
   }
   ```

3. Run database migrations:

   ```bash
   dotnet ef database update
   ```

## 🔑 Authentication

Protected endpoints require a valid **JWT token**.

1. Login using:

   ```
   POST /api/auth/login
   ```

2. Use the returned token in the request headers:

   ```
   Authorization: Bearer <your_token>
   ```

## 📊 Core Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| POST   | `/api/auth/register` | Register a new user (automatically creates wallet) |
| POST   | `/api/transfers`     | Transfer money between wallets |
| GET    | `/api/transfers?startDate=YYYY-MM-DD&endDate=YYYY-MM-DD` | List user transfers by date range |

## 🧪 Testing with Swagger

Once the API is running, access Swagger:

```
https://localhost:5001/swagger
```

🔐 Don't forget to click **"Authorize"** and insert your JWT token!

## ⚙️ Environment Variables

You can define secrets in a `.env` file or your hosting environment:

```
JWT__Key=YOUR_32_CHARACTER_SECRET_KEY
JWT__ExpiresInHours=8
```

## 📦 Packages Used

- **Entity Framework Core 8**
- **JWT Bearer Authentication**
- **Swagger / Swashbuckle**

## 🤝 Contributing

1. Fork the repository  
2. Create a new branch: `git checkout -b feature/your-feature`  
3. Commit your changes: `git commit -am 'Add new feature'`  
4. Push to the branch: `git push origin feature/your-feature`  
5. Open a Pull Request

## 📄 License

MIT License

### ✅ Includes:

- Full setup instructions  
- Usage examples and endpoints  
- JWT authentication  
- Swagger testing instructions  
- Clean RESTful architecture

### ✨ Suggestions to Add:

- Deployment steps (Docker, Azure, etc.)  
- Sample request/response payloads  
- Swagger screenshots  
- Custom security policies if any
