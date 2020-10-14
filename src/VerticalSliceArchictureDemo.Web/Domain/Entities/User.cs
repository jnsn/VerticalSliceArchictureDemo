using System;

namespace VerticalSliceArchictureDemo.Web.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime LastModifiedOn { get; set; }
    }
}
