using System.ComponentModel.DataAnnotations;

namespace WebClient2.Data
{
    public class User
    {
        
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
