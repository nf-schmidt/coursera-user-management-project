# User Management API

## 🚀 Features

* **CRUD Operations:** Create, Read, Update, and Delete user records efficiently.
* **JWT Authentication:** Secure endpoints using JSON Web Tokens (Bearer Auth).
* **Middleware Pipeline:**
    * **Request/Response Logging:** Audits all incoming traffic and outgoing responses.
    * **Global Exception Handling:** Standardized error responses (RFC 7807) to prevent crashes.
* **Thread-Safe Data:** Uses `ConcurrentDictionary` and `Interlocked` operations to handle simultaneous requests safely.
* **Data Validation:** strict input validation using Data Annotations.

---

## 🧪 Tutorial: How to Test the API

Since this API is secured with JWT (JSON Web Tokens), you cannot simply call the endpoints immediately. You must first "log in" to obtain a secure token.

Follow this step-by-step guide using the **Swagger UI**.

### Step 1: Obtain an Access Token (Login)

1.  Open the Swagger UI.
2.  Expand the **`POST /api/Auth/login`** endpoint.
3.  Click **Try it out**.
4.  You can enter any credentials in the Request Body:
    ```json
    {
      "username": "XXXX",
      "password": "XXXX"
    }
    ```
    *(Note: These are hardcoded for demonstration purposes).*
5.  Click **Execute**.
6.  Copy the long string inside the `"token"` field from the Response Body.

### Step 2: Authorize Your Session

1.  Scroll to the top of the Swagger page.
2.  Click the **Authorize** button (padlock icon).
3.  In the value box, type the word `Bearer`, followed by a space, and then paste your token.
    * **Format:** `Bearer eyJhbGciOiJIUzI1Ni...`
4.  Click **Authorize** and then **Close**.
    * *You are now authenticated as an Admin.*

    ---

## 📂 Project Structure

* **Controllers/**: Handles incoming HTTP requests (`UsersController`, `AuthController`).
* **Middleware/**: Custom pipeline components (`RequestResponseLogging`, `GlobalException`).
* **Models/**: Data structures and validation rules (`User`, `LoginModel`).
* **Program.cs**: Application entry point, service configuration, and middleware pipeline setup.
