using System;
using System.ComponentModel.DataAnnotations;

namespace MyNotes.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Title { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}