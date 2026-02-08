using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using UserManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace UserManagementAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        // Thread-safe collection acting as our Primary Key Index
        private static ConcurrentDictionary<int, User> _users = new ConcurrentDictionary<int, User>();

        // Atomic counter for ID generation (simulating IDENTITY column)
        private static int _nextId = 1;

        // Constructor to seed data
        static UsersController()
        {
            var u1 = new User { Id = 1, FirstName = "Alice", LastName = "Smith", Email = "alice@company.com", Department = "IT" };
            var u2 = new User { Id = 2, FirstName = "Bob", LastName = "Jones", Email = "bob@company.com", Department = "HR" };

            _users.TryAdd(u1.Id, u1);
            _users.TryAdd(u2.Id, u2);
            _nextId = 3;
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            try
            {
                return Ok(_users.Values);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error retrieving users.");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetUser(int id)
        {
            try
            {
                // O(1) Lookup - Highly Optimized
                if (_users.TryGetValue(id, out var user))
                {
                    return Ok(user);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [HttpPost]
        public ActionResult<User> CreateUser(User newUser)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (_users.Values.Any(u => u.Email == newUser.Email))
                {
                    return Conflict("A user with this email already exists.");
                }

                int newId = Interlocked.Increment(ref _nextId);
                newUser.Id = newId;

                if (_users.TryAdd(newId, newUser))
                {
                    return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
                }

                return StatusCode(500, "Failed to create user.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, User updatedUser)
        {
            try
            {
                if (!_users.ContainsKey(id))
                    return NotFound();

                // Optimistic concurrency: Try to update the value atomically
                var existingUser = _users[id];
                updatedUser.Id = id; // Ensure ID matches path

                // In a real DB, this would be an UPDATE statement
                _users[id] = updatedUser;

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error updating user.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                if (_users.TryRemove(id, out _))
                {
                    return NoContent();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error deleting user.");
            }
        }
    }
}