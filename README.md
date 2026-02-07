# coursera-user-management-project

Debugging & Optimization Report: User Management API

Issue: Data Integrity & Validation
The Problem: The initial User model lacked validation. The API accepted requests with empty names or malformed email addresses (e.g., "bob" instead of "bob@email.com"), leading to "dirty data" entering the system.

Copilot's Identification: Analyzed the User class and flagged the absence of Data Annotations. Identified this as a high-severity risk for downstream business processes.

Resolution:

Decorated the User model properties with [Required] and [StringLength].

Applied the [EmailAddress] attribute to the Email property.

Result: The API now automatically returns 400 Bad Request if the input does not meet the schema requirements, ensuring only valid data reaches the controller.

2. Issue: Concurrency & Thread Safety
The Problem: The API used a static List<User> to simulate the database. Standard lists are not thread-safe. If multiple users (HR and IT departments) tried to add or delete users simultaneously, the application would crash with an InvalidOperationException.

Copilot's Identification: Reviewed the data structure in UsersController and identified a "Race Condition" risk.

Resolution:

Replaced List<User> with ConcurrentDictionary<int, User>.

Replaced standard increment logic with Interlocked.Increment for ID generation.

Result: The API can now handle multiple simultaneous requests without crashing or corrupting the in-memory data store.

3. Issue: Performance Bottlenecks (O(n) vs O(1))
The Problem: The GetUser and DeleteUser endpoints relied on LINQ queries (e.g., _users.FirstOrDefault), which perform a linear search. As the user base grows, the lookup time increases linearly (O(n)).

Copilot's Identification: Acted as a Database Architect to critique the "indexing strategy." Pointed out that scanning the entire list for an ID is inefficient.

Resolution:

Optimized the storage mechanism to use a Dictionary where the Key is the Id.

Changed lookups to use .TryGetValue().

Result: Lookups are now O(1) (constant time). Retrieving User #10,000 is now just as fast as retrieving User #1.

4. Issue: Unhandled Exceptions
The Problem: The original code had no safety net. If a database error or unexpected null reference occurred, the API would return a raw 500 error or simply terminate the connection, confusing the client.

Copilot's Identification: Noted the lack of try-catch blocks around critical logic.

Resolution:

Wrapped all Controller actions in try-catch blocks.

Implemented UseExceptionHandler in Program.cs (Global Exception Handling).

Result: The API now gracefully catches errors and returns a standardized "Problem Details" JSON response, keeping the service running even when errors occur.
