using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Model.Json.Request
{
    public class JsonSms : JsonStatus
    {
        public int UserId { get; set; }

        public int LessonId { get; set; }

        public string Msg { get; set; }
    }
}
