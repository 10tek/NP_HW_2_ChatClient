using System;
using System.Collections.Generic;
using System.Text;

namespace ChatClientHw2
{
    public class Message
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public string User { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return $"  {User} :  {Text}    : {CreationDate}";
        }
    }
}
