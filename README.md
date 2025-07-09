# DigitalBankingApi

**DATABASE SETUP**
Db name: DigitalBankingDb
** The Db script was sent via email. Just drag and drop it in SQL Server and press execute.
** Please see the image (Digital Banking Schema overview) from the root folder.

**API**
Build and run the API (F5 button).

Business Rules Assumptions:
- Transaction Fee set to 25.
- Transaction fee is applicable only on transfer.
- Daily Transaction Limit set to 50,000. 
- Transaction Status: Failed, Pending & Completed

**BONUS FEATURES INCLUDED**
- Account statement generation (transactions within date range)
- Daily transaction limits
- Transaction fees calculation
- Search transactions by amount or description

**API Call sample using POSTMAN**
**1. Test first by adding a customer**
   [POST] - https://localhost:7109/api/Customers
   body: (raw)
   {
    "firstName": "Jane",
    "lastName": "De Leon",
    "email": "Jane.DeLeon@mail.com",
    "phone": "+639123212386"
   }
sample result:
{
    "success": true,
    "message": "Customer created successfully",
    "data": {
        "firstName": "Jane",
        "lastName": "De Leon",
        "email": "Jane.DeLeon@mail.com",
        "phone": "+639123212386"
    }
}

**2. Get the newly created customer by Customer ID**
[GET] - https://localhost:7109/api/Customers/{CustomerId}

sample result: 
{
    "success": true,
    "message": "Customer retrieved successfully",
    "data": {
        "id": "7053499e-583e-4c26-8dff-61706ebf46ba",
        "firstName": "Jane",
        "lastName": "De Leon",
        "email": "Jane.DeLeon@mail.com",
        "phone": "+639123212386",
        "createdDate": "2025-07-07T11:41:50.23",
        "accounts": []
    }
}
